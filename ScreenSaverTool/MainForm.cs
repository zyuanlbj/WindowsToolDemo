using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace ScreenSaverTool
{
    public class MainForm : Form
    {
        private TextBox txtImagePath;
        private Button btnSelect;
        private Button btnSetBg;
        private Button btnProtect;
        private Label lblStatus;
        private Button btnNoLockScreen;
        private Button btnScrSaver;
        private bool isProtected = false;
        private bool isNoLockScreen = false;
        private bool scrSaverDisabled = false;

        // 计算机配置 -> 管理模板 -> 控制面板 -> 个性化 -> 不显示锁屏
        private const string MachinePersonalization = @"Software\Policies\Microsoft\Windows\Personalization";

        // 屏幕保护程序设置注册表位置
        private const string Desk = @"HKEY_CURRENT_USER\Control Panel\Desktop";

        // 用户配置 -> 管理模板 -> 控制面板 -> 个性化 下的三项策略（功能保护按钮管理）
        // 注：“带密码的屏幕保护程序”(ScreenSaverIsSecure) 已从此数组移除，改为加载时单独设为“已禁用”。
        private static readonly PolicyItem[] ProtectionPolicies =
        {
            new PolicyItem(@"Software\Microsoft\Windows\CurrentVersion\Policies\System", "NoDispAppearancePage", 1, RegistryValueKind.DWord),
            new PolicyItem(@"Software\Microsoft\Windows\CurrentVersion\Policies\ActiveDesktop", "NoChangingWallpaper", 1, RegistryValueKind.DWord),
            new PolicyItem(@"Software\Microsoft\Windows\CurrentVersion\Policies\System", "NoDispScrSavPage", 1, RegistryValueKind.DWord),
        };

        private class PolicyItem
        {
            public string SubKeyPath { get; }
            public string ValueName { get; }
            public object Value { get; }
            public RegistryValueKind Kind { get; }
            public PolicyItem(string subKeyPath, string valueName, object value, RegistryValueKind kind)
            {
                SubKeyPath = subKeyPath; ValueName = valueName; Value = value; Kind = kind;
            }
        }

        public MainForm()
        {
            InitializeComponent();
            this.Load += MainForm_Load;
        }

        private void InitializeComponent()
        {
            this.Text = "屏保与背景设置工具";
            this.ClientSize = new Size(700, 290);
            this.ShowIcon = true;
            // 从嵌入资源加载窗口图标（资源名 = 根命名空间.文件名）
            using (var iconStream = typeof(MainForm).Assembly.GetManifestResourceStream("ScreenSaverTool.screensaver.ico"))
            {
                if (iconStream != null) this.Icon = new Icon(iconStream);
            }
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            var lblTitle = new Label
            {
                Text = "桌面背景 / 锁屏 / 屏保设置工具",
                Location = new Point(12, 12),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            txtImagePath = new TextBox
            {
                Location = new Point(12, 45),
                Size = new Size(540, 23),
                ReadOnly = true
            };

            btnSelect = new Button
            {
                Text = "选择图片",
                Location = new Point(560, 43),
                Size = new Size(128, 27)
            };
            btnSelect.Click += btnSelect_Click;

            btnSetBg = new Button
            {
                Text = "设置背景",
                Location = new Point(12, 90),
                Size = new Size(150, 30)
            };
            btnSetBg.Click += btnSetBg_Click;

            btnProtect = new Button
            {
                Text = "功能保护：未配置",
                Location = new Point(180, 90),
                Size = new Size(200, 30),
                BackColor = SystemColors.Control
            };
            btnProtect.Click += btnProtect_Click;

            btnNoLockScreen = new Button
            {
                Text = "不显示锁屏：未配置",
                Location = new Point(12, 130),
                Size = new Size(676, 30),
                BackColor = SystemColors.Control
            };
            btnNoLockScreen.Click += btnNoLockScreen_Click;

            btnScrSaver = new Button
            {
                Text = "启用屏幕保护程序：已禁用",
                Location = new Point(12, 170),
                Size = new Size(676, 30),
                BackColor = Color.LightGreen
            };
            btnScrSaver.Click += btnScrSaver_Click;

            lblStatus = new Label
            {
                Text = "就绪。",
                Location = new Point(12, 210),
                AutoSize = false,
                Size = new Size(676, 50)
            };

            this.Controls.AddRange(new Control[] { lblTitle, txtImagePath, btnSelect, btnSetBg, btnProtect, btnNoLockScreen, btnScrSaver, lblStatus });
        }

        #region 加载时初始化

        private void MainForm_Load(object sender, EventArgs e)
        {
            // (0) 若 exe 运行根目录存在“公司背景图.jpg”，则将其路径预填为待设置背景图片路径
            string defaultBg = Path.Combine(Application.StartupPath, "公司背景图.jpg");
            if (File.Exists(defaultBg))
            {
                txtImagePath.Text = defaultBg;
            }

            // (1) 加载时即将三项个性化策略设为“已启用”
            SetProtection(true);

            // (1a) 加载时将“带密码的屏幕保护程序”设为“已禁用”
            //      与功能保护同属用户配置，紧邻写入，避开后续计算机配置 GPO 操作对 .pol 落盘的干扰
            SetScreenSaverSecure(false);

            // (1b) 加载时即将“不显示锁屏”计算机策略设为“已启用”
            SetNoLockScreen(true);

            // (2) 屏幕保护程序设为“无”，并取消“在恢复时显示登录屏幕”
            ResetScreenSaverToNone();

            // (3) 彻底阻止电脑自动进入锁屏界面（禁用“计算机不活动限制”锁屏）
            PreventAutoLockScreen();

            // (4) 加载时即将“启用屏幕保护程序”设为“已禁用”
            SetScreenSaverPolicy(true);

            // (6) 强制刷新本地组策略，确保 gpedit 与系统一致反映上述设置
            RunGpUpdate();

            lblStatus.Text = "已初始化：保护策略 = 已启用；不显示锁屏 = 已启用；屏幕保护程序 = 无；自动锁屏 = 已阻止；启用屏幕保护程序 = 已禁用；带密码屏保 = 已禁用。";
        }

        /// <summary>
        /// 将屏幕保护程序设置为“无”，并取消“在恢复时显示登录屏幕”。
        /// 对应注册表：HKCU\Control Panel\Desktop
        ///   SCRNSAVE.EXE      -> 空（无屏保程序）
        ///   ScreenSaveActive  -> "0"（不启用）
        ///   ScreenSaverIsSecure -> "0"（恢复时不显示登录屏幕）
        /// </summary>
        private void ResetScreenSaverToNone()
        {
            try
            {
                Registry.SetValue(Desk, "SCRNSAVE.EXE", "", RegistryValueKind.String);
                Registry.SetValue(Desk, "ScreenSaveActive", "0", RegistryValueKind.String);
                Registry.SetValue(Desk, "ScreenSaverIsSecure", "0", RegistryValueKind.String);
                Native.SetScreenSaverActive(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show("重置屏幕保护程序设置失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 彻底阻止电脑因长时间无操作而自动进入锁屏界面。防护链：
        ///   (a) 屏保设为“无”且恢复时不显示登录屏幕（ResetScreenSaverToNone，已调用）；
        ///   (b) 不显示锁屏（NoLockScreen=1，SetNoLockScreen(true)，已调用）；
        ///   (c) 禁用“交互式登录：计算机不活动限制”（InactivityTimeoutSecs=0）；
        ///   (d) 禁用“唤醒时需要密码”（CONSOLELOCK=0）——关屏/睡眠后回到锁屏的真正主因。
        /// (c)(d) 同时写入 GPO 计算机段（gpedit 显示）与 HKLM 实时值，并用 powercfg 立即生效。
        /// 注意：计算机配置写入 HKLM，需要管理员权限。
        /// </summary>
        private void PreventAutoLockScreen()
        {
            string gpoError = null;

            // (c) 禁用“计算机不活动限制”
            const string inactGpoSubKey = @"Software\Policies\Microsoft\Windows\System";
            const string inactLiveSubKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System";
            const string inactValueName = "InactivityTimeoutSecs";
            try
            {
                GroupPolicyHelper.SetMachinePolicyValue(inactGpoSubKey, inactValueName, 0, RegistryValueKind.DWord);
            }
            catch (Exception ex) { gpoError = ex.Message; }
            try
            {
                Registry.SetValue("HKEY_LOCAL_MACHINE\\" + inactLiveSubKey, inactValueName, 0, RegistryValueKind.DWord);
            }
            catch (Exception ex)
            {
                MessageBox.Show("设置阻止自动锁屏失败（需管理员权限）：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // (d) 禁用“唤醒时需要密码”（电源 CONSOLELOCK=0）。这是关屏/睡眠后回锁屏的真正主因。
            const string wakeGpoSubKey = @"Software\Policies\Microsoft\Power\PowerSettings\0e796bdb-100d-47d6-a2d5-f7d2daa51f51";
            try
            {
                GroupPolicyHelper.SetMachinePolicyValue(wakeGpoSubKey, "ACSettingIndex", 0, RegistryValueKind.DWord);
                GroupPolicyHelper.SetMachinePolicyValue(wakeGpoSubKey, "DCSettingIndex", 0, RegistryValueKind.DWord);
            }
            catch (Exception ex) { gpoError = (gpoError == null ? "" : gpoError + "\n") + ex.Message; }

            // 用 powercfg 直接修改当前电源计划，最可靠地立即生效（需要管理员）
            try
            {
                RunPowerCfg("powercfg /setacvalueindex SCHEME_CURRENT SUB_NONE CONSOLELOCK 0");
                RunPowerCfg("powercfg /setdcvalueindex SCHEME_CURRENT SUB_NONE CONSOLELOCK 0");
                RunPowerCfg("powercfg /setactive SCHEME_CURRENT");
            }
            catch (Exception ex)
            {
                gpoError = (gpoError == null ? "" : gpoError + "\n") + "powercfg 执行失败：" + ex.Message;
            }

            if (!string.IsNullOrEmpty(gpoError))
            {
                MessageBox.Show("部分锁屏策略写入失败（多为权限不足）：\n" + gpoError +
                                "\n\n请务必以管理员身份运行本程序，否则计算机配置（不显示锁屏 / 不活动限制 / 唤醒密码）无法生效，电脑仍会自动锁屏。",
                                "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// 静默执行 powercfg 命令（修改当前电源计划，立即生效）。
        /// </summary>
        private void RunPowerCfg(string args)
        {
            var psi = new System.Diagnostics.ProcessStartInfo("powercfg", args)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            using (var p = System.Diagnostics.Process.Start(psi))
            {
                p.WaitForExit();
            }
        }

        /// <summary>
        /// 静默执行 gpupdate /force，使系统立即应用本地组策略（gpedit 重新打开/刷新后即显示最新状态）。
        /// 后台启动，不阻塞界面加载。
        /// </summary>
        private void RunGpUpdate()
        {
            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo("gpupdate", "/force")
                {
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                System.Diagnostics.Process.Start(psi); // 不 WaitForExit，后台刷新即可
            }
            catch
            {
                // gpupdate 不可用时忽略，已写入的注册表仍然生效
            }
        }

        #endregion

        #region 功能保护按钮（组策略开关）

        /// <summary>
        /// enable=true：将四项策略设为“已启用”，按钮变绿；
        /// enable=false：初始化为“未配置”，按钮恢复默认色。
        /// 会同时更新本地组策略（gpedit 显示）和 HKCU 注册表（立即生效）。
        /// </summary>
        private void SetProtection(bool enable)
        {
            string gpoError = null;

            // 1) 更新本地组策略 .pol 文件，使 gpedit.msc 显示正确
            try
            {
                foreach (var p in ProtectionPolicies)
                {
                    if (enable)
                        GroupPolicyHelper.SetUserPolicyValue(p.SubKeyPath, p.ValueName, p.Value, p.Kind);
                    else
                        GroupPolicyHelper.DeleteUserPolicyValue(p.SubKeyPath, p.ValueName);
                }
            }
            catch (Exception ex)
            {
                gpoError = ex.Message;
            }

            // 2) 写入/删除 HKCU 注册表，让策略立即生效
            try
            {
                foreach (var p in ProtectionPolicies)
                {
                    if (enable)
                        GroupPolicyHelper.SetLiveRegistryValue(p.SubKeyPath, p.ValueName, p.Value, p.Kind);
                    else
                        GroupPolicyHelper.DeleteLiveRegistryValue(p.SubKeyPath, p.ValueName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("设置注册表失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!string.IsNullOrEmpty(gpoError))
            {
                MessageBox.Show("本地组策略写入失败（gpedit 可能仍显示为未配置）：" + gpoError +
                                "\n\n但 HKCU 注册表已写入，功能保护实际已生效。若要让 gpedit 显示一致，请尝试以管理员身份运行本程序。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            isProtected = enable;
            btnProtect.BackColor = enable ? Color.LightGreen : SystemColors.Control;
            btnProtect.Text = enable ? "功能保护：已启用" : "功能保护：未配置";
        }

        private void btnProtect_Click(object sender, EventArgs e)
        {
            SetProtection(!isProtected);
        }

        #endregion

        #region 不显示锁屏按钮（计算机配置组策略开关）

        /// <summary>
        /// enable=true：计算机配置 -> 管理模板 -> 控制面板 -> 个性化 -> 不显示锁屏 = 已启用 (NoLockScreen=1)，按钮变绿；
        /// enable=false：初始化为“未配置”（删除 NoLockScreen 值），按钮恢复默认色。
        /// 同时更新本地组策略计算机段（gpedit 显示）和 HKLM 注册表（立即生效）。
        /// 注意：计算机配置写入 HKLM 与 Machine\Registry.pol，需要管理员权限。
        /// </summary>
        private void SetNoLockScreen(bool enable)
        {
            string gpoError = null;

            // 1) 更新本地组策略计算机段 .pol 文件，使 gpedit.msc 显示正确
            try
            {
                if (enable)
                    GroupPolicyHelper.SetMachinePolicyValue(MachinePersonalization, "NoLockScreen", 1, RegistryValueKind.DWord);
                else
                    GroupPolicyHelper.DeleteMachinePolicyValue(MachinePersonalization, "NoLockScreen");
            }
            catch (Exception ex)
            {
                gpoError = ex.Message;
            }

            // 2) 写入/删除 HKLM 注册表，让策略立即生效
            try
            {
                if (enable)
                    GroupPolicyHelper.SetLiveMachineValue(MachinePersonalization, "NoLockScreen", 1, RegistryValueKind.DWord);
                else
                    GroupPolicyHelper.DeleteLiveMachineValue(MachinePersonalization, "NoLockScreen");
            }
            catch (Exception ex)
            {
                MessageBox.Show("设置 HKLM 注册表失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!string.IsNullOrEmpty(gpoError))
            {
                MessageBox.Show("本地组策略(计算机配置)写入失败（gpedit 可能仍显示为未配置）：" + gpoError +
                                "\n\n但 HKLM 注册表已写入，设置实际已生效。若要让 gpedit 显示一致，请务必以管理员身份运行本程序。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            isNoLockScreen = enable;
            btnNoLockScreen.BackColor = enable ? Color.LightGreen : SystemColors.Control;
            btnNoLockScreen.Text = enable ? "不显示锁屏：已启用" : "不显示锁屏：未配置";
        }

        private void btnNoLockScreen_Click(object sender, EventArgs e)
        {
            SetNoLockScreen(!isNoLockScreen);
        }

        #endregion

        #region 启用屏幕保护程序策略（用户配置组策略开关）

        // 用户配置 -> 管理模板 -> 控制面板 -> 个性化 -> 启用屏幕保护程序
        // 注册表：HKCU\Software\Policies\Microsoft\Windows\Control Panel\Desktop\ScreenSaveActive (REG_SZ)
        //   已禁用="0"，未配置=删除值（本开关只在“已禁用”与“未配置”之间切换）
        private const string ScrSaverPolSubKey = @"Software\Policies\Microsoft\Windows\Control Panel\Desktop";
        private const string ScrSaverPolValue = "ScreenSaveActive";

        /// <summary>
        /// disabled=true：用户配置 -> 管理模板 -> 控制面板 -> 个性化 -> 启用屏幕保护程序 = 已禁用 (ScreenSaveActive="0")，按钮变绿；
        /// disabled=false：= 未配置（删除该策略值，不强制屏保状态），按钮恢复默认色。
        /// 同时更新本地组策略用户段（gpedit 显示）与 HKCU 注册表（立即生效）。
        /// 注意：这是用户配置，普通用户权限即可。
        /// </summary>
        private void SetScreenSaverPolicy(bool disabled)
        {
            string gpoError = null;

            if (disabled)
            {
                // 已禁用：写入 ScreenSaveActive="0"
                try
                {
                    GroupPolicyHelper.SetUserPolicyValue(ScrSaverPolSubKey, ScrSaverPolValue, "0", RegistryValueKind.String);
                }
                catch (Exception ex) { gpoError = ex.Message; }

                try
                {
                    Registry.SetValue("HKEY_CURRENT_USER\\" + ScrSaverPolSubKey, ScrSaverPolValue, "0", RegistryValueKind.String);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("设置注册表失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                // 未配置：删除该策略值（GPO .pol 与 HKCU 实时值都删）
                try
                {
                    GroupPolicyHelper.DeleteUserPolicyValue(ScrSaverPolSubKey, ScrSaverPolValue);
                }
                catch (Exception ex) { gpoError = ex.Message; }

                try
                {
                    using (var key = Registry.CurrentUser.OpenSubKey(ScrSaverPolSubKey, true))
                    {
                        if (key != null && key.GetValue(ScrSaverPolValue) != null)
                            key.DeleteValue(ScrSaverPolValue, false);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("设置注册表失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (!string.IsNullOrEmpty(gpoError))
            {
                MessageBox.Show("本地组策略写入失败（gpedit 可能仍显示为未配置）：" + gpoError +
                                "\n\n但 HKCU 注册表已写入，设置实际已生效。若要让 gpedit 显示一致，请尝试以管理员身份运行本程序。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            scrSaverDisabled = disabled;
            btnScrSaver.BackColor = disabled ? Color.LightGreen : SystemColors.Control;
            btnScrSaver.Text = disabled ? "启用屏幕保护程序：已禁用" : "启用屏幕保护程序：未配置";
        }

        private void btnScrSaver_Click(object sender, EventArgs e)
        {
            SetScreenSaverPolicy(!scrSaverDisabled);
        }

        #endregion

        #region 带密码的屏幕保护程序（加载时设为“已禁用”）

        // 用户配置 -> 管理模板 -> 控制面板 -> 个性化 -> 带密码的屏幕保护程序
        // 注册表：HKCU\Software\Policies\Microsoft\Windows\Control Panel\Desktop\ScreenSaverIsSecure
        //   类型必须是 **REG_SZ**（ADMX CPL_Personalization_ScreenSaverIsSecure 用 <string> 标签），
        //   已启用="1"（屏保恢复时要求密码）、已禁用="0"（屏保恢复时不要求密码）、未配置=删除值。
        //   写 REG_DWORD 0 会被 gpedit 判定为不匹配这条策略 → 显示"未设置"。
        private const string ScrSecureSubKey = @"Software\Policies\Microsoft\Windows\Control Panel\Desktop";
        private const string ScrSecureValue = "ScreenSaverIsSecure";

        /// <summary>
        /// secure=true  -> 带密码的屏幕保护程序 = 已启用 (ScreenSaverIsSecure=1)，屏保恢复时要求输入密码；
        /// secure=false -> = 已禁用 (ScreenSaverIsSecure=0)，屏保恢复时不要求密码。
        /// 同时更新本地组策略用户段（gpedit 显示）与 HKCU 注册表（立即生效）。
        /// 注：此策略已从 ProtectionPolicies 移除，目前仅在程序加载时设为“已禁用”，不提供按钮切换。
        /// </summary>
        private void SetScreenSaverSecure(bool secure)
        {
            string gpoError = null;

            // 1) 更新本地组策略用户段 .pol 文件，使 gpedit.msc 显示正确
            try
            {
                    GroupPolicyHelper.SetUserPolicyValue(ScrSecureSubKey, ScrSecureValue,
                        secure ? "1" : "0", RegistryValueKind.String);
            }
            catch (Exception ex)
            {
                gpoError = ex.Message;
            }

            // 2) 写入 HKCU 注册表，让策略立即生效
            try
            {
                Registry.SetValue("HKEY_CURRENT_USER\\" + ScrSecureSubKey, ScrSecureValue,
                    secure ? "1" : "0", RegistryValueKind.String);
            }
            catch (Exception ex)
            {
                MessageBox.Show("设置注册表失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!string.IsNullOrEmpty(gpoError))
            {
                MessageBox.Show("本地组策略写入失败（gpedit 可能仍显示为未配置）：" + gpoError +
                                "\n\n但 HKCU 注册表已写入，设置实际已生效。若要让 gpedit 显示一致，请尝试以管理员身份运行本程序。",
                                "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // 校验：读取实时注册表值 + 本地组策略 .pol 文件，确认是否真正写入成功。
            // 之前只校验实时注册表，导致 .pol 写入失败时出现“无报错、gpedit 也无变化”的静默失败，
            // 正是“程序启动后仍不是已禁用、也没弹出提示”的根因。现在 .pol 写入失败会主动弹窗提示。
            try
            {
                object live = Registry.GetValue("HKEY_CURRENT_USER\\" + ScrSecureSubKey, ScrSecureValue, null);
                if (!object.Equals(live, "0"))
                {
                    MessageBox.Show("提示：“带密码的屏幕保护程序”实时注册表值未能写入为 REG_SZ \"0\"（当前值=" + (live == null ? "空" : live.ToString()) +
                                    "）。gpedit 可能仍不显示“已禁用”。请确认以管理员身份运行本程序。",
                                    "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    // 实时值 OK，再校验 .pol 文件（gpedit 实际读取处）
                    object pol = GroupPolicyHelper.GetUserPolicyValue(ScrSecureSubKey, ScrSecureValue);
                    if (!object.Equals(pol, "0"))
                    {
                        MessageBox.Show("注意：实时注册表已写入 \"0\"，但本地组策略 .pol 文件中“带密码的屏幕保护程序”未正确写入为已禁用" +
                                        "（.pol 当前值=" + (pol == null ? "不存在" : pol.ToString()) + "）。\n" +
                                        "gpedit 仍可能显示“未配置/已启用”。请确认以管理员身份运行本程序，且程序未被 32 位文件系统重定向干扰。",
                                        "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch
            {
                // 忽略读取异常
            }
        }

        #endregion

        #region 选择图片 / 设置背景

        private void btnSelect_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "图片文件 (*.jpg;*.jpeg;*.png;*.bmp;*.gif)|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                dlg.Title = "选择背景图片";
                if (dlg.ShowDialog() == DialogResult.OK)
                    txtImagePath.Text = dlg.FileName;
            }
        }

        private void btnSetBg_Click(object sender, EventArgs e)
        {
            string src = txtImagePath.Text.Trim();
            if (string.IsNullOrEmpty(src) || !File.Exists(src))
            {
                MessageBox.Show("请先选择有效的图片文件。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnSetBg.Enabled = false;
            try
            {
                // 复制到稳定路径后再设置，避免原图被移动/删除
                string path = Native.CopyToWorkDir(src);

                Native.SetWallpaper(path);
                lblStatus.Text = "桌面背景已设置。";

                try
                {
                    Native.SetLockScreen(path);
                    lblStatus.Text += " 锁屏背景已设置。";
                }
                catch (Exception ex)
                {
                    lblStatus.Text += " 锁屏设置失败：" + ex.Message;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("设置失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSetBg.Enabled = true;
            }
        }

        #endregion
    }
}
