# WindowsToolDemo

一组** Windows 桌面小工具**（C# / WinForms），用于现场快速配置设备的网络与系统显示策略。
目前包含两个独立工具：**IPSetter**（设置有线网卡 IP）与 **ScreenSaverTool**（设置壁纸 / 锁屏 / 屏保）。

> 标签：`C#` `WinForms` `WMI` `组策略` `Windows` `上位机` `GitHub`

---

## 功能特性

- [x] **IPSetter**：扫描本机有线网卡，一键设置静态 IP / 子网掩码 / 默认网关，或切回 DHCP
- [x] **IPSetter**：自动过滤虚拟/无线适配器（Hyper-V、VMware、VPN、Wi-Fi 等），只列真实有线网卡
- [x] **IPSetter**：IPv4 格式校验 + 记忆上次填写值（持久化到 user.config）
- [x] **ScreenSaverTool**：选择图片设置桌面壁纸 + 锁屏背景
- [x] **ScreenSaverTool**：通过本地组策略（Registry.pol）固化「不显示锁屏 / 禁用屏保 / 阻止自动锁屏」
- [ ] 规划中：文件加密 / 解密工具
- [ ] 规划中：格式转换工具

---

## 技术栈

| 项目 | 说明 |
| --- | --- |
| 语言 | C#（.NET Framework） |
| UI | Windows Forms（界面全部代码生成，无设计器） |
| IPSetter 框架 | .NET Framework **4.7.2** |
| ScreenSaverTool 框架 | .NET Framework **4.8** |
| 关键技术 | WMI（`Win32_NetworkAdapter`）、注册表 + 本地组策略 PReg 二进制读写、Win32 API（`SystemParametersInfo`、`Wow64DisableWow64FsRedirection`）、`powercfg` |
| 构建 | Visual Studio 2022（含「.NET 桌面开发」工作负载） |

---

## 环境要求

- Windows 10 / 11
- 运行时：建议安装 **.NET Framework 4.8**（向下兼容 4.7.2）
- Visual Studio 2022（打开 `WindowsToolDemo.sln` 编译）
- **管理员权限**：`ScreenSaverTool` 需写计算机配置 / 电源策略（已通过 `app.manifest` 请求提权），请以管理员身份运行

---

## 快速开始

```bash
# 1. 克隆仓库（Gitee / GitHub 任选其一）
#    Gitee:
git clone https://gitee.com/zyuanlbj/WindowsToolDemo.git
#    GitHub:
git clone https://github.com/zyuanlbj/WindowsToolDemo.git

# 2. 用 VS2022 打开解决方案
WindowsToolDemo.sln

# 3. 生成解决方案（VS 打开一般自动还原，无需 NuGet 包）
#    生成 -> 生成解决方案（或 F6）

# 4. 运行
#    右键对应项目 -> 设为启动项目 -> F5
#    ScreenSaverTool 请「以管理员身份运行」，否则计算机配置类策略不生效
```

---

## 目录结构

```
WindowsToolDemo/
├─ WindowsToolDemo.sln        # 解决方案（含两个子项目）
├─ IPSetter/                  # 有线网卡 IP 设置工具
│  ├─ IPSetter.csproj
│  ├─ MainForm.cs             # 界面与交互
│  ├─ NetworkConfigurator.cs  # WMI 读写网卡配置（含虚拟适配器过滤）
│  ├─ Program.cs
│  ├─ app.manifest            # 提权清单
│  └─ ip.ico
└─ ScreenSaverTool/          # 壁纸 / 锁屏 / 屏保设置工具
   ├─ ScreenSaverTool.csproj
   ├─ MainForm.cs             # 界面与策略联动
   ├─ GroupPolicyHelper.cs    # 本地组策略 Registry.pol 二进制读写（PReg 格式）
   ├─ IGroupPolicyObject2.cs  # 组策略 COM 接口封装
   ├─ Native.cs               # Win32 API 封装（壁纸 / 屏保 / Wow64 重定向）
   ├─ Program.cs
   ├─ app.manifest            # 提权清单
   └─ screensaver.ico
```

---

## 界面预览

### IPSetter（有线网卡 IP 设置）

![IPSetter](docs/screenshots/ipsetter.png)

### ScreenSaverTool（屏保与背景设置）

![ScreenSaverTool](docs/screenshots/screensaver.png)

---

## 使用说明

### IPSetter（设置有线网卡 IP）

1. 启动后程序自动列出本机**有线网络适配器**。
2. 在「IP 地址 / 子网掩码 / 默认网关」填入目标值（默认已带常用占位值）。
3. 点 **应用静态 IP** 写入；点 **启用 DHCP** 切回自动获取。
4. 点 **刷新** 重新枚举适配器并刷新当前配置。

> 程序会过滤 Hyper-V / VMware / VPN / Wi-Fi 等虚拟与无线网卡，避免误改。

### ScreenSaverTool（壁纸 / 锁屏 / 屏保）

1. 程序**启动即应用一套预设**：保护策略启用、不显示锁屏、屏保设为「无」、阻止自动锁屏，并后台执行 `gpupdate /force`。
2. 点 **选择图片** 选取壁纸，再点 **设置背景** 应用到桌面壁纸 + 锁屏背景。
3. 三个策略开关可手动切换，状态实时反映到 `gpedit.msc`：
   - **功能保护**：禁用「外观 / 壁纸 / 屏保」设置页
   - **不显示锁屏**：计算机配置 → 个性化 → 不显示锁屏
   - **启用屏幕保护程序**：用户配置 → 个性化 → 设为「已禁用」
4. 所有策略同时写入本地组策略 `.pol` 文件（供 `gpedit.msc` 显示）与实时注册表（立即生效）。

> ⚠️ 该程序会修改**系统级组策略与电源策略**，建议在测试机 / 展示机上使用；生产环境慎用。
> 要用 `gpedit.msc` 验证效果，必须以管理员身份运行本程序，否则计算机配置段无法落盘。

---

## 技术亮点（作品集向）

- **本地组策略 PReg 二进制读写**：直接解析 / 生成 `Registry.pol`（`PReg` 头 + UTF-16LE 字段），让 `gpedit.msc` 正确显示策略状态，不依赖 `gpedit` UI。
- **SysWOW64 重定向处理**：32 位进程访问 `System32` 会被重定向到 `SysWOW64`，导致 `.pol` 写错位置、`gpedit` 看不到。已用 `Wow64DisableWow64FsRedirection` 关闭重定向，确保写进真正的 `System32\GroupPolicy\`。
- **防自动锁屏防护链**：屏保设为「无」 + 不显示锁屏 + 禁用「计算机不活动限制」+ 禁用「唤醒时需要密码（CONSOLELOCK）」+ `powercfg` 立即生效，多层兜底。

---

## 贡献

欢迎提 Issue 和 Pull Request。

1. 在 Gitee 或 GitHub 上 Fork 本仓库
2. 新建分支：`git switch -c feature/你的功能`
3. 提交：`git commit -m "feat: 新增 xxx"`
4. 推送并提 PR

> 提交信息推荐用 Conventional Commits：`feat` / `fix` / `docs` / `refactor`

---

## 许可证

[MIT](LICENSE)

---

## 联系方式

- 作者：zyuanlbj
- 邮箱：793194012@qq.com
- 备注：工业上位机 / PLC 通信相关交流欢迎联系
