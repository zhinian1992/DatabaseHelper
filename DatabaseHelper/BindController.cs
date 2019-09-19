using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DatabaseHelper
{
    public class BindController
    {
        private BusinessElement m_SelectedElement;
        private List<String> m_BusniessList;

        private DataTable m_SingleItemDt;
        private DataTable m_SingleSearchDt;
        private DataTable m_RelationDt;
        private DataTable m_MultiItemDt;
        private DataTable m_MultiSearchDt;

        public DataTable SingleItemDt { get => m_SingleItemDt; set => m_SingleItemDt = value; }
        public DataTable RelationDt { get => m_RelationDt; set => m_RelationDt = value; }
        public DataTable MultiItemDt { get => m_MultiItemDt; set => m_MultiItemDt = value; }
        public DataTable MultiSearchDt { get => m_MultiSearchDt; set => m_MultiSearchDt = value; }
        public List<string> BusniessList { get => m_BusniessList; set => m_BusniessList = value; }
        public DataTable SingleSearchDt { get => m_SingleSearchDt; set => m_SingleSearchDt = value; }

        public BindController()
        {
            this.getBusinessTypes();
        }

        /// <summary>
        /// 从配置文件中读取全部绑定业务类型
        /// </summary>
        public void getBusinessTypes()
        {
            BusniessList = new List<string>();
            BusniessList.Add("");
            BinderConfig binder = (BinderConfig)ConfigurationManager.GetSection("BinderSettings");
            for(int i=0;i<binder.BinderSettings.Count;i++)
            {
                BusniessList.Add(binder.BinderSettings[i].type);
            }
        }

        /// <summary>
        /// 获取当前选择的业务设置
        /// </summary>
        /// <param name="index"></param>
        public void setBusinessSelect(int index)
        {
            BinderConfig binder = (BinderConfig)ConfigurationManager.GetSection("BinderSettings");
            m_SelectedElement = binder.BinderSettings[index-1];
        }

        /// <summary>
        /// 执行配置文件SSFG1第一个查询SQL;获取单选项列表
        /// </summary>
        public void getSingleItemDt()
        {
            String connStr = m_SelectedElement.conn;
            String sql = m_SelectedElement.SSFG1;
            SingleItemDt = Form1.m_BusCtrl.getDataTable(connStr, sql);
        }

        /// <summary>
        /// 执行配置文件SSFG2第二个查询SQL;获取单选项已绑定多选项列表
        /// </summary>
        /// <param name="drugIndex"></param>
        public void getRelationDt(int drugIndex)
        {
            String id = this.SingleItemDt.Rows[drugIndex].ItemArray[0].ToString();
            String connStr = m_SelectedElement.conn;
            String sql = string.Format(m_SelectedElement.SSFG2, id);
            this.m_RelationDt = Form1.m_BusCtrl.getDataTable(connStr, sql);
        }

        /// <summary>
        /// 执行配置文件SSFC第三个查询SQL;获取多选项列表
        /// </summary>
        public void getMultiItemDt()
        {
            String connStr = m_SelectedElement.conn;
            String sql = m_SelectedElement.SSFC;
            m_MultiItemDt = Form1.m_BusCtrl.getDataTable(connStr, sql);
        }

        /// <summary>
        /// 更新单选项列表指定行数据至数据库
        /// </summary>
        /// <param name="singleIndex"></param>
        public void updateRowInSingleTable(int singleIndex)
        {
            try
            {
                if(m_SelectedElement.SUS == "")
                {
                    return;
                }
                String connStr = m_SelectedElement.conn;
                String sql = sqlStrParser(this.SingleItemDt.Rows[singleIndex], singleIndex, m_SelectedElement.SUS);

                if (!Form1.m_BusCtrl.writeRecordToDB(connStr, sql))
                    this.SingleItemDt.RejectChanges();
            }
            catch (Exception e)
            {
                MessageBox.Show("更新数据失败!失败原因是:" + e.ToString());
            }
        }

        /// <summary>
        /// 删除选定一对一关系记录
        /// </summary>
        /// <param name="relationIndex"></param>
        /// <param name="singleIndex"></param>
        public void deleteRowFromRelation(int relationIndex, int singleIndex)
        {
            try
            {
                String connStr = m_SelectedElement.conn;
                String sql = sqlStrParser(this.RelationDt.Rows[relationIndex], singleIndex,m_SelectedElement.SDR);
                if (Form1.m_BusCtrl.writeRecordToDB(connStr, sql))
                    this.RelationDt.Rows.Remove(this.RelationDt.Rows[relationIndex]);

            }
            catch (Exception e)
            {
                MessageBox.Show("删除数据失败!失败原因是:" + e.ToString());
            }
        }

        /// <summary>
        /// 插入新一对一数据到关系表
        /// </summary>
        /// <param name="multiIndex"></param>
        /// <param name="singleIndex"></param>
        public void addRowToRelation(int multiIndex, int singleIndex)
        {
            try
            {
                //过滤已存在数据
                foreach (DataRow d in this.RelationDt.Rows)
                {
                    if (d[0].ToString() == this.MultiSearchDt.Rows[multiIndex][0].ToString())
                    {
                        return;
                    }
                }

                String connStr = m_SelectedElement.conn;
                String sql = sqlStrParser(this.MultiSearchDt.Rows[multiIndex], singleIndex, m_SelectedElement.SIR);

                if (Form1.m_BusCtrl.writeRecordToDB(connStr, sql))
                {
                    this.getRelationDt(singleIndex);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("插入数据失败!失败原因是:" + e.ToString());
            }
        }

        /// <summary>
        /// 解析配置文件里的sql字符串 按格式添加数据项
        /// 配置文件字符串动态配置字段需要用{}包括;
        /// {}内容格式为{DataTableType@columnName} 如:{single@id} 为 SingleItemDt id字段
        /// DataTableType 格式共有两种:single,multi,relation ,time
        /// {}内容格式为{time@yyyy-MM-dd HH:mm:ss.fff}格式时按分隔符后格式获取系统时间
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="singleIndex"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        private String sqlStrParser(DataRow dr, int singleIndex, String sql)
        {
            try
            {
                List<int> split_left_list = new List<int>();
                List<int> split_right_list = new List<int>();
                List<String> typeList = new List<string>();
                List<String> valueList = new List<string>();

                char[] list = sql.ToArray();
                for(int i=0;i<sql.Length;i++)
                {
                    if(sql[i] == '{')
                    {
                        split_left_list.Add(i);
                        List<String> tpye_list = new List<String>();
                        List<String> value_list = new List<String>();
                        List<String> temp_list = tpye_list;
                        for (int j=i+1;j<sql.Length;j++)
                        {
                            if(sql[j] == '}')
                            {
                                split_right_list.Add(j);
                                i = j + 1;
                                break;
                            }
                            else
                            {
                                if(sql[j] == '@')
                                {
                                    temp_list = value_list;
                                    continue;
                                }
                                temp_list.Add(sql[j].ToString());
                            }
                        }
                        typeList.Add(string.Join("",tpye_list.ToArray()));
                        valueList.Add(string.Join("", value_list.ToArray()));
                    }
                }

                if(typeList.Count != valueList.Count || typeList.Count == 0)
                {
                    MessageBox.Show("配置文件插入SQL动态解析失败!失败原因是:{}标识符数量不匹配!");
                    return "";
                }

                String new_sql = "";
                int start = 0;
                for (int i = 0;i< split_left_list.Count;i++)
                {
                    new_sql += sql.Substring(start, split_left_list[i] - start);
                    start = split_right_list[i] + 1;
                    if (typeList[i] == "relation")
                    {
                        new_sql += dr[valueList[i]];
                    }
                    else if(typeList[i] == "single")
                    {
                        new_sql += SingleItemDt.Rows[singleIndex][valueList[i]];
                    }
                    else if (typeList[i] == "multi")
                    {
                        new_sql += MultiItemDt.Rows[singleIndex][valueList[i]];
                    }
                    else if (typeList[i] == "time")
                    {
                        new_sql += DateTime.Now.ToString(valueList[i]);
                    }
                }
                new_sql += sql.Substring(split_right_list.Last() + 1);

                return new_sql;
            }
            catch(Exception e)
            {
                MessageBox.Show("配置文件插入SQL动态解析失败!失败原因是:" + e.ToString());
                return "";
            }
        }
    }
}
