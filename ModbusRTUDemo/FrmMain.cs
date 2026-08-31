using ModbusRTUDemo.Helper;
using System;
using System.Globalization;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using xbd.DataConvertLib;

namespace ModbusRTUDemo
{
    #region 存储区枚举类型

    public enum StoreArea
    {
        输出线圈0x,
        输入状态1x,
        保持寄存器4x,
        输入寄存器3x
    }

    #endregion

    public partial class FrmMain : Form
    {
        #region 构造方法

        public FrmMain()
        {
            InitializeComponent();
            this.Load += FrmMain_Load;
        }

        #endregion

        #region 属性及对象

        private ModbusRtu modbusRtu = new ModbusRtu();
        private string CurrentTime => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", DateTimeFormatInfo.InvariantInfo);
        private bool isConnected = false;

        #endregion

        #region 窗体加载事件

        private void FrmMain_Load(object sender, EventArgs e)
        {
            lst_Info.Columns[1].Width = lst_Info.ClientSize.Width - lst_Info.Columns[0].Width;

            var portList = SerialPort.GetPortNames();
            if (portList.Length > 0)
            {
                this.cmb_Port.Items.AddRange(portList);
                this.cmb_Port.SelectedIndex = 0;
            }

            this.cmb_Paud.DataSource = new string[] { "2400", "4800", "9600", "19200", "38400" };
            this.cmb_Paud.SelectedIndex = 2;

            this.cmb_Parity.DataSource = Enum.GetNames(typeof(Parity));

            this.cmb_StopBits.DataSource = Enum.GetNames(typeof(StopBits));
            this.cmb_StopBits.SelectedIndex = 1;

            this.cmb_DataFormat.DataSource = Enum.GetNames(typeof(DataFormat));

            this.cmb_StoreArea.DataSource = Enum.GetNames(typeof(StoreArea));

            this.cmb_VarType.DataSource = Enum.GetNames(typeof(DataType));

        }

        #endregion

        #region 建立连接和断开连接

        private void btn_Connect_Click(object sender, EventArgs e)
        {
            if (isConnected)
            {
                AddLog(1, "ModbusRTU已经连接，请勿重复连接");
                return;
            }
            try
            {
                modbusRtu.OpenSerialPort(int.Parse(this.cmb_Paud.Text.Trim())
                    , this.cmb_Port.Text.Trim()
                    , int.Parse(this.txt_DataBits.Text.Trim())
                    , (Parity)Enum.Parse(typeof(Parity), this.cmb_Parity.SelectedItem.ToString(), false)
                    , (StopBits)Enum.Parse(typeof(StopBits), this.cmb_StopBits.SelectedItem.ToString(), false));
            }
            catch (Exception ex)
            {
                isConnected = false;
                AddLog(1, "ModbusRTU连接失败：" + ex.Message);
                return;
            }

            isConnected = true;
            AddLog(0, "ModbusRTU连接成功");
        }
        private void btn_DisConn_Click(object sender, EventArgs e)
        {
            modbusRtu.CloseSerialPort();
            isConnected = false;
            AddLog(0, "ModbusRTU断开连接");
        }

        #endregion

        #region 写入日志方法

        /// <summary>
        /// 写入日志的通用方法
        /// </summary>
        /// <param name="type">0信息，1警告，2错误</param>
        /// <param name="info"></param>
        private void AddLog(int type, string info)
        {
            ListViewItem lst = new ListViewItem("   " + CurrentTime, type);
            lst.SubItems.Add(info);
            lst_Info.Items.Insert(0, lst);
        }

        #endregion

        #region 修改字节顺序

        private void cmb_DataFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (modbusRtu != null)
            {
                modbusRtu.DataFormat = (DataFormat)Enum.Parse(typeof(DataFormat), this.cmb_DataFormat.SelectedItem.ToString(), false);
            }
        }

        #endregion

        #region 读取和写入

