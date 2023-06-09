﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Admin.Models
{
    public class GroupInsert
    {
        public int AddData(string cP_Name, string cP_Disp_Name, string cP_Manufacturer, string cP_Region, string cP_Part_No, string cP_Description, double cP_Cost, double cP_MRP, double cP_SP)
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();

            SqlCommand sql_cmnd = new SqlCommand("[dbo].[GroupBy]", Con);
            sql_cmnd.CommandType = CommandType.StoredProcedure;
            sql_cmnd.Parameters.AddWithValue("@Pname", SqlDbType.NVarChar).Value = cP_Name.ToUpper();
            sql_cmnd.Parameters.AddWithValue("@PDispname", SqlDbType.NVarChar).Value = cP_Disp_Name.ToUpper();
            int var;
            var = sql_cmnd.ExecuteNonQuery();

            return var;
        }
    }
}