using System;
using System.IO.Ports;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using SerialPortDemo.Properties;

namespace SerialPortDemo
{
    /// <summary>
    /// 串口通信助手工具
    /// 串口通信是物理层/链路层的数据传输通道（基于<see cref="SerialPort"/>）
    /// 关键参数：串口号、波特率、检验位、停止位、数据位
    /// </summary>
    public partial class FrmMain : Form
    {
        #region 构造方法

        public FrmMain()
        {
            InitializeComponent();
            //初始化串口
            InitializePort();
        }

        #endregion

        #region 系统对象

        //创建自动发送定时器
        private System.Timers.Timer autoSendTimer = new System.Timers.Timer();
        //创建编码格式对象
        private Encoding encoding = Encoding.Default;
        //新建串口对象
        private SerialPort serialPort = null;
        //定义接收区数据显示大小
        private int clearLimitNum = 4096;
        //定义默认设置
        private Settings defaultSetting = Settings.Default;

        //串口状态
        private bool isOpen;
        public bool IsOpen
        {
            get { return isOpen; }
            set
            {
                isOpen = value;
                if (value)
                {
                    //串口打开
                    this.btnOpen.Text = "关闭串口";
                    this.btnOpen.Image = Resources.poweron;

                    DisablePortProperties();
                }
                else
                {
                    //串口关闭
                    this.btnOpen.Text = "打开串口";
                    this.btnOpen.Image = Resources.poweroff;

                    EnablePortProperties();
                }
            }
        }

        //暂停接收
        private bool isPause;

        public bool IsPause
        {
            get { return isPause; }
            set
            {
                isPause = value;
                if (value)
                {
                    this.btnPause.Text = "继续接收";
                }
                else
                {
                    this.btnPause.Text = "暂停接收";
                }
            }
        }

        /// <summary>
        /// 禁止设置串口属性
        /// </summary>
        private void DisablePortProperties()
        {
            this.cbbPort.Enabled = false;
            this.cbbBaud.Enabled = false;
            this.cbbParity.Enabled = false;
            this.cbbData.Enabled = false;
            this.cbbStop.Enabled = false;
            this.chkRts.Enabled = false;
            this.chkDtr.Enabled = false;
        }
        /// <summary>
        /// 使能设置串口属性
        /// </summary>
        private void EnablePortProperties()
        {
            this.cbbPort.Enabled = true;
            this.cbbBaud.Enabled = true;
            this.cbbParity.Enabled = true;
            this.cbbData.Enabled = true;
            this.cbbStop.Enabled = true;
            this.chkRts.Enabled = true;
            this.chkDtr.Enabled = true;
        }

        //发送字节计数
        private int totalSendCount;

        public int TotalSendCount
        {
            get { return totalSendCount; }
            set
            {
                totalSendCount = value;
                this.tsslSendCount.Text = value.ToString();
            }
        }

        //接收字节计数
        private int totalReceiveCount;

        public int TotalReceiveCount
        {
            get { return totalReceiveCount; }
            set
            {
                totalReceiveCount = value;
                this.tsslReceiveCount.Text = value.ToString();
            }
        }

        //清空计数
        private void tsslClear_Click(object sender, EventArgs e)
        {
            TotalSendCount = TotalReceiveCount = 0;
        }
        #endregion

        #region 初始化串口

