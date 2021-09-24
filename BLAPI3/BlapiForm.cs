using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json.Linq;
using QRCoder;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BLAPI3
{
    public partial class BlapiForm : Form
    {
        private BLAPI_Processor processor;

        public BlapiForm()
        {
            InitializeComponent();
            int oauth1_timestamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            processor = new BLAPI_Processor(oauth1_timestamp);
        }

        private void toolStripButtonSlips_Click(object sender, EventArgs e)
        {
            //fonts for pdf document
            BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            iTextSharp.text.Font f25 = new iTextSharp.text.Font(bfTimes, 25);
            iTextSharp.text.Font f20 = new iTextSharp.text.Font(bfTimes, 20);
            iTextSharp.text.Font f16 = new iTextSharp.text.Font(bfTimes, 16);

            //pdf filename
            string filename = "\\list" + DateTime.Now.ToString("_MM_dd") + ".pdf";
            // create directory if it is not exist yet
            Directory.CreateDirectory(Properties.Settings.Default.FilesPath);
            string path = Properties.Settings.Default.FilesPath + filename;
            FileStream fs = new FileStream(path, FileMode.Create);

            Document document = new Document(PageSize.A4, 20, 20, 25, 25);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.AddCreator("BrickLink API paid orders list");
            document.Open();

            //First page - list of paid orders
            PdfPTable table = new PdfPTable(5);
            PdfPCell cell = new PdfPCell(new Phrase(DateTime.Now.ToString("MM/dd/yyyy")));
            cell.Colspan = 5;
            cell.HorizontalAlignment = 0;

            table.AddCell(cell);
            table.AddCell("#");
            table.AddCell("Name");
            table.AddCell("Total $");
            table.AddCell("My Cost $");
            table.AddCell("BL Order #");

            for (int i = 0; i < listViewOrders.Items.Count; i++)
            {
                if (listViewOrders.Items[i].Checked == true)
                {
                    table.AddCell(new PdfPCell(new Phrase(listViewOrders.Items[i].SubItems[1].Text, f16)));
                    table.AddCell(new PdfPCell(new Phrase(listViewOrders.Items[i].SubItems[2].Text, f16)));
                    table.AddCell(new PdfPCell(new Phrase(listViewOrders.Items[i].SubItems[3].Text, f16)));
                    table.AddCell(new PdfPCell(new Phrase(listViewOrders.Items[i].SubItems[4].Text, f16)));
                    table.AddCell(new PdfPCell(new Phrase(listViewOrders.Items[i].SubItems[5].Text, f16)));

                };
            }
            Paragraph p0 = new Paragraph();
            p0.Font = f16;
            p0.Add(table);
            document.Add(p0);
            document.NewPage();

            //For each order create and add to pdf file packing slip
            for (int i = 0; i < listViewOrders.Items.Count; i++)
            {
                if (listViewOrders.Items[i].Checked == true)
                {
                    Paragraph p1 = new Paragraph();
                    p1.Font = f25;
                    Paragraph p2 = new Paragraph();
                    p2.Font = f20;
                    Paragraph p3 = new Paragraph();
                    p3.Font = f20;

                    p1.Add("THANK YOU FOR YOUR\n\nBRICKLINK ORDER\n\n\n#" + listViewOrders.Items[i].SubItems[5].Text + "\n\n");
                    p2.Add(listViewOrders.Items[i].SubItems[2].Text + "\n" + "Total: $" + listViewOrders.Items[i].SubItems[3].Text + "\n");

                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode("https://www.bricklink.com/orderDetail.asp?ID=" + listViewOrders.Items[i].SubItems[5].Text, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    Bitmap qrCodeImage = qrCode.GetGraphic(20, System.Drawing.Color.Black, System.Drawing.Color.White, (Bitmap)Bitmap.FromFile(Properties.Settings.Default.LogoPath));
                    var pdfImage = iTextSharp.text.Image.GetInstance(qrCodeImage, System.Drawing.Imaging.ImageFormat.Bmp);
                    pdfImage.ScaleAbsolute(150F, 150F);
                    p3.Add("\n\nScan for order details on bricklink.com:\n");
                    p3.Add(pdfImage);

                    document.Add(p1);
                    document.Add(p2);
                    document.Add(p3);
                    document.NewPage();

                    //Move to "PACKED" status
                    if (Properties.Settings.Default.ToPacked)
                    {
                        processor.BL_set_order_status(listViewOrders.Items[i].SubItems[5].Text, BLAPI_Processor_status.BLAPI_Processor_PACKED);
                    }
                }
            }

            // Close the document  
            document.Close();
            writer.Close();
            fs.Close();

            string result_info = "File generated and saved in " + path;
            if (Properties.Settings.Default.ToPacked)
            {
                result_info += ".\nOrders status changed to PACKED"; 
            }
            MessageBox.Show(result_info);
        }

        private void toolStripButtonSettings_Click(object sender, EventArgs e)
        {
            SettingsForm sform = new SettingsForm();

            if (sform.ShowDialog() == DialogResult.OK)
            {
                //update OAuth values in processor, just in case if it was changed
                processor.BL_set_oauth1();
            }

            sform.Dispose();
        }

        private void listViewOrders_load(BLAPI_Processor_status status)
        {
            listViewOrders.Items.Clear();
            string orders = processor.Bl_orders_full_list(status);
            //orders is data in JSON format
            JObject orders_json = JObject.Parse(orders);
            JArray orders_arr = (JArray)orders_json["data"];
            int row_n = 1;
            foreach (var order in orders_arr)
            {
                string name = order["name"].ToString();
                string total = order["total"].ToString();
                string my_cost = order["my_cost"].ToString();
                string order_id = order["order_id"].ToString();

                string[] row = { "", row_n.ToString(), name, total, my_cost, order_id };
                var listViewItem = new ListViewItem(row);
                listViewItem.Checked = true;
                listViewOrders.Items.Add(listViewItem);
                row_n++;
            }
        }

        private void toolStripButtonOrders_Click(object sender, EventArgs e)
        {
            //Call BL API
            listViewOrders_load(BLAPI_Processor_status.BLAPI_Processor_PAID);
        }

        private void toolStripButtonPackedOrders_Click(object sender, EventArgs e)
        {
            listViewOrders_load(BLAPI_Processor_status.BLAPI_Processor_PACKED);
        }

        private void toolStripButtonToShipped_Click(object sender, EventArgs e)
        {
            //Move each order in teh listview to shipped 
            for (int i = 0; i < listViewOrders.Items.Count; i++)
            {
                if (listViewOrders.Items[i].Checked == true)
                {
                    //Move to "SHIPPED" status
                    processor.BL_set_order_status(listViewOrders.Items[i].SubItems[5].Text, BLAPI_Processor_status.BLAPI_Processor_SHIPPED);
                }
            }
            string result_info = "Orders status changed to SHIPPED";
            MessageBox.Show(result_info);
        }
    }
}
