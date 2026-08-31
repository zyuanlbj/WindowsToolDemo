# WindowsToolDemo

一组 **Windows 桌面小工具**（C# / WinForms），用于现场快速配置设备网络、系统显示策略，以及串口 / Modbus 设备调试。
目前包含四个独立工具：**IPSetter**（有线网卡 IP 设置）、**ScreenSaverTool**（壁纸 / 锁屏 / 屏保设置）、**SerialPortDemo**（串口调试助手）与 **ModbusRTUDemo**（Modbus RTU 主站调试）。

> 标签：`C#` `WinForms` `WMI` `组策略` `串口` `Modbus` `RS485` `Windows` `上位机` `GitHub`

---

## 功能特性

- **IPSetter**：扫描本机有线网卡，一键设置静态 IP / 子网掩码 / 默认网关或切回 DHCP，自动过滤虚拟与无线适配器并记忆上次配置。
- **ScreenSaverTool**：通过本地组策略设置桌面壁纸与锁屏背景，并固化「不显示锁屏 / 禁用屏保 / 阻止自动锁屏」等系统策略。
- **SerialPortDemo**：基于 .NET SerialPort 的串口调试助手，支持 ASCII/HEX 双模式收发、定时发送、接收暂停与快捷命令管理。
- **ModbusRTUDemo**：基于 RS485 串口的 Modbus RTU 主站调试工具，封装常用功能码（01/02/03/04/05/06/0F/10）与 CRC16 校验，支持多存储区读写与多种数据类型字节序组包。

- [ ] 规划中：文件加密 / 解密工具
- [ ] 规划中：格式转换工具

---

## 技术栈

| 项目                  | 说明                                                                                                                                                     |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| 语言                  | C#（.NET Framework）                                                                                                                                      |
| UI                  | Windows Forms（界面全部代码生成，无设计器）                                                                                                                            |
| IPSetter 框架         | .NET Framework **4.7.2**                                                                                                                                |
| ScreenSaverTool 框架  | .NET Framework **4.8**                                                                                                                                  |
| SerialPortDemo 框架   | .NET Framework **4.6**                                                                                                                                  |
| ModbusRTUDemo 框架    | .NET Framework **4.7.2**                                                                                                                                  |
| 关键技术                | WMI（`Win32_NetworkAdapter`）、注册表 + 本地组策略 PReg 二进制读写、Win32 API（`SystemParametersInfo`、`Wow64DisableWow64FsRedirection`）、`powercfg`、`.NET SerialPort`、`Modbus RTU / RS485（CRC16）` |
| 构建                  | Visual Studio 2022（含「.NET 桌面开发」工作负载）                                                                                                                |

---

## 环境要求

- Windows 10 / 11
- 运行时：建议安装 **.NET Framework 4.8**（向下兼容 4.6 及以上）
- Visual Studio 2022（打开 `WindowsToolDemo.sln` 编译）
- **管理员权限**：`ScreenSaverTool` 需写计算机配置 / 电源策略（已通过 `app.manifest` 请求提权），请以管理员身份运行
- **SerialPortDemo / ModbusRTUDemo 依赖**：`xbd.DataConvertLib` 为 NuGet 包，首次生成时自动还原

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

# 3. 生成解决方案（VS 打开一般自动还原 NuGet 包）
#    生成 -> 生成解决方案（或 F6）

# 4. 运行
#    右键对应项目 -> 设为启动项目 -> F5
#    ScreenSaverTool 请「以管理员身份运行」，否则计算机配置类策略不生效
```

---

## 目录结构

```
WindowsToolDemo/
├─ WindowsToolDemo.sln        # 解决方案（含四个子项目）
├─ IPSetter/                  # 有线网卡 IP 设置工具
│  ├─ IPSetter.csproj
│  ├─ MainForm.cs             # 界面与交互
│  ├─ NetworkConfigurator.cs  # WMI 读写网卡配置（含虚拟适配器过滤）
│  ├─ Program.cs
│  ├─ app.manifest            # 提权清单
│  └─ ip.ico
├─ ScreenSaverTool/          # 壁纸 / 锁屏 / 屏保设置工具
│  ├─ ScreenSaverTool.csproj
│  ├─ MainForm.cs             # 界面与策略联动
│  ├─ GroupPolicyHelper.cs    # 本地组策略 Registry.pol 二进制读写（PReg 格式）
│  ├─ IGroupPolicyObject2.cs  # 组策略 COM 接口封装
│  ├─ Native.cs               # Win32 API 封装（壁纸 / 屏保 / Wow64 重定向）
│  ├─ Program.cs
│  ├─ app.manifest            # 提权清单
│  └─ screensaver.ico
├─ SerialPortDemo/           # 串口调试助手
│  ├─ SerialPortDemo.csproj
│  ├─ FrmMain.cs              # 主窗体：串口参数配置、收发、定时发送
│  ├─ FrmMain.Designer.cs
│  ├─ FrmQuickSet.cs          # 快捷命令（命名 + HEX 内容校验）
│  ├─ ParityHelper.cs         # 校验辅助
│  ├─ Helper\HexHelper.cs     # HEX ⇄ ASCII 转换、字节拼接等
│  ├─ Program.cs
│  ├─ App.config
│  └─ USB.ico
└─ ModbusRTUDemo/            # Modbus RTU 主站调试（RS485 串口）
   ├─ ModbusRTUDemo.csproj
   ├─ FrmMain.cs             # 主窗体：从站地址、串口参数、读写操作、日志
   ├─ FrmMain.Designer.cs
   ├─ Helper\ModbusRtu.cs    # Modbus RTU 协议（功能码 01/02/03/04/05/06/0F/10 + CRC16）
   ├─ Helper\SimpleHybirdLock.cs # 串口读写互斥锁
   ├─ Program.cs
   ├─ App.config
   └─ system.ico
