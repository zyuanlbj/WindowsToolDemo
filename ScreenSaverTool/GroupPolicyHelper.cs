using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;

namespace ScreenSaverTool
{
    /// <summary>
    /// 通过直接写入本地组策略的 Registry.pol 二进制文件，使 gpedit.msc 正确显示策略状态。
    ///
    /// 关键事实（已在本机取证确认）：
    ///   1) 本机所有真实 .pol 文件（System32\GroupPolicy\User|Machine\Registry.pol、域 GPO）均为
    ///      **PReg 格式**（头部 "PReg" + 4 字节版本号），gpedit 只读这种格式、且只读
    ///      %SystemRoot%\System32\GroupPolicy\... 下的文件。
    ///   2) 程序若以 32 位进程运行（WinForms 默认 AnyCPU+“优先 32 位”），访问 System32 会被
    ///      文件系统重定向到 SysWOW64，写出的 .pol 在 SysWOW64\GroupPolicy\...，gpedit 看不到。
    ///      因此本类用 Wow64DisableWow64FsRedirection 关闭重定向，确保写进真正的 System32。
    ///
    /// PReg 二进制格式（全部 UTF-16LE，“;”为字段分隔符，“[”/“]”为条目起止）：
    ///   头部   : ASCII "PReg" + 4 字节版本号(=1)
    ///   每条策略:
    ///     "["  (UTF-16: 5B 00)
    ///     键名 (UTF-16LE, 含 00 00 结尾)
    ///     ";"  (UTF-16: 3B 00)
    ///     值名 (UTF-16LE, 含 00 00 结尾)
    ///     ";"  (3B 00)
    ///     4 字节类型 (REG_DWORD=4, REG_SZ=1 ...)
    ///     ";"  (3B 00)
    ///     4 字节数据长度
    ///     ";"  (3B 00)
    ///     数据
    ///     "]"  (UTF-16: 5D 00)
    ///
    /// 同时提供直接写 HKCU / HKLM 注册表的方法，让策略立即生效（不依赖 gpupdate）。
    /// 写 .pol 需管理员权限（程序已通过 app.manifest 提权）。
    /// </summary>
    public static class GroupPolicyHelper
    {
        #region 用户配置（HKCU）

        public static void SetUserPolicyValue(string subKeyPath, string valueName, object value, RegistryValueKind kind)
            => RegistryPol.SetUserValue(subKeyPath, valueName, kind, value);

        /// <summary>
        /// 读取本地组策略用户段 .pol 中某个策略的当前值（用于写回校验，确认 gpedit 实际能看到的内容）。
        /// REG_DWORD 返回 int；其它类型按字符串返回；不存在返回 null。
        /// </summary>
        public static object GetUserPolicyValue(string subKeyPath, string valueName)
            => RegistryPol.GetUserValue(subKeyPath, valueName);

        public static void DeleteUserPolicyValue(string subKeyPath, string valueName)
            => RegistryPol.DeleteUserValue(subKeyPath, valueName);

        public static void SetLiveRegistryValue(string subKeyPath, string valueName, object value, RegistryValueKind kind)
            => Registry.SetValue("HKEY_CURRENT_USER\\" + subKeyPath, valueName, value, kind);

