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
        private int oauth1_timestamp;
        public BlapiForm()
        {
            InitializeComponent();
            oauth1_timestamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            iTextSharp.text.Font f25 = new iTextSharp.text.Font(bfTimes, 25);
            iTextSharp.text.Font f18 = new iTextSharp.text.Font(bfTimes, 20);

            string filename = "\\list" + DateTime.Now.ToString("_MM_dd") + ".pdf";
            string path = Properties.Settings.Default.FilesPath + filename;

            FileStream fs = new FileStream(path, FileMode.Create);

            // Create an instance of the document class which represents the PDF document itself.  
            Document document = new Document(PageSize.A4, 20, 20, 25, 25);
            // Create an instance to the PDF file by creating an instance of the PDF   
            // Writer class using the document and the filestrem in the constructor.  

            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            // Add meta information to the document  
            //document.AddAuthor("Micke Blomquist");
            document.AddCreator("BrickLink API paid orders list");
            //document.AddKeywords("PDF tutorial education");
            //document.AddSubject("Document subject - Describing the steps creating a PDF document");
            //document.AddTitle("The document title - PDF creation using iTextSharp");
            // Open the document to enable you to write to the document  
            document.Open();

            PdfPTable table = new PdfPTable(5);
            PdfPCell cell = new PdfPCell(new Phrase(DateTime.Now.ToString("dd/MM/yyyy")));
            cell.Colspan = 5;
            cell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right

            table.AddCell(cell);
            table.AddCell("#");
            table.AddCell("Name");
            table.AddCell("Total $");
            table.AddCell("My Cost $");
            table.AddCell("BL Order #");

            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if (listView1.Items[i].Checked == true)
                {
                    table.AddCell(new PdfPCell(new Phrase(listView1.Items[i].SubItems[1].Text, f18)));
                    table.AddCell(new PdfPCell(new Phrase(listView1.Items[i].SubItems[2].Text, f18)));
                    table.AddCell(new PdfPCell(new Phrase(listView1.Items[i].SubItems[3].Text, f18)));
                    table.AddCell(new PdfPCell(new Phrase(listView1.Items[i].SubItems[4].Text, f18)));
                    table.AddCell(new PdfPCell(new Phrase(listView1.Items[i].SubItems[5].Text, f18)));

                };
            }
            Paragraph p0 = new Paragraph();
            p0.Font = f18;
            p0.Add(table);

            document.Add(p0);
            document.NewPage();
            document.Add(new Table(1));
            document.NewPage();

            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if (listView1.Items[i].Checked == true)
                {

                    Paragraph p1 = new Paragraph();
                    p1.Font = f25;
                    Paragraph p2 = new Paragraph();
                    p2.Font = f18;

                    p1.Add("THANK YOU FOR YOUR\n\nBRICKLINK ORDER\n\n\n#" + listView1.Items[i].SubItems[5].Text + "\n\n");
                    p2.Add(listView1.Items[i].SubItems[2].Text + "\n" + "Total: $" + listView1.Items[i].SubItems[3].Text + "\n");

                    document.Add(p1);
                    document.Add(p2);

                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode("https://www.bricklink.com/orderDetail.asp?ID=" + listView1.Items[i].SubItems[5].Text, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    Bitmap qrCodeImage = qrCode.GetGraphic(20);

                    var pdfImage = iTextSharp.text.Image.GetInstance(qrCodeImage, System.Drawing.Imaging.ImageFormat.Bmp);
                    pdfImage.ScaleAbsolute(150F, 150F);
                    Paragraph p3 = new Paragraph();
                    p3.Font = f18;
                    p3.Add("\n\nScan for order details on bricklink.com:\n");
                    p3.Add(pdfImage);

                    document.Add(p3);
                    document.NewPage();
                }
            }

            // Close the document  
            document.Close();
            // Close the writer instance  
            writer.Close();
            // Always close open filehandles explicity  
            fs.Close();

            MessageBox.Show("File generated and saved in " + path);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            SettingsForm sform = new SettingsForm();

            if (sform.ShowDialog() == DialogResult.OK)
            {

            }

            sform.Dispose();
        }


        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            //Call BL API
            BLAPI_Processor pro = new BLAPI_Processor(oauth1_timestamp);

            listView1.Items.Clear();
            string orders = pro.bl_orders_full_list();
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
                listView1.Items.Add(listViewItem);
                row_n++;
            }
            oauth1_timestamp = pro.oauth1_last_timestamp + 1;
        }
    }
}
