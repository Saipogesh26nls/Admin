using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;

namespace Admin.Models
{
    public class MFR_Details_Upload
    {
        public int MFRUpload(string cM_Name, string cM_Country, string cM_Region, string cM_Address, string cM_Support_No, string cM_Contact_No, string cM_Sales_No, string cM_Website, string cM_Support_Email, string cM_Contact_Email, string cM_Sales_Email, string cM_Payment)
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "Update Number_master Set M_Id = M_Id + 1 ";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlCmd1.ExecuteNonQuery();
            string cmd2 = "Select M_Id from Number_master ";
            SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
            SqlDataReader dr = SqlCmd2.ExecuteReader();
            dr.Read();
            int M_Id = dr.GetInt32(0);
            Con.Close();
            Con.Open();
            string cmd3 = "insert into Manufacturer_Details (M_Id,M_Name, M_Country, M_Region, M_Address, M_Support_No, M_Contact_No, M_Sales_No, M_Website, M_Support_Email, M_Contact_Email, M_Sales_Email, M_Payment) VALUES('" + M_Id + "','" + cM_Name + "','" + cM_Country + "','" + cM_Region + "','" + cM_Address + "','" + cM_Support_No + "','" + cM_Contact_No + "','" + cM_Sales_No + "','" + cM_Website + "','" + cM_Support_Email + "','" + cM_Contact_Email + "','" + cM_Sales_Email + "','" + cM_Payment + "')";
            string cmd4 = "update Manufacturer_Details set M_Name = UPPER(M_Name), M_Country = UPPER(M_Country), M_Region = UPPER(M_Region), M_Address = UPPER(M_Address), M_Payment = UPPER(M_Payment)";
            SqlCommand SqlCmd3 = new SqlCommand(cmd3, Con);
            SqlCommand SqlCmd4 = new SqlCommand(cmd4, Con);
            int var1, var2;
            var1 = SqlCmd3.ExecuteNonQuery();
            var2 = SqlCmd4.ExecuteNonQuery();
            Con.Close();
            return var1;
        }
    }
}