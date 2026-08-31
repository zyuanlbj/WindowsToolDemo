using System;
using System.IO;
using System.IO.Ports;
using xbd.DataConvertLib;

namespace ModbusRTUDemo.Helper
{
    /// <summary>
    /// Modbus RTU协议，基于RS485接口
    /// </summary>
    public class ModbusRtu
    {
        #region 对象或属性

        //定义串口通信的对象
        private SerialPort _serialPort = new SerialPort();

        //创建通信超时的属性
        public int ReadTimeout { get; set; } = 2000;
        public int WriteTimeout { get; set; } = 2000;

        //读取返回报文超时时间
        public int ReceiveTimeout { get; set; } = 2000;

        //字节顺序
        public DataFormat DataFormat { get; set; } = DataFormat.ABCD;

        /// <summary>
        /// 创建一个互斥锁对象
        /// </summary>
        private SimpleHybirdLock _interactiveLock = new SimpleHybirdLock();

        #endregion

        #region 打开/关闭 串口

        /// <summary>
        /// 打开串口
        /// </summary>
        /// <param name="baudRate">波特率</param>
        /// <param name="portName">串口号</param>
        /// <param name="dataBits">数据位</param>
        /// <param name="parity">校验位</param>
        /// <param name="stopBits">停止位</param>
        public void OpenSerialPort(int baudRate, string portName, int dataBits, Parity parity, StopBits stopBits)
        {
            if (_serialPort.IsOpen) _serialPort.Close();

            _serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits)
            {
                ReadTimeout = this.ReadTimeout,
                WriteTimeout = this.WriteTimeout
            };

            _serialPort.Open();
        }

        /// <summary>
        /// 关闭串口
        /// </summary>
        public void CloseSerialPort()
        {
            if (_serialPort.IsOpen) _serialPort.Close();
        }

        #endregion

        #region 读取输出线圈 功能码01H

