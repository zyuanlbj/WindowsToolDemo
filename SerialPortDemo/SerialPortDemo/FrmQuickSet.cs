using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SerialPortDemo
{
    public partial class FrmQuickSet : Form
    {
        public string CmdName { get; set; }
        public string CmdContent { get; set; }

        public FrmQuickSet()
        {
            InitializeComponent();
        }
        public FrmQuickSet(string cmdName,string cmdContent):this()
        {
            this.txtCmdName.Text = cmdName;
            this.txtCmdContent.Text = cmdContent;

            this.txtCmdName.Focus();
            this.txtCmdName.SelectAll();
        }
    
        //确定
        private void btnConfirm_Click(object sender, EventArgs e)
        {
            string temp = this.txtCmdContent.Text.Trim().Replace(" ", "");
            if (HexHelper.IsHexString(temp)&&temp.Length%2==0)
            {
                this.CmdName = this.txtCmdName.Text;
                this.CmdContent = this.txtCmdContent.Text;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("请检查命名内容格式是否为16进制！", "命令错误");
            }
        }
        //取消
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtCmdContent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode==Keys.Enter)
            {
                btnConfirm_Click(null, null);
            }
        }

        private void txtCmdName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode==Keys.Enter)
            {
                this.txtCmdContent.Focus();
            }
        }
    }
}