        public void InitializePort()
        {
            //通信端口
            string[] portList = SerialPort.GetPortNames();
            if (portList.Length > 0)
            {
                this.cbbPort.DataSource = portList;
                this.cbbPort.SelectedIndex = 0;
            }

            //波特率
            this.cbbBaud.DataSource = new string[] { "2400", "4800", "9600", "19200", "38400" };
            this.cbbBaud.SelectedIndex = 2;

            //校验位
            string[] parity = Enum.GetNames(typeof(Parity));
            this.cbbParity.DataSource = parity;
            this.cbbParity.SelectedIndex = 0;

            //数据位
            this.cbbData.DataSource = new string[] { "5", "6", "7", "8" };
            this.cbbData.SelectedIndex = 3;

            //停止位
            string[] stopBits = Enum.GetNames(typeof(StopBits));
            this.cbbStop.DataSource = stopBits;
            this.cbbStop.SelectedIndex = 1;

            //动态绑定校验方式
            this.cbbParityMethod.DataSource = Enum.GetNames(typeof(ParityMethod));
            this.cbbParityMethod.SelectedIndex = 0;

            //读取命令按钮
            this.btnQuickCmd1.Text = defaultSetting.QuickButton1;
            this.btnQuickCmd1.Tag = defaultSetting.QuickCommand1;
            this.btnQuickCmd2.Text = defaultSetting.QuickButton2;
            this.btnQuickCmd2.Tag = defaultSetting.QuickCommand2;
            this.btnQuickCmd3.Text = defaultSetting.QuickButton3;
            this.btnQuickCmd3.Tag = defaultSetting.QuickCommand3;
            this.btnQuickCmd4.Text = defaultSetting.QuickButton4;
            this.btnQuickCmd4.Tag = defaultSetting.QuickCommand4;

            //读取命令模式配置信息
            this.chkHead.Checked = defaultSetting.ChkHead == "1";
            this.txtHead.Text = defaultSetting.Head;
            this.chkData.Checked = defaultSetting.ChkData == "1";
            this.txtData.Text = defaultSetting.Data;
            this.chkParity.Checked = defaultSetting.ChkParity == "1";
            this.cbbParityMethod.Text = defaultSetting.ParityMethod;
            this.chkTail.Checked = defaultSetting.ChkTail == "1";
            this.txtTail.Text = defaultSetting.Tail;

        }

        #endregion

        #region 打开串口

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (IsOpen)
            {
                //关闭串口
                if (serialPort != null)
                {
                    if (serialPort.IsOpen)
                    {
                        serialPort.Close();
                    }

                    IsOpen = false;
                    this.tsslStatus.Text = this.cbbPort.Text + "串口关闭成功。";
                }
            }
            else
            {
                //打开串口

                //实例化对象
                serialPort = new SerialPort();

                try
                {
                    //设置串口属性
                    serialPort.PortName = this.cbbPort.Text.Trim();
                    serialPort.BaudRate = Convert.ToInt32(this.cbbBaud.Text.Trim());
                    serialPort.Parity = (Parity)Enum.Parse(typeof(Parity), this.cbbParity.Text.Trim(), true);
                    serialPort.DataBits = Convert.ToInt32(this.cbbData.Text.Trim());
                    serialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), this.cbbStop.Text.Trim(), true);

                    serialPort.RtsEnable = this.chkRts.Checked;//该值指示在串行通信中是否启用请求发送 (RTS) 信号；如果为 true，则启用；否则为 false
                    serialPort.DtrEnable = this.chkDtr.Checked;//该值在串行通信过程中启用数据终端就绪 (DTR) 信号；如果为 true，则启用；否则为 false

                    serialPort.ReceivedBytesThreshold = 1;//DataReceived 事件激发前内部输入缓冲区中的字节数。 默认值为 1

                    //关联事件
                    serialPort.DataReceived += SerialPort_DataReceived;

                    if (serialPort.IsOpen)
                    {
                        serialPort.Close();
                    }

                    //打开串口
                    serialPort.Open();

                    //设置串口状态
                    IsOpen = true;
                    //窗体显示串口状态
                    this.tsslStatus.Text = this.cbbPort.Text + " 串口打开成功。";
                }

