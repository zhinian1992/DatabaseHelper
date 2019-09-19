using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace DatabaseHelper
{
    public class BusinessController
    {
        public BusinessController()
        {
            
        }

        /// <summary>
        /// 连接数据库测试
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="dbName"></param>
        /// <param name="user"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public bool connectToDB(String host, String port, String dbName, String user, String pwd)
        {
            String connStr = string.Format("server={0};port={1};database={2};user={3};password={4};", host, port, dbName, user, pwd);
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                conn.Close();
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("连接数据库失败!错误原因:"+e.ToString());
                return false;
            }
        }

        /// <summary>
        /// 查询数据库获取数据
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable getDataTable(String connStr,String sql)
        {
            DataTable dt = new DataTable();
            try
            {
                MySqlConnection conn = new MySqlConnection(connStr);
                conn.Open();
                MySqlCommand mysqlCmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = mysqlCmd.ExecuteReader();
                dt.Load(reader, LoadOption.Upsert, null);
                conn.Close(); 
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("连接数据库失败!\r\n查询连接:{0}\r\n查询SQL:{1}\r\n错误原因:{2}", 
                    connStr, sql,e.ToString()));
            }
            return dt;
        }

        /// <summary>
        /// 执行非select sql语句
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public bool writeRecordToDB(String connStr, String sql)
        {
            int iRet = 0;
            DataTable dt = new DataTable();
            try
            {
                connStr += "Charset=utf8;";
                MySqlConnection conn = new MySqlConnection(connStr);
                conn.Open();
                MySqlCommand mysqlCmd = new MySqlCommand(sql, conn);
                iRet = mysqlCmd.ExecuteNonQuery();
                conn.Close();
                if (iRet == 1)
                    return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("连接数据库失败!\r\n更新连接:{0}\r\n更新SQL:{1}\r\n错误原因:{2}",
                    connStr, sql, e.ToString()));
            }
            return false;
        }
    }
}
