using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SerialPortDemo
{
    public static  class HexHelper
    {

        #region  判断是否为16进制字符串
        /// <summary>
        /// 判断是否为16进制字符串
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static bool IsHexString(string hexString)
        {
            //十六进制发送时，发送框数据进行十六进制数据正则校验
            if (Regex.IsMatch(hexString, "^[0-9A-Fa-f]+$"))
            {
                //校验成功
                return true;
            }
            //校验失败
            return false;
        }
        #endregion

        #region 将ASCII码字符串转换成16进制字符串
        /// <summary>
        /// 将ascii字符串转换成Hex显示
        /// </summary>
        /// <param name="asciiString"></param>
        /// <returns></returns>
        public static string ConvertStringToHexString(string asciiString, Encoding encoding)
        {
            string hexString = "";
            asciiString = asciiString.Replace("\n", "\r\n");                //解决richTextBox获取不到"\r\n"换行符
            byte[] arrByte = encoding.GetBytes(asciiString);
            foreach (char c in arrByte)
            {
                int tmp = c;
                hexString += String.Format("{0:x2}", (uint)System.Convert.ToUInt32(tmp.ToString())).ToUpper() + " ";
            }
            return hexString;
        }

        #endregion

        #region 将16进制字符串转换成ASCII码字符串
        /// <summary>
        /// 将Hex转换成ascii字符串显示
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static string ConvertHexStringToString(string hexString, Encoding encoding)
        {
            string result = string.Empty;
            hexString = hexString.Replace(" ", "");
            //解决十六进制出现单数据问题，以补0
            if (hexString.Length % 2 == 1)
            {
                hexString = hexString.Insert(hexString.Length - 1, "0");
            }
            byte[] arrByte = new byte[hexString.Length / 2];
            int index = 0;
            for (int i = 0; i < hexString.Length; i += 2)
            {
                //Convert.ToByte(string,16)把十六进制string转化成byte 
                arrByte[index++] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }
            result = encoding.GetString(arrByte);
            return result;
        }
        #endregion

        #region 获取16进制字符串的字节数
        /// <summary>
        /// 获取hexString中Byte数量
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static int GetByteCount(string hexString)
        {
            if (IsHexString(hexString) && (hexString.Length % 2 == 0))
            {
                // 2 characters per byte
                return hexString.Length / 2;
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region 单个16进制数转字节
        /// <summary>
        /// 16进制数转字节
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        private static byte HexToByte(string hex)
        {
            if (hex.Length > 2 || hex.Length <= 0)
                throw new ArgumentException("hex must be 1 or 2 characters in length");
            byte newByte = byte.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            return newByte;
        }
        #endregion

        #region 16进制字符串转换成字节数组
        /// <summary>
        ///  16进制字符串转换成字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] HexStringToBytes(string hexString)
        {
            //string newString = "";

            if (IsHexString(hexString) == false)
            { return null; }

            int byteLength = hexString.Length / 2;
            byte[] bytes = new byte[byteLength];
            string hex;
            int j = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                hex = new String(new Char[] { hexString[j], hexString[j + 1] });
                bytes[i] = HexToByte(hex);
                j = j + 2;
            }
            return bytes;
        }
        #endregion

        #region 字节数组转换成16进制字符串
        /// <summary>
        /// 字节数组转出成16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string BytesToHexString(byte[] bytes)
        {
            string hexString = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                hexString += bytes[i].ToString("X2");
            }
            return hexString;
        }
        #endregion

        #region 拼接字节数组
        public static byte[] ConcatArrays(byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
        }

        public static byte[] ConcatArrays(byte[] first, byte[] second, byte[] third)
        {
            byte[] ret = new byte[first.Length + second.Length + third.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            Buffer.BlockCopy(third, 0, ret, first.Length + second.Length,
                             third.Length);
            return ret;
        }

        public static byte[] ConcatArrays(params byte[][] arrays)
        {
            byte[] ret = new byte[arrays.Sum(x => x.Length)];
            int offset = 0;
            foreach (byte[] data in arrays)
            {
                Buffer.BlockCopy(data, 0, ret, offset, data.Length);
                offset += data.Length;
            }
            return ret;
        }
        #endregion

    }
}
