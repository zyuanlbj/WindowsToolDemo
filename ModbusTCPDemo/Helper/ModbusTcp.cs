using ModbusRTUDemo.Helper;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using xbd.DataConvertLib;

namespace ModbusTCPDemo.Helper
{
    /// <summary>
    /// ModbusTCP通信类
    /// </summary>
    public class ModbusTcp
    {
        #region 属性及变量

        //创建通信对象
        private Socket tcpClient;

        public int SendTimeout { get; set; } = 2000;
        public int ReceiveTimeout { get; set; } = 2000;
        public int SlaveAddr { get; set; } = 1;
        public int MaxCycleTimes { get; set; } = 5;

        public DataFormat DataFormat { get; set; } = DataFormat.ABCD;
        private SimpleHybirdLock _interactiveLock = new SimpleHybirdLock();

        #endregion

        #region 建立及断开连接

        public void Connect(string ip, string port)
        {
            tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                SendTimeout = this.SendTimeout,
                ReceiveTimeout = this.ReceiveTimeout
            };
            var endPoint = new IPEndPoint(IPAddress.Parse(ip), int.Parse(port));
            tcpClient.Connect(endPoint);
        }
        public void Disconnect()
        {
            tcpClient?.Close();
        }

        #endregion

        #region 功能码01H 读取输出线圈

