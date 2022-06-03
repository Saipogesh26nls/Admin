using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Admin.Models
{
    public class WO_Insert
    {
        public int Add_WO(List<Workorder> data, int id)
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "Update Number_master Set WO_No = WO_No + 1 ";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlCmd1.ExecuteNonQuery();
            string cmd2 = "Select WO_No from Number_master ";
            SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
            SqlDataReader dr = SqlCmd2.ExecuteReader();
            dr.Read();
            int WO_No = dr.GetInt32(0);
            dr.Close();
            Con.Close();

            SqlConnection Con1 = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con1.Open();
            int i = 0;
            DateTime now = DateTime.Now;
            while (i < data.Count())
            {
                SqlCommand sql_cmnd = new SqlCommand("[dbo].[add_WO]", Con1);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@wo_no", SqlDbType.NVarChar).Value = WO_No;
                sql_cmnd.Parameters.AddWithValue("@wo_date", SqlDbType.Date).Value = data[i].WO_Date;
                sql_cmnd.Parameters.AddWithValue("@partno", SqlDbType.NVarChar).Value = data[i].PartNo;
                sql_cmnd.Parameters.AddWithValue("@process", SqlDbType.Int).Value = data[i].Process;
                sql_cmnd.Parameters.AddWithValue("@qty", SqlDbType.Int).Value = data[i].Quantity;
                sql_cmnd.Parameters.AddWithValue("@mfropt", SqlDbType.NVarChar).Value = data[i].Mfr_option;
                sql_cmnd.Parameters.AddWithValue("@mfr",SqlDbType.NVarChar).Value=data[i].Mfr;
                sql_cmnd.Parameters.AddWithValue("@note", SqlDbType.NVarChar).Value = data[i].Note;
                sql_cmnd.Parameters.AddWithValue("@time", SqlDbType.Time).Value = now.ToLongTimeString();
                sql_cmnd.Parameters.AddWithValue("@employee", SqlDbType.Int).Value = id;
                sql_cmnd.ExecuteNonQuery();
                if (i == data.Count() - 1)
                {
                    break;
                }
                i++;
            }
            Con1.Close();
            return WO_No;
        }
        public List<GoodsRI> Descp_Qty(string part_no)
        {
            List<GoodsRI> ItemQm = new List<GoodsRI>();
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "select P_Closing_Balance,P_Description,P_code from Product_Master where P_Part_No = '" + part_no + "'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlDataReader dr = SqlCmd1.ExecuteReader();
            while (dr.Read())
            {
                ItemQm.Add(new GoodsRI
                {
                    Description = dr["P_Description"].ToString(),
                    P_code = dr["P_code"].ToString()
                }
                );
            }
            Con.Close();
            return ItemQm;
        }
    }
}