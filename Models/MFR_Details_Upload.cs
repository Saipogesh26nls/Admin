using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace Admin.Models
{
    public class MFR_Details_Upload
    {
        public int MFRUpload(string cM_Name, string cM_Country, string cM_Region, string cM_Address, string cM_Support_No, string cM_Contact_No, string cM_Sales_No, string cM_Website, string cM_Support_Email, string cM_Contact_Email, string cM_Sales_Email, string cM_Payment)
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            SqlCommand sql_cmnd = new SqlCommand("[dbo].[addMfr]", Con);
            sql_cmnd.CommandType = CommandType.StoredProcedure;
            sql_cmnd.Parameters.AddWithValue("@m_name", SqlDbType.NVarChar).Value = cM_Name;
            sql_cmnd.Parameters.AddWithValue("@m_country", SqlDbType.NVarChar).Value = cM_Country;
            sql_cmnd.Parameters.AddWithValue("@m_region", SqlDbType.NVarChar).Value = cM_Region;
            sql_cmnd.Parameters.AddWithValue("@m_address", SqlDbType.NVarChar).Value = cM_Address;
            sql_cmnd.Parameters.AddWithValue("@m_support_no", SqlDbType.NVarChar).Value = cM_Support_No;
            sql_cmnd.Parameters.AddWithValue("@m_contact_no", SqlDbType.NVarChar).Value = cM_Contact_No;
            sql_cmnd.Parameters.AddWithValue("@m_sales_no", SqlDbType.NVarChar).Value = cM_Sales_No;
            sql_cmnd.Parameters.AddWithValue("@m_website", SqlDbType.NVarChar).Value = cM_Website;
            sql_cmnd.Parameters.AddWithValue("@m_support_email", SqlDbType.NVarChar).Value = cM_Support_Email;
            sql_cmnd.Parameters.AddWithValue("@m_contact_email", SqlDbType.NVarChar).Value = cM_Contact_Email;
            sql_cmnd.Parameters.AddWithValue("@m_sales_email", SqlDbType.NVarChar).Value = cM_Sales_Email;
            sql_cmnd.Parameters.AddWithValue("@m_payment", SqlDbType.NVarChar).Value = cM_Payment;

            int var1 = sql_cmnd.ExecuteNonQuery();
            Con.Close();
            return var1;
        }
    }
}