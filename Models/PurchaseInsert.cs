using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Data;

namespace Admin.Models
{
    public class PurchaseInsert
    {
        public int Add_Data(string Voucher_No, string Voucher_Date, string Invoice_No, string Invoice_Date, string A_Name, string Part_No, string I_Ledger, string A_Ledger, int P_Qty, double P_Rate, double P_Discount, double P_Tax1, double P_Tax2, double P_Sub_Total, double P_Total)
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();

            SqlCommand sql_cmnd = new SqlCommand("[dbo].[addPurchase]", Con);
            sql_cmnd.CommandType = CommandType.StoredProcedure;
            sql_cmnd.Parameters.AddWithValue("@voucher_no", SqlDbType.NVarChar).Value = Voucher_No;
            sql_cmnd.Parameters.AddWithValue("@voucher_date", Voucher_Date);
            sql_cmnd.Parameters.AddWithValue("@invoice_no", SqlDbType.NVarChar).Value = Invoice_No;
            sql_cmnd.Parameters.AddWithValue("@invoice_date", Invoice_Date);
            sql_cmnd.Parameters.AddWithValue("@a_code", SqlDbType.NVarChar).Value = A_Name;
            sql_cmnd.Parameters.AddWithValue("@p_code", SqlDbType.NVarChar).Value = Part_No;
            sql_cmnd.Parameters.AddWithValue("@p_qty", SqlDbType.Int).Value = P_Qty;
            sql_cmnd.Parameters.AddWithValue("@iledger", SqlDbType.NVarChar).Value = I_Ledger;
            sql_cmnd.Parameters.AddWithValue("@aledger", SqlDbType.NVarChar).Value = A_Ledger;
            sql_cmnd.Parameters.AddWithValue("@p_rate", SqlDbType.Money).Value = P_Rate;
            sql_cmnd.Parameters.AddWithValue("@p_discount", SqlDbType.Decimal).Value = P_Discount;
            sql_cmnd.Parameters.AddWithValue("@purchase_tax1", SqlDbType.Money).Value = P_Tax1;
            sql_cmnd.Parameters.AddWithValue("@purchase_tax2", SqlDbType.Money).Value = P_Tax2;
            sql_cmnd.Parameters.AddWithValue("@purchase_subtotal", SqlDbType.Money).Value = P_Sub_Total;
            sql_cmnd.Parameters.AddWithValue("@purchase_total", SqlDbType.Money).Value = P_Total;

            int var1;
            var1 = sql_cmnd.ExecuteNonQuery();
            return var1;
        }
    }
}