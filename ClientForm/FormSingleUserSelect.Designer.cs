namespace ClientForm
{
    partial class FormSingleUserSelect
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonSubmit = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.NameColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderID = new System.Windows.Forms.ColumnHeader();
            this.listView2 = new System.Windows.Forms.ListView();
            this.NameColumnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderID2 = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // buttonSubmit
            // 
            this.buttonSubmit.AutoSize = true;
            this.buttonSubmit.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonSubmit.Location = new System.Drawing.Point(0, 483);
            this.buttonSubmit.Name = "buttonSubmit";
            this.buttonSubmit.Size = new System.Drawing.Size(206, 25);
            this.buttonSubmit.TabIndex = 2;
            this.buttonSubmit.Text = "Подтвердить выбор";
            this.buttonSubmit.UseVisualStyleBackColor = true;
            // 
            // listView1
            // 
            this.listView1.BackgroundImageTiled = true;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.NameColumnHeader,
            this.columnHeaderID});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Margin = new System.Windows.Forms.Padding(4);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(206, 483);
            this.listView1.TabIndex = 4;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // NameColumnHeader
            // 
            this.NameColumnHeader.Text = "Имя";
            this.NameColumnHeader.Width = 400;
            // 
            // columnHeaderID
            // 
            this.columnHeaderID.Width = 0;
            // 
            // listView2
            // 
            this.listView2.BackgroundImageTiled = true;
            this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.NameColumnHeader1,
            this.columnHeaderID2});
            this.listView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView2.GridLines = true;
            this.listView2.Location = new System.Drawing.Point(0, 0);
            this.listView2.Margin = new System.Windows.Forms.Padding(4);
            this.listView2.MultiSelect = false;
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(206, 483);
            this.listView2.TabIndex = 5;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.Details;
            // 
            // NameColumnHeader1
            // 
            this.NameColumnHeader1.Text = "Имя";
            this.NameColumnHeader1.Width = 400;
            // 
            // columnHeaderID2
            // 
            this.columnHeaderID2.Width = 0;
            // 
            // FormSingleUserSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(206, 508);
            this.Controls.Add(this.listView2);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.buttonSubmit);
            this.Name = "FormSingleUserSelect";
            this.Text = "FormSingleUserSelect";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button buttonSubmit;
        private ListView listView1;
        private ColumnHeader NameColumnHeader;
        private ColumnHeader columnHeaderID;
        private ListView listView2;
        private ColumnHeader NameColumnHeader1;
        private ColumnHeader columnHeaderID2;
    }
}