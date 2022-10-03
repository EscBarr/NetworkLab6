namespace ClientForm
{
    partial class Form1
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.подключенияToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ConnectToServer = new System.Windows.Forms.ToolStripMenuItem();
            this.DisconnectFromServer = new System.Windows.Forms.ToolStripMenuItem();
            this.CreateChatMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GroupChatMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PersonalChatMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listView1 = new System.Windows.Forms.ListView();
            this.NameColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage0 = new System.Windows.Forms.TabPage();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.Sendbutton = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.подключенияToolStripMenuItem,
            this.CreateChatMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(1029, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // подключенияToolStripMenuItem
            // 
            this.подключенияToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ConnectToServer,
            this.DisconnectFromServer});
            this.подключенияToolStripMenuItem.Name = "подключенияToolStripMenuItem";
            this.подключенияToolStripMenuItem.Size = new System.Drawing.Size(96, 19);
            this.подключенияToolStripMenuItem.Text = "Подключения";
            // 
            // ConnectToServer
            // 
            this.ConnectToServer.Name = "ConnectToServer";
            this.ConnectToServer.Size = new System.Drawing.Size(291, 22);
            this.ConnectToServer.Text = "Подключиться";
            this.ConnectToServer.Click += new System.EventHandler(this.ConnectToServer_Click);
            // 
            // DisconnectFromServer
            // 
            this.DisconnectFromServer.Enabled = false;
            this.DisconnectFromServer.Name = "DisconnectFromServer";
            this.DisconnectFromServer.Size = new System.Drawing.Size(291, 22);
            this.DisconnectFromServer.Text = "Отключиться от текущего сервера";
            this.DisconnectFromServer.Click += new System.EventHandler(this.DisconnectFromServer_Click);
            // 
            // CreateChatMenuItem
            // 
            this.CreateChatMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.GroupChatMenuItem,
            this.PersonalChatMenuItem});
            this.CreateChatMenuItem.Enabled = false;
            this.CreateChatMenuItem.Name = "CreateChatMenuItem";
            this.CreateChatMenuItem.Size = new System.Drawing.Size(96, 19);
            this.CreateChatMenuItem.Text = "Создать чат";
            // 
            // GroupChatMenuItem
            // 
            this.GroupChatMenuItem.Name = "GroupChatMenuItem";
            this.GroupChatMenuItem.Size = new System.Drawing.Size(137, 22);
            this.GroupChatMenuItem.Text = "Групповой";
            this.GroupChatMenuItem.Click += new System.EventHandler(this.GroupChatMenuItem_Click);
            // 
            // PersonalChatMenuItem
            // 
            this.PersonalChatMenuItem.Name = "PersonalChatMenuItem";
            this.PersonalChatMenuItem.Size = new System.Drawing.Size(137, 22);
            this.PersonalChatMenuItem.Text = "Личный";
            // 
            // listView1
            // 
            this.listView1.BackgroundImageTiled = true;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.NameColumnHeader});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(0, 19);
            this.listView1.Margin = new System.Windows.Forms.Padding(4);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(315, 617);
            this.listView1.TabIndex = 2;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // NameColumnHeader
            // 
            this.NameColumnHeader.Text = "Имя";
            this.NameColumnHeader.Width = 400;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(270, 19);
            this.label1.TabIndex = 3;
            this.label1.Text = "Список пользователей в канале";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage0);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(698, 605);
            this.tabControl1.TabIndex = 4;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            this.tabControl1.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl1_Selecting);
            this.tabControl1.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl1_Selected);
            // 
            // tabPage0
            // 
            this.tabPage0.Location = new System.Drawing.Point(4, 28);
            this.tabPage0.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage0.Name = "tabPage0";
            this.tabPage0.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage0.Size = new System.Drawing.Size(690, 573);
            this.tabPage0.TabIndex = 0;
            this.tabPage0.Text = "Главный";
            this.tabPage0.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.AllowDrop = true;
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textBox1.Location = new System.Drawing.Point(0, 605);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(698, 31);
            this.textBox1.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.listView1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(4, 4);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(315, 636);
            this.panel1.TabIndex = 9;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 31.46667F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 68.53333F));
            this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 25);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1029, 644);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.Sendbutton);
            this.panel2.Controls.Add(this.tabControl1);
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(327, 4);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(698, 636);
            this.panel2.TabIndex = 10;
            // 
            // Sendbutton
            // 
            this.Sendbutton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Sendbutton.Enabled = false;
            this.Sendbutton.Location = new System.Drawing.Point(0, 577);
            this.Sendbutton.Name = "Sendbutton";
            this.Sendbutton.Size = new System.Drawing.Size(698, 28);
            this.Sendbutton.TabIndex = 6;
            this.Sendbutton.Text = "Отправить";
            this.Sendbutton.UseVisualStyleBackColor = true;
            this.Sendbutton.Click += new System.EventHandler(this.Sendbutton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1029, 669);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(1045, 609);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem подключенияToolStripMenuItem;
        private ToolStripMenuItem ConnectToServer;
        private ToolStripMenuItem DisconnectFromServer;
        private ListView listView1;
        private Label label1;
        private TabControl tabControl1;
        private TabPage tabPage0;
        private TextBox textBox1;
        private ColumnHeader NameColumnHeader;
        private TableLayoutPanel tableLayoutPanel1;
        private Panel panel1;
        private Panel panel2;
        private ToolStripMenuItem CreateChatMenuItem;
        private ToolStripMenuItem GroupChatMenuItem;
        private ToolStripMenuItem PersonalChatMenuItem;
        private Button Sendbutton;
    }
}