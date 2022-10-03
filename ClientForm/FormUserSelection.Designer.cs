namespace ClientForm
{
    partial class FormUserSelection
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
            this.NameColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeaderID = new System.Windows.Forms.ColumnHeader();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonSubmit
            // 
            this.buttonSubmit.AutoSize = true;
            this.buttonSubmit.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonSubmit.Location = new System.Drawing.Point(0, 483);
            this.buttonSubmit.Name = "buttonSubmit";
            this.buttonSubmit.Size = new System.Drawing.Size(206, 25);
            this.buttonSubmit.TabIndex = 1;
            this.buttonSubmit.Text = "Подтвердить выбор";
            this.buttonSubmit.UseVisualStyleBackColor = true;
            this.buttonSubmit.Click += new System.EventHandler(this.buttonSubmit_Click);
            // 
            // NameColumnHeader
            // 
            this.NameColumnHeader.Text = "Имя";
            this.NameColumnHeader.Width = 400;
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
            this.listView1.TabIndex = 3;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderID
            // 
            this.columnHeaderID.Width = 0;
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textBox1.Location = new System.Drawing.Point(0, 460);
            this.textBox1.MaxLength = 255;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(206, 23);
            this.textBox1.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label1.Location = new System.Drawing.Point(0, 445);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 15);
            this.label1.TabIndex = 5;
            this.label1.Text = "Введите название чата";
            // 
            // FormUserSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(206, 508);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.buttonSubmit);
            this.Name = "FormUserSelection";
            this.Text = "FormUserSelection";
            this.Load += new System.EventHandler(this.FormUserSelection_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Button buttonSubmit;
        private ColumnHeader NameColumnHeader;
        private ListView listView1;
        private ColumnHeader columnHeaderID;
        private TextBox textBox1;
        private Label label1;
    }
}