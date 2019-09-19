using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace DatabaseHelper
{
    public partial class Form1 : Form
    {
        public static BusinessController m_BusCtrl;
        private SettingForm m_SettingWnd;
        private BindingForm m_DrugWnd;

        public Form1()
        {
            InitializeComponent();

            m_BusCtrl = new BusinessController();
        }

        private void 建立连接ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_SettingWnd = new SettingForm();
            m_SettingWnd.MdiParent = this;

            m_SettingWnd.Show();
        }

        private void 中成药数据绑定ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_DrugWnd = new BindingForm();
            m_DrugWnd.Show();
        }
    }
}
