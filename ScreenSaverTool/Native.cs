using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace ScreenSaverTool
{
    /// <summary>
    /// 本机 API 与注册表辅助方法（设置桌面壁纸、屏保、锁屏）。
    /// </summary>
    internal static class Native
    {
        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPI_SETSCREENSAVEACTIVE = 97;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDCHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        /// <summary>
        /// 设置桌面壁纸（路径会被系统读取并应用）。
        /// </summary>
        public static void SetWallpaper(string path)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true))
            {
                if (key != null)
                {
                    key.SetValue("WallpaperStyle", "10", RegistryValueKind.String); // 10 = 填充
                    key.SetValue("TileWallpaper", "0", RegistryValueKind.String);
                }
            }
            int result = SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
            if (result == 0)
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
        }

        /// <summary>
        /// 启用 / 停用屏幕保护程序。
        /// </summary>
        public static void SetScreenSaverActive(bool active)
        {
            SystemParametersInfo(SPI_SETSCREENSAVEACTIVE, active ? 1 : 0, null, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        }

        /// <summary>
        /// 将选中的图片复制到本机工作目录，避免原图被移动/删除后壁纸失效。
        /// 返回复制后的稳定路径。
        /// </summary>
        public static string CopyToWorkDir(string srcPath)
        {
            string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ScreenSaverTool");
            Directory.CreateDirectory(dir);
            string ext = Path.GetExtension(srcPath);
            if (string.IsNullOrEmpty(ext)) ext = ".jpg";
            string dest = Path.Combine(dir, "background" + ext);
            File.Copy(srcPath, dest, true);
            return dest;
        }

        /// <summary>
        /// 通过本地组策略注册表设置锁屏背景。
        /// 策略位置：计算机配置 → 管理模板 → 控制面板 → 个性化 → 锁屏界面图像
        /// 对应注册表：HKLM\SOFTWARE\Policies\Microsoft\Windows\Personalization\LockScreenImage
        /// 注意：写入 HKLM 需要管理员权限。
        /// </summary>
        public static void SetLockScreen(string path)
        {
            const string keyPath = @"SOFTWARE\Policies\Microsoft\Windows\Personalization";
            using (var key = Registry.LocalMachine.CreateSubKey(keyPath))
            {
                if (key == null)
                    throw new UnauthorizedAccessException("无法写入 HKLM 注册表，请以管理员身份运行程序。");
                key.SetValue("LockScreenImage", path, RegistryValueKind.String);
            }
        }
    }
}
