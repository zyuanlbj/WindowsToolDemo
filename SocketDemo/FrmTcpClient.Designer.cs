namespace SocketDemo
{
    partial class FrmTcpClient
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_SendFile = new System.Windows.Forms.Button();
            this.btn_Send = new System.Windows.Forms.Button();
            this.btn_Connect = new System.Windows.Forms.Button();
            this.txt_Name = new System.Windows.Forms.TextBox();
            this.txt_Port = new System.Windows.Forms.TextBox();
            this.txt_IP = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_SelectFile = new System.Windows.Forms.Button();
            this.txt_Send = new System.Windows.Forms.TextBox();
            this.txt_Rcv = new System.Windows.Forms.TextBox();
            this.txt_SelectFile = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btn_SendFile
            // 
            this.btn_SendFile.Location = new System.Drawing.Point(428, 362);
            this.btn_SendFile.Name = "btn_SendFile";
            this.btn_SendFile.Size = new System.Drawing.Size(109, 34);
            this.btn_SendFile.TabIndex = 23;
            this.btn_SendFile.Text = "发送文件";
            this.btn_SendFile.UseVisualStyleBackColor = true;
            this.btn_SendFile.Click += new System.EventHandler(this.btn_SendFile_Click);
            // 
            // btn_Send
            // 
            this.btn_Send.Location = new System.Drawing.Point(428, 287);
            this.btn_Send.Name = "btn_Send";
            this.btn_Send.Size = new System.Drawing.Size(109, 34);
            this.btn_Send.TabIndex = 24;
            this.btn_Send.Text = "发送消息";
            this.btn_Send.UseVisualStyleBackColor = true;
            this.btn_Send.Click += new System.EventHandler(this.btn_Send_Click);
            // 
            // btn_Connect
            // 
            this.btn_Connect.Location = new System.Drawing.Point(428, 212);
            this.btn_Connect.Name = "btn_Connect";
            this.btn_Connect.Size = new System.Drawing.Size(109, 34);
            this.btn_Connect.TabIndex = 25;
            this.btn_Connect.Text = "连接服务器";
            this.btn_Connect.UseVisualStyleBackColor = true;
            this.btn_Connect.Click += new System.EventHandler(this.btn_Connect_Click);
            // 
            // txt_Name
            // 
            this.txt_Name.Location = new System.Drawing.Point(489, 146);
            this.txt_Name.Name = "txt_Name";
            this.txt_Name.Size = new System.Drawing.Size(118, 22);
            this.txt_Name.TabIndex = 20;
            this.txt_Name.Text = "远程1号";
            // 
            // txt_Port
            // 
            this.txt_Port.Location = new System.Drawing.Point(489, 96);
            this.txt_Port.Name = "txt_Port";
            this.txt_Port.Size = new System.Drawing.Size(118, 22);
            this.txt_Port.TabIndex = 21;
            this.txt_Port.Text = "1234";
            // 
            // txt_IP
            // 
            this.txt_IP.Location = new System.Drawing.Point(489, 45);
            this.txt_IP.Name = "txt_IP";
            this.txt_IP.Size = new System.Drawing.Size(118, 22);
            this.txt_IP.TabIndex = 22;
            this.txt_IP.Text = "127.0.0.1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(376, 146);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 19);
            this.label3.TabIndex = 17;
            this.label3.Text = "本机名称：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(376, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 19);
            this.label2.TabIndex = 18;
            this.label2.Text = "服务器Port：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(376, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 19);
            this.label1.TabIndex = 19;
            this.label1.Text = "服务器IP：";
            // 
            // btn_SelectFile
            // 
            this.btn_SelectFile.Location = new System.Drawing.Point(281, 389);
            this.btn_SelectFile.Name = "btn_SelectFile";
            this.btn_SelectFile.Size = new System.Drawing.Size(75, 23);
            this.btn_SelectFile.TabIndex = 16;
            this.btn_SelectFile.Text = "选择文件";
            this.btn_SelectFile.UseVisualStyleBackColor = true;
            this.btn_SelectFile.Click += new System.EventHandler(this.btn_SelectFile_Click);
            // 
            // txt_Send
            // 
            this.txt_Send.Location = new System.Drawing.Point(12, 195);
            this.txt_Send.Multiline = true;
            this.txt_Send.Name = "txt_Send";
            this.txt_Send.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_Send.Size = new System.Drawing.Size(348, 178);
            this.txt_Send.TabIndex = 13;
            // 
            // txt_Rcv
            // 
            this.txt_Rcv.Location = new System.Drawing.Point(12, 12);
            this.txt_Rcv.Multiline = true;
            this.txt_Rcv.Name = "txt_Rcv";
            this.txt_Rcv.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_Rcv.Size = new System.Drawing.Size(348, 167);
            this.txt_Rcv.TabIndex = 14;
            // 
            // txt_SelectFile
            // 
            this.txt_SelectFile.Location = new System.Drawing.Point(12, 389);
            this.txt_SelectFile.Name = "txt_SelectFile";
            this.txt_SelectFile.Size = new System.Drawing.Size(254, 22);
            this.txt_SelectFile.TabIndex = 15;
            // 
            // FrmTcpClient
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(621, 425);
            this.Controls.Add(this.btn_SendFile);
            this.Controls.Add(this.btn_Send);
            this.Controls.Add(this.btn_Connect);
            this.Controls.Add(this.txt_Name);
            this.Controls.Add(this.txt_Port);
            this.Controls.Add(this.txt_IP);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_SelectFile);
            this.Controls.Add(this.txt_Send);
            this.Controls.Add(this.txt_Rcv);
            this.Controls.Add(this.txt_SelectFile);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.Name = "FrmTcpClient";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "基于Socket开发的TCP客户端";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_SendFile;
        private System.Windows.Forms.Button btn_Send;
        private System.Windows.Forms.Button btn_Connect;
        private System.Windows.Forms.TextBox txt_Name;
        private System.Windows.Forms.TextBox txt_Port;
        private System.Windows.Forms.TextBox txt_IP;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_SelectFile;
        private System.Windows.Forms.TextBox txt_Send;
        private System.Windows.Forms.TextBox txt_Rcv;
        private System.Windows.Forms.TextBox txt_SelectFile;
    }
}