        public byte[] ReadOutputStatus(int address, int length)
        {
            ByteArray sendCommand = new ByteArray();
            sendCommand.Add(new byte[] { 0, 0, 0, 0, 0, 6, (byte)SlaveAddr });
            sendCommand.Add(new byte[] { 0x01 });
            sendCommand.Add(new byte[] { (byte)(address / 256), (byte)(address % 256) });
            sendCommand.Add(new byte[] { (byte)(length / 256), (byte)(length % 256) });

            int byteCount = length % 8 == 0 ? length / 8 : length / 8 + 1;
            var rcv = SendAndReceiveData(sendCommand.array);
            if (rcv != null && rcv.Length == 9 + byteCount)
            {
                if (rcv[7] == 0x01 && rcv[8] == byteCount)
                {
                    return ByteArrayLib.GetByteArrayFromByteArray(rcv, 9, byteCount);
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

        #region 功能码02H 读取输入线圈

        public byte[] ReadInputCoil(int address, int length)
        {
            ByteArray sendCommand = new ByteArray();
            sendCommand.Add(new byte[] { 0, 0, 0, 0, 0, 6, (byte)SlaveAddr });
            sendCommand.Add(new byte[] { 0x02 });
            sendCommand.Add(new byte[] { (byte)(address / 256), (byte)(address % 256) });
            sendCommand.Add(new byte[] { (byte)(length / 256), (byte)(length % 256) });

            int byteCount = length % 8 == 0 ? length / 8 : length / 8 + 1;
            var rcv = SendAndReceiveData(sendCommand.array);
            if (rcv != null && rcv.Length == 9 + byteCount)
            {
                if (rcv[7] == 0x02 && rcv[8] == byteCount)
                {
                    return ByteArrayLib.GetByteArrayFromByteArray(rcv, 9, byteCount);
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

        #region 功能码03H 读取保持寄存器

        public byte[] ReadKeepRegister(int address, int length)
        {
            ByteArray sendCommand = new ByteArray();
            sendCommand.Add(new byte[] { 0, 0, 0, 0, 0, 6, (byte)SlaveAddr });
            sendCommand.Add(new byte[] { 0x03 });
            sendCommand.Add(new byte[] { (byte)(address / 256), (byte)(address % 256) });
            sendCommand.Add(new byte[] { (byte)(length / 256), (byte)(length % 256) });

            int byteCount = length * 2;
            var rcv = SendAndReceiveData(sendCommand.array);
            if (rcv != null && rcv.Length == 9 + byteCount)
            {
                if (rcv[7] == 0x03 && rcv[8] == byteCount)
                {
                    return ByteArrayLib.GetByteArrayFromByteArray(rcv, 9, byteCount);
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

        #region 功能码04H 读取输入寄存器

        public byte[] ReadInputRegister(int address, int length)
        {
            ByteArray sendCommand = new ByteArray();
            sendCommand.Add(new byte[] { 0, 0, 0, 0, 0, 6, (byte)SlaveAddr });
            sendCommand.Add(new byte[] { 0x04 });
            sendCommand.Add(new byte[] { (byte)(address / 256), (byte)(address % 256) });
            sendCommand.Add(new byte[] { (byte)(length / 256), (byte)(length % 256) });

            int byteCount = length * 2;
            var rcv = SendAndReceiveData(sendCommand.array);
            if (rcv != null && rcv.Length == 9 + byteCount)
            {
                if (rcv[7] == 0x04 && rcv[8] == byteCount)
                {
                    return ByteArrayLib.GetByteArrayFromByteArray(rcv, 9, byteCount);
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

        #region 功能码05H 预置单线圈

        public bool ForceCoil(int address, bool setValue)
        {
            ByteArray sendCommand = new ByteArray();
            sendCommand.Add(new byte[] { 0, 0, 0, 0, 0, 6, (byte)SlaveAddr });
            sendCommand.Add(new byte[] { 0x05 });
            sendCommand.Add(new byte[] { (byte)(address / 256), (byte)(address % 256) });
            if (setValue)
            {
                sendCommand.Add(new byte[] { 0xFF, 0x00 });
            }
            else
            {
                sendCommand.Add(new byte[] { 0x00, 0x00 });
            }
            var rcv = SendAndReceiveData(sendCommand.array);
            if (rcv != null)
            {
                return ByteArrayLib.GetByteArrayEquals(sendCommand.array, rcv);
            }
            return false;
        }

        #endregion

        #region 功能码06H 预置单寄存器

        public bool PreSetSingleRegister(int address, short setValue)
        {
            ByteArray sendCommand = new ByteArray();
            sendCommand.Add(new byte[] { 0, 0, 0, 0, 0, 6, (byte)SlaveAddr });
            sendCommand.Add(new byte[] { 0x06 });
            sendCommand.Add(new byte[] { (byte)(address / 256), (byte)(address % 256) });
            sendCommand.Add(ByteArrayLib.GetByteArrayFromShort(setValue, DataFormat));
            var rcv = SendAndReceiveData(sendCommand.array);
            if (rcv != null)
            {
                return ByteArrayLib.GetByteArrayEquals(sendCommand.array, rcv);
            }
            return false;
        }
        public bool PreSetSingleRegister(int address, ushort setValue)
        {
            ByteArray sendCommand = new ByteArray();
            sendCommand.Add(new byte[] { 0, 0, 0, 0, 0, 6, (byte)SlaveAddr });
            sendCommand.Add(new byte[] { 0x06 });
            sendCommand.Add(new byte[] { (byte)(address / 256), (byte)(address % 256) });
            sendCommand.Add(ByteArrayLib.GetByteArrayFromUShort(setValue, DataFormat));
            var rcv = SendAndReceiveData(sendCommand.array);
            if (rcv != null)
            {
                return ByteArrayLib.GetByteArrayEquals(sendCommand.array, rcv);
            }
            return false;
        }

        #endregion

        #region 功能码0FH 预置多个线圈

        public bool ForceMultiCoils(int address, bool[] setValue)
        {
            var setByte = ByteArrayLib.GetByteArrayFromBoolArray(setValue);
            ByteArray sendCommand = new ByteArray();
            sendCommand.Add(new byte[] { 0, 0, 0, 0 });
            int byteLength = 7 + setByte.Length;

            //长度
            sendCommand.Add(new byte[] { (byte)(byteLength / 256), (byte)(byteLength % 256) });

            //单元标识符和功能码
            sendCommand.Add(new byte[] { (byte)SlaveAddr, 0x0F });

            //起始地址
            sendCommand.Add(new byte[] { (byte)(address / 256), (byte)(address % 256) });

            //线圈数量
            sendCommand.Add(new byte[] { (byte)(setValue.Length / 256), (byte)(setValue.Length % 256) });

            //字节计数
            sendCommand.Add((byte)setByte.Length);

            //具体写入值
            sendCommand.Add(setByte);

            byte[] rcv = SendAndReceiveData(sendCommand.array);
            if (rcv != null)
            {
                var send = ByteArrayLib.GetByteArrayFromByteArray(sendCommand.array, 0, 12);
                send[4] = 0x00;
                send[5] = 0x06;

                return ByteArrayLib.GetByteArrayEquals(send, rcv) && rcv[7] == 0x0F;
            }
            return false;
        }

        #endregion

        #region 功能码10H 预置多个寄存器

        public bool PreSetMultiRegisters(int address, byte[] setValue)
        {
            if (setValue == null || setValue.Length == 0 || setValue.Length % 2 == 1)
            {
                return false;
            }

            int regLength = setValue.Length / 2;

            ByteArray sendCommand = new ByteArray();
            sendCommand.Add(new byte[] { 0, 0, 0, 0 });
            int byteLength = 7 + setValue.Length;

            //长度
            sendCommand.Add(new byte[] { (byte)(byteLength / 256), (byte)(byteLength % 256) });

            //单元标识符和功能码
            sendCommand.Add(new byte[] { (byte)SlaveAddr, 0x10 });

            //起始地址
            sendCommand.Add(new byte[] { (byte)(address / 256), (byte)(address % 256) });

            //寄存器数量
            sendCommand.Add(new byte[] { (byte)(regLength / 256), (byte)(regLength % 256) });

            //字节计数
            sendCommand.Add((byte)(setValue.Length));

            //具体写入值
            sendCommand.Add(setValue);

            byte[] rcv = SendAndReceiveData(sendCommand.array);
            if (rcv != null)
            {
                var send = ByteArrayLib.GetByteArrayFromByteArray(sendCommand.array, 0, 12);
                send[4] = 0x00;
                send[5] = 0x06;

                return ByteArrayLib.GetByteArrayEquals(send, rcv) && rcv[7] == 0x10;
            }
            return false;
        }
        public bool PreSetMultiRegisters(int address,short setValue)
        {
            return PreSetMultiRegisters(address, ByteArrayLib.GetByteArrayFromShort(setValue, DataFormat));
        }
        public bool PreSetMultiRegisters(int address, ushort setValue)
        {
            return PreSetMultiRegisters(address, ByteArrayLib.GetByteArrayFromUShort(setValue, DataFormat));
        }
        public bool PreSetMultiRegisters(int address, int setValue)
        {
            return PreSetMultiRegisters(address, ByteArrayLib.GetByteArrayFromInt(setValue, DataFormat));
        }
        public bool PreSetMultiRegisters(int address, uint setValue)
        {
            return PreSetMultiRegisters(address, ByteArrayLib.GetByteArrayFromUInt(setValue, DataFormat));
        }
        public bool PreSetMultiRegisters(int address, float setValue)
        {
            return PreSetMultiRegisters(address, ByteArrayLib.GetByteArrayFromFloat(setValue, DataFormat));
        }
        public bool PreSetMultiRegisters(int address, double setValue)
        {
            return PreSetMultiRegisters(address, ByteArrayLib.GetByteArrayFromDouble(setValue, DataFormat));
        }
        public bool PreSetMultiRegisters(int address, long setValue)
        {
            return PreSetMultiRegisters(address, ByteArrayLib.GetByteArrayFromLong(setValue, DataFormat));
        }
        public bool PreSetMultiRegisters(int address, ulong setValue)
        {
            return PreSetMultiRegisters(address, ByteArrayLib.GetByteArrayFromULong(setValue, DataFormat));
        }

        #endregion

        #region 发送并接收报文

        private byte[] SendAndReceiveData(byte[] array)
        {
            _interactiveLock.Enter();

            try
            {
                tcpClient.Send(array);
                return ParseResponse();
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                _interactiveLock.Leave();
            }
        }

        private byte[] ParseResponse()
        {
            int count = tcpClient.Available;
            int cycle = 0;
            while (count == 0)
            {
                count = tcpClient.Available;
                cycle++;
                Thread.Sleep(20);
                if (cycle > MaxCycleTimes)
                {
                    break;
                }
            }
            if (count == 0)
            {
                return null;
            }
            else
            {
                var buffer = new byte[count];
                tcpClient.Receive(buffer, count, SocketFlags.None);
                return buffer;
            }
        }

        #endregion
    }
}
