using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SocketDemo
{
    public partial class FrmTcpClient : Form
    {
        public FrmTcpClient()
        {
            InitializeComponent();
            saveFileDelegate = SaveFileDelegateMethod;
            this.FormClosing += (s, e) =>
            {
                isRunning = false;
                socket?.Close();
            };
        }

        #region 属性及对象

        private Socket socket = null;
        private bool isRunning = true;
        private readonly Action<byte[], int> saveFileDelegate;

        #endregion

        #region 连接服务器

        private void btn_Connect_Click(object sender, EventArgs e)
        {
            IPAddress address = IPAddress.Parse(this.txt_IP.Text.Trim());
            IPEndPoint endPoint = new IPEndPoint(address, int.Parse(this.txt_Port.Text.Trim()));
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                this.txt_Rcv.AppendText("与服务器连接中..." + Environment.NewLine);
                socket.Connect(endPoint);
            }
            catch (Exception ex)
            {
                MessageBox.Show("连接失败：" + ex.Message);
            }
            this.txt_Rcv.AppendText("与服务器连接成功" + Environment.NewLine);
            this.btn_Connect.Enabled = false;

            //开启接收线程
            Task.Run(() => { ReceiveMessageThreadMethod(); });
        }

        #endregion

        #region 接收消息线程方法

        private void ReceiveMessageThreadMethod()
        {
            while (isRunning)
            {
                byte[] buffer = new byte[2 * 1024 * 1024];
                int length = -1;
                try
                {
                    length = socket.Receive(buffer);
                }
                catch (SocketException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() => { this.txt_Rcv.AppendText("断开连接" + ex.Message + Environment.NewLine); }));
                    break;
                }
                if (length > 0)
                {
                    if (buffer[0] == 0)
                    {
                        var value = Encoding.UTF8.GetString(buffer, 1, length - 1);
                        var msg = "[接收]    " + value + Environment.NewLine;
                        Invoke(new Action(() => this.txt_Rcv.AppendText(msg)));
                    }
                    else if (buffer[0] == 1)
                    {
                        Invoke(saveFileDelegate, buffer, length);
                    }
                }
            }
        }

        #endregion

        #region 委托方法

        private void SaveFileDelegateMethod(byte[] array, int length)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog()
                {
                    Filter = "word files(*.docx)|*.docx|txt files(*.txt)|*.txt|xls files(*.xls)|*.xls|All files(*.*)|*.*"
                };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string filePath = sfd.FileName;
                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        fs.Write(array, 1, length - 1);
                        Invoke(new Action(() => this.txt_Rcv.AppendText("[保存]    保存文件成功" + filePath + Environment.NewLine)));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存失败：" + ex.Message);
            }
        }

        #endregion

        #region 发送消息

        private void btn_Send_Click(object sender, EventArgs e)
        {
            var value = "来自" + this.txt_Name.Text.Trim() + "：" + this.txt_Send.Text.Trim();
            var array = Encoding.UTF8.GetBytes(value);
            var sendBytes = new byte[array.Length + 1];
            sendBytes[0] = 0;
            Buffer.BlockCopy(array, 0, sendBytes, 1, array.Length);
            socket.Send(sendBytes);
            Invoke(new Action(() => this.txt_Rcv.AppendText("[发送]    " + this.txt_Send.Text.Trim() + Environment.NewLine)));
        }

        #endregion

        #region 发送文件

        private void btn_SendFile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txt_SelectFile.Text))
            {
                MessageBox.Show("请选择您要发送的文件！");
                return;
            }

            using (FileStream fs = new FileStream(txt_SelectFile.Text, FileMode.Open))
            {
                string filename = Path.GetFileName(txt_SelectFile.Text);
                string StrMsg = "发送文件为：" + filename;
                byte[] arrMsg = Encoding.UTF8.GetBytes(StrMsg);

                byte[] arrSend = new byte[arrMsg.Length + 1];
                arrSend[0] = 0;
                Buffer.BlockCopy(arrMsg, 0, arrSend, 1, arrMsg.Length);

                socket.Send(arrSend);


                byte[] arrfileSend = new byte[1024 * 1024 * 2];
                int length = fs.Read(arrfileSend, 0, arrfileSend.Length);

                byte[] arrfile = new byte[length + 1];
                arrfile[0] = 1;
                Buffer.BlockCopy(arrfileSend, 0, arrfile, 1, length);

                socket.Send(arrfile);
            }
        }

        #endregion

        #region 选择文件

        private void btn_SelectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                InitialDirectory = "E:\\"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                this.txt_SelectFile.Text = ofd.FileName;
            }
        }

        #endregion

    }
}
