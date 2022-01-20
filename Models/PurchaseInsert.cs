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
        public void Add_Data (List<PurchaseField> data)
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "Update Number_master Set Voucher_No = Voucher_No + 1 ";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlCmd1.ExecuteNonQuery();
            string cmd2 = "Select Voucher_No from Number_master ";
            SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
            SqlDataReader dr = SqlCmd2.ExecuteReader();
            dr.Read();
            int Voucher_No = dr.GetInt32(0);
            Con.Close();

            SqlConnection Con1 = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con1.Open();
            //Condition value to execute A_Ledger
            int key = 1;

            int j = 0;
            int i = 0;
            while (i < data.Count())
            {
                SqlCommand sql_cmnd = new SqlCommand("[dbo].[addPurchase]", Con1);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@voucher_no", SqlDbType.NVarChar).Value = Voucher_No;
                sql_cmnd.Parameters.AddWithValue("@invoice_no", SqlDbType.NVarChar).Value = data[i].Invoice_No;
                sql_cmnd.Parameters.AddWithValue("@invoice_date", data[i].Invoice_Date);
                sql_cmnd.Parameters.AddWithValue("@part_no", SqlDbType.NVarChar).Value = data[i].Part_No;
                sql_cmnd.Parameters.AddWithValue("@acc_name", SqlDbType.NVarChar).Value = data[i].A_Name;
                sql_cmnd.Parameters.AddWithValue("@p_qty", SqlDbType.Int).Value = data[i].P_Qty;
                sql_cmnd.Parameters.AddWithValue("@iledger", SqlDbType.NVarChar).Value = data[i].I_Ledger;
                sql_cmnd.Parameters.AddWithValue("@aledger", SqlDbType.NVarChar).Value = data[i].A_Ledger;
                sql_cmnd.Parameters.AddWithValue("@i_rate", SqlDbType.Money).Value = data[i].P_Rate;
                sql_cmnd.Parameters.AddWithValue("@i_discount", SqlDbType.Decimal).Value = data[i].I_Discount;
                sql_cmnd.Parameters.AddWithValue("@i_tax1", SqlDbType.Money).Value = data[i].I_Tax1;
                sql_cmnd.Parameters.AddWithValue("@i_tax2", SqlDbType.Money).Value = data[i].I_Tax2;
                sql_cmnd.Parameters.AddWithValue("@i_subtotal", SqlDbType.Money).Value = data[i].I_Sub_Total;
                sql_cmnd.Parameters.AddWithValue("@i_total", SqlDbType.Money).Value = data[i].I_Total;
                sql_cmnd.Parameters.AddWithValue("@final_qty", SqlDbType.Int).Value = data[i].Total_Qty;
                sql_cmnd.Parameters.AddWithValue("@final_discount", SqlDbType.Decimal).Value = data[i].P_Discount;
                sql_cmnd.Parameters.AddWithValue("@final_tax1", SqlDbType.Money).Value = data[i].P_Tax1;
                sql_cmnd.Parameters.AddWithValue("@final_tax2", SqlDbType.Money).Value = data[i].P_Tax2;
                sql_cmnd.Parameters.AddWithValue("@final_subtotal", SqlDbType.Money).Value = data[i].P_Sub_Total;
                sql_cmnd.Parameters.AddWithValue("@final_total", SqlDbType.Money).Value = data[i].Total;
                sql_cmnd.Parameters.AddWithValue("@goodsissue", SqlDbType.Money).Value = data[i].Reason_Tag;
                if (j == data.Count()-1)
                {
                    sql_cmnd.Parameters.AddWithValue("@value", SqlDbType.Int).Value = key;
                }
                sql_cmnd.ExecuteNonQuery();
                if (i == data.Count()-1)
                {
                    break;
                }
                i++;
                j++;
            }
            Con1.Close();
        }
        /*public int Add_Data(string Voucher_No, string Voucher_Date, string Invoice_No, string Invoice_Date, string A_Name, string Part_No, string I_Ledger, string A_Ledger, int P_Qty, double P_Rate, double P_Discount, double P_Tax1, double P_Tax2, double P_Sub_Total, double P_Total)
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
        }*/
    }
}