using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using Admin.Models;
using System.Configuration;

namespace Admin.Models
{
    public class BOM_Insert
    {
        public string P_Description(string cSP_Part_No)
        {
            List<BOMFields> ItemQm = new List<BOMFields>();
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1="select P_Description from Product_Master where P_Part_No = '" + cSP_Part_No+ "'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlDataReader dr = SqlCmd1.ExecuteReader();
            while (dr.Read())
            {
                ItemQm.Add(new BOMFields
                {
                    SP_Description = dr["P_Description"].ToString()
                }
                );
            }
            if (ItemQm.Count()!=0)
            {
                string item = string.Join("", ItemQm.Select(m => m.SP_Description));
                Con.Close();
                return item;
            }
            else
            {
                return null;
            }
            
        }
        public string SP_Description(string cPart_No)
        {
            List<BOMFields> ItemQm = new List<BOMFields>();
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "select P_Description from Product_Master where P_Part_No = '" + cPart_No + "'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlDataReader dr = SqlCmd1.ExecuteReader();
            while (dr.Read())
            {
                ItemQm.Add(new BOMFields
                {
                    Description = dr["P_Description"].ToString()
                }
                );
            }
            if (ItemQm.Count() != 0)
            {
                string item = string.Join("", ItemQm.Select(m => m.Description));
                Con.Close();
                return item;
            }
            else
            {
                return null;
            }

        }
        public int AddOrderDetails(List<BOM_Table> orderDetail)
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "Update Number_master Set BOM_No = BOM_No + 1 ";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlCmd1.ExecuteNonQuery();
            string cmd2 = "Select BOM_No from Number_master ";
            SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
            SqlDataReader dr = SqlCmd2.ExecuteReader();
            dr.Read();
            int BOM_No = dr.GetInt32(0);
            Con.Close();

            SqlConnection Con1 = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con1.Open();
            int i = 0;
            while (i <= orderDetail.Count())
            {
                if (i == orderDetail.Count())
                {
                    break;
                }
                SqlCommand sql_cmnd = new SqlCommand("[dbo].[BOM_Prod]", Con1);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@bom_no", SqlDbType.Int).Value = BOM_No;
                sql_cmnd.Parameters.AddWithValue("@bom_date", SqlDbType.NVarChar).Value = orderDetail[i].BOM_Date;
                sql_cmnd.Parameters.AddWithValue("@part_no", SqlDbType.NVarChar).Value = orderDetail[i].Part_No.ToUpper();
                sql_cmnd.Parameters.AddWithValue("@mp_partno", SqlDbType.NVarChar).Value = orderDetail[i].SP_Part_No.ToUpper();
                sql_cmnd.Parameters.AddWithValue("@quantity", SqlDbType.NVarChar).Value = orderDetail[i].Quantity;
                sql_cmnd.ExecuteNonQuery();
                i++;
            }
            Con1.Close();
            return BOM_No;
        }
        public List<BOMFields> Descp_Qty(string part_no)
        {
            List<BOMFields> ItemQm = new List<BOMFields>();
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "select P_Closing_Balance,P_Description from Product_Master where P_Part_No = '" + part_no + "'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlDataReader dr = SqlCmd1.ExecuteReader();
            while (dr.Read())
            {
                ItemQm.Add(new BOMFields
                {
                    Description = dr["P_Description"].ToString(),
                    Quantity = dr["P_Closing_Balance"].ToString()
                }
                );
            }
            return ItemQm;
        }
        public int EditOrder(List<BOMEdit> orderDetail, string bom_date)
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "Delete from BOM where BOM_No = "+orderDetail[0].BOM_No+"";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlCmd1.ExecuteNonQuery();
            Con.Close();

            SqlConnection Con1 = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con1.Open();
            int i = 0;
            while (i <= orderDetail.Count())
            {
                if (i == orderDetail.Count())
                {
                    break;
                }
                SqlCommand sql_cmnd = new SqlCommand("[dbo].[BOM_Prod]", Con1);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@bom_no", SqlDbType.Int).Value = orderDetail[i].BOM_No;
                sql_cmnd.Parameters.AddWithValue("@bom_date", SqlDbType.NVarChar).Value = bom_date;
                sql_cmnd.Parameters.AddWithValue("@part_no", SqlDbType.NVarChar).Value = orderDetail[i].Part_No.ToUpper();
                sql_cmnd.Parameters.AddWithValue("@mp_partno", SqlDbType.NVarChar).Value = orderDetail[i].SP_Part_No.ToUpper();
                sql_cmnd.Parameters.AddWithValue("@quantity", SqlDbType.NVarChar).Value = orderDetail[i].Quantity;
                sql_cmnd.ExecuteNonQuery();
                i++;
            }
            Con1.Close();
            return i;
        }
        public void Update_BOM_Row(string Part_No, string bomno)
        {
            SqlConnection Con1 = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con1.Open();
            List<GoodsList> ItemQm = new List<GoodsList>();
            string cmd2 = "Select P_code from Product_Master where P_Part_No = '" + Part_No + "'";
            SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con1);
            SqlDataReader dr = SqlCmd2.ExecuteReader();
            while (dr.Read())
            {
                ItemQm.Add(new GoodsList
                {
                    P_code = dr["P_code"].ToString()
                }
                );
            }
            dr.Close();
            string pcode = string.Join("", ItemQm.Select(m => m.P_code));
            int bom_no = Convert.ToInt32(bomno);
            string cmd3 = "Delete from BOM where SP_code = '" + pcode + "' and BOM_No = '" + bomno + "'";
            SqlCommand SqlCmd3 = new SqlCommand(cmd3, Con1);
            SqlCmd3.ExecuteNonQuery();
            Con1.Close();
        }

    } 
}