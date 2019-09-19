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
    public partial class BindingForm : Form
    {
        private BindController m_DBFCtrl;
        private int m_RelationSelectRow;
        private int m_SingleItemSelectRow;
        private bool m_SingleDGVCellValueChanged;

        public BindingForm()
        {
            InitializeComponent();

            m_DBFCtrl = new BindController();
            m_RelationSelectRow = -1;
            m_SingleItemSelectRow = -1;
            m_SingleDGVCellValueChanged = false;
        }

        private void BindingForm_Load(object sender, EventArgs e)
        {
            this.comboBox2.DataSource = m_DBFCtrl.BusniessList;
        }

        /// <summary>
        /// 行选择事件: 更新行对应一对多关系表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            m_SingleItemSelectRow = e.RowIndex;
            this.textBox_effect.Text = this.dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
            m_DBFCtrl.getRelationDt(e.RowIndex);
            this.dataGridView2.DataSource = m_DBFCtrl.RelationDt;
        }

        /// <summary>
        /// 删除一对多关系中单条数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //选择行非标题行 删除该行
            if(m_RelationSelectRow != -1)
            {
                m_DBFCtrl.deleteRowFromRelation(m_RelationSelectRow, m_SingleItemSelectRow);
            }
        }

        /// <summary>
        /// 一对多关系列表行选择事件:右键点击弹出删除菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView2_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                if (e.Button == MouseButtons.Right)
                {
                    this.m_RelationSelectRow = e.RowIndex;
                    this.contextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
                }
            }
        }

        /// <summary>
        /// 插入按钮响应事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                int index = this.comboBox1.SelectedIndex;
                m_DBFCtrl.addRowToRelation(index, this.m_SingleItemSelectRow);
                this.dataGridView2.DataSource = m_DBFCtrl.RelationDt;
            }
            catch(Exception ex)
            {
                MessageBox.Show("插入数据失败!失败原因是:"+ex.ToString());
            }
        }

        /// <summary>
        /// 更新疾病证候对应列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            m_DBFCtrl.getMultiItemDt();
        }

        /// <summary>
        /// ComboBox1文本更新事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_TextUpdate(object sender, EventArgs e)
        {
            String str = this.comboBox1.Text;

            m_DBFCtrl.MultiSearchDt = m_DBFCtrl.MultiItemDt.Clone();
            this.comboBox1.Items.Clear();

            foreach (DataRow dr in m_DBFCtrl.MultiItemDt.Rows)
            {
                if (dr["search"].ToString().Contains(str))
                {
                    m_DBFCtrl.MultiSearchDt.Rows.Add(dr.ItemArray);
                    this.comboBox1.Items.Add(dr["search"].ToString());
                }
            }
            this.comboBox1.SelectionStart = this.comboBox1.Text.Length;
            Cursor = Cursors.Default;
        }

        /// <summary>
        /// 业务类型选择修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = this.comboBox2.SelectedIndex;
            if(index != 0)
            {
                this.comboBox1.Text = "";
                this.comboBox1.Items.Clear();
                m_DBFCtrl.setBusinessSelect(index);
                m_DBFCtrl.getSingleItemDt();
                m_DBFCtrl.getMultiItemDt();
                this.dataGridView1.DataSource = m_DBFCtrl.SingleItemDt;
            }
        }

        /// <summary>
        /// 下拉框点击回车弹出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                this.comboBox1.DroppedDown = true;
            }
        }

        /// <summary>
        /// 列表单元格字段修改完成事件，修改列表1当前行修改状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            m_SingleDGVCellValueChanged = true;
        }

        /// <summary>
        /// 接收回车弹出下拉框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox3_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.comboBox3.DroppedDown = true;
            }
        }

        /// <summary>
        /// ComboBox3文本更新事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox3_TextUpdate(object sender, EventArgs e)
        {
            String str = this.comboBox3.Text;

            m_DBFCtrl.SingleSearchDt = m_DBFCtrl.SingleItemDt.Clone();
            this.comboBox3.Items.Clear();

            foreach (DataRow dr in m_DBFCtrl.SingleItemDt.Rows)
            {
                if (dr["search"].ToString().Contains(str))
                {
                    m_DBFCtrl.SingleSearchDt.Rows.Add(dr.ItemArray);
                    this.comboBox3.Items.Add(dr["search"].ToString());
                }
            }
            this.comboBox3.SelectionStart = this.comboBox3.Text.Length;
            Cursor = Cursors.Default;
        }

        /// <summary>
        /// ComboBox3选择变更事件 定位所选datagridwiew行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            //TODO: 跳转指定行
            foreach(DataRow dr in this.m_DBFCtrl.SingleItemDt.Rows)
            {
                if(dr[0].ToString() == this.m_DBFCtrl.SingleSearchDt.Rows[this.comboBox3.SelectedIndex][0].ToString())
                {
                    int index = this.m_DBFCtrl.SingleItemDt.Rows.IndexOf(dr);
                    this.dataGridView1.CurrentCell = this.dataGridView1.Rows[index].Cells[0];
                    return;
                }
            }
            
            
        }

        /// <summary>
        /// 列表1 行失去焦点时判断是否做出过修改 如果修改过则更新数据库记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_RowLeave(object sender, DataGridViewCellEventArgs e)
        {
            if(this.m_SingleDGVCellValueChanged)
            {
                m_DBFCtrl.updateRowInSingleTable(this.m_SingleItemSelectRow);
                this.m_SingleDGVCellValueChanged = false;
            }
        }
    }
}