        private void btn_Read_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                AddLog(1, "请先检查与从站之间的连接");
                return;
            }
            if (!ushort.TryParse(this.txt_SlaveAdd.Text.Trim(), out ushort slaveAddr))
            {
                AddLog(1, "读取失败，从站地址必须为正整数");
                return;
            }
            if (!ushort.TryParse(this.txt_Variable.Text.Trim(), out ushort address))
            {
                AddLog(1, "读取失败，起始地址必须为正整数");
                return;
            }
            if (!ushort.TryParse(this.txt_Length.Text.Trim(), out ushort length))
            {
                AddLog(1, "读取失败，读取长度必须为正整数");
                return;
            }

            var dataType = (DataType)Enum.Parse(typeof(DataType), this.cmb_VarType.SelectedItem.ToString(), false);
            var storeArea = (StoreArea)Enum.Parse(typeof(StoreArea), this.cmb_StoreArea.SelectedItem.ToString(), false);
            //创建字节数组
            byte[] result = null;
            string value = string.Empty;
            switch (dataType)
            {
                case DataType.Bool:
                    switch (storeArea)
                    {
                        case StoreArea.输出线圈0x:
                            result = modbusRtu.ReadOutputStatus(slaveAddr, address, length);
                            break;
                        case StoreArea.输入状态1x:
                            result = modbusRtu.ReadInputStatus(slaveAddr, address, length);
                            break;
                        case StoreArea.保持寄存器4x:
                        case StoreArea.输入寄存器3x:
                            AddLog(1, "读取失败，存储区类型不正确");
                            return;
                    }
                    if (result != null)
                    {
                        foreach (var item in result)
                        {
                            char[] array = Convert.ToString(item, 2).PadLeft(8, '0').ToCharArray();
                            Array.Reverse(array);
                            value += new string(array);
                        }
                        AddLog(0, "读取成功，结果为：" + value.Substring(0, length));
                    }
                    else
                    {
                        AddLog(1, "读取失败，地址或长度不正确");
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
                            result = modbusRtu.ReadKeepRegister(slaveAddr, address, length);
                            break;
                        case StoreArea.输入寄存器3x:
                            result = modbusRtu.ReadInputRegister(slaveAddr, address, length);
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
                            result = modbusRtu.ReadKeepRegister(slaveAddr, address, length);
                            break;
                        case StoreArea.输入寄存器3x:
                            result = modbusRtu.ReadInputRegister(slaveAddr, address, length);
                            break;
                    }
                    if (result != null && result.Length == length * 2)
                    {
                        for (int i = 0; i < result.Length; i += 2)
                        {
                            value += ShortLib.GetShortFromByteArray(result, i, modbusRtu.DataFormat).ToString() + " ";
                        }
                        AddLog(0, "读取成功，结果为：" + value.Trim());
                    }
                    else
                    {
                        AddLog(1, "读取失败，地址或长度不正确");
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
                            result = modbusRtu.ReadKeepRegister(slaveAddr, address, length);
                            break;
                        case StoreArea.输入寄存器3x:
                            result = modbusRtu.ReadInputRegister(slaveAddr, address, length);
                            break;
                    }
                    if (result != null && result.Length == length * 2)
                    {
                        for (int i = 0; i < result.Length; i += 2)
                        {
                            value += UShortLib.GetUShortFromByteArray(result, i, modbusRtu.DataFormat).ToString() + " ";
                        }
                        AddLog(0, "读取成功，结果为：" + value.Trim());
                    }
                    else
                    {
                        AddLog(1, "读取失败，地址或长度不正确");
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
                            result = modbusRtu.ReadKeepRegister(slaveAddr, address, length * 2);
                            break;
                        case StoreArea.输入寄存器3x:
                            result = modbusRtu.ReadInputRegister(slaveAddr, address, length * 2);
                            break;
                    }
                    if (result != null && result.Length == length * 4)
                    {
                        for (int i = 0; i < result.Length; i += 4)
                        {
                            value += IntLib.GetIntFromByteArray(result, i, modbusRtu.DataFormat).ToString() + " ";
                        }
                        AddLog(0, "读取成功，结果为：" + value.Trim());
                    }
                    else
                    {
                        AddLog(1, "读取失败，地址或长度不正确");
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
                            result = modbusRtu.ReadKeepRegister(slaveAddr, address, length * 2);
                            break;
                        case StoreArea.输入寄存器3x:
                            result = modbusRtu.ReadInputRegister(slaveAddr, address, length * 2);
                            break;
                    }
                    if (result != null && result.Length == length * 4)
                    {
                        for (int i = 0; i < result.Length; i += 4)
                        {
                            value += UIntLib.GetUIntFromByteArray(result, i, modbusRtu.DataFormat).ToString() + " ";
                        }
                        AddLog(0, "读取成功，结果为：" + value.Trim());
                    }
                    else
                    {
                        AddLog(1, "读取失败，地址或长度不正确");
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
                            result = modbusRtu.ReadKeepRegister(slaveAddr, address, length * 2);
                            break;
                        case StoreArea.输入寄存器3x:
                            result = modbusRtu.ReadInputRegister(slaveAddr, address, length * 2);
                            break;
                    }
                    if (result != null && result.Length == length * 4)
                    {
                        for (int i = 0; i < result.Length; i += 4)
                        {
                            value += FloatLib.GetFloatFromByteArray(result, i, modbusRtu.DataFormat).ToString() + " ";
                        }
                        AddLog(0, "读取成功，结果为：" + value.Trim());
                    }
                    else
                    {
                        AddLog(1, "读取失败，地址或长度不正确");
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
                            result = modbusRtu.ReadKeepRegister(slaveAddr, address, length * 4);
                            break;
                        case StoreArea.输入寄存器3x:
                            result = modbusRtu.ReadInputRegister(slaveAddr, address, length * 4);
                            break;
                    }
                    if (result != null && result.Length == length * 8)
                    {
                        for (int i = 0; i < result.Length; i += 8)
                        {
                            value += DoubleLib.GetDoubleFromByteArray(result, i, modbusRtu.DataFormat).ToString() + " ";
                        }
                        AddLog(0, "读取成功，结果为：" + value.Trim());
                    }
                    else
                    {
                        AddLog(1, "读取失败，地址或长度不正确");
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
                            result = modbusRtu.ReadKeepRegister(slaveAddr, address, length * 4);
                            break;
                        case StoreArea.输入寄存器3x:
                            result = modbusRtu.ReadInputRegister(slaveAddr, address, length * 4);
                            break;
                    }
                    if (result != null && result.Length == length * 8)
                    {
                        for (int i = 0; i < result.Length; i += 8)
                        {
                            value += LongLib.GetLongFromByteArray(result, i, modbusRtu.DataFormat).ToString() + " ";
                        }
                        AddLog(0, "读取成功，结果为：" + value.Trim());
                    }
                    else
                    {
                        AddLog(1, "读取失败，地址或长度不正确");
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
                            result = modbusRtu.ReadKeepRegister(slaveAddr, address, length * 4);
                            break;
                        case StoreArea.输入寄存器3x:
                            result = modbusRtu.ReadInputRegister(slaveAddr, address, length * 4);
                            break;
                    }
                    if (result != null && result.Length == length * 8)
                    {
                        for (int i = 0; i < result.Length; i += 8)
                        {
                            value += ULongLib.GetULongFromByteArray(result, i, modbusRtu.DataFormat).ToString() + " ";
                        }
                        AddLog(0, "读取成功，结果为：" + value.Trim());
                    }
                    else
                    {
                        AddLog(1, "读取失败，地址或长度不正确");
                    }
                    break;
                default:
                    break;
            }
        }
        private void btn_Write_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                AddLog(1, "请先检查与从站之间的连接");
                return;
            }
            if (!ushort.TryParse(this.txt_SlaveAdd.Text.Trim(), out ushort slaveAddr))
            {
                AddLog(1, "写入失败，从站地址必须为正整数");
                return;
            }
            if (!ushort.TryParse(this.txt_Variable.Text.Trim(), out ushort address))
            {
                AddLog(1, "写入失败，起始地址必须为正整数");
                return;
            }
            if (!ushort.TryParse(this.txt_Length.Text.Trim(), out ushort length))
            {
                AddLog(1, "写入失败，读取长度必须为正整数");
                return;
            }
            try
            {
                bool result = false;
                string setValue = this.txt_SetValue.Text.Trim();
                var dataType = (DataType)Enum.Parse(typeof(DataType), this.cmb_VarType.SelectedItem.ToString(), false);
                var storeArea = (StoreArea)Enum.Parse(typeof(StoreArea), this.cmb_StoreArea.SelectedItem.ToString(), false);
                switch (dataType)
                {
                    case DataType.Bool:
                        switch (storeArea)
                        {
                            case StoreArea.输出线圈0x:
                                result = modbusRtu.ForeMultiCoils(slaveAddr, address, BitLib.GetBitArrayFromBitArrayString(setValue));
                                break;
                            case StoreArea.输入状态1x:
                            case StoreArea.保持寄存器4x:
                            case StoreArea.输入寄存器3x:
                                AddLog(1, "写入失败，类型不支持");
                                return;
                        }
                        AddLog(result ? 0 : 1, result ? "写入成功，写入数值为：" + setValue : "写入失败");
                        break;
                    case DataType.Byte:
                        switch (storeArea)
                        {
                            case StoreArea.保持寄存器4x:
                                if (setValue.Contains(" "))
                                {
                                    string[] str = Regex.Split(setValue, "\\s+", RegexOptions.IgnoreCase);
                                    var byteArray = new byte[str.Length];
                                    for (int i = 0; i < str.Length; i++)
                                    {
                                        byteArray[i] = byte.Parse(str[i]);
                                    }
                                    result = modbusRtu.PreSetMultiRegisters(slaveAddr, address, byteArray);
                                }
                                break;
                            case StoreArea.输出线圈0x:
                            case StoreArea.输入状态1x:
                            case StoreArea.输入寄存器3x:
                                AddLog(1, "写入失败，类型不支持");
                                return;
                        }
                        AddLog(result ? 0 : 1, result ? "写入成功，写入数值为：" + setValue : "写入失败");
                        break;
                    case DataType.Short:
                        switch (storeArea)
                        {
                            case StoreArea.保持寄存器4x:
                                result = modbusRtu.PreSetMultiRegisters(slaveAddr, address, ByteArrayLib.GetByteArrayFromShortArray(ShortLib.GetShortArrayFromString(setValue), modbusRtu.DataFormat));
                                break;
                            case StoreArea.输出线圈0x:
                            case StoreArea.输入状态1x:
                            case StoreArea.输入寄存器3x:
                                AddLog(1, "写入失败，类型不支持");
                                return;
                        }
                        AddLog(result ? 0 : 1, result ? "写入成功，写入数值为：" + setValue : "写入失败");
                        break;
                    case DataType.UShort:
                        switch (storeArea)
                        {
                            case StoreArea.保持寄存器4x:
                                result = modbusRtu.PreSetMultiRegisters(slaveAddr, address, ByteArrayLib.GetByteArrayFromUShortArray(UShortLib.GetUShortArrayFromString(setValue), modbusRtu.DataFormat));
                                break;
                            case StoreArea.输出线圈0x:
                            case StoreArea.输入状态1x:
                            case StoreArea.输入寄存器3x:
                                AddLog(1, "写入失败，类型不支持");
                                return;
                        }
                        AddLog(result ? 0 : 1, result ? "写入成功，写入数值为：" + setValue : "写入失败");
                        break;
                    case DataType.Int:
                        switch (storeArea)
                        {
                            case StoreArea.保持寄存器4x:
                                result = modbusRtu.PreSetMultiRegisters(slaveAddr, address, ByteArrayLib.GetByteArrayFromIntArray(IntLib.GetIntArrayFromString(setValue), modbusRtu.DataFormat));
                                break;
                            case StoreArea.输出线圈0x:
                            case StoreArea.输入状态1x:
                            case StoreArea.输入寄存器3x:
                                AddLog(1, "写入失败，类型不支持");
                                return;
                        }
                        AddLog(result ? 0 : 1, result ? "写入成功，写入数值为：" + setValue : "写入失败");
                        break;
                    case DataType.UInt:
                        switch (storeArea)
                        {
                            case StoreArea.保持寄存器4x:
                                result = modbusRtu.PreSetMultiRegisters(slaveAddr, address, ByteArrayLib.GetByteArrayFromUIntArray(UIntLib.GetUIntArrayFromString(setValue), modbusRtu.DataFormat));
                                break;
                            case StoreArea.输出线圈0x:
                            case StoreArea.输入状态1x:
                            case StoreArea.输入寄存器3x:
                                AddLog(1, "写入失败，类型不支持");
                                return;
                        }
                        AddLog(result ? 0 : 1, result ? "写入成功，写入数值为：" + setValue : "写入失败");
                        break;
                    case DataType.Float:
                        switch (storeArea)
                        {
                            case StoreArea.保持寄存器4x:
                                result = modbusRtu.PreSetMultiRegisters(slaveAddr, address, ByteArrayLib.GetByteArrayFromFloatArray(FloatLib.GetFloatArrayFromString(setValue), modbusRtu.DataFormat));
                                break;
                            case StoreArea.输出线圈0x:
                            case StoreArea.输入状态1x:
                            case StoreArea.输入寄存器3x:
                                AddLog(1, "写入失败，类型不支持");
                                return;
                        }
                        AddLog(result ? 0 : 1, result ? "写入成功，写入数值为：" + setValue : "写入失败");
                        break;
                    case DataType.Double:
                        switch (storeArea)
                        {
                            case StoreArea.保持寄存器4x:
                                result = modbusRtu.PreSetMultiRegisters(slaveAddr, address, ByteArrayLib.GetByteArrayFromDoubleArray(DoubleLib.GetDoubleArrayFromString(setValue), modbusRtu.DataFormat));
                                break;
                            case StoreArea.输出线圈0x:
                            case StoreArea.输入状态1x:
                            case StoreArea.输入寄存器3x:
                                AddLog(1, "写入失败，类型不支持");
                                return;
                        }
                        AddLog(result ? 0 : 1, result ? "写入成功，写入数值为：" + setValue : "写入失败");
                        break;
                    case DataType.Long:
                        switch (storeArea)
                        {
                            case StoreArea.保持寄存器4x:
                                result = modbusRtu.PreSetMultiRegisters(slaveAddr, address, ByteArrayLib.GetByteArrayFromLongArray(LongLib.GetLongArrayFromString(setValue), modbusRtu.DataFormat));
                                break;
                            case StoreArea.输出线圈0x:
                            case StoreArea.输入状态1x:
                            case StoreArea.输入寄存器3x:
                                AddLog(1, "写入失败，类型不支持");
                                return;
                        }
                        AddLog(result ? 0 : 1, result ? "写入成功，写入数值为：" + setValue : "写入失败");
                        break;
                    case DataType.ULong:
                        switch (storeArea)
                        {
                            case StoreArea.保持寄存器4x:
                                result = modbusRtu.PreSetMultiRegisters(slaveAddr, address, ByteArrayLib.GetByteArrayFromULongArray(ULongLib.GetULongArrayFromString(setValue), modbusRtu.DataFormat));
                                break;
                            case StoreArea.输出线圈0x:
                            case StoreArea.输入状态1x:
                            case StoreArea.输入寄存器3x:
                                AddLog(1, "写入失败，类型不支持");
                                return;
                        }
                        AddLog(result ? 0 : 1, result ? "写入成功，写入数值为：" + setValue : "写入失败");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                AddLog(1, "写入失败：" + ex.Message);
                return;
            }
        }

        #endregion


    }
}