```

---

## 界面预览

### IPSetter（有线网卡 IP 设置）

![IPSetter](docs/screenshots/ipsetter.png)

### ScreenSaverTool（屏保与背景设置）

![ScreenSaverTool](docs/screenshots/screensaver.png)

### SerialPortDemo（串口调试助手）

![SerialPortDemo](docs/screenshots/serialport.png)

### ModbusRTUDemo（Modbus RTU 主站调试）

![ModbusRTUDemo](docs/screenshots/Modbusrtu.png)

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

### SerialPortDemo（串口调试助手）

1. 选择**串口号**，并设置**波特率 / 校验位 / 数据位 / 停止位 / RTS**（默认常用参数）。
2. 点 **打开串口** 建立连接；再次点 **关闭串口** 断开。
3. **发送**：在发送区输入文本或 HEX（勾选 HEX 模式），点发送；可开启**定时发送**循环下发。
4. **接收**：接收区实时显示，支持 ASCII / HEX 显示切换；可**暂停接收**与**清空**。
5. **快捷命令**：通过「快捷命令」管理常用指令（名称 + HEX 内容，自动校验格式），一键发送。
6. 底部状态栏显示发送 / 接收字节计数与连接状态。

> 依赖：`xbd.DataConvertLib`（NuGet 包，首次生成时自动还原）。

### ModbusRTUDemo（Modbus RTU 主站调试）

1. 选择**串口号**与**波特率 / 校验位 / 数据位 / 停止位**（默认 9600），填写**从站地址（Slave）**，点 **连接** 建立 RS485 串口链路。
2. 选择**存储区**：输出线圈 `0x`、输入状态 `1x`、保持寄存器 `4x`、输入寄存器 `3x`，并填写起始地址与长度。
3. 点 **读取** 下发对应功能码（01/02/03/04）并解析返回报文，结果在日志区显示。
4. 点 **写入** 执行单点 / 多点写操作（05/06/0F/10）；可写入 `short / ushort / int / uint / float` 及数组，按 **DataFormat** 字节序（ABCD 等）组包。
5. 底部日志区实时显示连接状态与收发报文，便于排查通信异常。

> 基于 RS485 半双工总线，调试时请确保接线（A/B）与终端电阻正确；更多协议基础见 `docs/md/01 Modbus基础.md`、`docs/md/02 ModbusRTU.md`。

---

## 技术亮点（作品集向）

- **本地组策略 PReg 二进制读写**：直接解析 / 生成 `Registry.pol`（`PReg` 头 + UTF-16LE 字段），让 `gpedit.msc` 正确显示策略状态，不依赖 `gpedit` UI。
- **SysWOW64 重定向处理**：32 位进程访问 `System32` 会被重定向到 `SysWOW64`，导致 `.pol` 写错位置、`gpedit` 看不到。已用 `Wow64DisableWow64FsRedirection` 关闭重定向，确保写进真正的 `System32\GroupPolicy\`。
- **防自动锁屏防护链**：屏保设为「无」 + 不显示锁屏 + 禁用「计算机不活动限制」+ 禁用「唤醒时需要密码（CONSOLELOCK）」+ `powercfg` 立即生效，多层兜底。
- **串口调试助手**：基于 `.NET SerialPort` 实现 ASCII / HEX 双模式收发、定时发送、接收暂停与字节计数；`HexHelper` 提供 HEX ⇄ ASCII 互转与字节拼接，`ParityHelper` 提供校验辅助。
- **Modbus RTU 协议栈**：自实现 8 个常用功能码（读 / 写线圈与寄存器）的报文拼接与解析、表驱动 CRC16 校验，并用互斥锁保证半双工总线单次交互原子性；`xbd.DataConvertLib` 负责多数据类型按字节序组包 / 拆包。

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
