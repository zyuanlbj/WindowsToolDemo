using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialPortDemo
{
    public class ParityHelper
    {
        /// <summary>
        /// 校验类型
        /// </summary>
        public const string XOR异或校验 = "XOR异或校验";
        public const string SUM8累加校验 = "SUM8累加校验";
        public const string ModBusLRC校验 = "ModBusLRC校验";
        public const string CRC16小端 = "CRC16小端";
        public const string CRC16大端 = "CRC16大端";
        public const string SUM16小端 = "SUM16小端";
        public const string SUM16大端 = "SUM16大端";
        public const string ModBusCRC小端 = "ModBusCRC小端";
        public const string ModBusCRC大端 = "ModBusCRC大端";

        /// <summary>
        /// 大小端数据模式
        /// </summary>
        public enum BigOrLittle
        {
            //大端
            BigEndian = 0,
            //小端
            LittleEndian = 1
        }

        #region 数据翻转

        /// <summary>
        /// 翻转byte数组
        /// </summary>
        /// <param name="bytes">字节数组</param>
        public static void ReverseBytes(byte[] bytes)
        {
            byte tmp;
            int len = bytes.Length;

            for (int i = 0; i < len / 2; i++)
            {
                tmp = bytes[len - 1 - i];
                bytes[len - 1 - i] = bytes[i];
                bytes[i] = tmp;
            }
        }

        /// <summary>
        /// 规定转换起始位置和长度
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="start">开始位置</param>
        /// <param name="len">长度</param>
        public static void ReverseBytes(byte[] bytes, int start, int len)
        {
            int end = start + len - 1;
            byte tmp;
            int i = 0;
            for (int index = start; index < start + len / 2; index++, i++)
            {
                tmp = bytes[end - i];
                bytes[end - i] = bytes[index];
                bytes[index] = tmp;
            }
        }

        /// <summary>
        /// 翻转字节顺序 (16-bit)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static UInt16 ReverseBytes(UInt16 value)
        {
            return (UInt16)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
        }

        /// <summary>
        /// 翻转字节顺序 (32-bit)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static UInt32 ReverseBytes(UInt32 value)
        {
            return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                   (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
        }

        /// <summary>
        /// 翻转字节顺序 (64-bit)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static UInt64 ReverseBytes(UInt64 value)
        {
            return (value & 0x00000000000000FFUL) << 56 | (value & 0x000000000000FF00UL) << 40 |
                   (value & 0x0000000000FF0000UL) << 24 | (value & 0x00000000FF000000UL) << 8 |
                   (value & 0x000000FF00000000UL) >> 8 | (value & 0x0000FF0000000000UL) >> 24 |
                   (value & 0x00FF000000000000UL) >> 40 | (value & 0xFF00000000000000UL) >> 56;
        }

        #endregion

        #region 校验和  

        /// <summary>  
        /// 异或校验和 
        /// </summary>  
        /// <param name="data">要校验的数据数组</param>  
        /// <returns>校验和值</returns>  
        public static byte[] DataXOR(byte[] data)
        {
            byte check = 0;
            for (int i = 0; i < data.Length; i++)
            {
                check ^= data[i];
            }
            return new byte[] { check };
        }

        /// <summary>  
        /// 异或校验和
        /// </summary>  
        /// <param name="data">要校验的数据数组</param>  
        /// <returns>校验的数据数组+校验和</returns>  
        public static byte[] DataXORFull(byte[] data)
        {
            byte[] check = DataXOR(data);
            List<byte> newdata = new List<byte>();
            newdata.AddRange(data);
            newdata.AddRange(check);
            return newdata.ToArray();
        }

        /// <summary>
        /// 累加校验和
        /// </summary>
        /// <param name="data">要校验的数据数组</param>
        /// <returns>返回校验和结果</returns>
        public static byte[] DataSum8(byte[] data)
        {
            byte check = 0;
            for (int i = 0; i < data.Length; i++)
            {
                check = (byte)((check + data[i]) % 0xff);
            }
            //返回累加校验和
            return new byte[] { check };
        }

        /// <summary>  
        /// 累加校验和
        /// </summary>  
        /// <param name="data">要校验的数据数组</param>  
        /// <returns>校验的数据数组+校验和</returns>  
        public static byte[] DataSum8Full(byte[] data)
        {
            byte[] check = DataSum8(data);
            List<byte> newdata = new List<byte>();
            newdata.AddRange(data);
            newdata.AddRange(check);
            return newdata.ToArray();
        }

        /// <summary>
        /// 累加校验和
        /// </summary>
        /// <param name="data">要校验的数据数组</param>
        /// <param name="mode">大小端模式</param>
        /// <returns>返回校验和结果</returns>
        public static byte[] DataSum16(byte[] data, BigOrLittle mode)
        {
            int num = 0;
            for (int i = 0; i < data.Length; i++)
            {
                num = (num + data[i]) % 0xffff;
            }
            //返回累加校验和
            if (mode ==BigOrLittle.LittleEndian)

                return BitConverter.GetBytes(ReverseBytes((UInt16)num));
            else
                return BitConverter.GetBytes((UInt16)num);
        }

        /// <summary>  
        /// 累加校验和
        /// </summary>  
        /// <param name="data">要校验的数据数组</param>  
        /// <param name="mode">大小端模式</param>
        /// <returns>校验的数据数组+校验和</returns>  
        public static byte[] DataSum16Full(byte[] data, BigOrLittle mode)
        {
            byte[] check = DataSum16(data, mode);
            List<byte> newdata = new List<byte>();
            newdata.AddRange(data);
            newdata.AddRange(check);
            return newdata.ToArray();
        }

        #endregion

        #region CRC校验
        /// <summary>
        /// CRC16-CCITT(初始值FFFF，多项式1021，异或值FFFF，异或输出，表逆序，算法逆序)
        /// </summary>
        private static int[] CRC16_CCITT_TABLE = {
            0x0000, 0x1189, 0x2312, 0x329b, 0x4624, 0x57ad, 0x6536, 0x74bf,
            0x8c48, 0x9dc1, 0xaf5a, 0xbed3, 0xca6c, 0xdbe5, 0xe97e, 0xf8f7,
            0x1081, 0x0108, 0x3393, 0x221a, 0x56a5, 0x472c, 0x75b7, 0x643e,
            0x9cc9, 0x8d40, 0xbfdb, 0xae52, 0xdaed, 0xcb64, 0xf9ff, 0xe876,
            0x2102, 0x308b, 0x0210, 0x1399, 0x6726, 0x76af, 0x4434, 0x55bd,
            0xad4a, 0xbcc3, 0x8e58, 0x9fd1, 0xeb6e, 0xfae7, 0xc87c, 0xd9f5,
            0x3183, 0x200a, 0x1291, 0x0318, 0x77a7, 0x662e, 0x54b5, 0x453c,
            0xbdcb, 0xac42, 0x9ed9, 0x8f50, 0xfbef, 0xea66, 0xd8fd, 0xc974,
            0x4204, 0x538d, 0x6116, 0x709f, 0x0420, 0x15a9, 0x2732, 0x36bb,
            0xce4c, 0xdfc5, 0xed5e, 0xfcd7, 0x8868, 0x99e1, 0xab7a, 0xbaf3,
            0x5285, 0x430c, 0x7197, 0x601e, 0x14a1, 0x0528, 0x37b3, 0x263a,
            0xdecd, 0xcf44, 0xfddf, 0xec56, 0x98e9, 0x8960, 0xbbfb, 0xaa72,
            0x6306, 0x728f, 0x4014, 0x519d, 0x2522, 0x34ab, 0x0630, 0x17b9,
            0xef4e, 0xfec7, 0xcc5c, 0xddd5, 0xa96a, 0xb8e3, 0x8a78, 0x9bf1,
            0x7387, 0x620e, 0x5095, 0x411c, 0x35a3, 0x242a, 0x16b1, 0x0738,
            0xffcf, 0xee46, 0xdcdd, 0xcd54, 0xb9eb, 0xa862, 0x9af9, 0x8b70,
            0x8408, 0x9581, 0xa71a, 0xb693, 0xc22c, 0xd3a5, 0xe13e, 0xf0b7,
            0x0840, 0x19c9, 0x2b52, 0x3adb, 0x4e64, 0x5fed, 0x6d76, 0x7cff,
            0x9489, 0x8500, 0xb79b, 0xa612, 0xd2ad, 0xc324, 0xf1bf, 0xe036,
            0x18c1, 0x0948, 0x3bd3, 0x2a5a, 0x5ee5, 0x4f6c, 0x7df7, 0x6c7e,
            0xa50a, 0xb483, 0x8618, 0x9791, 0xe32e, 0xf2a7, 0xc03c, 0xd1b5,
            0x2942, 0x38cb, 0x0a50, 0x1bd9, 0x6f66, 0x7eef, 0x4c74, 0x5dfd,
            0xb58b, 0xa402, 0x9699, 0x8710, 0xf3af, 0xe226, 0xd0bd, 0xc134,
            0x39c3, 0x284a, 0x1ad1, 0x0b58, 0x7fe7, 0x6e6e, 0x5cf5, 0x4d7c,
            0xc60c, 0xd785, 0xe51e, 0xf497, 0x8028, 0x91a1, 0xa33a, 0xb2b3,
            0x4a44, 0x5bcd, 0x6956, 0x78df, 0x0c60, 0x1de9, 0x2f72, 0x3efb,
            0xd68d, 0xc704, 0xf59f, 0xe416, 0x90a9, 0x8120, 0xb3bb, 0xa232,
            0x5ac5, 0x4b4c, 0x79d7, 0x685e, 0x1ce1, 0x0d68, 0x3ff3, 0x2e7a,
            0xe70e, 0xf687, 0xc41c, 0xd595, 0xa12a, 0xb0a3, 0x8238, 0x93b1,
            0x6b46, 0x7acf, 0x4854, 0x59dd, 0x2d62, 0x3ceb, 0x0e70, 0x1ff9,
            0xf78f, 0xe606, 0xd49d, 0xc514, 0xb1ab, 0xa022, 0x92b9, 0x8330,
            0x7bc7, 0x6a4e, 0x58d5, 0x495c, 0x3de3, 0x2c6a, 0x1ef1, 0x0f78
        };

        /// <summary>
        /// CRC16-CCITT CRC校验
        /// </summary>
        /// <param name="data">要校验的数据数组</param>
        /// <param name="mode">大小端模式</param>
        /// <returns>校验结果</returns>
        public static byte[] DataCrc16_Ccitt(byte[] data, BigOrLittle mode)
        {
            UInt16 check = 0x0000;
            int i, j;
            for (i = 0; i < data.Length; i++)
            {
                check ^= (UInt16)((UInt16)data[i] << 8);
                for (j = 0; j < 8; j++)
                {
                    if ((check & 0x8000) != 0)
                    {
                        check <<= 1;
                        check ^= 0x1021;
                    }
                    else
                    {
                        check <<= 1;
                    }
                }
            }
            if (mode == BigOrLittle.LittleEndian)

                return BitConverter.GetBytes(ReverseBytes(check));
            else
                return BitConverter.GetBytes(check);
        }

        /// <summary>
        /// CRC16-CCITT CRC校验
        /// </summary>
        /// <param name="data">要校验的数据数组</param>
        /// <param name="mode">大小端模式</param>
        /// <returns>校验是数据数组+校验值</returns>
        public static byte[] DataCrc16Full_Ccitt(byte[] data, BigOrLittle mode)
        {
            byte[] check = DataCrc16_Ccitt(data, mode);
            List<byte> newdata = new List<byte>();
            newdata.AddRange(data);
            newdata.AddRange(check);
            return newdata.ToArray();
        }

        /// <summary>
        /// CRC16-MODBUS CRC校验
        /// </summary>
        /// <param name="data">要校验的数据数组</param>
        /// <param name="mode">大小端模式</param>
        /// <returns>校验值</returns>
        public static byte[] DataCrc16_Modbus(byte[] data, BigOrLittle mode)
        {
            UInt16 check = 0xFFFF;
            int i, j;
            for (i = 0; i < data.Length; i++)
            {
                check ^= data[i];
                for (j = 0; j < 8; j++)
                {
                    if ((check & 0x0001) != 0)
                    {
                        check >>= 1;
                        check ^= 0xA001;
                    }
                    else
                    {
                        check >>= 1;
                    }
                }
            }
            if (mode == BigOrLittle.LittleEndian)

                return BitConverter.GetBytes(check);
            else
                return BitConverter.GetBytes(ReverseBytes(check));
        }

        /// <summary>
        /// CRC16-MODBUS CRC
        /// </summary>
        /// <param name="data">要校验的数据数组</param>
        /// <param name="mode">大小端模式</param>
        /// <returns>校验是数据数组+校验值</returns>
        public static byte[] DataCrc16Full_Modbus(byte[] data, BigOrLittle mode)
        {
            byte[] check = DataCrc16_Modbus(data, mode);
            List<byte> newdata = new List<byte>();
            newdata.AddRange(data);
            newdata.AddRange(check);
            return newdata.ToArray();
        }

        #endregion

        #region LRC校验



        #endregion



    }
}
