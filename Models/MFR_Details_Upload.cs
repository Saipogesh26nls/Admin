using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;

namespace Admin.Models
{
    public class MFR_Details_Upload
    {
        public int MFRUpload(string cM_Name, string cM_Country, string cM_Region, string cM_Address, string cM_Support_No, string cM_Contact_No, string cM_Sales_No, string cM_Website, string cM_Support_Email, string cM_Contact_Email, string cM_Sales_Email, string cM_Payment)
        {
            DB_Con_Str OCon = new DB_Con_Str();
            string ConString = OCon.DB_Data();
            SqlConnection Con = new SqlConnection(ConString);
            Con.Open();
            string cmd2 = "insert into Manufacturer_Details (M_Name, M_Country, M_Region, M_Address, M_Support_No, M_Contact_No, M_Sales_No, M_Website, M_Support_Email, M_Contact_Email, M_Sales_Email, M_Payment) VALUES('" + cM_Name.ToUpper() + "','" + cM_Country.ToUpper() + "','" + cM_Region.ToUpper() + "','" + cM_Address.ToUpper() + "','" + cM_Support_No + "','" + cM_Contact_No + "','" + cM_Sales_No + "','" + cM_Website + "','" + cM_Support_Email + "','" + cM_Contact_Email + "','" + cM_Sales_Email + "','" + cM_Payment.ToUpper() + "')";
            SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);

            int var;
            var = SqlCmd2.ExecuteNonQuery();

            return var;
        }
    }
}