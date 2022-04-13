using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Admin.Models
{
    public class Price_Updation
    {
        public int AddPrice(string Part_No, double P_Cost, double P_Price_USD, double P_MRP, double P_SP, int stock)
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();

            SqlCommand sql_cmnd = new SqlCommand("[dbo].[addPrice]", Con);
            sql_cmnd.CommandType = CommandType.StoredProcedure;
            sql_cmnd.Parameters.AddWithValue("@PartNo", SqlDbType.NVarChar).Value = Part_No.ToUpper();
            sql_cmnd.Parameters.AddWithValue("@p_cost", SqlDbType.Money).Value = P_Cost;
            sql_cmnd.Parameters.AddWithValue("@p_price_USD", SqlDbType.Money).Value = P_Price_USD;
            sql_cmnd.Parameters.AddWithValue("@p_mrp", SqlDbType.Money).Value = P_MRP;
            sql_cmnd.Parameters.AddWithValue("@p_sp", SqlDbType.Money).Value = P_SP;
            sql_cmnd.Parameters.AddWithValue("@stock", SqlDbType.Money).Value = stock;
            int var1;
            var1 = sql_cmnd.ExecuteNonQuery();
            return var1;
        }
    }
}