        /// <summary>
        /// 读取输出线圈方法
        /// </summary>
        /// <param name="slaveAddr">从站地址</param>
        /// <param name="address">起始地址</param>
        /// <param name="length">线圈数量</param>
        /// <returns>返回读取结果：字节数组</returns>
        public byte[] ReadOutputStatus(int slaveAddr, int address, int length)
        {
            //第一步：拼接报文
            //从站地址 功能码 起始地址 线圈数量 CRC
            ByteArray sendCommand = new ByteArray();
            sendCommand.Add(new byte[] { (byte)slaveAddr });
            sendCommand.Add(new byte[] { 0x01 });
            sendCommand.Add(new byte[] { (byte)(address / 256), (byte)(address % 256) });
            sendCommand.Add(new byte[] { (byte)(length / 256), (byte)(length % 256) });
            sendCommand.Add(Crc16(sendCommand.array, 6));

            //第二步：发送报文 接收报文
            //从站地址 功能码 字节计数    CRC 2个字节
            //响应报文的字节计数
            int byteLength = length % 8 == 0 ? length / 8 : length / 8 + 1;
            byte[] response = new byte[5 + byteLength];
            if (SendData(sendCommand.array, ref response))
            {
                //第三步：解析报文
                //验证：功能码+字节计数
                if (response[1] == 0x01 && response[2] == byteLength)
                {
                    return ByteArrayLib.GetByteArrayFromByteArray(response, 3, byteLength);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region 读取输入线圈 功能码02H

        /// <summary>
        /// 读取输入线圈方法
        /// </summary>
        /// <param name="slaveAddr">从站地址</param>
        /// <param name="address">起始地址</param>
        /// <param name="length">线圈数量</param>
        /// <returns>返回读取结果：字节数组</returns>
        public byte[] ReadInputStatus(int slaveAddr, int address, int length)
        {
            ByteArray sendCommand = new ByteArray();
            sendCommand.Add(new byte[] { (byte)slaveAddr });
            sendCommand.Add(new byte[] { 0x02 });
            sendCommand.Add(new byte[] { (byte)(address / 256), (byte)(address % 256) });
            sendCommand.Add(new byte[] { (byte)(length / 256), (byte)(length % 256) });
            sendCommand.Add(Crc16(sendCommand.array, 6));

            int byteLength = length % 8 == 0 ? length / 8 : length / 8 + 1;
            byte[] response = new byte[5 + byteLength];
            if (SendData(sendCommand.array, ref response))
            {
                if (response[1] == 0x02 && response[2] == byteLength)
                {
                    return ByteArrayLib.GetByteArrayFromByteArray(response, 3, byteLength);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region 读取保持寄存器 功能码03H

        /// <summary>
        /// 读取保持寄存器方法
        /// </summary>
        /// <param name="slaveAddr">从站地址</param>
        /// <param name="address">起始地址</param>
        /// <param name="length">寄存器数量</param>
        /// <returns>返回读取结果：字节数组</returns>
        public byte[] ReadKeepRegister(int slaveAddr, int address, int length)
        {
            ByteArray sendCommand = new ByteArray();
            sendCommand.Add(new byte[] { (byte)slaveAddr });
            sendCommand.Add(new byte[] { 0x03 });
            sendCommand.Add(new byte[] { (byte)(address / 256), (byte)(address % 256) });
            sendCommand.Add(new byte[] { (byte)(length / 256), (byte)(length % 256) });
            sendCommand.Add(Crc16(sendCommand.array, 6));

            int byteLength = length * 2;
            byte[] response = new byte[5 + byteLength];
            if (SendData(sendCommand.array, ref response))
            {
                if (response[1] == 0x03 && response[2] == byteLength)
                {
                    return ByteArrayLib.GetByteArrayFromByteArray(response, 3, byteLength);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region 读取输入寄存器 功能码04H

        /// <summary>
        /// 读取输入寄存器方法
        /// </summary>
        /// <param name="slaveAddr">从站地址</param>
        /// <param name="address">起始地址</param>
        /// <param name="length">寄存器数量</param>
        /// <returns>返回读取结果：字节数组</returns>
        public byte[] ReadInputRegister(int slaveAddr, int address, int length)
        {
            ByteArray sendCommand = new ByteArray();
            sendCommand.Add(new byte[] { (byte)slaveAddr });
            sendCommand.Add(new byte[] { 0x04 });
            sendCommand.Add(new byte[] { (byte)(address / 256), (byte)(address % 256) });
            sendCommand.Add(new byte[] { (byte)(length / 256), (byte)(length % 256) });
            sendCommand.Add(Crc16(sendCommand.array, 6));

            int byteLength = length * 2;
            byte[] response = new byte[5 + byteLength];
            if (SendData(sendCommand.array, ref response))
            {
                if (response[1] == 0x04 && response[2] == byteLength)
                {
                    return ByteArrayLib.GetByteArrayFromByteArray(response, 3, byteLength);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region 预置单线圈 功能码05H

        /// <summary>
        /// 预置单线圈
        /// </summary>
        /// <param name="slaveAddr">从站地址</param>
        /// <param name="address">起始地址</param>
        /// <param name="setValue">设定值</param>
        /// <returns>是否写入成功</returns>
        public bool ForceCoil(int slaveAddr, int address, bool setValue)
        {
            ByteArray sendCommand = new ByteArray();
            sendCommand.Add(new byte[] { (byte)slaveAddr, 0x05, (byte)(address / 256), (byte)(address % 256) });
            if (setValue)
            {
                sendCommand.Add(new byte[] { 0xFF, 0x00 });
            }
            else
            {
                sendCommand.Add(new byte[] { 0x00, 0x00 });
            }
            sendCommand.Add(Crc16(sendCommand.array, 6));
            byte[] response = new byte[8];
            if (SendData(sendCommand.array, ref response))
            {
                return ByteArrayLib.GetByteArrayEquals(sendCommand.array, response);
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region 预置单寄存器 功能码06H

        /// <summary>
        /// 预置单寄存器
        /// </summary>
        /// <param name="slaveAddr">从站地址</param>
        /// <param name="address">起始地址</param>
        /// <param name="setValue">设定值</param>
        /// <returns>是否写入成功</returns>
        public bool PreSetSingleRegister(int slaveAddr, int address, short setValue)
        {
            ByteArray sendCommand = new ByteArray();
            sendCommand.Add(new byte[] { (byte)slaveAddr, 0x05, (byte)(address / 256), (byte)(address % 256) });
            sendCommand.Add(ByteArrayLib.GetByteArrayFromShort(setValue));
            sendCommand.Add(Crc16(sendCommand.array, 6));

            byte[] response = new byte[8];
            if (SendData(sendCommand.array, ref response))
            {
                return ByteArrayLib.GetByteArrayEquals(sendCommand.array, response);
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region 预置多线圈 功能码0FH

        /// <summary>
        /// 预置多线圈
        /// </summary>
        /// <param name="slaveAddr">从站地址</param>
        /// <param name="address">起始地址</param>
        /// <param name="setValue">设定值</param>
        /// <returns>是否写入成功</returns>
        public bool ForeMultiCoils(int slaveAddr, int address, bool[] setValue)
        {
            ByteArray sendCommand = new ByteArray();
            sendCommand.Add(new byte[] { (byte)slaveAddr, 0x0F, (byte)(address / 256), (byte)(address % 256) });
            sendCommand.Add(new byte[] { (byte)(setValue.Length / 256), (byte)(setValue.Length % 256) });

            byte[] setByteArray = ByteArrayLib.GetByteArrayFromBoolArray(setValue);
            sendCommand.Add((byte)setByteArray.Length);
            sendCommand.Add(setByteArray);
            sendCommand.Add(Crc16(sendCommand.array, 7 + setByteArray.Length));

            byte[] response = new byte[8];
            if (SendData(sendCommand.array, ref response))
            {
                byte[] front6 = ByteArrayLib.GetByteArrayFromByteArray(response, 0, 6);
                byte[] crc = Crc16(front6, 6);
                return ByteArrayLib.GetByteArrayEquals(ByteArrayLib.GetByteArrayFromByteArray(sendCommand.array, 0, 6), front6)
                    && crc[0] == response[6]
                    && crc[1] == response[7];
            }
            return false;
        }

        #endregion

        #region 预置多个寄存器 功能码10H

        /// <summary>
        /// 预置多个寄存器
        /// </summary>
        /// <param name="slaveAddr">从站地址</param>
        /// <param name="address">起始地址</param>
        /// <param name="setValue">设定值</param>
        /// <returns>是否写入成功</returns>
        public bool PreSetMultiRegisters(int slaveAddr, int address, byte[] setValue)
        {
            if (setValue == null || setValue.Length == 0 || setValue.Length % 2 == 1)
            {
                return false;
            }

            int regLength = setValue.Length / 2;

            ByteArray sendCommand = new ByteArray();
            sendCommand.Add(new byte[] { (byte)slaveAddr, 0x10, (byte)(address / 256), (byte)(address % 256) });
            sendCommand.Add(new byte[] { (byte)(regLength / 256), (byte)(regLength % 256) });
            sendCommand.Add(new byte[] { (byte)setValue.Length });
            sendCommand.Add(setValue);
            sendCommand.Add(Crc16(sendCommand.array, 7 + setValue.Length));

            byte[] response = new byte[8];
            if (SendData(sendCommand.array, ref response))
            {
                byte[] front6 = ByteArrayLib.GetByteArrayFromByteArray(response, 0, 6);
                byte[] crc = Crc16(front6, 6);
                return ByteArrayLib.GetByteArrayEquals(ByteArrayLib.GetByteArrayFromByteArray(sendCommand.array, 0, 6), front6)
                    && crc[0] == response[6]
                    && crc[1] == response[7];
            }
            return false;
        }

        public bool PreSetMultiRegisters(int slaveAddr, int address, short setValue)
        {
            return PreSetMultiRegisters(slaveAddr, address, ByteArrayLib.GetByteArrayFromShort(setValue, DataFormat));
        }
        public bool PreSetMultiRegisters(int slaveAddr, int address, ushort setValue)
        {
            return PreSetMultiRegisters(slaveAddr, address, ByteArrayLib.GetByteArrayFromUShort(setValue, DataFormat));
        }
        public bool PreSetMultiRegisters(int slaveAddr, int address, int setValue)
        {
            return PreSetMultiRegisters(slaveAddr, address, ByteArrayLib.GetByteArrayFromInt(setValue, DataFormat));
        }
        public bool PreSetMultiRegisters(int slaveAddr, int address, uint setValue)
        {
            return PreSetMultiRegisters(slaveAddr, address, ByteArrayLib.GetByteArrayFromUInt(setValue, DataFormat));
        }
        public bool PreSetMultiRegisters(int slaveAddr, int address, float setValue)
        {
            return PreSetMultiRegisters(slaveAddr, address, ByteArrayLib.GetByteArrayFromFloat(setValue, DataFormat));
        }
        public bool PreSetMultiRegisters(int slaveAddr, int address, short[] setValue)
        {
            return PreSetMultiRegisters(slaveAddr, address, ByteArrayLib.GetByteArrayFromShortArray(setValue, DataFormat));
        }
        public bool PreSetMultiRegisters(int slaveAddr, int address, ushort[] setValue)
        {
            return PreSetMultiRegisters(slaveAddr, address, ByteArrayLib.GetByteArrayFromUShortArray(setValue, DataFormat));
        }
        public bool PreSetMultiRegisters(int slaveAddr, int address, int[] setValue)
        {
            return PreSetMultiRegisters(slaveAddr, address, ByteArrayLib.GetByteArrayFromIntArray(setValue, DataFormat));
        }
        public bool PreSetMultiRegisters(int slaveAddr, int address, uint[] setValue)
        {
            return PreSetMultiRegisters(slaveAddr, address, ByteArrayLib.GetByteArrayFromUIntArray(setValue, DataFormat));
        }
        public bool PreSetMultiRegisters(int slaveAddr, int address, float[] setValue)
        {
            return PreSetMultiRegisters(slaveAddr, address, ByteArrayLib.GetByteArrayFromFloatArray(setValue, DataFormat));
        }

        #endregion

        #region 通用发送报文并接收

        /// <summary>
        /// 通用发送报文并接收报文方法
        /// </summary>
        /// <param name="sendByte">发送字节数组</param>
        /// <param name="response">接收的字节数组</param>
        /// <returns>是否发送成功</returns>
        private bool SendData(byte[] sendByte, ref byte[] response)
        {
            //上锁
            _interactiveLock.Enter();

            try
            {
                //发送报文
                _serialPort.Write(sendByte, 0, sendByte.Length);
                //定义一个Buffer
                byte[] buffer = new byte[1024];
                //定义一个内存
                MemoryStream ms = new MemoryStream();
                //定义开始读取时间
                DateTime start = DateTime.Now;
                while (true)
                {
                    if (_serialPort.BytesToRead >= 1)
                    {
                        int count = _serialPort.Read(buffer, 0, buffer.Length);
                        ms.Write(buffer, 0, count);
                    }
                    else
                    {
                        //判断是否超时
                        if ((DateTime.Now - start).TotalMilliseconds > this.ReceiveTimeout)
                        {
                            ms.Dispose();
                            return false;
                        }
                        else if (ms.Length > 0)
                        {
                            break;
                        }
                    }
                }
                response = ms.ToArray();
                ms.Dispose();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                _interactiveLock.Leave();
            }
        }

        #endregion

        #region  CRC校验

        private static readonly byte[] aucCRCHi = {
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
             0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
             0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40
         };
        private static readonly byte[] aucCRCLo = {
             0x00, 0xC0, 0xC1, 0x01, 0xC3, 0x03, 0x02, 0xC2, 0xC6, 0x06, 0x07, 0xC7,
             0x05, 0xC5, 0xC4, 0x04, 0xCC, 0x0C, 0x0D, 0xCD, 0x0F, 0xCF, 0xCE, 0x0E,
             0x0A, 0xCA, 0xCB, 0x0B, 0xC9, 0x09, 0x08, 0xC8, 0xD8, 0x18, 0x19, 0xD9,
             0x1B, 0xDB, 0xDA, 0x1A, 0x1E, 0xDE, 0xDF, 0x1F, 0xDD, 0x1D, 0x1C, 0xDC,
             0x14, 0xD4, 0xD5, 0x15, 0xD7, 0x17, 0x16, 0xD6, 0xD2, 0x12, 0x13, 0xD3,
             0x11, 0xD1, 0xD0, 0x10, 0xF0, 0x30, 0x31, 0xF1, 0x33, 0xF3, 0xF2, 0x32,
             0x36, 0xF6, 0xF7, 0x37, 0xF5, 0x35, 0x34, 0xF4, 0x3C, 0xFC, 0xFD, 0x3D,
             0xFF, 0x3F, 0x3E, 0xFE, 0xFA, 0x3A, 0x3B, 0xFB, 0x39, 0xF9, 0xF8, 0x38,
             0x28, 0xE8, 0xE9, 0x29, 0xEB, 0x2B, 0x2A, 0xEA, 0xEE, 0x2E, 0x2F, 0xEF,
             0x2D, 0xED, 0xEC, 0x2C, 0xE4, 0x24, 0x25, 0xE5, 0x27, 0xE7, 0xE6, 0x26,
             0x22, 0xE2, 0xE3, 0x23, 0xE1, 0x21, 0x20, 0xE0, 0xA0, 0x60, 0x61, 0xA1,
             0x63, 0xA3, 0xA2, 0x62, 0x66, 0xA6, 0xA7, 0x67, 0xA5, 0x65, 0x64, 0xA4,
             0x6C, 0xAC, 0xAD, 0x6D, 0xAF, 0x6F, 0x6E, 0xAE, 0xAA, 0x6A, 0x6B, 0xAB,
             0x69, 0xA9, 0xA8, 0x68, 0x78, 0xB8, 0xB9, 0x79, 0xBB, 0x7B, 0x7A, 0xBA,
             0xBE, 0x7E, 0x7F, 0xBF, 0x7D, 0xBD, 0xBC, 0x7C, 0xB4, 0x74, 0x75, 0xB5,
             0x77, 0xB7, 0xB6, 0x76, 0x72, 0xB2, 0xB3, 0x73, 0xB1, 0x71, 0x70, 0xB0,
             0x50, 0x90, 0x91, 0x51, 0x93, 0x53, 0x52, 0x92, 0x96, 0x56, 0x57, 0x97,
             0x55, 0x95, 0x94, 0x54, 0x9C, 0x5C, 0x5D, 0x9D, 0x5F, 0x9F, 0x9E, 0x5E,
             0x5A, 0x9A, 0x9B, 0x5B, 0x99, 0x59, 0x58, 0x98, 0x88, 0x48, 0x49, 0x89,
             0x4B, 0x8B, 0x8A, 0x4A, 0x4E, 0x8E, 0x8F, 0x4F, 0x8D, 0x4D, 0x4C, 0x8C,
             0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 0x46, 0x86, 0x82, 0x42, 0x43, 0x83,
             0x41, 0x81, 0x80, 0x40
         };
        private byte[] Crc16(byte[] pucFrame, int usLen)
        {
            int i = 0;
            byte[] res = new byte[2] { 0xFF, 0xFF };
            while (usLen-- > 0)
            {
                ushort iIndex = (UInt16)(res[0] ^ pucFrame[i++]);
                res[0] = (byte)(res[1] ^ aucCRCHi[iIndex]);
                res[1] = aucCRCLo[iIndex];
            }
            return res;
        }

        #endregion
    }
}
