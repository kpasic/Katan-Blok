namespace ClientApp
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
            mainGrid = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            btnConnectServer = new Button();
            ActionsLayout = new TableLayoutPanel();
            btnBuildHouse = new Button();
            btnBuildRoad = new Button();
            btnStartServer = new Button();
            mainGrid.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            ActionsLayout.SuspendLayout();
            SuspendLayout();
            // 
            // mainGrid
            // 
            mainGrid.AutoSize = true;
            mainGrid.ColumnCount = 2;
            mainGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            mainGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            mainGrid.Controls.Add(tableLayoutPanel2, 1, 0);
            mainGrid.Controls.Add(ActionsLayout, 1, 1);
            mainGrid.Dock = DockStyle.Fill;
            mainGrid.Location = new Point(0, 0);
            mainGrid.Margin = new Padding(0);
            mainGrid.MinimumSize = new Size(10, 10);
            mainGrid.Name = "mainGrid";
            mainGrid.RowCount = 2;
            mainGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 80F));
            mainGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            mainGrid.Size = new Size(765, 628);
            mainGrid.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Controls.Add(btnConnectServer, 0, 0);
            tableLayoutPanel2.Controls.Add(btnStartServer, 0, 1);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(538, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 2;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 80F));
            tableLayoutPanel2.Size = new Size(224, 496);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // btnConnectServer
            // 
            btnConnectServer.Dock = DockStyle.Left;
            btnConnectServer.Location = new Point(3, 3);
            btnConnectServer.Name = "btnConnectServer";
            btnConnectServer.Size = new Size(99, 93);
            btnConnectServer.TabIndex = 0;
            btnConnectServer.Text = "Connect to server";
            btnConnectServer.UseVisualStyleBackColor = true;
            btnConnectServer.Click += btnConnectServer_Click;
            // 
            // ActionsLayout
            // 
            ActionsLayout.ColumnCount = 2;
            ActionsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            ActionsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            ActionsLayout.Controls.Add(btnBuildHouse, 0, 0);
            ActionsLayout.Controls.Add(btnBuildRoad, 1, 0);
            ActionsLayout.Dock = DockStyle.Fill;
            ActionsLayout.Location = new Point(538, 505);
            ActionsLayout.Name = "ActionsLayout";
            ActionsLayout.RowCount = 2;
            ActionsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            ActionsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            ActionsLayout.Size = new Size(224, 120);
            ActionsLayout.TabIndex = 1;
            // 
            // btnBuildHouse
            // 
            btnBuildHouse.Dock = DockStyle.Fill;
            btnBuildHouse.Location = new Point(5, 5);
            btnBuildHouse.Margin = new Padding(5);
            btnBuildHouse.Name = "btnBuildHouse";
            btnBuildHouse.Size = new Size(102, 50);
            btnBuildHouse.TabIndex = 0;
            btnBuildHouse.Text = "Build House";
            btnBuildHouse.UseVisualStyleBackColor = true;
            btnBuildHouse.Click += btnBuildHouse_Click;
            // 
            // btnBuildRoad
            // 
            btnBuildRoad.Dock = DockStyle.Fill;
            btnBuildRoad.Location = new Point(117, 5);
            btnBuildRoad.Margin = new Padding(5);
            btnBuildRoad.Name = "btnBuildRoad";
            btnBuildRoad.Size = new Size(102, 50);
            btnBuildRoad.TabIndex = 1;
            btnBuildRoad.Text = "Build Road";
            btnBuildRoad.UseVisualStyleBackColor = true;
            // 
            // btnStartServer
            // 
            btnStartServer.Location = new Point(3, 102);
            btnStartServer.Name = "btnStartServer";
            btnStartServer.Size = new Size(146, 122);
            btnStartServer.TabIndex = 1;
            btnStartServer.Text = "button1";
            btnStartServer.UseVisualStyleBackColor = true;
            btnStartServer.Click += btnStartServer_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(765, 628);
            Controls.Add(mainGrid);
            DoubleBuffered = true;
            Name = "MainForm";
            Text = "Form1";
            Paint += MainForm_Paint;
            mainGrid.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            ActionsLayout.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanel mainGrid;
        private TableLayoutPanel tableLayoutPanel2;
        private Button btnConnectServer;
        private TableLayoutPanel ActionsLayout;
        private Button btnBuildHouse;
        private Button btnBuildRoad;
        private Button btnStartServer;
    }
}
