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
    public partial class SettingForm : Form
    {
        public SettingForm()
        {
            InitializeComponent();

            this.textBox_host.Text = "192.168.103.191";
            this.textBox_port.Text = "3306";
            this.textBox_dbName.Text = "mgrdb";
            this.textBox_user.Text = "root";
            this.textBox_pwd.Text = "sinosoft-123456";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String host = this.textBox_host.Text;
            String port = this.textBox_port.Text;
            String dbName = this.textBox_dbName.Text;
            String user = this.textBox_user.Text;
            String password = this.textBox_pwd.Text;

            if (host.Equals(""))
            {
                MessageBox.Show("主机不可为空!");
                return;
            }

            if (port.Equals(""))
            {
                port = "3306";
            }

            if (dbName.Equals(""))
            {
                MessageBox.Show("数据库名不可为空!");
                return;
            }

            if (user.Equals(""))
            {
                MessageBox.Show("用户名不可为空!");
                return;
            }

            if (password.Equals(""))
            {
                MessageBox.Show("密码不可为空!");
                return;
            }

            if (Form1.m_BusCtrl.connectToDB(host,port,dbName,user,password))
            {
                MessageBox.Show("连接数据库成功!");
                this.Close();
            }
        }
    }
}
