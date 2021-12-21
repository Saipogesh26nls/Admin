using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using Admin.Models;

namespace Admin.Models
{
    public class BOM_Insert
    {
        public string P_Description(string cSP_Part_No)
        {
            List<BOMFields> ItemQm = new List<BOMFields>();
            DB_Con_Str OCon = new DB_Con_Str();
            string ConString = OCon.DB_Data();
            SqlConnection Con = new SqlConnection(ConString);
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
        public void AddOrderDetails(List<BOMFields> orderDetail)
        {
            DB_Con_Str OCon = new DB_Con_Str();
            string ConString = OCon.DB_Data();
            SqlConnection Con = new SqlConnection(ConString);
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

            DB_Con_Str OCon1 = new DB_Con_Str();
            string ConString1 = OCon1.DB_Data();
            SqlConnection Con1 = new SqlConnection(ConString1);
            Con1.Open();
            int i = 1;
            while (i <= orderDetail.Count())
            {
                if (i == orderDetail.Count())
                {
                    break;
                }
                /*DB_Con_Str OCon = new DB_Con_Str();
                string ConString = OCon.DB_Data();
                SqlConnection Con = new SqlConnection(ConString);*/
                SqlCommand sql_cmnd = new SqlCommand("[dbo].[BOM_Prod]", Con1);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@bom_no", SqlDbType.Int).Value = BOM_No;
                sql_cmnd.Parameters.AddWithValue("@part_no", SqlDbType.NVarChar).Value = orderDetail[i].Part_No1.ToUpper();
                sql_cmnd.Parameters.AddWithValue("@descp", SqlDbType.NVarChar).Value = orderDetail[i].Description1.ToUpper();
                sql_cmnd.Parameters.AddWithValue("@mp_descp", SqlDbType.NVarChar).Value = orderDetail[i].SP_Description.ToUpper();
                sql_cmnd.Parameters.AddWithValue("@quantity", SqlDbType.NVarChar).Value = orderDetail[i].Quantity1.ToUpper();
                sql_cmnd.ExecuteNonQuery();
                i++;
            }
            Con1.Close();

        }
    } 
}