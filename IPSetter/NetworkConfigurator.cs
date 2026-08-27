using System;
using System.Collections.Generic;
using System.Management;

namespace IPSetter
{
    /// <summary>
    /// 网络适配器信息（有线网卡）
    /// </summary>
    public class AdapterInfo
    {
        public string Index { get; set; }         // WMI Index，用于定位配置对象
        public string Name { get; set; }           // 友好名称（如“以太网”）
        public string Description { get; set; }    // 设备描述
        public string[] IPAddresses { get; set; }
        public string[] SubnetMasks { get; set; }
        public string[] Gateways { get; set; }
        public bool DhcpEnabled { get; set; }
        public bool IpEnabled { get; set; }
    }

    /// <summary>
    /// 通过 WMI (Win32_NetworkAdapter / Win32_NetworkAdapterConfiguration) 读写网卡配置
    /// </summary>
    public static class NetworkConfigurator
    {
        // 常见的虚拟/非物理适配器关键字，用于在“有线网卡”列表中过滤掉
        private static readonly string[] VirtualKeywords =
        {
            "virtual", "hyper-v", "vmware", "vpn", "tap", "loopback",
            "bluetooth", "wan miniport", "wi-fi", "wireless", "microsoft kernel"
        };

        /// <summary>
        /// 获取本机所有有线（Ethernet 802.3）网络适配器及其当前配置
        /// </summary>
        public static List<AdapterInfo> GetAdapters()
        {
            var list = new List<AdapterInfo>();

            // 1) 先从 Win32_NetworkAdapter 拿到“有线网卡”的索引与友好名称
            var adapterMap = new Dictionary<string, (string Name, string Description)>();
            using (var searcher = new ManagementObjectSearcher(
                "SELECT Index, NetConnectionId, Description, AdapterTypeId " +
                "FROM Win32_NetworkAdapter WHERE AdapterTypeId = 0"))
            {
                foreach (ManagementObject mo in searcher.Get())
                {
                    var idx = mo["Index"]?.ToString();
                    if (string.IsNullOrEmpty(idx)) continue;

                    var desc = mo["Description"]?.ToString() ?? string.Empty;
                    if (IsVirtualAdapter(desc)) continue;

                    var nc = mo["NetConnectionId"]?.ToString();
                    var name = string.IsNullOrEmpty(nc) ? desc : nc;
                    adapterMap[idx] = (name, desc);
                }
            }

            // 2) 再从 Win32_NetworkAdapterConfiguration 读取/匹配详细配置
            foreach (var kvp in adapterMap)
            {
                var idx = kvp.Key;
                using (var config = FindConfig(idx))
                {
                    if (config == null) continue;

                    var info = new AdapterInfo
                    {
                        Index = idx,
                        Name = kvp.Value.Name,
                        Description = kvp.Value.Description,
                        IPAddresses = (config["IPAddress"] as string[]) ?? new string[0],
                        SubnetMasks = (config["IPSubnet"] as string[]) ?? new string[0],
                        Gateways = (config["DefaultIPGateway"] as string[]) ?? new string[0],
                        DhcpEnabled = config["DHCPEnabled"] is bool b && b,
                        IpEnabled = config["IPEnabled"] is bool b2 && b2,
                    };
                    list.Add(info);
                }
            }

            return list;
        }

        /// <summary>
        /// 设置静态 IP / 子网掩码 / 默认网关
        /// </summary>
        public static void SetStaticIp(string index, string ip, string subnet, string gateway)
        {
            using (var mo = FindConfig(index))
            {
                if (mo == null) throw new Exception("未找到对应的网络适配器配置。");

                var setIp = (uint)mo.InvokeMethod("EnableStatic",
                    new object[] { new string[] { ip }, new string[] { subnet } });
                if (setIp != 0)
                    throw new Exception($"设置 IP 地址失败，WMI 错误码：{setIp}");

                if (!string.IsNullOrWhiteSpace(gateway))
                {
                    var setGw = (uint)mo.InvokeMethod("SetGateways",
                        new object[] { new string[] { gateway }, new string[] { "1" } });
                    if (setGw != 0)
                        throw new Exception($"设置默认网关失败，WMI 错误码：{setGw}");
                }
            }
        }

        /// <summary>
        /// 切换到 DHCP 自动获取（IP 与网关均自动）
        /// </summary>
        public static void SetDhcp(string index)
        {
            using (var mo = FindConfig(index))
            {
                if (mo == null) throw new Exception("未找到对应的网络适配器配置。");

                var r = (uint)mo.InvokeMethod("EnableDHCP", null);
                if (r != 0)
                    throw new Exception($"启用 DHCP 失败，WMI 错误码：{r}");
            }
        }

        private static ManagementObject FindConfig(string index)
        {
            using (var searcher = new ManagementObjectSearcher(
                $"SELECT * FROM Win32_NetworkAdapterConfiguration WHERE Index = '{index}'"))
            {
                foreach (ManagementObject mo in searcher.Get())
                    return mo; // 返回后由调用方 using 释放
            }
            return null;
        }

        private static bool IsVirtualAdapter(string description)
        {
            if (string.IsNullOrEmpty(description)) return false;
            var lower = description.ToLowerInvariant();
            foreach (var kw in VirtualKeywords)
                if (lower.Contains(kw)) return true;
            return false;
        }
    }
}
