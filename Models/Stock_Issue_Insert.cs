using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Admin.Models
{
    public class Stock_Issue_Insert
    {
        public void Temp_StockIssue_add(GoodsRI data)
        {
            SqlConnection Con1 = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con1.Open();
                SqlCommand sql_cmnd = new SqlCommand("[dbo].[Temp_StockIssue]", Con1);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@ref_no", SqlDbType.Int).Value = data.Ref_No;
                sql_cmnd.Parameters.AddWithValue("@ref_date", data.Ref_Date);
                sql_cmnd.Parameters.AddWithValue("@GI", SqlDbType.Int).Value = data.GI_Tag;
                sql_cmnd.Parameters.AddWithValue("@Process", SqlDbType.Int).Value = data.Process_Tag;
                sql_cmnd.Parameters.AddWithValue("@Project", SqlDbType.Int).Value = data.Project;
                sql_cmnd.Parameters.AddWithValue("@Employee", SqlDbType.Int).Value = data.Employee;
                sql_cmnd.Parameters.AddWithValue("@note", SqlDbType.NVarChar).Value = data.Note;
                sql_cmnd.Parameters.AddWithValue("@part_no", SqlDbType.NVarChar).Value = data.Part_No;
                sql_cmnd.Parameters.AddWithValue("@descp", SqlDbType.NChar).Value = data.Description;
                sql_cmnd.Parameters.AddWithValue("@qty", SqlDbType.Int).Value = data.Quantity;
                sql_cmnd.ExecuteNonQuery();
            Con1.Close();
        }
        public void StockIssue_add(GoodsRI data)
        {
            SqlConnection Con1 = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con1.Open();
            SqlCommand sql_cmnd = new SqlCommand("[dbo].[StockIssue]", Con1);
            sql_cmnd.CommandType = CommandType.StoredProcedure;
            sql_cmnd.Parameters.AddWithValue("@ref_no", SqlDbType.Int).Value = data.Ref_No;
            sql_cmnd.Parameters.AddWithValue("@ref_date", data.Ref_Date);
            sql_cmnd.Parameters.AddWithValue("@GI", SqlDbType.Int).Value = data.GI_Tag;
            sql_cmnd.Parameters.AddWithValue("@Process", SqlDbType.Int).Value = data.Process_Tag;
            sql_cmnd.Parameters.AddWithValue("@Project", SqlDbType.Int).Value = data.Project;
            sql_cmnd.Parameters.AddWithValue("@Employee", SqlDbType.Int).Value = data.Employee;
            sql_cmnd.Parameters.AddWithValue("@note", SqlDbType.NVarChar).Value = data.Note;
            sql_cmnd.Parameters.AddWithValue("@part_no", SqlDbType.NVarChar).Value = data.Part_No;
            sql_cmnd.Parameters.AddWithValue("@descp", SqlDbType.NChar).Value = data.Description;
            sql_cmnd.Parameters.AddWithValue("@qty", SqlDbType.Int).Value = data.Quantity;
            sql_cmnd.ExecuteNonQuery();
            Con1.Close();
        }
        public GoodsRI Issue_Descp_Qty(string part_no)
        {
            GoodsRI ItemQm = new GoodsRI();
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "select P_Closing_Balance,P_Description,P_code from Product_Master where P_Part_No = '" + part_no + "'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlDataReader dr = SqlCmd1.ExecuteReader();
            while (dr.Read())
            {
                ItemQm.Description = dr["P_Description"].ToString();
                ItemQm.Quantity = (int)dr["P_Closing_Balance"];
                ItemQm.P_code = dr["P_code"].ToString();
            }
            Con.Close();
            return ItemQm;
        }
        public GoodsRI Issue_Descp(string part_no)
        {
            GoodsRI ItemQm = new GoodsRI();
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "select P_Closing_Balance,P_Description,P_code from Product_Master where P_Part_No = '" + part_no + "'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlDataReader dr = SqlCmd1.ExecuteReader();
            while (dr.Read())
            {
                ItemQm.Description = dr["P_Description"].ToString();
                ItemQm.db_qty = (int)dr["P_Closing_Balance"];
            }
            Con.Close();
            return ItemQm;
        }
    }
}