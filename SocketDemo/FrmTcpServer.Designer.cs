namespace SocketDemo
{
    partial class FrmTcpServer
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
            this.btn_SendToAll = new System.Windows.Forms.Button();
            this.btn_SendToSingle = new System.Windows.Forms.Button();
            this.btn_StartServer = new System.Windows.Forms.Button();
            this.btn_Client = new System.Windows.Forms.Button();
            this.btn_SendFile = new System.Windows.Forms.Button();
            this.btn_SelectFile = new System.Windows.Forms.Button();
            this.lbOnline = new System.Windows.Forms.ListBox();
            this.txt_Send = new System.Windows.Forms.TextBox();
            this.txt_Rcv = new System.Windows.Forms.TextBox();
            this.txt_SelectFile = new System.Windows.Forms.TextBox();
            this.txt_Port = new System.Windows.Forms.TextBox();
            this.txt_IP = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btn_SendToAll
            // 
            this.btn_SendToAll.Location = new System.Drawing.Point(433, 287);
            this.btn_SendToAll.Name = "btn_SendToAll";
            this.btn_SendToAll.Size = new System.Drawing.Size(105, 33);
            this.btn_SendToAll.TabIndex = 13;
            this.btn_SendToAll.Text = "群发消息";
            this.btn_SendToAll.UseVisualStyleBackColor = true;
            this.btn_SendToAll.Click += new System.EventHandler(this.btn_SendToAll_Click);
            // 
            // btn_SendToSingle
            // 
            this.btn_SendToSingle.Location = new System.Drawing.Point(433, 240);
            this.btn_SendToSingle.Name = "btn_SendToSingle";
            this.btn_SendToSingle.Size = new System.Drawing.Size(105, 33);
            this.btn_SendToSingle.TabIndex = 14;
            this.btn_SendToSingle.Text = "发送消息";
            this.btn_SendToSingle.UseVisualStyleBackColor = true;
            this.btn_SendToSingle.Click += new System.EventHandler(this.btn_SendToSingle_Click);
            // 
            // btn_StartServer
            // 
            this.btn_StartServer.Location = new System.Drawing.Point(433, 193);
            this.btn_StartServer.Name = "btn_StartServer";
            this.btn_StartServer.Size = new System.Drawing.Size(105, 33);
            this.btn_StartServer.TabIndex = 15;
            this.btn_StartServer.Text = "启动服务";
            this.btn_StartServer.UseVisualStyleBackColor = true;
            this.btn_StartServer.Click += new System.EventHandler(this.btn_StartServer_Click);
            // 
            // btn_Client
            // 
            this.btn_Client.Location = new System.Drawing.Point(433, 334);
            this.btn_Client.Name = "btn_Client";
            this.btn_Client.Size = new System.Drawing.Size(105, 33);
            this.btn_Client.TabIndex = 16;
            this.btn_Client.Text = "客户端";
            this.btn_Client.UseVisualStyleBackColor = true;
            this.btn_Client.Click += new System.EventHandler(this.btn_Client_Click);
            // 
            // btn_SendFile
            // 
            this.btn_SendFile.Location = new System.Drawing.Point(433, 381);
            this.btn_SendFile.Name = "btn_SendFile";
            this.btn_SendFile.Size = new System.Drawing.Size(105, 33);
            this.btn_SendFile.TabIndex = 17;
            this.btn_SendFile.Text = "发送文件";
            this.btn_SendFile.UseVisualStyleBackColor = true;
            this.btn_SendFile.Click += new System.EventHandler(this.btn_SendFile_Click);
            // 
            // btn_SelectFile
            // 
            this.btn_SelectFile.Location = new System.Drawing.Point(272, 389);
            this.btn_SelectFile.Name = "btn_SelectFile";
            this.btn_SelectFile.Size = new System.Drawing.Size(75, 23);
            this.btn_SelectFile.TabIndex = 18;
            this.btn_SelectFile.Text = "选择文件";
            this.btn_SelectFile.UseVisualStyleBackColor = true;
            this.btn_SelectFile.Click += new System.EventHandler(this.btn_SelectFile_Click);
            // 
            // lbOnline
            // 
            this.lbOnline.FormattingEnabled = true;
            this.lbOnline.ItemHeight = 14;
            this.lbOnline.Location = new System.Drawing.Point(382, 107);
            this.lbOnline.Name = "lbOnline";
            this.lbOnline.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lbOnline.Size = new System.Drawing.Size(202, 60);
            this.lbOnline.TabIndex = 12;
            // 
            // txt_Send
            // 
            this.txt_Send.Location = new System.Drawing.Point(12, 195);
            this.txt_Send.Multiline = true;
            this.txt_Send.Name = "txt_Send";
            this.txt_Send.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_Send.Size = new System.Drawing.Size(348, 178);
            this.txt_Send.TabIndex = 7;
            // 
            // txt_Rcv
            // 
            this.txt_Rcv.Location = new System.Drawing.Point(12, 12);
            this.txt_Rcv.Multiline = true;
            this.txt_Rcv.Name = "txt_Rcv";
            this.txt_Rcv.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_Rcv.Size = new System.Drawing.Size(348, 167);
            this.txt_Rcv.TabIndex = 8;
            // 
            // txt_SelectFile
            // 
            this.txt_SelectFile.Location = new System.Drawing.Point(12, 389);
            this.txt_SelectFile.Name = "txt_SelectFile";
            this.txt_SelectFile.Size = new System.Drawing.Size(254, 22);
            this.txt_SelectFile.TabIndex = 9;
            // 
            // txt_Port
            // 
            this.txt_Port.Location = new System.Drawing.Point(466, 49);
            this.txt_Port.Name = "txt_Port";
            this.txt_Port.Size = new System.Drawing.Size(118, 22);
            this.txt_Port.TabIndex = 10;
            this.txt_Port.Text = "1234";
            // 
            // txt_IP
            // 
            this.txt_IP.Location = new System.Drawing.Point(466, 20);
            this.txt_IP.Name = "txt_IP";
            this.txt_IP.Size = new System.Drawing.Size(118, 22);
            this.txt_IP.TabIndex = 11;
            this.txt_IP.Text = "127.0.0.1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(380, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 19);
            this.label3.TabIndex = 4;
            this.label3.Text = "在线列表：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(380, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 19);
            this.label2.TabIndex = 5;
            this.label2.Text = "端口号：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(380, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 19);
            this.label1.TabIndex = 6;
            this.label1.Text = "本机IP地址：";
            // 
            // FrmTcpServer
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(621, 425);
            this.Controls.Add(this.btn_SendToAll);
            this.Controls.Add(this.btn_SendToSingle);
            this.Controls.Add(this.btn_StartServer);
            this.Controls.Add(this.btn_Client);
            this.Controls.Add(this.btn_SendFile);
            this.Controls.Add(this.btn_SelectFile);
            this.Controls.Add(this.lbOnline);
            this.Controls.Add(this.txt_Send);
            this.Controls.Add(this.txt_Rcv);
            this.Controls.Add(this.txt_SelectFile);
            this.Controls.Add(this.txt_Port);
            this.Controls.Add(this.txt_IP);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.Name = "FrmTcpServer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "基于Socket开发的TCP服务器";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_SendToAll;
        private System.Windows.Forms.Button btn_SendToSingle;
        private System.Windows.Forms.Button btn_StartServer;
        private System.Windows.Forms.Button btn_Client;
        private System.Windows.Forms.Button btn_SendFile;
        private System.Windows.Forms.Button btn_SelectFile;
        private System.Windows.Forms.ListBox lbOnline;
        private System.Windows.Forms.TextBox txt_Send;
        private System.Windows.Forms.TextBox txt_Rcv;
        private System.Windows.Forms.TextBox txt_SelectFile;
        private System.Windows.Forms.TextBox txt_Port;
        private System.Windows.Forms.TextBox txt_IP;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}