                catch (Exception ex)
                {
                    this.tsslStatus.Text = this.cbbPort.Text + " 串口打开失败：" + ex.Message;
                }
            }
        }

        #endregion

        #region 串口数据接收事件

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (IsPause)
            {
                return;
            }
            Invoke(new Action(() =>
            {
                try
                {
                    //定义最终字符串
                    string result = string.Empty;
                    //定义一个字节数组，用来接收缓冲区的字节数
                    byte[] data = new byte[serialPort.BytesToRead];
                    //读取缓冲区值到字节数组
                    serialPort.Read(data, 0, data.Length);

                    //以16进制接收
                    if (this.chkHexReceive.Checked)
                    {
                        //拼接显示
                        foreach (var item in data)
                        {
                            string hex = Convert.ToString(item, 16).ToUpper();
                            result += (hex.Length == 1 ? "0" + hex : hex) + " ";
                        }
                        this.rtbReceive.AppendText(result + "\r\n");//显示
                    }
                    else
                    {
                        this.rtbReceive.AppendText(encoding.GetString(data) + "\r\n");//显示
                    }

                    //更新接收字节总数
                    TotalReceiveCount += data.Length;
                }
                catch (Exception ex)
                {
                    this.tsslStatus.Text = "接收出现错误：" + ex.Message;
                }

                //是否开启自动清空
                if (this.chkAutoClear.Checked)
                {
                    if (this.rtbReceive.Text.Length > this.clearLimitNum)
                    {
                        this.rtbReceive.Clear();
                    }
                }
            }));
        }

        #endregion

        #region 手动发送

        private void btnHandSend_Click(object sender, EventArgs e)
        {
            SendData();
        }

        #endregion

        #region 发送数据

        private void SendData()
        {
            //去掉空格 01 04 50
            string sendString = this.rtbSend.Text.Replace(" ", "");

            //十六进制发送
            if (this.chkHexSend.Checked)
            {
                if (!HexHelper.IsHexString(sendString))
                {
                    this.tsslStatus.Text = "您发送的内容不符合16进制格式。";
                    return;
                }
                try
                {
                    //定义一个字节数组，用来接收发送的字节
                    byte[] sendBytes = HexHelper.HexStringToBytes(sendString);
                    serialPort.Write(sendBytes, 0, sendBytes.Length);
                    TotalSendCount += sendBytes.Length;
                    this.tsslStatus.Text = this.cbbPort.Text + " 发送数据成功。";
                }
                catch (Exception ex)
                {
                    this.tsslStatus.Text = "发送失败：" + ex.Message;
                }
            }

            else//ASCII码形式发送
            {
                try
                {
                    byte[] sendBytes = encoding.GetBytes(this.rtbSend.Text);
                    serialPort.Write(sendBytes, 0, sendBytes.Length);
                    TotalSendCount += sendBytes.Length;
                    this.tsslStatus.Text = this.cbbPort.Text + " 发送数据成功。";

                }
                catch (Exception ex)
                {
                    this.tsslStatus.Text = "发送失败：" + ex.Message;
                }
            }
        }

        #endregion

        #region 定时发送

        private void ChkAutoSend_CheckedChanged(object sender, EventArgs e)
        {
            //串口没有打开
            if (IsOpen == false && this.chkAutoSend.CheckState == CheckState.Checked)
            {
                this.chkAutoSend.CheckState = CheckState.Unchecked;

                //停止定时器
                this.autoSendTimer.Enabled = false;

                this.tsslStatus.Text = "自动发送失败：串口未连接。";
                return;
            }
            if (this.rtbSend.Text.Length == 0)
            {
                this.chkAutoSend.Checked = false;
                this.autoSendTimer.Enabled = false;
                this.tsslStatus.Text = "发送内容为空。";
                return;
            }
            //串口打开，并且自动发送处于被选中状态
            if (IsOpen == true && this.chkAutoSend.CheckState == CheckState.Checked)
            {
                //禁用手动发送
                this.btnHandSend.Enabled = false;

                //获取时间周期
                int interval = 0;

                if (int.TryParse(this.txtPeriod.Text.Trim(), out interval))
                {
                    if (interval < 1 || interval > 60000)
                    {
                        interval = 1000;
                        this.txtPeriod.Text = "1000";
                        this.tsslStatus.Text = "周期设定过大，限制为1000ms";
                    }

                    //开始自动发送
                    this.autoSendTimer.Interval = interval;

                    this.autoSendTimer.Enabled = true;
                    this.autoSendTimer.Elapsed += AutoSendTimer_Elapsed;

                    //禁用周期设置
                    this.txtPeriod.Enabled = false;
                }
                else
                {
                    this.chkAutoSend.CheckState = CheckState.Unchecked;

                    //停止定时器
                    this.autoSendTimer.Enabled = false;

                    this.tsslStatus.Text = "自动发送失败：周期设定格式不正确。";
                    return;
                }
            }
            else
            {
                this.btnHandSend.Enabled = true;
                this.txtPeriod.Enabled = true;
                this.autoSendTimer.Enabled = false;
                //如果不把事件关联取消，则会叠加发送
                this.autoSendTimer.Elapsed -= AutoSendTimer_Elapsed;
            }
        }

        private void AutoSendTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Invoke(new Action(() =>
            {
                SendData();
            }));

            //if (this.InvokeRequired)
            //{
            //    Invoke(new Action(() =>
            //    {
            //        SendData();
            //    }));
            //}
            //else
            //{
            //    SendData();
            //}
        }

        #endregion

        #region 清空发送接收区

        private void btnClearSend_Click(object sender, EventArgs e)
        {
            this.rtbSend.Clear();
        }
        private void btnHandClear_Click(object sender, EventArgs e)
        {
            this.rtbReceive.Clear();
        }

        #endregion

        #region 打开文件

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            //创建打开对话框
            OpenFileDialog ofd = new OpenFileDialog();
            //设置ofd属性
            ofd.Title = "请选择要发送的文件";
            ofd.Filter = "文本文件(*.txt)|*.txt|二进制文件(*.bin)|*.bin|HEX文件(*.hex)|*.hex";
            ofd.RestoreDirectory = true;
            //打开ofd
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //把十六进制的选择取消
                this.chkHexSend.Checked = false;
                //获取文件名称并显示
                string fileName = ofd.FileName;
                this.txtSendPath.Text = fileName;

                //读取内容显示
                StreamReader sr = new StreamReader(fileName, Encoding.UTF8);
                this.rtbSend.Text = sr.ReadToEnd();
                sr.Close();
            }
        }

        #endregion

        #region 发送文件

        private void btnSendFile_Click(object sender, EventArgs e)
        {
            if (IsOpen)
            {
                if (this.txtSendPath.Text.Length == 0)
                {
                    this.tsslStatus.Text = "请先选择要发送的文件。";
                    return;
                }
                if (this.rtbSend.Text.Length==0)
                {
                    this.tsslStatus.Text = "发送内容为空。";
                    return;
                }
                byte[] sendFile = encoding.GetBytes(this.rtbSend.Text);
                int sendCount = sendFile.Length / 4096;
                int sendRemain = sendFile.Length % 4096;
                try
                {
                    //循环发送
                    for (int i = 0; i < sendCount; i++)
                    {
                        this.serialPort.Write(sendFile, 4096 * i, 4096);
                        Thread.Sleep(50);
                    }
                    //最后一次发送
                    if (sendRemain > 0)
                    {
                        this.serialPort.Write(sendFile, 4096 * sendCount, sendRemain);
                    }
                }
                catch (Exception ex)
                {
                    this.tsslStatus.Text = "发送文件失败：" + ex.Message;
                    return;
                }

                //更新发送计数
                TotalSendCount += sendFile.Length;
            }
            else
            {
                this.tsslStatus.Text = "发送文件失败：串口未连接。";
            }
        }

        #endregion

        #region 暂停接收

        private void btnPause_Click(object sender, EventArgs e)
        {
            IsPause = !IsPause;
        }

        #endregion

        #region 选择路径

        private void btnSelectPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                this.txtSavePath.Text = fbd.SelectedPath;
            }
        }

        #endregion

        #region 保存数据

        private void btnSaveFile_Click(object sender, EventArgs e)
        {
            if (this.rtbReceive.Text.Length == 0)
            {
                this.tsslStatus.Text = "接收数据为空，请检查。";
                return;
            }
            if (this.txtSavePath.Text.Length == 0)
            {
                this.tsslStatus.Text = "请先设置要保存的路径。";
                return;
            }
            string savePath = this.txtSavePath.Text + "\\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";

            //创建写入器
            StreamWriter sw = new StreamWriter(savePath);
            sw.Write(this.rtbReceive.Text);
            sw.Flush();
            sw.Close();
            this.tsslStatus.Text = "文件保存成功。";
        }

        #endregion

        #region 快捷右击事件

        private void btnQuickCmd_MouseDown(object sender, MouseEventArgs e)
        {
            //鼠标右击
            if (e.Button == MouseButtons.Right)
            {
                if (sender is Button btn)
                {
                    //打开设置窗体并传值
                    FrmQuickSet objFrm = new FrmQuickSet(btn.Text, btn.Tag == null ? string.Empty : btn.Tag.ToString());
                    if (objFrm.ShowDialog() == DialogResult.OK)
                    {
                        //获取设置的最新值
                        btn.Text = objFrm.CmdName;
                        btn.Tag = objFrm.CmdContent;

                        //保存配置
                        switch (btn.Name)
                        {
                            case "btnQuickCmd1":
                                defaultSetting.QuickButton1 = objFrm.CmdName;
                                defaultSetting.QuickCommand1 = objFrm.CmdContent;
                                break;
                            case "btnQuickCmd2":
                                defaultSetting.QuickButton2 = objFrm.CmdName;
                                defaultSetting.QuickCommand2 = objFrm.CmdContent;
                                break;
                            case "btnQuickCmd3":
                                defaultSetting.QuickButton3 = objFrm.CmdName;
                                defaultSetting.QuickCommand3 = objFrm.CmdContent;
                                break;
                            case "btnQuickCmd4":
                                defaultSetting.QuickButton4 = objFrm.CmdName;
                                defaultSetting.QuickCommand4 = objFrm.CmdContent;
                                break;
                        }
                        defaultSetting.Save();
                    }
                }
            }
        }

        #endregion

        #region 快捷命令发送事件

        private void btnQuickCmd_Click(object sender, EventArgs e)
        {
            if (IsOpen)
            {
                if (sender is Button btn)
                {
                    if (btn.Tag != null && btn.Tag.ToString().Length > 0)
                    {
                        SendCommand(btn.Tag.ToString());
                    }
                    else
                    {
                        this.tsslStatus.Text = "发送失败：发送命令未设置。";
                    }
                }
            }
            else
            {
                this.tsslStatus.Text = "发送失败：串口未连接。";
            }
        }

        #endregion

        #region 发送字符串命令

        /// <summary>
        /// 发送字符串命令
        /// </summary>
        /// <param name="cmdContent">16进制字符串</param>
        private void SendCommand(string cmdContent)
        {
            this.rtbSend.Text = cmdContent;

            cmdContent = cmdContent.Replace(" ", "");
            try
            {
                this.serialPort.Write(HexHelper.HexStringToBytes(cmdContent), 0, HexHelper.GetByteCount(cmdContent));

                //更新发送计数
                TotalSendCount += HexHelper.GetByteCount(cmdContent);
            }
            catch (Exception ex)
            {
                this.tsslStatus.Text = "发送失败：" + ex.Message;
            }
        }

        #endregion

        #region 命令模式

        private void btnSendCommand_Click(object sender, EventArgs e)
        {
            //验证数据
            bool dataVerify = true;

            if (this.chkHead.Checked)
            {
                dataVerify &= VerifyHexString(this.txtHead.Text.Trim());
            }
            if (this.chkData.Checked)
            {
                dataVerify &= VerifyHexString(this.txtData.Text.Trim());
            }
            if (this.chkTail.Checked)
            {
                dataVerify &= VerifyHexString(this.txtTail.Text.Trim());
            }

            if (!dataVerify)
            {
                this.tsslStatus.Text = "命令模式数据格式必须是16进制。";
                return;
            }

            if (IsOpen)
            {
                string fullCmd = string.Empty;

                //是否选择帧头
                if (this.chkHead.Checked)
                {
                    fullCmd += this.txtHead.Text;
                }
                //是否选择数据
                if (this.chkData.Checked)
                {
                    fullCmd += this.txtData.Text;
                }

                //是否选择校验
                if (this.chkParity.Checked)
                {
                    fullCmd = fullCmd.Replace(" ", "");

                    ParityMethod parityMethod = (ParityMethod)Enum.Parse(typeof(ParityMethod), this.cbbParityMethod.Text, true);

                    switch (parityMethod)
                    {
                        case ParityMethod.XOR异或:
                            fullCmd = HexHelper.BytesToHexString(ParityHelper.DataXORFull(HexHelper.HexStringToBytes(fullCmd)));
                            break;
                        case ParityMethod.SUM8累加:
                            fullCmd = HexHelper.BytesToHexString(ParityHelper.DataSum8Full(HexHelper.HexStringToBytes(fullCmd)));
                            break;
                        case ParityMethod.SUM16大端:
                            fullCmd = HexHelper.BytesToHexString(ParityHelper.DataSum16Full(HexHelper.HexStringToBytes(fullCmd), ParityHelper.BigOrLittle.BigEndian));
                            break;
                        case ParityMethod.SUM16小端:
                            fullCmd = HexHelper.BytesToHexString(ParityHelper.DataSum16Full(HexHelper.HexStringToBytes(fullCmd), ParityHelper.BigOrLittle.LittleEndian));
                            break;
                        case ParityMethod.CRC16大端:
                            fullCmd = HexHelper.BytesToHexString(ParityHelper.DataCrc16Full_Ccitt(HexHelper.HexStringToBytes(fullCmd), ParityHelper.BigOrLittle.BigEndian));
                            break;
                        case ParityMethod.CRC16小端:
                            fullCmd = HexHelper.BytesToHexString(ParityHelper.DataCrc16Full_Ccitt(HexHelper.HexStringToBytes(fullCmd), ParityHelper.BigOrLittle.LittleEndian));
                            break;
                        case ParityMethod.ModbusCRC大端:
                            fullCmd = HexHelper.BytesToHexString(ParityHelper.DataCrc16Full_Modbus(HexHelper.HexStringToBytes(fullCmd), ParityHelper.BigOrLittle.BigEndian));
                            break;
                        case ParityMethod.ModbusCRC小端:
                            fullCmd = HexHelper.BytesToHexString(ParityHelper.DataCrc16Full_Modbus(HexHelper.HexStringToBytes(fullCmd), ParityHelper.BigOrLittle.LittleEndian));
                            break;
                        case ParityMethod.ModbusLRC:
                            break;
                        default:
                            break;
                    }
                }

                //是否选择帧尾
                if (this.chkTail.Checked)
                {
                    fullCmd += this.txtTail.Text;
                }

                this.rtbSend.Text = fullCmd;
                SendCommand(fullCmd);
            }
            else
            {
                this.tsslStatus.Text = "命名模式发送失败：串口未连接。";
            }
        }

        /// <summary>
        /// 验证数据是否为16进制字符串，并且长度必须是偶数
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>返回布尔结果</returns>
        private bool VerifyHexString(string str)
        {
            string temp = str.Replace(" ", "");
            return HexHelper.IsHexString(temp) && temp.Length % 2 == 0;
        }

        #endregion

        #region 关闭窗体

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("确定要退出系统吗？", "退出", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

            if (result == DialogResult.OK)
            {
                //保存命令模式配置
                SaveSettings();

                if (autoSendTimer.Enabled)
                {
                    autoSendTimer.Enabled = false;
                }

            }
            else
            {
                e.Cancel = true;
            }

        }

        /// <summary>
        /// 保存命名模式配置
        /// </summary>
        private void SaveSettings()
        {
            defaultSetting.ChkHead = this.chkHead.Checked ? "1" : "0";
            defaultSetting.Head = this.txtHead.Text;

            defaultSetting.ChkData = this.chkData.Checked ? "1" : "0";
            defaultSetting.Data = this.txtData.Text;

            defaultSetting.ChkParity = this.chkParity.Checked ? "1" : "0";
            defaultSetting.ParityMethod = this.cbbParityMethod.Text;

            defaultSetting.ChkTail = this.chkTail.Checked ? "1" : "0";
            defaultSetting.Tail = this.txtTail.Text;

            defaultSetting.Save();
        }

        #endregion

        #region RTS/DTR设置

        private void chkRts_CheckedChanged(object sender, EventArgs e)
        {
            if (serialPort != null)
            {
                this.serialPort.RtsEnable = this.chkRts.Checked;
            }
        }

        private void chkDtr_CheckedChanged(object sender, EventArgs e)
        {
            if (serialPort != null)
            {
                this.serialPort.DtrEnable = this.chkDtr.Checked;
            }
        }

        #endregion

        #region 编码格式设置

        private void aSCIIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            encoding = Encoding.ASCII;
        }

        private void defaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            encoding = Encoding.Default;
        }

        private void uTF8ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            encoding = Encoding.UTF8;
        }

        private void unicodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            encoding = Encoding.Unicode;
        }

        private void gB3212ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            encoding = Encoding.GetEncoding("GB3212");
        }

        #endregion

        #region 关于我们

        private void tsmiMore_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/zyuanlbj/WindowsToolDemo");

        }

        private void tsmiAbout_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://gitee.com/zyuanlbj/WindowsToolDemo");
        }

        #endregion

        #region 双缓冲实现

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;//Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        #endregion

    }

    #region 校验枚举

    /// <summary>
    /// 校验方式
    /// </summary>
    public enum ParityMethod
    {
        XOR异或,
        SUM8累加,
        SUM16大端,
        SUM16小端,
        CRC16大端,
        CRC16小端,
        ModbusCRC大端,
        ModbusCRC小端,
        ModbusLRC
    }

    #endregion

}
