using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Admin.Models;
using System.Configuration;

namespace Admin.Models
{
    public class ProductInsert
    {
        public int AddData(string cP_Name, string cP_Disp_Name, string cP_Manufacturer, string cP_Region, string cP_Part_No, string cP_Description, double cP_Cost, double cP_MRP, double cP_SP) 
        {
             SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
             Con.Open();

             SqlCommand sql_cmnd = new SqlCommand("[dbo].[addProduct]", Con);
             sql_cmnd.CommandType = CommandType.StoredProcedure;
             sql_cmnd.Parameters.AddWithValue("@Pname", SqlDbType.NVarChar).Value = cP_Disp_Name;
             sql_cmnd.Parameters.AddWithValue("@PDispname", SqlDbType.NVarChar).Value = cP_Disp_Name;
             sql_cmnd.Parameters.AddWithValue("@Pmfr", SqlDbType.Int).Value = cP_Manufacturer;
             sql_cmnd.Parameters.AddWithValue("@PRegion", SqlDbType.NVarChar).Value = cP_Region;
             sql_cmnd.Parameters.AddWithValue("@PPartNo", SqlDbType.NVarChar).Value = cP_Part_No;
             sql_cmnd.Parameters.AddWithValue("@PGroup", SqlDbType.NVarChar).Value = cP_Name;
             sql_cmnd.Parameters.AddWithValue("@PDescription", SqlDbType.NVarChar).Value = cP_Description;
             sql_cmnd.Parameters.AddWithValue("@Pcost", SqlDbType.Money).Value = cP_Cost;
             sql_cmnd.Parameters.AddWithValue("@pMRp", SqlDbType.Money).Value = cP_MRP;
             sql_cmnd.Parameters.AddWithValue("@psp", SqlDbType.Money).Value = cP_SP;
             string cmd1 = "update Product_Master set P_Name = UPPER(P_Name), P_Disp_Name = UPPER(P_Disp_Name), P_Region = UPPER(P_Region), P_Part_No = UPPER(P_Part_No), P_Description = UPPER(P_Description)";
             SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);

             int var1, var2;
             var1= sql_cmnd.ExecuteNonQuery();
             var2 = SqlCmd1.ExecuteNonQuery();
            return var1;
        }
    }
}