using ModbusRTUDemo;
using ModbusRTUDemo.Helper;
using ModbusTCPDemo.Helper;
using System;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using xbd.DataConvertLib;

namespace ModbusTCPDemo
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
            this.Load += FrmMain_Load;
        }

        #region 属性及变量

        private ModbusTcp client = new ModbusTcp();
        private bool isConnected = false;
        public string CurrentTime => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", DateTimeFormatInfo.InvariantInfo);

        #endregion

        #region 加载事件

        private void FrmMain_Load(object sender, EventArgs e)
        {
            this.cmb_DataFormat.DataSource = Enum.GetNames(typeof(DataFormat));
            this.cmb_StoreArea.DataSource = Enum.GetNames(typeof(StoreArea));
            this.cmb_VarType.DataSource = Enum.GetNames(typeof(DataType));
            this.lstInfo.Columns[1].Width = lstInfo.ClientSize.Width - this.lstInfo.Columns[0].Width;
        }

        #endregion

        #region 连接与断开连接

        private void btn_Connect_Click(object sender, EventArgs e)
        {
            if (!IPAddress.TryParse(this.txt_IP.Text.Trim(), out IPAddress ip))
            {
                AddLog(1, "IP地址格式不正确");
                return;
            }
            if (!int.TryParse(this.txt_Port.Text.Trim(), out int port))
            {
                AddLog(1, "端口号格式不正确");
                return;
            }
            try
            {
                client.Connect(this.txt_IP.Text.Trim(), this.txt_Port.Text.Trim());
            }
            catch (Exception ex)
            {
                isConnected = false;
                AddLog(2, "连接失败：" + ex.Message);
                return;
            }
            isConnected = true;
            AddLog(0, "连接成功");
        }

        private void btn_DisConn_Click(object sender, EventArgs e)
        {
            client?.Disconnect();
            isConnected = false;
            AddLog(0, "断开连接");
        }

        #endregion

        #region 读取数据

        private void btn_Read_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                AddLog(1, "请检查通信连接状态");
                return;
            }
            if (!ushort.TryParse(this.txt_SlaveAdd.Text.Trim(), out ushort slaveAddr))
            {
                AddLog(1, "请检查从站地址的数据格式");
                return;
            }
            if (!ushort.TryParse(this.txt_Variable.Text.Trim(), out ushort address))
            {
                AddLog(1, "请检查起始地址的数据格式");
                return;
            }
            if (!ushort.TryParse(this.txt_Length.Text.Trim(), out ushort length))
            {
                AddLog(1, "请检查读取长度的数据格式");
                return;
            }
            //存储区
            StoreArea storeArea = (StoreArea)Enum.Parse(typeof(StoreArea), this.cmb_StoreArea.SelectedItem.ToString());
            //变量类型
            DataType varType = (DataType)Enum.Parse(typeof(DataType), this.cmb_VarType.SelectedItem.ToString());
            client.SlaveAddr = slaveAddr;
            byte[] result = null;
            string value = string.Empty;
            switch (varType)
            {
                case DataType.Bool:
                    switch (storeArea)
                    {
                        case StoreArea.输出线圈0x:
                            result = client.ReadOutputStatus(address, length);
                            break;
                        case StoreArea.输入状态1x:
                            result = client.ReadInputCoil(address, length);
                            break;
                        case StoreArea.保持寄存器4x:
                        case StoreArea.输入寄存器3x:
                            AddLog(1, "读取失败，存储区类型不正确");
                            return;
                    }
                    if (result != null)
                    {
                        //111       00000111
                        foreach (var item in result)
                        {
                            char[] c = Convert.ToString(item, 2).PadLeft(8, '0').ToCharArray();
                            Array.Reverse(c);
                            value += new string(c);
                        }
                        AddLog(0, "读取成功，结果为：" + value.Substring(0, length));
                    }
                    else
                    {
                        AddLog(1, "读取失败，请检查地址、类型或连接状态");
                    }
                    break;
                case DataType.Byte:
                    switch (storeArea)
                    {
                        case StoreArea.输出线圈0x:
                        case StoreArea.输入状态1x:
                            AddLog(1, "读取失败，存储区类型不正确");
                            return;
                        case StoreArea.保持寄存器4x:
                            result = client.ReadKeepRegister(address, length);
                            break;
                        case StoreArea.输入寄存器3x:
                            result = client.ReadInputRegister(address, length);
                            break;
                    }
                    if (result != null)
                    {
                        foreach (var item in result)
                        {
                            value += item.ToString() + " ";
                        }
                        AddLog(0, "读取成功，结果为：" + value.Trim());
                    }
                    else
                    {
                        AddLog(1, "读取失败，地址或长度不正确");
                    }
                    break;
                case DataType.Short:
                    switch (storeArea)
                    {
                        case StoreArea.输出线圈0x:
                        case StoreArea.输入状态1x:
                            AddLog(1, "读取失败，存储区类型不正确");
                            return;
                        case StoreArea.保持寄存器4x:
                            result = client.ReadKeepRegister(address, length);
                            break;
                        case StoreArea.输入寄存器3x:
                            result = client.ReadInputRegister(address, length);
                            break;
                        default:
                            break;
                    }
                    if (result != null && result.Length == length * 2)
                    {
                        for (int i = 0; i < result.Length; i += 2)
                        {
                            value += ShortLib.GetShortFromByteArray(result, i, client.DataFormat) + " ";
                        }
                        AddLog(0, "读取成功，结果为：" + value.Trim());
                    }
                    else
                    {
                        AddLog(1, "读取失败，请检查地址、类型或连接状态");
                    }
                    break;
                case DataType.UShort:
                    switch (storeArea)
                    {
                        case StoreArea.输出线圈0x:
                        case StoreArea.输入状态1x:
                            AddLog(1, "读取失败，存储区类型不正确");
                            return;
                        case StoreArea.保持寄存器4x:
                            result = client.ReadKeepRegister(address, length);
                            break;
                        case StoreArea.输入寄存器3x:
                            result = client.ReadInputRegister(address, length);
                            break;
                        default:
                            break;
                    }
                    if (result != null && result.Length == length * 2)
                    {
                        for (int i = 0; i < result.Length; i += 2)
                        {
                            value += UShortLib.GetUShortFromByteArray(result, i, client.DataFormat) + " ";
                        }
                        AddLog(0, "读取成功，结果为：" + value.Trim());
                    }
                    else
                    {
                        AddLog(1, "读取失败，请检查地址、类型或连接状态");
                    }
                    break;
                case DataType.Int:
                    switch (storeArea)
                    {
                        case StoreArea.输出线圈0x:
                        case StoreArea.输入状态1x:
                            AddLog(1, "读取失败，存储区类型不正确");
                            return;
                        case StoreArea.保持寄存器4x:
                            result = client.ReadKeepRegister(address, length * 2);
                            break;
                        case StoreArea.输入寄存器3x:
                            result = client.ReadInputRegister(address, length * 2);
                            break;
                        default:
                            break;
                    }
                    if (result != null && result.Length == length * 4)
                    {
                        for (int i = 0; i < result.Length; i += 4)
                        {
                            value += IntLib.GetIntFromByteArray(result, i, client.DataFormat) + " ";
                        }
                        AddLog(0, "读取成功，结果为：" + value.Trim());
                    }
                    else
                    {
                        AddLog(1, "读取失败，请检查地址、类型或连接状态");
                    }
                    break;
                case DataType.UInt:
                    switch (storeArea)
                    {
                        case StoreArea.输出线圈0x:
                        case StoreArea.输入状态1x:
                            AddLog(1, "读取失败，存储区类型不正确");
                            return;
                        case StoreArea.保持寄存器4x:
                            result = client.ReadKeepRegister(address, length * 2);
                            break;
                        case StoreArea.输入寄存器3x:
                            result = client.ReadInputRegister(address, length * 2);
                            break;
                        default:
                            break;
                    }
                    if (result != null && result.Length == length * 4)
                    {
                        for (int i = 0; i < result.Length; i += 4)
                        {
                            value += UIntLib.GetUIntFromByteArray(result, i, client.DataFormat) + " ";
                        }
                        AddLog(0, "读取成功，结果为：" + value.Trim());
                    }
                    else
                    {
                        AddLog(1, "读取失败，请检查地址、类型或连接状态");
                    }
                    break;
                case DataType.Float:
                    switch (storeArea)
                    {
                        case StoreArea.输出线圈0x:
                        case StoreArea.输入状态1x:
                            AddLog(1, "读取失败，存储区类型不正确");
                            return;
                        case StoreArea.保持寄存器4x:
                            result = client.ReadKeepRegister(address, length * 2);
                            break;
                        case StoreArea.输入寄存器3x:
                            result = client.ReadInputRegister(address, length * 2);
                            break;
                        default:
                            break;
                    }
                    if (result != null && result.Length == length * 4)
                    {
                        for (int i = 0; i < result.Length; i += 4)
                        {
                            value += FloatLib.GetFloatFromByteArray(result, i, client.DataFormat) + " ";
                        }
                        AddLog(0, "读取成功，结果为：" + value.Trim());
                    }
                    else
                    {
                        AddLog(1, "读取失败，请检查地址、类型或连接状态");
                    }
                    break;
                case DataType.Double:
                    switch (storeArea)
                    {
                        case StoreArea.输出线圈0x:
                        case StoreArea.输入状态1x:
                            AddLog(1, "读取失败，存储区类型不正确");
                            return;
                        case StoreArea.保持寄存器4x:
                            result = client.ReadKeepRegister(address, length * 4);
                            break;
                        case StoreArea.输入寄存器3x:
                            result = client.ReadInputRegister(address, length * 4);
                            break;
                        default:
                            break;
                    }
                    if (result != null && result.Length == length * 8)
                    {
                        for (int i = 0; i < result.Length; i += 8)
                        {
                            value += DoubleLib.GetDoubleFromByteArray(result, i, client.DataFormat) + " ";
                        }
                        AddLog(0, "读取成功，结果为：" + value.Trim());
                    }
                    else
                    {
                        AddLog(1, "读取失败，请检查地址、类型或连接状态");
                    }
                    break;
                case DataType.Long:
                    switch (storeArea)
                    {
                        case StoreArea.输出线圈0x:
                        case StoreArea.输入状态1x:
                            AddLog(1, "读取失败，存储区类型不正确");
                            return;
                        case StoreArea.保持寄存器4x:
                            result = client.ReadKeepRegister(address, length * 4);
                            break;
                        case StoreArea.输入寄存器3x:
                            result = client.ReadInputRegister(address, length * 4);
                            break;
                        default:
                            break;
                    }
                    if (result != null && result.Length == length * 8)
                    {
                        for (int i = 0; i < result.Length; i += 8)
                        {
                            value += LongLib.GetLongFromByteArray(result, i, client.DataFormat) + " ";
                        }
                        AddLog(0, "读取成功，结果为：" + value.Trim());
                    }
                    else
                    {
                        AddLog(1, "读取失败，请检查地址、类型或连接状态");
                    }
                    break;
                case DataType.ULong:
                    switch (storeArea)
                    {
                        case StoreArea.输出线圈0x:
                        case StoreArea.输入状态1x:
                            AddLog(1, "读取失败，存储区类型不正确");
                            return;
                        case StoreArea.保持寄存器4x:
                            result = client.ReadKeepRegister(address, length * 4);
                            break;
                        case StoreArea.输入寄存器3x:
                            result = client.ReadInputRegister(address, length * 4);
                            break;
                        default:
                            break;
                    }
                    if (result != null && result.Length == length * 8)
                    {
                        for (int i = 0; i < result.Length; i += 8)
                        {
                            value += ULongLib.GetULongFromByteArray(result, i, client.DataFormat) + " ";
                        }
                        AddLog(0, "读取成功，结果为：" + value.Trim());
                    }
                    else
                    {
                        AddLog(1, "读取失败，请检查地址、类型或连接状态");
                    }
                    break;
            }
        }

        #endregion

        #region 写入数据

        private void btn_Write_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                AddLog(1, "请检查通信连接状态");
                return;
            }
            if (!ushort.TryParse(this.txt_SlaveAdd.Text.Trim(), out ushort slaveAddr))
            {
                AddLog(1, "请检查从站地址的数据格式");
                return;
            }
            if (!ushort.TryParse(this.txt_Variable.Text.Trim(), out ushort address))
            {
                AddLog(1, "请检查起始地址的数据格式");
                return;
            }

            //存储区
            StoreArea storeArea = (StoreArea)Enum.Parse(typeof(StoreArea), this.cmb_StoreArea.SelectedItem.ToString());

            //变量类型
            DataType varType = (DataType)Enum.Parse(typeof(DataType), this.cmb_VarType.SelectedItem.ToString());

            client.SlaveAddr = slaveAddr;

            string setText = this.txt_SetValue.Text.Trim();

            bool result = false;
            switch (varType)
            {
                case DataType.Bool:
                    switch (storeArea)
                    {
                        case StoreArea.输出线圈0x:
                            result = client.ForceMultiCoils(address, BitLib.GetBitArrayFromBitArrayString(setText));
                            break;
                        case StoreArea.输入状态1x:
                        case StoreArea.保持寄存器4x:
                        case StoreArea.输入寄存器3x:
                            AddLog(1, "写入失败，类型不支持");
                            return;
                    }
                    AddLog(result ? 0 : 1, result ? "写入成功，写入数值：" + setText : "写入失败");
                    break;
                case DataType.Byte:
                    switch (storeArea)
                    {
                        case StoreArea.保持寄存器4x:
                            if (setText.Contains(" "))
                            {
                                string[] str = Regex.Split(setText, "\\s+", RegexOptions.IgnoreCase);
                                var byteArray = new byte[str.Length];
                                for (int i = 0; i < str.Length; i++)
                                {
                                    byteArray[i] = byte.Parse(str[i]);
                                }
                                result = client.PreSetMultiRegisters(address, byteArray);
                            }
                            break;
                        case StoreArea.输出线圈0x:
                        case StoreArea.输入状态1x:
                        case StoreArea.输入寄存器3x:
                            AddLog(1, "写入失败，类型不支持");
                            return;
                    }
                    AddLog(result ? 0 : 1, result ? "写入成功，写入数值为：" + setText : "写入失败");
                    break;
                case DataType.Short:
                    switch (storeArea)
                    {
                        case StoreArea.保持寄存器4x:
                            result = client.PreSetMultiRegisters(address, ByteArrayLib.GetByteArrayFromShortArray(ShortLib.GetShortArrayFromString(setText), client.DataFormat));
                            break;
                        case StoreArea.输出线圈0x:
                        case StoreArea.输入状态1x:
                        case StoreArea.输入寄存器3x:
                            AddLog(1, "写入失败，类型不支持");
                            return;
                    }
                    AddLog(result ? 0 : 1, result ? "写入成功，写入数值为：" + setText : "写入失败");
                    break;
                case DataType.UShort:
                    switch (storeArea)
                    {
                        case StoreArea.保持寄存器4x:
                            result = client.PreSetMultiRegisters(address, ByteArrayLib.GetByteArrayFromUShortArray(UShortLib.GetUShortArrayFromString(setText), client.DataFormat));
                            break;
                        case StoreArea.输出线圈0x:
                        case StoreArea.输入状态1x:
                        case StoreArea.输入寄存器3x:
                            AddLog(1, "写入失败，类型不支持");
                            return;
                    }
                    AddLog(result ? 0 : 1, result ? "写入成功，写入数值为：" + setText : "写入失败");
                    break;
                case DataType.Int:
                    switch (storeArea)
                    {
                        case StoreArea.保持寄存器4x:
                            result = client.PreSetMultiRegisters(address, ByteArrayLib.GetByteArrayFromIntArray(IntLib.GetIntArrayFromString(setText), client.DataFormat));
                            break;
                        case StoreArea.输出线圈0x:
                        case StoreArea.输入状态1x:
                        case StoreArea.输入寄存器3x:
                            AddLog(1, "写入失败，类型不支持");
                            return;
                    }
                    AddLog(result ? 0 : 1, result ? "写入成功，写入数值为：" + setText : "写入失败");
                    break;
                case DataType.UInt:
                    switch (storeArea)
                    {
                        case StoreArea.保持寄存器4x:
                            result = client.PreSetMultiRegisters(address, ByteArrayLib.GetByteArrayFromUIntArray(UIntLib.GetUIntArrayFromString(setText), client.DataFormat));
                            break;
                        case StoreArea.输出线圈0x:
                        case StoreArea.输入状态1x:
                        case StoreArea.输入寄存器3x:
                            AddLog(1, "写入失败，类型不支持");
                            return;
                    }
                    AddLog(result ? 0 : 1, result ? "写入成功，写入数值为：" + setText : "写入失败");
                    break;
                case DataType.Float:
                    switch (storeArea)
                    {
                        case StoreArea.保持寄存器4x:
                            result = client.PreSetMultiRegisters(address, ByteArrayLib.GetByteArrayFromFloatArray(FloatLib.GetFloatArrayFromString(setText), client.DataFormat));
                            break;
                        case StoreArea.输出线圈0x:
                        case StoreArea.输入状态1x:
                        case StoreArea.输入寄存器3x:
                            AddLog(1, "写入失败，类型不支持");
                            return;
                    }
                    AddLog(result ? 0 : 1, result ? "写入成功，写入数值为：" + setText : "写入失败");
                    break;
                case DataType.Double:
                    switch (storeArea)
                    {
                        case StoreArea.保持寄存器4x:
                            result = client.PreSetMultiRegisters(address, ByteArrayLib.GetByteArrayFromDoubleArray(DoubleLib.GetDoubleArrayFromString(setText), client.DataFormat));
                            break;
                        case StoreArea.输出线圈0x:
                        case StoreArea.输入状态1x:
                        case StoreArea.输入寄存器3x:
                            AddLog(1, "写入失败，类型不支持");
                            return;
                    }
                    AddLog(result ? 0 : 1, result ? "写入成功，写入数值为：" + setText : "写入失败");
                    break;
                case DataType.Long:
                    switch (storeArea)
                    {
                        case StoreArea.保持寄存器4x:
                            result = client.PreSetMultiRegisters(address, ByteArrayLib.GetByteArrayFromLongArray(LongLib.GetLongArrayFromString(setText), client.DataFormat));
                            break;
                        case StoreArea.输出线圈0x:
                        case StoreArea.输入状态1x:
                        case StoreArea.输入寄存器3x:
                            AddLog(1, "写入失败，类型不支持");
                            return;
                    }
                    AddLog(result ? 0 : 1, result ? "写入成功，写入数值为：" + setText : "写入失败");
                    break;
                case DataType.ULong:
                    switch (storeArea)
                    {
                        case StoreArea.保持寄存器4x:
                            result = client.PreSetMultiRegisters(address, ByteArrayLib.GetByteArrayFromULongArray(ULongLib.GetULongArrayFromString(setText), client.DataFormat));
                            break;
                        case StoreArea.输出线圈0x:
                        case StoreArea.输入状态1x:
                        case StoreArea.输入寄存器3x:
                            AddLog(1, "写入失败，类型不支持");
                            return;
                    }
                    AddLog(result ? 0 : 1, result ? "写入成功，写入数值为：" + setText : "写入失败");
                    break;
            }
        }

        #endregion

        #region 写入日志通用方法

        /// <summary>
        /// 写入日志通用方法
        /// </summary>
        /// <param name="type"></param>
        /// <param name="info"></param>
        private void AddLog(int type, string info)
        {
            //类型  时间  日志

            ListViewItem lst = new ListViewItem("   " + CurrentTime, type);
            lst.SubItems.Add(info);
            lstInfo.Items.Insert(0, lst);
        }

        #endregion

        #region 选择字节顺序

        private void cmb_DataFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (client != null)
            {
                client.DataFormat = (DataFormat)Enum.Parse(typeof(DataFormat), this.cmb_DataFormat.SelectedItem.ToString(), false);
            }
        }

        #endregion

    }
}
