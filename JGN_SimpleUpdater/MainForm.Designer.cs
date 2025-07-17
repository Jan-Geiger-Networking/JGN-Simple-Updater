namespace JGN_SimpleUpdater
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            btnStartUpdate = new Button();
            btnExit = new Button();
            btnAbout = new Button();
            progressBar1 = new ProgressBar();
            labelPackageStatus = new Label();
            textBox1 = new TextBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // btnStartUpdate
            // 
            btnStartUpdate.Location = new Point(3, 3);
            btnStartUpdate.Name = "btnStartUpdate";
            btnStartUpdate.Size = new Size(75, 23);
            btnStartUpdate.TabIndex = 0;
            btnStartUpdate.Text = "Update All";
            btnStartUpdate.UseVisualStyleBackColor = false;
            btnStartUpdate.BackColor = Color.LightGreen;
            btnStartUpdate.ForeColor = Color.Black;
            btnStartUpdate.FlatStyle = FlatStyle.Flat;
            btnStartUpdate.Click += btnStartUpdate_Click;
            // 
            // btnExit
            // 
            btnExit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExit.Location = new Point(641, 3);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(75, 23);
            btnExit.TabIndex = 1;
            btnExit.Text = "Exit";
            btnExit.UseVisualStyleBackColor = true;
            btnExit.Click += btnExit_Click;
            // 
            // btnAbout
            // 
            btnAbout.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAbout.Location = new Point(722, 3);
            btnAbout.Name = "btnAbout";
            btnAbout.Size = new Size(75, 23);
            btnAbout.TabIndex = 4;
            btnAbout.Text = "Über";
            btnAbout.UseVisualStyleBackColor = true;
            btnAbout.Click += btnAbout_Click;
            // 
            // progressBar1
            // 
            progressBar1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            progressBar1.Location = new Point(84, 3);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(551, 23);
            progressBar1.TabIndex = 2;
            // 
            // labelPackageStatus
            // 
            labelPackageStatus.Text = "Paketstatus: -";
            labelPackageStatus.Dock = DockStyle.Fill;
            labelPackageStatus.TextAlign = ContentAlignment.MiddleLeft;
            labelPackageStatus.AutoSize = false;
            labelPackageStatus.Height = 24;
            // 
            // textBox1
            // 
            textBox1 = new TextBox();
            textBox1.Dock = DockStyle.Fill;
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.ScrollBars = ScrollBars.Vertical;
            textBox1.Size = new Size(776, 200);
            textBox1.TabIndex = 7;
            textBox1.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            textBox1.MinimumSize = new Size(0, 100);
            textBox1.BorderStyle = BorderStyle.FixedSingle;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 4;
            tableLayoutPanel1.ColumnStyles.Clear();
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Buttons/ProgressBar1
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F)); // labelPackageStatus mit fester Höhe
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // progressBarPackage
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // textBox1 nimmt immer den Rest
            tableLayoutPanel1.Controls.Add(btnStartUpdate, 0, 0);
            tableLayoutPanel1.Controls.Add(progressBar1, 1, 0);
            tableLayoutPanel1.Controls.Add(btnExit, 2, 0);
            tableLayoutPanel1.Controls.Add(btnAbout, 3, 0);
            tableLayoutPanel1.Controls.Add(labelPackageStatus, 0, 1);
            tableLayoutPanel1.SetColumnSpan(labelPackageStatus, 4);
            tableLayoutPanel1.Controls.Add(textBox1, 0, 3);
            tableLayoutPanel1.SetColumnSpan(textBox1, 4);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Size = new Size(800, 450);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tableLayoutPanel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            Text = "JGN Updater (Winget)";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Button btnStartUpdate;
        private Button btnExit;
        private Button btnAbout;
        private ProgressBar progressBar1;
        private Label labelPackageStatus;
        private TextBox textBox1;
        private TableLayoutPanel tableLayoutPanel1;
    }
}
