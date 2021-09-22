
namespace BLAPI3
{
    partial class BlapiForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BlapiForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonSettings = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonPaidOrders = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSlips = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonPackedOrders = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonToShipped = new System.Windows.Forms.ToolStripButton();
            this.listViewOrders = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonSettings,
            this.toolStripSeparator1,
            this.toolStripButtonPaidOrders,
            this.toolStripButtonSlips,
            this.toolStripSeparator2,
            this.toolStripButtonPackedOrders,
            this.toolStripButtonToShipped});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(800, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonSettings
            // 
            this.toolStripButtonSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSettings.Name = "toolStripButtonSettings";
            this.toolStripButtonSettings.Size = new System.Drawing.Size(53, 22);
            this.toolStripButtonSettings.Text = "Settings";
            this.toolStripButtonSettings.Click += new System.EventHandler(this.toolStripButtonSettings_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonPaidOrders
            // 
            this.toolStripButtonPaidOrders.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonPaidOrders.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPaidOrders.Name = "toolStripButtonPaidOrders";
            this.toolStripButtonPaidOrders.Size = new System.Drawing.Size(101, 22);
            this.toolStripButtonPaidOrders.Text = "Load Paid Orders";
            this.toolStripButtonPaidOrders.Click += new System.EventHandler(this.toolStripButtonOrders_Click);
            // 
            // toolStripButtonSlips
            // 
            this.toolStripButtonSlips.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonSlips.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSlips.Name = "toolStripButtonSlips";
            this.toolStripButtonSlips.Size = new System.Drawing.Size(130, 22);
            this.toolStripButtonSlips.Text = "Generate Packing Slips";
            this.toolStripButtonSlips.Click += new System.EventHandler(this.toolStripButtonSlips_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonPackedOrders
            // 
            this.toolStripButtonPackedOrders.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonPackedOrders.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonPackedOrders.Image")));
            this.toolStripButtonPackedOrders.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPackedOrders.Name = "toolStripButtonPackedOrders";
            this.toolStripButtonPackedOrders.Size = new System.Drawing.Size(116, 22);
            this.toolStripButtonPackedOrders.Text = "Load Packed Orders";
            // 
            // toolStripButtonToShipped
            // 
            this.toolStripButtonToShipped.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonToShipped.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonToShipped.Image")));
            this.toolStripButtonToShipped.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonToShipped.Name = "toolStripButtonToShipped";
            this.toolStripButtonToShipped.Size = new System.Drawing.Size(102, 22);
            this.toolStripButtonToShipped.Text = "Move To Shipped";
            // 
            // listViewOrders
            // 
            this.listViewOrders.Alignment = System.Windows.Forms.ListViewAlignment.Default;
            this.listViewOrders.CheckBoxes = true;
            this.listViewOrders.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.listViewOrders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewOrders.HideSelection = false;
            this.listViewOrders.Location = new System.Drawing.Point(0, 25);
            this.listViewOrders.Name = "listViewOrders";
            this.listViewOrders.Size = new System.Drawing.Size(800, 425);
            this.listViewOrders.TabIndex = 1;
            this.listViewOrders.UseCompatibleStateImageBehavior = false;
            this.listViewOrders.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Use";
            this.columnHeader1.Width = 40;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "#";
            this.columnHeader2.Width = 50;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Name";
            this.columnHeader3.Width = 180;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Total";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "My Cost";
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "BL Order #";
            this.columnHeader6.Width = 120;
            // 
            // BlapiForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.listViewOrders);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BlapiForm";
            this.Text = "BrickLink Packing API";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonSlips;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonSettings;
        private System.Windows.Forms.ListView listViewOrders;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ToolStripButton toolStripButtonPaidOrders;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonPackedOrders;
        private System.Windows.Forms.ToolStripButton toolStripButtonToShipped;
    }
}

