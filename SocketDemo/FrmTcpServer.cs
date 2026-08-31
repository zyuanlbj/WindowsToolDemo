using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SocketDemo
{
    public partial class FrmTcpServer : Form
    {
        public FrmTcpServer()
        {
            InitializeComponent();
            addOnlineDelegate = AddOnline;
            rcvMessageDelegate = ReceiveMessage;
            saveFileDelegate = SaveFile;
        }

        #region 属性及变量

        /// <summary>
        /// 创建套接字
        /// </summary>
        private Socket socket = null;
        /// <summary>
        /// 创建URL与Socket的字典集合
        /// </summary>
        private readonly Dictionary<string, Socket> sockets = new Dictionary<string, Socket>();
        /// <summary>
        /// 客户端上线委托对象
        /// </summary>
        private readonly Action<string, bool> addOnlineDelegate;
        /// <summary>
        /// 接收消息委托对象
        /// </summary>
        private readonly Action<string> rcvMessageDelegate;
        /// <summary>
        /// 保存文件委托对象
        /// </summary>
        private readonly Action<byte[], int> saveFileDelegate;

        #endregion

        #region 启动服务按钮事件

        private void btn_StartServer_Click(object sender, EventArgs e)
        {
            //创建负责监听的套接字，IPV4 字节流 TCP
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress address = IPAddress.Parse(this.txt_IP.Text.Trim());
            IPEndPoint endPoint = new IPEndPoint(address, int.Parse(this.txt_Port.Text.Trim()));
            try
            {
                socket.Bind(endPoint);
                Invoke(rcvMessageDelegate, "服务器开启成功！");
                MessageBox.Show("开启服务成功！");

            }
            catch (Exception ex)
            {
                MessageBox.Show("开启服务失败：" + ex.Message);
                return;
            }
            socket.Listen(10);
            Task.Run(() => { ListenConnecting(); });
            this.btn_StartServer.Enabled = false;
        }

        #endregion

        #region 监听线程

        private void ListenConnecting()
        {
            while (true)
            {
                //一旦监听到客户端的连接，将会创建一个与该客户端连接的套接字
                Socket client = socket.Accept();
                string clientURL = client.RemoteEndPoint.ToString();
                sockets.Add(clientURL, client);

                Invoke(addOnlineDelegate, clientURL, true);
                Invoke(rcvMessageDelegate, clientURL + "上线了！");

                //启动接收线程
                Task.Run(() => { ReceiveMessageThread(client); });
            }
        }

        #endregion

        #region 接收线程

        private void ReceiveMessageThread(Socket client)
        {
            while (true)
            {
                byte[] buffer = new byte[2 * 1024 * 1024];
                int length;
                try
                {
                    length = client.Receive(buffer);
                }
                catch (Exception)
                {
                    string url = client.RemoteEndPoint.ToString();
                    Invoke(rcvMessageDelegate, url + "下线了！");
                    Invoke(addOnlineDelegate, url, false);
                    sockets.Remove(url);
                    break;
                }
                if (length == 0)
                {
                    string url = client.RemoteEndPoint.ToString();
                    Invoke(rcvMessageDelegate, url + "下线了！");
                    Invoke(addOnlineDelegate, url, false);
                    sockets.Remove(url);
                    break;
                }
                else
                {
                    if (buffer[0] == 0)
                    {
                        string value = Encoding.UTF8.GetString(buffer, 1, length - 1);
                        string msg = "[接收]    " + client.RemoteEndPoint.ToString() + "    " + value;
                        Invoke(rcvMessageDelegate, msg);
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

        private void AddOnline(string url, bool flag)
        {
            if (flag)
            {
                this.lbOnline.Items.Add(url);
            }
            else
            {
                this.lbOnline.Items.Remove(url);
            }
        }
        private void ReceiveMessage(string message)
        {
            this.txt_Rcv.AppendText(message + Environment.NewLine);
        }
        private void SaveFile(byte[] array, int length)
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

        private void btn_SendToSingle_Click(object sender, EventArgs e)
        {
            string StrMsg = this.txt_Send.Text.Trim();
            byte[] arrMsg = Encoding.UTF8.GetBytes(StrMsg);

            byte[] arrSend = new byte[arrMsg.Length + 1];
            arrSend[0] = 0;
            Buffer.BlockCopy(arrMsg, 0, arrSend, 1, arrMsg.Length);


            if (this.lbOnline.SelectedItems.Count == 0)
            {
                MessageBox.Show("请选择你要发送的对象!");
                return;
            }
            else
            {
                foreach (string item in this.lbOnline.SelectedItems)
                {
                    sockets[item].Send(arrSend);
                    string Msg = "[发送]     " + item + "     " + StrMsg;
                    Invoke(rcvMessageDelegate, Msg);
                }
            }
        }

        #endregion

        #region 群发消息

        private void btn_SendToAll_Click(object sender, EventArgs e)
        {
            string StrMsg = this.txt_Send.Text.Trim();
            byte[] arrMsg = Encoding.UTF8.GetBytes(StrMsg);

            byte[] arrSend = new byte[arrMsg.Length + 1];
            arrSend[0] = 0;
            Buffer.BlockCopy(arrMsg, 0, arrSend, 1, arrMsg.Length);

            foreach (string item in this.sockets.Keys)
            {
                sockets[item].Send(arrSend);

                string Msg = "[发送]     " + item + "     " + StrMsg;

                Invoke(rcvMessageDelegate, Msg);
            }
            Invoke(rcvMessageDelegate, "[群发]     群发完毕!");
        }

        #endregion

        #region 打开客户端

        private void btn_Client_Click(object sender, EventArgs e)
        {
            var client = new FrmTcpClient();
            client.Show();
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
            string online = this.lbOnline.Text.Trim();
            if (string.IsNullOrEmpty(online))
            {
                MessageBox.Show("请选择您要发送的对象！");
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

                sockets[online].Send(arrSend);

                byte[] arrfileSend = new byte[1024 * 1024 * 2];
                int length = fs.Read(arrfileSend, 0, arrfileSend.Length);

                byte[] arrfile = new byte[length + 1];
                arrfile[0] = 1;
                Buffer.BlockCopy(arrfileSend, 0, arrfile, 1, length);

                sockets[online].Send(arrfile);
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
