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
    public class NewPurchase_Insert
    {
        public string SP_Description(string cPart_No)
        {
            List<BOMFields> ItemQm = new List<BOMFields>();
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "select P_Description from Product_Master where P_Part_No = '" + cPart_No + "'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlDataReader dr = SqlCmd1.ExecuteReader();
            while (dr.Read())
            {
                ItemQm.Add(new BOMFields
                {
                    Description = dr["P_Description"].ToString()
                }
                );
            }
            if (ItemQm.Count() != 0)
            {
                string item = string.Join("", ItemQm.Select(m => m.Description));
                Con.Close();
                return item;
            }
            else
            {
                return null;
            }

        }
        public List<New_Purchase> Product_Master()
        {
            List<New_Purchase> ItemQm = new List<New_Purchase>();
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "select P_Part_No,P_Description from Product_Master where P_Level<0 ORDER BY P_code Asc;";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlDataReader dr = SqlCmd1.ExecuteReader();
            while (dr.Read())
            {
                ItemQm.Add(new New_Purchase
                {
                    P_Part_No = dr["P_Part_No"].ToString(),
                    P_Description = dr["P_Description"].ToString()
                }
                );
            }
            Con.Close();
            return ItemQm;

        }
        public void Add_Data(List<PurchaseTable> data, int Qty, double total, double final_total)
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
                sql_cmnd.Parameters.AddWithValue("@p_qty", SqlDbType.Int).Value = data[i].Quantity;
                sql_cmnd.Parameters.AddWithValue("@iledger", SqlDbType.NVarChar).Value = data[i].ILedger;
                sql_cmnd.Parameters.AddWithValue("@aledger", SqlDbType.NVarChar).Value = data[i].ALedger;
                sql_cmnd.Parameters.AddWithValue("@i_rate", SqlDbType.Money).Value = data[i].Price_Per_Unit;
                sql_cmnd.Parameters.AddWithValue("@i_subtotal", SqlDbType.Money).Value = data[i].Sub_Total;
                sql_cmnd.Parameters.AddWithValue("@i_discount", SqlDbType.Decimal).Value = data[i].Discount;
                sql_cmnd.Parameters.AddWithValue("@i_tax1", SqlDbType.Money).Value = data[i].Tax1;
                sql_cmnd.Parameters.AddWithValue("@i_tax2", SqlDbType.Money).Value = data[i].Tax2;
                sql_cmnd.Parameters.AddWithValue("@i_total", SqlDbType.Money).Value = data[i].Total;
                sql_cmnd.Parameters.AddWithValue("@Total", SqlDbType.Money).Value = total;
                sql_cmnd.Parameters.AddWithValue("@Total_Qty", SqlDbType.Money).Value = Qty;
                sql_cmnd.Parameters.AddWithValue("@Final_Total", SqlDbType.Money).Value = final_total;
                sql_cmnd.Parameters.AddWithValue("@acc_name", SqlDbType.NVarChar).Value = null;
                sql_cmnd.Parameters.AddWithValue("@goodsissue", SqlDbType.NVarChar).Value = null;

                if (j == data.Count() - 1)
                {
                    sql_cmnd.Parameters.AddWithValue("@value", SqlDbType.Int).Value = key;
                }
                sql_cmnd.ExecuteNonQuery();
                if (i == data.Count() - 1)
                {
                    break;
                }
                i++;
                j++;
            }
            Con1.Close();
        }
    }
}