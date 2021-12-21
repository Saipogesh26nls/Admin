using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Admin.Models;

namespace Admin.Models
{
    public class ProductInsert
    {
        public int AddData(string cP_Name, string cP_Disp_Name, string cP_Manufacturer, string cP_Region, string cP_Part_No, string cP_Description, double cP_Cost, double cP_MRP, double cP_SP) 
        {
             DB_Con_Str OCon = new DB_Con_Str();
             string ConString = OCon.DB_Data();
             SqlConnection Con = new SqlConnection(ConString);
             Con.Open();

             SqlCommand sql_cmnd = new SqlCommand("[dbo].[addProduct]", Con);
             sql_cmnd.CommandType = CommandType.StoredProcedure;
             sql_cmnd.Parameters.AddWithValue("@Pname", SqlDbType.NVarChar).Value = cP_Disp_Name.ToUpper();
             sql_cmnd.Parameters.AddWithValue("@PDispname", SqlDbType.NVarChar).Value = cP_Disp_Name.ToUpper();
             sql_cmnd.Parameters.AddWithValue("@Pmfr", SqlDbType.Int).Value = cP_Manufacturer;
             sql_cmnd.Parameters.AddWithValue("@PRegion", SqlDbType.NVarChar).Value = cP_Region.ToUpper();
             sql_cmnd.Parameters.AddWithValue("@PPartNo", SqlDbType.NVarChar).Value = cP_Part_No.ToUpper();
             sql_cmnd.Parameters.AddWithValue("@PGroup", SqlDbType.NVarChar).Value = cP_Name;
             sql_cmnd.Parameters.AddWithValue("@PDescription", SqlDbType.NVarChar).Value = cP_Description.ToUpper();
             sql_cmnd.Parameters.AddWithValue("@Pcost", SqlDbType.Money).Value = cP_Cost;
             sql_cmnd.Parameters.AddWithValue("@pMRp", SqlDbType.Money).Value = cP_MRP;
             sql_cmnd.Parameters.AddWithValue("@psp", SqlDbType.Money).Value = cP_SP;
             int var;
             var = sql_cmnd.ExecuteNonQuery();

             return var;
        }
    }
}