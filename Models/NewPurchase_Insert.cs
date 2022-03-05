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
        public List<string> SP_Description(string cPart_No)
        {
            List<BOMFields> ItemQm = new List<BOMFields>();
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "select P_Description, P_code from Product_Master where P_Part_No = '" + cPart_No + "'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlDataReader dr = SqlCmd1.ExecuteReader();
            while (dr.Read())
            {
                ItemQm.Add(new BOMFields
                {
                    Description = dr["P_Description"].ToString(),
                    P_code = dr["P_code"].ToString()
                }
                );
            }
            if (ItemQm.Count() != 0)
            {
                string item1 = string.Join("", ItemQm.Select(m => m.Description));
                string item2 = string.Join("", ItemQm.Select(m => m.P_code));
                List<string> vs = new List<string>();
                vs.Add(item1);
                vs.Add(item2);
                Con.Close();
                return vs;
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
        public void Add_Data(List<PurchaseTable> data, int Qty, double total, double final_total, double final_discount, double final_tax1, double final_tax2)
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
            dr.Close();
            string cmd3 = "update Number_Master set Ref_No = Ref_No + 1";
            SqlCommand SqlCmd3 = new SqlCommand(cmd3, Con);
            SqlCmd3.ExecuteNonQuery();
            string cmd4 = "select Ref_No from Number_Master";
            SqlCommand SqlCmd4 = new SqlCommand(cmd4, Con);
            SqlDataReader dr1 = SqlCmd4.ExecuteReader();
            dr1.Read();
            int Ref_No = dr1.GetInt32(0);
            dr1.Close();
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
                sql_cmnd.Parameters.AddWithValue("@ref_no", SqlDbType.NVarChar).Value = Ref_No;
                sql_cmnd.Parameters.AddWithValue("@part_no", SqlDbType.NVarChar).Value = data[i].Part_No;
                sql_cmnd.Parameters.AddWithValue("@p_qty", SqlDbType.Int).Value = data[i].Quantity;
                sql_cmnd.Parameters.AddWithValue("@iledger", SqlDbType.NVarChar).Value = data[i].ILedger;
                sql_cmnd.Parameters.AddWithValue("@aledger", SqlDbType.NVarChar).Value = data[i].ALedger;
                sql_cmnd.Parameters.AddWithValue("@i_rate", SqlDbType.Money).Value = data[i].Price;
                sql_cmnd.Parameters.AddWithValue("@i_subtotal", SqlDbType.Money).Value = data[i].SubTotal;
                sql_cmnd.Parameters.AddWithValue("@i_discount", SqlDbType.Decimal).Value = data[i].Discount;
                sql_cmnd.Parameters.AddWithValue("@i_tax1", SqlDbType.Money).Value = data[i].Tax1;
                sql_cmnd.Parameters.AddWithValue("@i_tax2", SqlDbType.Money).Value = data[i].Tax2;
                sql_cmnd.Parameters.AddWithValue("@i_total", SqlDbType.Money).Value = data[i].Total;
                sql_cmnd.Parameters.AddWithValue("@Total", SqlDbType.Money).Value = total;
                sql_cmnd.Parameters.AddWithValue("@Total_Qty", SqlDbType.Money).Value = Qty;
                sql_cmnd.Parameters.AddWithValue("@Final_Total", SqlDbType.Money).Value = final_total;
                sql_cmnd.Parameters.AddWithValue("@Final_Discount", SqlDbType.Money).Value = final_discount;
                sql_cmnd.Parameters.AddWithValue("@Final_Tax1", SqlDbType.Money).Value = final_tax1;
                sql_cmnd.Parameters.AddWithValue("@Final_Tax2", SqlDbType.Money).Value = final_tax2;
                sql_cmnd.Parameters.AddWithValue("@acc_name", SqlDbType.NVarChar).Value = data[i].supplier;
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
        public List<PurchaseList> Purchase_List()
        {
            List<PurchaseList> ItemQm = new List<PurchaseList>();
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "select Invoice_No,Invoice_Date,Voucher_No,Voucher_Date,A_code from Purchase ORDER BY Voucher_No Asc;";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlDataReader dr = SqlCmd1.ExecuteReader();
            while (dr.Read())
            {
                ItemQm.Add(new PurchaseList
                {
                    Invoice_No = dr["Invoice_No"].ToString(),
                    Invoice_Date = dr["Invoice_Date"].ToString(),
                    Voucher_No = dr["Voucher_No"].ToString(),
                    Voucher_Date = dr["Voucher_Date"].ToString(),
                    A_code = dr["A_code"].ToString()
                }
                );
            }
            Con.Close();
            return ItemQm;

        }
        public List<New_Purchase> Pcode_to_PartNo(string data)
        {
            List<New_Purchase> ItemQm = new List<New_Purchase>();
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "select P_Part_No,P_Description from Product_Master where P_code = '" + data + "'";
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
            return ItemQm;
        }

        public void Edit_and_Delete(List<PurchaseTable> data, int final_Qty, double final_subtotal, double final_discount, double final_tax1, double final_tax2, double final_total, int ref_no)
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "delete from Purchase where Voucher_No ='"+ data[0].Voucher_No + "'";
            string cmd2 = "delete from I_Ledger where Voucher_No ='" + data[0].Voucher_No + "'";
            string cmd3 = "delete from A_Ledger where Voucher_No ='" + data[0].Voucher_No + "'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
            SqlCommand SqlCmd3 = new SqlCommand(cmd3, Con);
            SqlCmd1.ExecuteNonQuery();
            SqlCmd2.ExecuteNonQuery();
            SqlCmd3.ExecuteNonQuery();
            Con.Close();

            SqlConnection Con1 = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con1.Open();
            //Condition value to execute A_Ledger
            int key = 1;

            int j = 0;
            int i = 0;
            while (i < data.Count())
            {
                SqlCommand sql_cmnd = new SqlCommand("[dbo].[EditPurchase]", Con1);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@pcode", SqlDbType.NVarChar).Value = data[i].Pcode;
                sql_cmnd.Parameters.AddWithValue("@part_no", SqlDbType.NVarChar).Value = data[i].Part_No;
                sql_cmnd.Parameters.AddWithValue("@description", SqlDbType.NVarChar).Value = data[i].Description;
                sql_cmnd.Parameters.AddWithValue("@i_qty", SqlDbType.Int).Value = data[i].Quantity;
                sql_cmnd.Parameters.AddWithValue("@i_rate", SqlDbType.Money).Value = data[i].Price;
                sql_cmnd.Parameters.AddWithValue("@i_subtotal", SqlDbType.Money).Value = data[i].SubTotal;
                sql_cmnd.Parameters.AddWithValue("@i_discount", SqlDbType.Decimal).Value = data[i].Discount;
                sql_cmnd.Parameters.AddWithValue("@i_tax1", SqlDbType.Money).Value = data[i].Tax1;
                sql_cmnd.Parameters.AddWithValue("@i_tax2", SqlDbType.Money).Value = data[i].Tax2;
                sql_cmnd.Parameters.AddWithValue("@i_total", SqlDbType.Money).Value = data[i].Total;
                sql_cmnd.Parameters.AddWithValue("@invoice_no", SqlDbType.NVarChar).Value = data[i].Invoice_No;
                sql_cmnd.Parameters.AddWithValue("@invoice_date", data[i].Invoice_Date);
                sql_cmnd.Parameters.AddWithValue("@voucher_no", SqlDbType.Int).Value = data[i].Voucher_No;
                sql_cmnd.Parameters.AddWithValue("@voucher_date", data[i].Voucher_Date);
                sql_cmnd.Parameters.AddWithValue("@ref_no", SqlDbType.Int).Value = ref_no;
                sql_cmnd.Parameters.AddWithValue("@iledger", SqlDbType.NVarChar).Value = data[i].ILedger;
                sql_cmnd.Parameters.AddWithValue("@aledger", SqlDbType.NVarChar).Value = data[i].ALedger;
                sql_cmnd.Parameters.AddWithValue("@Final_Qty", SqlDbType.Money).Value = final_Qty;
                sql_cmnd.Parameters.AddWithValue("@Final_SubTotal", SqlDbType.Money).Value = final_subtotal;
                sql_cmnd.Parameters.AddWithValue("@Final_Discount", SqlDbType.Money).Value = final_discount;
                sql_cmnd.Parameters.AddWithValue("@Final_Tax1", SqlDbType.Money).Value = final_tax1;
                sql_cmnd.Parameters.AddWithValue("@Final_Tax2", SqlDbType.Money).Value = final_tax2;
                sql_cmnd.Parameters.AddWithValue("@Final_Total", SqlDbType.Money).Value = final_total;
                sql_cmnd.Parameters.AddWithValue("@acc_name", SqlDbType.NVarChar).Value = data[i].supplier;
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