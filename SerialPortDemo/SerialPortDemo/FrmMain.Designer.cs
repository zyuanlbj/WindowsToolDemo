namespace SerialPortDemo
{
    partial class FrmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.grbPort = new System.Windows.Forms.GroupBox();
            this.btnOpen = new System.Windows.Forms.Button();
            this.chkDtr = new System.Windows.Forms.CheckBox();
            this.chkRts = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cbbStop = new System.Windows.Forms.ComboBox();
            this.cbbData = new System.Windows.Forms.ComboBox();
            this.cbbParity = new System.Windows.Forms.ComboBox();
            this.cbbBaud = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbbPort = new System.Windows.Forms.ComboBox();
            this.grbReceive = new System.Windows.Forms.GroupBox();
            this.txtSavePath = new System.Windows.Forms.TextBox();
            this.btnSelectPath = new System.Windows.Forms.Button();
            this.btnSaveFile = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnHandClear = new System.Windows.Forms.Button();
            this.chkHexReceive = new System.Windows.Forms.CheckBox();
            this.chkAutoClear = new System.Windows.Forms.CheckBox();
            this.grbSend = new System.Windows.Forms.GroupBox();
            this.txtPeriod = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtSendPath = new System.Windows.Forms.TextBox();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.btnSendFile = new System.Windows.Forms.Button();
            this.btnClearSend = new System.Windows.Forms.Button();
            this.btnHandSend = new System.Windows.Forms.Button();
            this.chkHexSend = new System.Windows.Forms.CheckBox();
            this.chkAutoSend = new System.Windows.Forms.CheckBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslSendCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslReceiveCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslClear = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsmiEncode = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMore = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rtbReceive = new System.Windows.Forms.RichTextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rtbSend = new System.Windows.Forms.RichTextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnQuickCmd4 = new System.Windows.Forms.Button();
            this.btnQuickCmd3 = new System.Windows.Forms.Button();
            this.btnQuickCmd2 = new System.Windows.Forms.Button();
            this.btnQuickCmd1 = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnSendCommand = new System.Windows.Forms.Button();
            this.txtTail = new System.Windows.Forms.TextBox();
            this.cbbParityMethod = new System.Windows.Forms.ComboBox();
            this.txtData = new System.Windows.Forms.TextBox();
            this.chkTail = new System.Windows.Forms.CheckBox();
            this.chkData = new System.Windows.Forms.CheckBox();
            this.txtHead = new System.Windows.Forms.TextBox();
            this.chkParity = new System.Windows.Forms.CheckBox();
            this.chkHead = new System.Windows.Forms.CheckBox();
            this.aSCIIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.defaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uTF8ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unicodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gB3212ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grbPort.SuspendLayout();
            this.grbReceive.SuspendLayout();
            this.grbSend.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // grbPort
            // 
            this.grbPort.Controls.Add(this.btnOpen);
            this.grbPort.Controls.Add(this.chkDtr);
            this.grbPort.Controls.Add(this.chkRts);
            this.grbPort.Controls.Add(this.label5);
            this.grbPort.Controls.Add(this.cbbStop);
            this.grbPort.Controls.Add(this.cbbData);
            this.grbPort.Controls.Add(this.cbbParity);
            this.grbPort.Controls.Add(this.cbbBaud);
            this.grbPort.Controls.Add(this.label4);
            this.grbPort.Controls.Add(this.label3);
            this.grbPort.Controls.Add(this.label2);
            this.grbPort.Controls.Add(this.label1);
            this.grbPort.Controls.Add(this.cbbPort);
            this.grbPort.Location = new System.Drawing.Point(19, 12);
            this.grbPort.Name = "grbPort";
            this.grbPort.Size = new System.Drawing.Size(217, 275);
            this.grbPort.TabIndex = 0;
            this.grbPort.TabStop = false;
            this.grbPort.Text = "串口配置";
            // 
            // btnOpen
            // 
            this.btnOpen.Image = global::SerialPortDemo.Properties.Resources.poweroff;
            this.btnOpen.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOpen.Location = new System.Drawing.Point(100, 214);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(100, 42);
            this.btnOpen.TabIndex = 7;
            this.btnOpen.Text = "打开串口";
            this.btnOpen.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // chkDtr
            // 
            this.chkDtr.AutoSize = true;
            this.chkDtr.Location = new System.Drawing.Point(24, 239);
            this.chkDtr.Name = "chkDtr";
            this.chkDtr.Size = new System.Drawing.Size(56, 24);
            this.chkDtr.TabIndex = 6;
            this.chkDtr.Text = "DTR";
            this.chkDtr.UseVisualStyleBackColor = true;
            this.chkDtr.CheckedChanged += new System.EventHandler(this.chkDtr_CheckedChanged);
            // 
            // chkRts
            // 
            this.chkRts.AutoSize = true;
            this.chkRts.Location = new System.Drawing.Point(24, 209);
            this.chkRts.Name = "chkRts";
            this.chkRts.Size = new System.Drawing.Size(53, 24);
            this.chkRts.TabIndex = 5;
            this.chkRts.Text = "RTS";
            this.chkRts.UseVisualStyleBackColor = true;
            this.chkRts.CheckedChanged += new System.EventHandler(this.chkRts_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(26, 177);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 20);
            this.label5.TabIndex = 4;
            this.label5.Text = "停止位";
            // 
            // cbbStop
            // 
            this.cbbStop.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbStop.FormattingEnabled = true;
            this.cbbStop.Location = new System.Drawing.Point(100, 174);
            this.cbbStop.Name = "cbbStop";
            this.cbbStop.Size = new System.Drawing.Size(100, 27);
            this.cbbStop.TabIndex = 3;
            // 
            // cbbData
            // 
            this.cbbData.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbData.FormattingEnabled = true;
            this.cbbData.Location = new System.Drawing.Point(100, 138);
            this.cbbData.Name = "cbbData";
            this.cbbData.Size = new System.Drawing.Size(100, 27);
            this.cbbData.TabIndex = 3;
            // 
            // cbbParity
            // 
            this.cbbParity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbParity.FormattingEnabled = true;
            this.cbbParity.Location = new System.Drawing.Point(100, 101);
            this.cbbParity.Name = "cbbParity";
            this.cbbParity.Size = new System.Drawing.Size(100, 27);
            this.cbbParity.TabIndex = 3;
            // 
            // cbbBaud
            // 
            this.cbbBaud.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbBaud.FormattingEnabled = true;
            this.cbbBaud.Location = new System.Drawing.Point(100, 66);
            this.cbbBaud.Name = "cbbBaud";
            this.cbbBaud.Size = new System.Drawing.Size(100, 27);
            this.cbbBaud.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(26, 141);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 20);
            this.label4.TabIndex = 4;
            this.label4.Text = "数据位";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "校验位";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "波特率";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "端口号";
            // 
            // cbbPort
            // 
            this.cbbPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbPort.FormattingEnabled = true;
            this.cbbPort.Location = new System.Drawing.Point(100, 30);
            this.cbbPort.Name = "cbbPort";
            this.cbbPort.Size = new System.Drawing.Size(100, 27);
            this.cbbPort.TabIndex = 0;
            // 
            // grbReceive
            // 
            this.grbReceive.Controls.Add(this.txtSavePath);
            this.grbReceive.Controls.Add(this.btnSelectPath);
            this.grbReceive.Controls.Add(this.btnSaveFile);
            this.grbReceive.Controls.Add(this.btnPause);
            this.grbReceive.Controls.Add(this.btnHandClear);
            this.grbReceive.Controls.Add(this.chkHexReceive);
            this.grbReceive.Controls.Add(this.chkAutoClear);
            this.grbReceive.Location = new System.Drawing.Point(19, 294);
            this.grbReceive.Name = "grbReceive";
            this.grbReceive.Size = new System.Drawing.Size(217, 183);
            this.grbReceive.TabIndex = 1;
            this.grbReceive.TabStop = false;
            this.grbReceive.Text = "接收配置";
            // 
            // txtSavePath
            // 
            this.txtSavePath.Location = new System.Drawing.Point(24, 148);
            this.txtSavePath.Name = "txtSavePath";
            this.txtSavePath.ReadOnly = true;
            this.txtSavePath.Size = new System.Drawing.Size(176, 25);
            this.txtSavePath.TabIndex = 2;
            // 
            // btnSelectPath
            // 
            this.btnSelectPath.Location = new System.Drawing.Point(24, 108);
            this.btnSelectPath.Name = "btnSelectPath";
            this.btnSelectPath.Size = new System.Drawing.Size(80, 33);
            this.btnSelectPath.TabIndex = 1;
            this.btnSelectPath.Text = "选择路径";
            this.btnSelectPath.UseVisualStyleBackColor = true;
            this.btnSelectPath.Click += new System.EventHandler(this.btnSelectPath_Click);
            // 
            // btnSaveFile
            // 
            this.btnSaveFile.Location = new System.Drawing.Point(120, 108);
            this.btnSaveFile.Name = "btnSaveFile";
            this.btnSaveFile.Size = new System.Drawing.Size(80, 33);
            this.btnSaveFile.TabIndex = 1;
            this.btnSaveFile.Text = "保存数据";
            this.btnSaveFile.UseVisualStyleBackColor = true;
            this.btnSaveFile.Click += new System.EventHandler(this.btnSaveFile_Click);
            // 
            // btnPause
            // 
            this.btnPause.Location = new System.Drawing.Point(120, 69);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(80, 33);
            this.btnPause.TabIndex = 1;
            this.btnPause.Text = "暂停接收";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnHandClear
            // 
            this.btnHandClear.Location = new System.Drawing.Point(120, 30);
            this.btnHandClear.Name = "btnHandClear";
            this.btnHandClear.Size = new System.Drawing.Size(80, 33);
            this.btnHandClear.TabIndex = 1;
            this.btnHandClear.Text = "手动清空";
            this.btnHandClear.UseVisualStyleBackColor = true;
            this.btnHandClear.Click += new System.EventHandler(this.btnHandClear_Click);
            // 
            // chkHexReceive
            // 
            this.chkHexReceive.AutoSize = true;
            this.chkHexReceive.Location = new System.Drawing.Point(24, 74);
            this.chkHexReceive.Name = "chkHexReceive";
            this.chkHexReceive.Size = new System.Drawing.Size(84, 24);
            this.chkHexReceive.TabIndex = 0;
            this.chkHexReceive.Text = "十六进制";
            this.chkHexReceive.UseVisualStyleBackColor = true;
            // 
            // chkAutoClear
            // 
            this.chkAutoClear.AutoSize = true;
            this.chkAutoClear.Location = new System.Drawing.Point(24, 35);
            this.chkAutoClear.Name = "chkAutoClear";
            this.chkAutoClear.Size = new System.Drawing.Size(84, 24);
            this.chkAutoClear.TabIndex = 0;
            this.chkAutoClear.Text = "自动清空";
            this.chkAutoClear.UseVisualStyleBackColor = true;
            // 
            // grbSend
            // 
            this.grbSend.Controls.Add(this.txtPeriod);
            this.grbSend.Controls.Add(this.label6);
            this.grbSend.Controls.Add(this.txtSendPath);
            this.grbSend.Controls.Add(this.btnOpenFile);
            this.grbSend.Controls.Add(this.btnSendFile);
            this.grbSend.Controls.Add(this.btnClearSend);
            this.grbSend.Controls.Add(this.btnHandSend);
            this.grbSend.Controls.Add(this.chkHexSend);
            this.grbSend.Controls.Add(this.chkAutoSend);
            this.grbSend.Location = new System.Drawing.Point(19, 483);
            this.grbSend.Name = "grbSend";
            this.grbSend.Size = new System.Drawing.Size(217, 215);
            this.grbSend.TabIndex = 3;
            this.grbSend.TabStop = false;
            this.grbSend.Text = "发送配置";
            // 
            // txtPeriod
            // 
            this.txtPeriod.Location = new System.Drawing.Point(145, 180);
            this.txtPeriod.Name = "txtPeriod";
            this.txtPeriod.Size = new System.Drawing.Size(55, 25);
            this.txtPeriod.TabIndex = 4;
            this.txtPeriod.Text = "1000";
            this.txtPeriod.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(20, 182);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(136, 20);
            this.label6.TabIndex = 3;
            this.label6.Text = "自动发送周期(ms)：";
            // 
            // txtSendPath
            // 
            this.txtSendPath.Location = new System.Drawing.Point(24, 147);
            this.txtSendPath.Name = "txtSendPath";
            this.txtSendPath.ReadOnly = true;
            this.txtSendPath.Size = new System.Drawing.Size(176, 25);
            this.txtSendPath.TabIndex = 2;
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Location = new System.Drawing.Point(24, 108);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(80, 33);
            this.btnOpenFile.TabIndex = 1;
            this.btnOpenFile.Text = "打开文件";
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // btnSendFile
            // 
            this.btnSendFile.Location = new System.Drawing.Point(120, 108);
            this.btnSendFile.Name = "btnSendFile";
            this.btnSendFile.Size = new System.Drawing.Size(80, 33);
            this.btnSendFile.TabIndex = 1;
            this.btnSendFile.Text = "发送文件";
            this.btnSendFile.UseVisualStyleBackColor = true;
            this.btnSendFile.Click += new System.EventHandler(this.btnSendFile_Click);
            // 
            // btnClearSend
            // 
            this.btnClearSend.Location = new System.Drawing.Point(120, 69);
            this.btnClearSend.Name = "btnClearSend";
            this.btnClearSend.Size = new System.Drawing.Size(80, 33);
            this.btnClearSend.TabIndex = 1;
            this.btnClearSend.Text = "清空发送";
            this.btnClearSend.UseVisualStyleBackColor = true;
            this.btnClearSend.Click += new System.EventHandler(this.btnClearSend_Click);
            // 
            // btnHandSend
            // 
            this.btnHandSend.Location = new System.Drawing.Point(120, 30);
            this.btnHandSend.Name = "btnHandSend";
            this.btnHandSend.Size = new System.Drawing.Size(80, 33);
            this.btnHandSend.TabIndex = 1;
            this.btnHandSend.Text = "手动发送";
            this.btnHandSend.UseVisualStyleBackColor = true;
            this.btnHandSend.Click += new System.EventHandler(this.btnHandSend_Click);
            // 
            // chkHexSend
            // 
            this.chkHexSend.AutoSize = true;
            this.chkHexSend.Location = new System.Drawing.Point(24, 74);
            this.chkHexSend.Name = "chkHexSend";
            this.chkHexSend.Size = new System.Drawing.Size(84, 24);
            this.chkHexSend.TabIndex = 0;
            this.chkHexSend.Text = "十六进制";
            this.chkHexSend.UseVisualStyleBackColor = true;
            // 
            // chkAutoSend
            // 
            this.chkAutoSend.AutoSize = true;
            this.chkAutoSend.Location = new System.Drawing.Point(24, 35);
            this.chkAutoSend.Name = "chkAutoSend";
            this.chkAutoSend.Size = new System.Drawing.Size(84, 24);
            this.chkAutoSend.TabIndex = 0;
            this.chkAutoSend.Text = "自动发送";
            this.chkAutoSend.UseVisualStyleBackColor = true;
            this.chkAutoSend.CheckedChanged += new System.EventHandler(this.ChkAutoSend_CheckedChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.tsslStatus,
            this.toolStripStatusLabel2,
            this.tsslSendCount,
            this.toolStripStatusLabel3,
            this.tsslReceiveCount,
            this.tsslClear,
            this.toolStripDropDownButton1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 703);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(776, 25);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(83, 20);
            this.toolStripStatusLabel1.Text = "系统工作状态:";
            // 
            // tsslStatus
            // 
            this.tsslStatus.AutoSize = false;
            this.tsslStatus.Name = "tsslStatus";
            this.tsslStatus.Size = new System.Drawing.Size(230, 20);
            this.tsslStatus.Text = "初始化正常！";
            this.tsslStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(83, 20);
            this.toolStripStatusLabel2.Text = "发送字节计数:";
            // 
            // tsslSendCount
            // 
            this.tsslSendCount.AutoSize = false;
            this.tsslSendCount.Name = "tsslSendCount";
            this.tsslSendCount.Size = new System.Drawing.Size(60, 20);
            this.tsslSendCount.Text = "0";
            this.tsslSendCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(83, 20);
            this.toolStripStatusLabel3.Text = "接收字节计数:";
            // 
            // tsslReceiveCount
            // 
            this.tsslReceiveCount.AutoSize = false;
            this.tsslReceiveCount.Name = "tsslReceiveCount";
            this.tsslReceiveCount.Size = new System.Drawing.Size(60, 20);
            this.tsslReceiveCount.Text = "0";
            this.tsslReceiveCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tsslClear
            // 
            this.tsslClear.AutoSize = false;
            this.tsslClear.Name = "tsslClear";
            this.tsslClear.Size = new System.Drawing.Size(70, 20);
            this.tsslClear.Text = "清空计数";
            this.tsslClear.Click += new System.EventHandler(this.tsslClear_Click);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.AutoSize = false;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiEncode,
            this.tsmiMore,
            this.tsmiAbout});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(90, 23);
            this.toolStripDropDownButton1.Text = "更多操作";
            this.toolStripDropDownButton1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStripDropDownButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            // 
            // tsmiEncode
            // 
            this.tsmiEncode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aSCIIToolStripMenuItem,
            this.defaultToolStripMenuItem,
            this.uTF8ToolStripMenuItem,
            this.unicodeToolStripMenuItem,
            this.gB3212ToolStripMenuItem});
            this.tsmiEncode.Image = global::SerialPortDemo.Properties.Resources.settings;
            this.tsmiEncode.Name = "tsmiEncode";
            this.tsmiEncode.Size = new System.Drawing.Size(180, 22);
            this.tsmiEncode.Text = "编码设置";
            // 
            // tsmiMore
            // 
            this.tsmiMore.Image = global::SerialPortDemo.Properties.Resources.diamond;
            this.tsmiMore.Name = "tsmiMore";
            this.tsmiMore.Size = new System.Drawing.Size(180, 22);
            this.tsmiMore.Text = "更多学习";
            this.tsmiMore.Click += new System.EventHandler(this.tsmiMore_Click);
            // 
            // tsmiAbout
            // 
            this.tsmiAbout.Image = global::SerialPortDemo.Properties.Resources.info;
            this.tsmiAbout.Name = "tsmiAbout";
            this.tsmiAbout.Size = new System.Drawing.Size(180, 22);
            this.tsmiAbout.Text = "关于我们";
            this.tsmiAbout.Click += new System.EventHandler(this.tsmiAbout_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rtbReceive);
            this.groupBox1.Location = new System.Drawing.Point(253, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(504, 275);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "接收区";
            // 
            // rtbReceive
            // 
            this.rtbReceive.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.rtbReceive.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbReceive.ForeColor = System.Drawing.Color.Lime;
            this.rtbReceive.Location = new System.Drawing.Point(3, 21);
            this.rtbReceive.Name = "rtbReceive";
            this.rtbReceive.ReadOnly = true;
            this.rtbReceive.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtbReceive.Size = new System.Drawing.Size(498, 251);
            this.rtbReceive.TabIndex = 0;
            this.rtbReceive.Text = "";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rtbSend);
            this.groupBox2.Location = new System.Drawing.Point(253, 294);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(504, 181);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "发送区";
            // 
            // rtbSend
            // 
            this.rtbSend.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.rtbSend.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbSend.ForeColor = System.Drawing.Color.Lime;
            this.rtbSend.Location = new System.Drawing.Point(3, 21);
            this.rtbSend.Name = "rtbSend";
            this.rtbSend.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtbSend.Size = new System.Drawing.Size(498, 157);
            this.rtbSend.TabIndex = 0;
            this.rtbSend.Text = "";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnQuickCmd4);
            this.groupBox3.Controls.Add(this.btnQuickCmd3);
            this.groupBox3.Controls.Add(this.btnQuickCmd2);
            this.groupBox3.Controls.Add(this.btnQuickCmd1);
            this.groupBox3.Location = new System.Drawing.Point(253, 483);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(504, 82);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "快捷命令（Hex）";
            // 
            // btnQuickCmd4
            // 
            this.btnQuickCmd4.Location = new System.Drawing.Point(396, 31);
            this.btnQuickCmd4.Name = "btnQuickCmd4";
            this.btnQuickCmd4.Size = new System.Drawing.Size(90, 30);
            this.btnQuickCmd4.TabIndex = 0;
            this.btnQuickCmd4.Text = "快捷命令04";
            this.btnQuickCmd4.UseVisualStyleBackColor = true;
            this.btnQuickCmd4.Click += new System.EventHandler(this.btnQuickCmd_Click);
            this.btnQuickCmd4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnQuickCmd_MouseDown);
            // 
            // btnQuickCmd3
            // 
            this.btnQuickCmd3.Location = new System.Drawing.Point(270, 31);
            this.btnQuickCmd3.Name = "btnQuickCmd3";
            this.btnQuickCmd3.Size = new System.Drawing.Size(90, 30);
            this.btnQuickCmd3.TabIndex = 0;
            this.btnQuickCmd3.Text = "快捷命令03";
            this.btnQuickCmd3.UseVisualStyleBackColor = true;
            this.btnQuickCmd3.Click += new System.EventHandler(this.btnQuickCmd_Click);
            this.btnQuickCmd3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnQuickCmd_MouseDown);
            // 
            // btnQuickCmd2
            // 
            this.btnQuickCmd2.Location = new System.Drawing.Point(144, 31);
            this.btnQuickCmd2.Name = "btnQuickCmd2";
            this.btnQuickCmd2.Size = new System.Drawing.Size(90, 30);
            this.btnQuickCmd2.TabIndex = 0;
            this.btnQuickCmd2.Text = "快捷命令02";
            this.btnQuickCmd2.UseVisualStyleBackColor = true;
            this.btnQuickCmd2.Click += new System.EventHandler(this.btnQuickCmd_Click);
            this.btnQuickCmd2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnQuickCmd_MouseDown);
            // 
            // btnQuickCmd1
            // 
            this.btnQuickCmd1.Location = new System.Drawing.Point(18, 31);
            this.btnQuickCmd1.Name = "btnQuickCmd1";
            this.btnQuickCmd1.Size = new System.Drawing.Size(90, 30);
            this.btnQuickCmd1.TabIndex = 0;
            this.btnQuickCmd1.Text = "快捷命令01";
            this.btnQuickCmd1.UseVisualStyleBackColor = true;
            this.btnQuickCmd1.Click += new System.EventHandler(this.btnQuickCmd_Click);
            this.btnQuickCmd1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnQuickCmd_MouseDown);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnSendCommand);
            this.groupBox4.Controls.Add(this.txtTail);
            this.groupBox4.Controls.Add(this.cbbParityMethod);
            this.groupBox4.Controls.Add(this.txtData);
            this.groupBox4.Controls.Add(this.chkTail);
            this.groupBox4.Controls.Add(this.chkData);
            this.groupBox4.Controls.Add(this.txtHead);
            this.groupBox4.Controls.Add(this.chkParity);
            this.groupBox4.Controls.Add(this.chkHead);
            this.groupBox4.Location = new System.Drawing.Point(253, 572);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(504, 126);
            this.groupBox4.TabIndex = 7;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "命令模式（Hex）";
            // 
            // btnSendCommand
            // 
            this.btnSendCommand.Location = new System.Drawing.Point(396, 73);
            this.btnSendCommand.Name = "btnSendCommand";
            this.btnSendCommand.Size = new System.Drawing.Size(90, 30);
            this.btnSendCommand.TabIndex = 5;
            this.btnSendCommand.Text = "发送命令";
            this.btnSendCommand.UseVisualStyleBackColor = true;
            this.btnSendCommand.Click += new System.EventHandler(this.btnSendCommand_Click);
            // 
            // txtTail
            // 
            this.txtTail.Location = new System.Drawing.Point(272, 76);
            this.txtTail.Name = "txtTail";
            this.txtTail.Size = new System.Drawing.Size(79, 25);
            this.txtTail.TabIndex = 4;
            this.txtTail.Text = "0103";
            // 
            // cbbParityMethod
            // 
            this.cbbParityMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbParityMethod.FormattingEnabled = true;
            this.cbbParityMethod.Location = new System.Drawing.Point(80, 76);
            this.cbbParityMethod.Name = "cbbParityMethod";
            this.cbbParityMethod.Size = new System.Drawing.Size(114, 27);
            this.cbbParityMethod.TabIndex = 1;
            // 
            // txtData
            // 
            this.txtData.Location = new System.Drawing.Point(272, 36);
            this.txtData.Name = "txtData";
            this.txtData.Size = new System.Drawing.Size(214, 25);
            this.txtData.TabIndex = 3;
            // 
            // chkTail
            // 
            this.chkTail.AutoSize = true;
            this.chkTail.Checked = true;
            this.chkTail.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTail.Location = new System.Drawing.Point(210, 79);
            this.chkTail.Name = "chkTail";
            this.chkTail.Size = new System.Drawing.Size(56, 24);
            this.chkTail.TabIndex = 2;
            this.chkTail.Text = "帧尾";
            this.chkTail.UseVisualStyleBackColor = true;
            // 
            // chkData
            // 
            this.chkData.AutoSize = true;
            this.chkData.Checked = true;
            this.chkData.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkData.Location = new System.Drawing.Point(210, 37);
            this.chkData.Name = "chkData";
            this.chkData.Size = new System.Drawing.Size(56, 24);
            this.chkData.TabIndex = 2;
            this.chkData.Text = "数据";
            this.chkData.UseVisualStyleBackColor = true;
            // 
            // txtHead
            // 
            this.txtHead.Location = new System.Drawing.Point(80, 36);
            this.txtHead.Name = "txtHead";
            this.txtHead.Size = new System.Drawing.Size(114, 25);
            this.txtHead.TabIndex = 1;
            this.txtHead.Text = "0103";
            // 
            // chkParity
            // 
            this.chkParity.AutoSize = true;
            this.chkParity.Checked = true;
            this.chkParity.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkParity.Location = new System.Drawing.Point(18, 79);
            this.chkParity.Name = "chkParity";
            this.chkParity.Size = new System.Drawing.Size(56, 24);
            this.chkParity.TabIndex = 0;
            this.chkParity.Text = "校验";
            this.chkParity.UseVisualStyleBackColor = true;
            // 
            // chkHead
            // 
            this.chkHead.AutoSize = true;
            this.chkHead.Checked = true;
            this.chkHead.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkHead.Location = new System.Drawing.Point(18, 37);
            this.chkHead.Name = "chkHead";
            this.chkHead.Size = new System.Drawing.Size(56, 24);
            this.chkHead.TabIndex = 0;
            this.chkHead.Text = "帧头";
            this.chkHead.UseVisualStyleBackColor = true;
            // 
            // aSCIIToolStripMenuItem
            // 
            this.aSCIIToolStripMenuItem.Name = "aSCIIToolStripMenuItem";
            this.aSCIIToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.aSCIIToolStripMenuItem.Text = "ASCII";
            this.aSCIIToolStripMenuItem.Click += new System.EventHandler(this.aSCIIToolStripMenuItem_Click);
            // 
            // defaultToolStripMenuItem
            // 
            this.defaultToolStripMenuItem.Name = "defaultToolStripMenuItem";
            this.defaultToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.defaultToolStripMenuItem.Text = "Default";
            this.defaultToolStripMenuItem.Click += new System.EventHandler(this.defaultToolStripMenuItem_Click);
            // 
            // uTF8ToolStripMenuItem
            // 
            this.uTF8ToolStripMenuItem.Name = "uTF8ToolStripMenuItem";
            this.uTF8ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.uTF8ToolStripMenuItem.Text = "UTF8";
            this.uTF8ToolStripMenuItem.Click += new System.EventHandler(this.uTF8ToolStripMenuItem_Click);
            // 
            // unicodeToolStripMenuItem
            // 
            this.unicodeToolStripMenuItem.Name = "unicodeToolStripMenuItem";
            this.unicodeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.unicodeToolStripMenuItem.Text = "Unicode";
            this.unicodeToolStripMenuItem.Click += new System.EventHandler(this.unicodeToolStripMenuItem_Click);
            // 
            // gB3212ToolStripMenuItem
            // 
            this.gB3212ToolStripMenuItem.Name = "gB3212ToolStripMenuItem";
            this.gB3212ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.gB3212ToolStripMenuItem.Text = "GB3212";
            this.gB3212ToolStripMenuItem.Click += new System.EventHandler(this.gB3212ToolStripMenuItem_Click);
            
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(776, 728);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.grbSend);
            this.Controls.Add(this.grbReceive);
            this.Controls.Add(this.grbPort);
            this.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.MaximizeBox = false;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "串口调试助手V1.0";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.grbPort.ResumeLayout(false);
            this.grbPort.PerformLayout();
            this.grbReceive.ResumeLayout(false);
            this.grbReceive.PerformLayout();
            this.grbSend.ResumeLayout(false);
            this.grbSend.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grbPort;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbbPort;
        private System.Windows.Forms.ComboBox cbbBaud;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.CheckBox chkDtr;
        private System.Windows.Forms.CheckBox chkRts;
        private System.Windows.Forms.ComboBox cbbStop;
        private System.Windows.Forms.ComboBox cbbData;
        private System.Windows.Forms.ComboBox cbbParity;
        private System.Windows.Forms.GroupBox grbReceive;
        private System.Windows.Forms.TextBox txtSavePath;
        private System.Windows.Forms.Button btnSelectPath;
        private System.Windows.Forms.Button btnSaveFile;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnHandClear;
        private System.Windows.Forms.CheckBox chkHexReceive;
        private System.Windows.Forms.CheckBox chkAutoClear;
        private System.Windows.Forms.GroupBox grbSend;
        private System.Windows.Forms.TextBox txtSendPath;
        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.Button btnSendFile;
        private System.Windows.Forms.Button btnClearSend;
        private System.Windows.Forms.Button btnHandSend;
        private System.Windows.Forms.CheckBox chkHexSend;
        private System.Windows.Forms.CheckBox chkAutoSend;
        private System.Windows.Forms.TextBox txtPeriod;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel tsslStatus;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel tsslSendCount;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel tsslReceiveCount;
        private System.Windows.Forms.ToolStripStatusLabel tsslClear;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem tsmiEncode;
        private System.Windows.Forms.ToolStripMenuItem tsmiMore;
        private System.Windows.Forms.ToolStripMenuItem tsmiAbout;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox rtbReceive;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RichTextBox rtbSend;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnQuickCmd4;
        private System.Windows.Forms.Button btnQuickCmd3;
        private System.Windows.Forms.Button btnQuickCmd2;
        private System.Windows.Forms.Button btnQuickCmd1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnSendCommand;
        private System.Windows.Forms.TextBox txtTail;
        private System.Windows.Forms.ComboBox cbbParityMethod;
        private System.Windows.Forms.TextBox txtData;
        private System.Windows.Forms.CheckBox chkTail;
        private System.Windows.Forms.CheckBox chkData;
        private System.Windows.Forms.TextBox txtHead;
        private System.Windows.Forms.CheckBox chkParity;
        private System.Windows.Forms.CheckBox chkHead;
        private System.Windows.Forms.ToolStripMenuItem aSCIIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem defaultToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uTF8ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unicodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gB3212ToolStripMenuItem;
    }
}

