using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace Admin.Models
{
    public class PurchaseInsert
    {
        public int Add_Data(string Voucher_No, string Voucher_Date, string Invoice_No, string Invoice_Date, string A_code, string P_code, string I_Ledger, string A_Ledger, int P_Qty, double P_Rate, double P_Discount, double P_Tax1, double P_Tax2, double P_Sub_Total, double P_Total)
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            return null;
        }
    }
}