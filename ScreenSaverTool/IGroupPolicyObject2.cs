using System;
using System.Text;
using System.Runtime.InteropServices;

// 来源：基于 github.com/sjitech/powershell-local-group-policy-reg（MIT）
// 作用：通过 COM 接口 IGroupPolicyObject2 加载/保存本地组策略的注册表 hive，
//       使得 programmatic 修改的策略能在 gpedit.msc 中正确显示为 已启用/未配置。
namespace GroupPolicy
{
    public class Reg
    {
        internal static IGroupPolicyObject2 gpo;

        public static void Load()
        {
            gpo = (IGroupPolicyObject2)new GroupPolicyClass();
            gpo.OpenLocalMachineGPO(GroupPolicyFlags.LoadRegistryInformation);

            StringBuilder rootRegPath = new StringBuilder(1024);
            gpo.GetRegistryKeyPath(GroupPolicySection.Machine, rootRegPath, rootRegPath.Capacity);
            machineRegPath = rootRegPath.ToString();

            gpo.GetRegistryKeyPath(GroupPolicySection.User, rootRegPath, rootRegPath.Capacity);
            userRegPath = rootRegPath.ToString();
        }

        public static string machineRegPath;
        public static string userRegPath;

        // 稳定 GUID，用于标识本应用写入的组策略。
        static Guid myGuid = new Guid("{B4C8A1E2-9F3D-4A7B-8C1D-2E5F6A9B0C3D}");

        public static void Save(bool machine = false)
        {
            gpo.Save(machine, /*extension add:*/true, GroupPolicyExtensionGuids.Registry, myGuid);
        }

        public static void Unload()
        {
            if (gpo != null)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(gpo);
                gpo = null;
            }
        }
    }

    [ComImport, Guid("EA502722-A23D-11d1-A7D3-0000F87571E3")]
    public class GroupPolicyClass
    {
    }

    [ComImport, Guid("7E37D5E7-263D-45CF-842B-96A95C63E46C"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IGroupPolicyObject2
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords")]
        uint New(
              [MarshalAs(UnmanagedType.LPWStr)] string domainName,
              [MarshalAs(UnmanagedType.LPWStr)] string displayName,
              uint flags);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        uint OpenDSGPO(
            [MarshalAs(UnmanagedType.LPWStr)] string path,
            uint flags);

        uint OpenLocalMachineGPO(
            uint flags);

        uint OpenRemoteMachineGPO(
            [MarshalAs(UnmanagedType.LPWStr)] string computerName,
            uint flags);

        uint Save(
            [MarshalAs(UnmanagedType.Bool)] bool machine,
            [MarshalAs(UnmanagedType.Bool)] bool add,
            [MarshalAs(UnmanagedType.LPStruct)] Guid extension,
            [MarshalAs(UnmanagedType.LPStruct)] Guid app);

        uint Delete();

        uint GetName(
            [MarshalAs(UnmanagedType.LPWStr)] StringBuilder name,
            int maxLength);

        uint GetDisplayName(
            [MarshalAs(UnmanagedType.LPWStr)] StringBuilder name,
            int maxLength);

        uint SetDisplayName(
            [MarshalAs(UnmanagedType.LPWStr)] string name);

        uint GetPath(
            [MarshalAs(UnmanagedType.LPWStr)] StringBuilder path,
            int maxPath);

        uint GetDSPath(
            uint section,
            [MarshalAs(UnmanagedType.LPWStr)] StringBuilder path,
            int maxPath);

        uint GetFileSysPath(
            uint section,
            [MarshalAs(UnmanagedType.LPWStr)] StringBuilder path,
            int maxPath);

        uint GetRegistryKey(
            uint section,
            out IntPtr key);

        uint GetOptions(out uint options);

        uint SetOptions(
            uint options,
            uint mask);

        uint GetType(
            out IntPtr gpoType
        );

        uint GetMachineName(
            [MarshalAs(UnmanagedType.LPWStr)] StringBuilder name,
            int maxLength);

        uint GetPropertySheetPages(
            out IntPtr pages);

        uint OpenLocalMachineGPOForPrincipal(
            [MarshalAs(UnmanagedType.LPWStr)] string pszLocalUserOrGroupSID,
            uint dwFlags
        );

        uint GetRegistryKeyPath(
            uint section,
            [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszRegistryKeyPath,
            int maxPath);
    }

    public class GroupPolicyFlags
    {
        public const uint LoadRegistryInformation = 0x00000001;
        public const uint Readonly = 0x00000002;
    }

    public class GroupPolicySection
    {
        public const uint User = 1;
        public const uint Machine = 2;
    }

    public class GroupPolicyExtensionGuids
    {
        /// <summary>
        /// The snap-in that processes .pol files
        /// </summary>
        public static readonly Guid Registry = new Guid(0x35378EAC, 0x683F, 0x11D2, 0xA8, 0x9A, 0x00, 0xC0, 0x4F, 0xBB, 0xCF, 0xA2);
    }
}
