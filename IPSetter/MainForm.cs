using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace IPSetter
{
    public class MainForm : Form
    {
        private ComboBox cboAdapters;
        private TextBox txtIp;
        private TextBox txtSubnet;
        private TextBox txtGateway;
        private Button btnRefresh;
        private Button btnApply;
        private Button btnDhcp;
        private Label lblStatus;

        // 下拉项显示名 -> 适配器信息
        private Dictionary<string, AdapterInfo> adapterDict =
            new Dictionary<string, AdapterInfo>();

        // 输入框默认值
        private const string DefaultIp = "10.1.10.55";
        private const string DefaultSubnet = "255.255.255.0";
        private const string DefaultGateway = "10.1.10.1";

        public MainForm()
        {
            InitializeComponent();
            LoadAdapters();
            LoadSavedValues();
        }

        private void InitializeComponent()
        {
            this.Text = "有线网卡 IP 设置工具";
            this.Size = new Size(470, 330);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = true;

            // 设置窗体图标（从 exe 内嵌的应用图标 ip.ico 提取）
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            }
            catch
            {
                // 图标缺失时忽略，不影响功能
            }

            var lblAdapter = new Label { Text = "网络适配器：", Left = 20, Top = 22, Width = 100 };
            cboAdapters = new ComboBox
            {
                Left = 130,
                Top = 19,
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboAdapters.SelectedIndexChanged += CboAdapters_SelectedIndexChanged;

            var lblIp = new Label { Text = "IP 地址：", Left = 20, Top = 62, Width = 100 };
            txtIp = new TextBox { Left = 130, Top = 59, Width = 300, Text = DefaultIp };

            var lblSubnet = new Label { Text = "子网掩码：", Left = 20, Top = 97, Width = 100 };
            txtSubnet = new TextBox { Left = 130, Top = 94, Width = 300, Text = DefaultSubnet };

            var lblGateway = new Label { Text = "默认网关：", Left = 20, Top = 132, Width = 100 };
            txtGateway = new TextBox { Left = 130, Top = 129, Width = 300, Text = DefaultGateway };

            btnRefresh = new Button { Text = "刷新", Left = 20, Top = 175, Width = 85, Height = 32 };
            btnRefresh.Click += BtnRefresh_Click;

            btnApply = new Button { Text = "应用静态 IP", Left = 120, Top = 175, Width = 150, Height = 32 };
            btnApply.Click += BtnApply_Click;

            btnDhcp = new Button { Text = "启用 DHCP", Left = 285, Top = 175, Width = 145, Height = 32 };
            btnDhcp.Click += BtnDhcp_Click;

            lblStatus = new Label
            {
                Text = "就绪。",
                Left = 20,
                Top = 222,
                Width = 410,
                Height = 60,
                ForeColor = Color.DarkGreen
            };

            this.Controls.AddRange(new Control[]
            {
                lblAdapter, cboAdapters,
                lblIp, txtIp,
                lblSubnet, txtSubnet,
                lblGateway, txtGateway,
                btnRefresh, btnApply, btnDhcp,
                lblStatus
            });
        }

        private void LoadAdapters()
        {
            try
            {
                cboAdapters.Items.Clear();
                adapterDict.Clear();

                var adapters = NetworkConfigurator.GetAdapters();
                foreach (var a in adapters)
                {
                    var display = string.IsNullOrEmpty(a.Name) ? a.Description : a.Name;
                    cboAdapters.Items.Add(display);
                    adapterDict[display] = a;
                }

                if (cboAdapters.Items.Count > 0)
                    cboAdapters.SelectedIndex = 0;

                SetStatus($"已找到 {adapters.Count} 个有线网络适配器。", Color.DarkGreen);
            }
            catch (Exception ex)
            {
                SetStatus("加载适配器失败：" + ex.Message, Color.Red);
            }
        }

        private void CboAdapters_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboAdapters.SelectedItem == null) return;
            var a = adapterDict[cboAdapters.SelectedItem.ToString()];
            // 有当前配置则显示当前值，否则显示已保存（或默认）值
            txtIp.Text = a.IPAddresses.Length > 0 ? a.IPAddresses[0] : GetSavedOrDefault(Properties.Settings.Default.IpAddress, DefaultIp);
            txtSubnet.Text = a.SubnetMasks.Length > 0 ? a.SubnetMasks[0] : GetSavedOrDefault(Properties.Settings.Default.SubnetMask, DefaultSubnet);
            txtGateway.Text = a.Gateways.Length > 0 ? a.Gateways[0] : GetSavedOrDefault(Properties.Settings.Default.DefaultGateway, DefaultGateway);
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadAdapters();
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            if (cboAdapters.SelectedItem == null)
            {
                MessageBox.Show("请先选择一个网络适配器。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var ip = txtIp.Text.Trim();
            var subnet = txtSubnet.Text.Trim();
            var gateway = txtGateway.Text.Trim();

            if (!IsValidIp(ip) || !IsValidIp(subnet) ||
                (!string.IsNullOrEmpty(gateway) && !IsValidIp(gateway)))
            {
                MessageBox.Show("请输入合法的 IPv4 地址（例如 192.168.1.100）。",
                    "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var a = adapterDict[cboAdapters.SelectedItem.ToString()];
                NetworkConfigurator.SetStaticIp(a.Index, ip, subnet, gateway);

                // 保存到应用程序设置（用户作用域，自动持久化到 user.config）
                Properties.Settings.Default.IpAddress = ip;
                Properties.Settings.Default.SubnetMask = subnet;
                Properties.Settings.Default.DefaultGateway = gateway;
                Properties.Settings.Default.Save();

                SetStatus("静态 IP 设置成功，请确认网络连接是否正常。", Color.DarkGreen);
                LoadAdapters();

                var gwText = string.IsNullOrEmpty(gateway) ? "（无）" : gateway;
                MessageBox.Show(
                    $"静态 IP 设置成功！\n\n" +
                    $"网络适配器：{a.Name}\n" +
                    $"IP 地址：{ip}\n" +
                    $"子网掩码：{subnet}\n" +
                    $"默认网关：{gwText}",
                    "设置成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                SetStatus("设置失败：" + ex.Message, Color.Red);
                MessageBox.Show("静态 IP 设置失败：\n\n" + ex.Message,
                    "设置失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDhcp_Click(object sender, EventArgs e)
        {
            if (cboAdapters.SelectedItem == null)
            {
                MessageBox.Show("请先选择一个网络适配器。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var a = adapterDict[cboAdapters.SelectedItem.ToString()];
                NetworkConfigurator.SetDhcp(a.Index);
                SetStatus("已切换到 DHCP 自动获取，请稍候网络会重新连接。", Color.DarkGreen);
                LoadAdapters();
            }
            catch (Exception ex)
            {
                SetStatus("设置失败：" + ex.Message, Color.Red);
            }
        }

        private static bool IsValidIp(string s)
        {
            return IPAddress.TryParse(s, out var ip) &&
                   ip.AddressFamily == AddressFamily.InterNetwork;
        }

        private void SetStatus(string msg, Color color)
        {
            lblStatus.Text = msg;
            lblStatus.ForeColor = color;
        }

        /// <summary>
        /// 启动时加载已保存的 IP/子网/网关（无保存值时回退到默认值）
        /// </summary>
        private void LoadSavedValues()
        {
            txtIp.Text = GetSavedOrDefault(Properties.Settings.Default.IpAddress, DefaultIp);
            txtSubnet.Text = GetSavedOrDefault(Properties.Settings.Default.SubnetMask, DefaultSubnet);
            txtGateway.Text = GetSavedOrDefault(Properties.Settings.Default.DefaultGateway, DefaultGateway);
        }

        private static string GetSavedOrDefault(string saved, string def)
        {
            return string.IsNullOrWhiteSpace(saved) ? def : saved.Trim();
        }
    }
}