        public static void DeleteLiveRegistryValue(string subKeyPath, string valueName)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(subKeyPath, true))
            {
                if (key != null && key.GetValue(valueName) != null)
                    key.DeleteValue(valueName, false);
            }
        }

        #endregion

        #region 计算机配置（HKLM）

        public static void SetMachinePolicyValue(string subKeyPath, string valueName, object value, RegistryValueKind kind)
            => RegistryPol.SetMachineValue(subKeyPath, valueName, kind, value);

        /// <summary>
        /// 读取本地组策略计算机段 .pol 中某个策略的当前值（用于写回校验）。
        /// </summary>
        public static object GetMachinePolicyValue(string subKeyPath, string valueName)
            => RegistryPol.GetMachineValue(subKeyPath, valueName);

        public static void DeleteMachinePolicyValue(string subKeyPath, string valueName)
            => RegistryPol.DeleteMachineValue(subKeyPath, valueName);

        public static void SetLiveMachineValue(string subKeyPath, string valueName, object value, RegistryValueKind kind)
            => Registry.SetValue("HKEY_LOCAL_MACHINE\\" + subKeyPath, valueName, value, kind);

        public static void DeleteLiveMachineValue(string subKeyPath, string valueName)
        {
            using (var key = Registry.LocalMachine.OpenSubKey(subKeyPath, true))
            {
                if (key != null && key.GetValue(valueName) != null)
                    key.DeleteValue(valueName, false);
            }
        }

        #endregion
    }

    /// <summary>
    /// 直接读写本地组策略的 Registry.pol 二进制文件（PReg 格式）。
    /// 关键修正：
    ///   (1) 头部用 "PReg"（不是 "[Pol"），每条策略用 [ ; ; ; ; ] 分隔符（不是单字节 ';'）。
    ///   (2) 用 Wow64DisableWow64FsRedirection 关闭 32 位进程的文件系统重定向，
    ///       保证 .pol 写入真实 %SystemRoot%\System32\GroupPolicy\...（gpedit 实际读取处）。
    /// </summary>
    internal static class RegistryPol
    {
        private static string UserPolPath =>
            Path.Combine(Environment.GetEnvironmentVariable("SystemRoot") ?? "C:\\Windows",
                         @"System32\GroupPolicy\User\Registry.pol");

        private static string MachinePolPath =>
            Path.Combine(Environment.GetEnvironmentVariable("SystemRoot") ?? "C:\\Windows",
                         @"System32\GroupPolicy\Machine\Registry.pol");

        public static void SetUserValue(string key, string value, RegistryValueKind kind, object data)
            => WritePol(UserPolPath, key, value, kind, data);

        public static void SetMachineValue(string key, string value, RegistryValueKind kind, object data)
            => WritePol(MachinePolPath, key, value, kind, data);

        public static object GetUserValue(string key, string value)
        {
            object result = null;
            WithRealSystem32(() => { result = ReadEntryData(UserPolPath, key, value); });
            return result;
        }

        public static object GetMachineValue(string key, string value)
        {
            object result = null;
            WithRealSystem32(() => { result = ReadEntryData(MachinePolPath, key, value); });
            return result;
        }

        private static object ReadEntryData(string path, string key, string value)
        {
            foreach (var e in ReadEntries(path))
            {
                if (e.Key == key && e.Value == value)
                {
                    if (e.Type == 0x00000004) // REG_DWORD
                        return e.Data.Length >= 4 ? (object)BitConverter.ToInt32(e.Data, 0) : 0;
                    return Encoding.Unicode.GetString(e.Data).TrimEnd('\0');
                }
            }
            return null;
        }

        public static void DeleteUserValue(string key, string value)
            => DeletePol(UserPolPath, key, value);

        public static void DeleteMachineValue(string key, string value)
            => DeletePol(MachinePolPath, key, value);

        #region WOW64 文件系统重定向（确保写进真正的 System32）

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool Wow64DisableWow64FsRedirection(out IntPtr oldValue);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool Wow64RevertWow64FsRedirection(IntPtr oldValue);

        private static void WithRealSystem32(Action action)
        {
            IntPtr old = IntPtr.Zero;
            bool disabled = false;
            // 仅当“运行在 64 位系统上的 32 位进程”时才需要关闭重定向
            if (Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess)
                disabled = Wow64DisableWow64FsRedirection(out old);
            try
            {
                action();
            }
            finally
            {
                if (disabled)
                    Wow64RevertWow64FsRedirection(old);
            }
        }

        #endregion

        private static void WritePol(string path, string key, string value, RegistryValueKind kind, object data)
        {
            WithRealSystem32(() =>
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                var entries = ReadEntries(path);
                // 移除同名项（若有），以最新写入为准
                entries.RemoveAll(e => e.Key == key && e.Value == value);
                entries.Add(new PolEntry
                {
                    Key = key,
                    Value = value,
                    Type = (uint)ToPolType(kind),
                    Data = ToPolBytes(data, kind)
                });
                WriteEntries(path, entries);
            });
        }

        private static void DeletePol(string path, string key, string value)
        {
            WithRealSystem32(() =>
            {
                if (!File.Exists(path)) return;
                var entries = ReadEntries(path);
                entries.RemoveAll(e => e.Key == key && e.Value == value);
                WriteEntries(path, entries);
            });
        }

        private struct PolEntry
        {
            public string Key;
            public string Value;
            public uint Type;
            public byte[] Data;
        }

        private static List<PolEntry> ReadEntries(string path)
        {
            var list = new List<PolEntry>();
            if (!File.Exists(path)) return list;

            byte[] bytes = File.ReadAllBytes(path);
            // 头部：PReg + 4 字节版本号
            if (bytes.Length < 8 ||
                bytes[0] != (byte)'P' || bytes[1] != (byte)'R' ||
                bytes[2] != (byte)'e' || bytes[3] != (byte)'g')
            {
                return list; // 非 PReg 文件，放弃解析
            }
            int i = 8; // 跳过 PReg + 版本号

            while (i < bytes.Length)
            {
                // 跳过条目起始符 '[' (5B 00)
                if (i + 1 < bytes.Length && bytes[i] == 0x5B && bytes[i + 1] == 0x00) i += 2;
                else if (bytes[i] == (byte)']') break; // 兜底：遇到 ']' 结束

                string key = ReadUtf16Null(ref bytes, ref i);
                if (key == null) break;
                SkipDelim(ref bytes, ref i);              // ';'
                string val = ReadUtf16Null(ref bytes, ref i);
                if (val == null) break;
                SkipDelim(ref bytes, ref i);              // ';'
                if (i + 4 > bytes.Length) break;
                uint type = BitConverter.ToUInt32(bytes, i); i += 4;
                SkipDelim(ref bytes, ref i);              // ';'
                if (i + 4 > bytes.Length) break;
                uint size = BitConverter.ToUInt32(bytes, i); i += 4;
                SkipDelim(ref bytes, ref i);              // ';'
                if ((long)i + size > bytes.Length) break;
                byte[] data = new byte[size];
                Array.Copy(bytes, i, data, 0, (int)size);
                i += (int)size;
                // 跳过条目结束符 ']' (5D 00)
                if (i + 1 < bytes.Length && bytes[i] == 0x5D && bytes[i + 1] == 0x00) i += 2;

                list.Add(new PolEntry { Key = key, Value = val, Type = type, Data = data });
            }
            return list;
        }

        private static void SkipDelim(ref byte[] bytes, ref int i)
        {
            // 字段分隔符 ';' 以 UTF-16 存储 (3B 00)
            if (i + 1 < bytes.Length && bytes[i] == 0x3B && bytes[i + 1] == 0x00) i += 2;
        }

        private static string ReadUtf16Null(ref byte[] bytes, ref int i)
        {
            int start = i;
            while (i + 1 < bytes.Length)
            {
                if (bytes[i] == 0 && bytes[i + 1] == 0)
                {
                    i += 2;
                    return Encoding.Unicode.GetString(bytes, start, i - 2 - start);
                }
                i += 2;
            }
            return null;
        }

        private static void WriteEntries(string path, List<PolEntry> entries)
        {
            using (var ms = new MemoryStream())
            using (var w = new BinaryWriter(ms))
            {
                // 头部：PReg + 版本号 1
                w.Write(new byte[] { (byte)'P', (byte)'R', (byte)'e', (byte)'g' });
                w.Write((uint)1);

                foreach (var e in entries)
                {
                    w.Write((byte)0x5B); w.Write((byte)0x00);   // '['
                    WriteUtf16Null(w, e.Key);
                    w.Write((byte)0x3B); w.Write((byte)0x00);   // ';'
                    WriteUtf16Null(w, e.Value);
                    w.Write((byte)0x3B); w.Write((byte)0x00);   // ';'
                    w.Write(e.Type);                            // 4 字节类型
                    w.Write((byte)0x3B); w.Write((byte)0x00);   // ';'
                    w.Write((uint)e.Data.Length);               // 4 字节长度
                    w.Write((byte)0x3B); w.Write((byte)0x00);   // ';'
                    w.Write(e.Data);                            // 数据
                    w.Write((byte)0x5D); w.Write((byte)0x00);   // ']'
                }

                File.WriteAllBytes(path, ms.ToArray());
            }
        }

        private static void WriteUtf16Null(BinaryWriter w, string s)
        {
            byte[] b = Encoding.Unicode.GetBytes(s ?? string.Empty);
            w.Write(b);
            w.Write((byte)0); w.Write((byte)0); // null 结尾（2 字节）
        }

        private static uint ToPolType(RegistryValueKind kind)
        {
            switch (kind)
            {
                case RegistryValueKind.String: return 0x00000001;
                case RegistryValueKind.ExpandString: return 0x00000002;
                case RegistryValueKind.Binary: return 0x00000003;
                case RegistryValueKind.DWord: return 0x00000004;
                case RegistryValueKind.MultiString: return 0x00000007;
                case RegistryValueKind.QWord: return 0x0000000B;
                default: return 0x00000004;
            }
        }

        private static byte[] ToPolBytes(object value, RegistryValueKind kind)
        {
            switch (kind)
            {
                case RegistryValueKind.DWord:
                    return BitConverter.GetBytes(Convert.ToInt32(value));
                case RegistryValueKind.QWord:
                    return BitConverter.GetBytes(Convert.ToInt64(value));
                case RegistryValueKind.String:
                case RegistryValueKind.ExpandString:
                    return Encoding.Unicode.GetBytes((value?.ToString() ?? string.Empty) + "\0");
                case RegistryValueKind.Binary:
                    return (byte[])value;
                case RegistryValueKind.MultiString:
                    var sb = new StringBuilder();
                    foreach (var part in (string[])value) { sb.Append(part); sb.Append('\0'); }
                    sb.Append('\0');
                    return Encoding.Unicode.GetBytes(sb.ToString());
                default:
                    return new byte[0];
            }
        }
    }
}
