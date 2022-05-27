using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Admin.Models;
using System.Configuration;
using Newtonsoft.Json;
using System.Globalization;
using System.Reflection;

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
                Con.Close();
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
        public List<PurchaseTable> Add_PO_to_purchase(string po_no)
        {
            List<PurchaseTable> ItemQm = new List<PurchaseTable>();
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "SELECT * FROM Purchase_Order WHERE PO_No = " + Convert.ToInt32(po_no) + "";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlDataReader dr = SqlCmd1.ExecuteReader();
            while (dr.Read())
            {
                ItemQm.Add(new PurchaseTable
                {
                    PO_No = (int)dr["PO_No"],
                    supplier = dr["Supplier_Acode"].ToString(),
                    BillTo = dr["BillTo_Acode"].ToString(),
                    Pcode = dr["P_code"].ToString(),
                    Part_No = dr["Part_No"].ToString(),
                    Description = dr["Description"].ToString(),
                    project = dr["Project"].ToString(),
                    Quantity = (int)dr["PO_Qty"],
                    Price = Convert.ToDouble(dr["PO_Price"]),
                    Dis_per = Convert.ToDouble(dr["PO_Dis_Per"]),
                    Dis_Rs = Convert.ToDouble(dr["PO_Dis_val"]),
                    Igst_per = Convert.ToDouble(dr["PO_Igst_Per"]),
                    Igst_Rs = Convert.ToDouble(dr["PO_Igst_val"]),
                    Cgst_per = Convert.ToDouble(dr["PO_Cgst_Per"]),
                    Cgst_Rs = Convert.ToDouble(dr["PO_Cgst_val"]),
                    Sgst_per = Convert.ToDouble(dr["PO_Sgst_Per"]),
                    Sgst_Rs = Convert.ToDouble(dr["PO_Sgst_val"]),
                    SubTotal = Convert.ToDouble(dr["PO_Subtotal"]),
                    Total = Convert.ToDouble(dr["PO_Total"]),
                    ILedger = 1,
                    ALedger = 2
                }
                );
            }
            dr.Close();
            for (int j = 0; j < ItemQm.Count(); j++)
            {
                string cmd3 = "select * from PO_A_Ledger where PO_No = " + Convert.ToInt32(po_no) + "";
                SqlCommand SqlCmd3 = new SqlCommand(cmd3, Con);
                SqlDataReader dr3 = SqlCmd3.ExecuteReader();
                while (dr3.Read())
                {
                    ItemQm[j].Final_Dis_per = Convert.ToDouble(dr3["Final_Dis_Per"]);
                    ItemQm[j].Final_Dis_Rs = Convert.ToDouble(dr3["Final_Dis_Rs"]);
                    ItemQm[j].Final_Igst_per = Convert.ToDouble(dr3["Final_Igst_Per"]);
                    ItemQm[j].Final_Igst_Rs = Convert.ToDouble(dr3["Final_Igst_Rs"]);
                    ItemQm[j].Final_Cgst_per = Convert.ToDouble(dr3["Final_Cgst_Per"]);
                    ItemQm[j].Final_Cgst_Rs = Convert.ToDouble(dr3["Final_Cgst_Rs"]);
                    ItemQm[j].Final_Sgst_per = Convert.ToDouble(dr3["Final_Sgst_Per"]);
                    ItemQm[j].Final_Sgst_Rs = Convert.ToDouble(dr3["Final_Sgst_Rs"]);
                    ItemQm[j].Final_Qty = (int)dr3["Final_Qty"];
                    ItemQm[j].Final_Sub_Total = Convert.ToDouble(dr3["Final_Subtotal"]);
                    ItemQm[j].Final_total = Convert.ToDouble(dr3["Final_Total"]);
                }
                dr3.Close();
            }
            Con.Close();
            return ItemQm;
        }
        public int Add_Data(List<PurchaseTable> data)
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "Update Number_master Set Purchase_Voucher_No = Purchase_Voucher_No + 1 ";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlCmd1.ExecuteNonQuery();
            string cmd2 = "Select Purchase_Voucher_No from Number_master ";
            SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
            SqlDataReader dr = SqlCmd2.ExecuteReader();
            dr.Read();
            int Voucher_No = dr.GetInt32(0);
            dr.Close();
            Con.Close();

            SqlConnection Con1 = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con1.Open();
            //Condition value to execute A_Ledger
            int key = 1;
            int ARefNo = 3;
            int j = 0;
            int i = 0;
            DateTime now = DateTime.Now;
            while (i < data.Count())
            {
                SqlCommand sql_cmnd = new SqlCommand("[dbo].[addPurchase]", Con1);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@voucher_no", SqlDbType.NVarChar).Value = Voucher_No;
                sql_cmnd.Parameters.AddWithValue("@invoice_no", SqlDbType.NVarChar).Value = data[0].Invoice_No;
                sql_cmnd.Parameters.AddWithValue("@invoice_date", data[0].Invoice_Date);
                sql_cmnd.Parameters.AddWithValue("@part_no", SqlDbType.NVarChar).Value = data[i].Part_No;
                sql_cmnd.Parameters.AddWithValue("@qty", SqlDbType.Int).Value = data[i].Quantity;
                sql_cmnd.Parameters.AddWithValue("@price", SqlDbType.Money).Value = data[i].Price;
                sql_cmnd.Parameters.AddWithValue("@Dis_per", SqlDbType.Float).Value = data[i].Dis_per;
                sql_cmnd.Parameters.AddWithValue("@Dis_val", SqlDbType.Money).Value = data[i].Dis_Rs;
                sql_cmnd.Parameters.AddWithValue("@Igst_per", SqlDbType.Float).Value = data[i].Igst_per;
                sql_cmnd.Parameters.AddWithValue("@Igst_val", SqlDbType.Money).Value = data[i].Igst_Rs;
                sql_cmnd.Parameters.AddWithValue("@Cgst_per", SqlDbType.Float).Value = data[i].Cgst_per;
                sql_cmnd.Parameters.AddWithValue("@Cgst_val", SqlDbType.Money).Value = data[i].Cgst_Rs;
                sql_cmnd.Parameters.AddWithValue("@Sgst_per", SqlDbType.Float).Value = data[i].Sgst_per;
                sql_cmnd.Parameters.AddWithValue("@Sgst_val", SqlDbType.Money).Value = data[i].Sgst_Rs;
                sql_cmnd.Parameters.AddWithValue("@subtotal", SqlDbType.Money).Value = data[i].SubTotal;
                sql_cmnd.Parameters.AddWithValue("@total", SqlDbType.Money).Value = data[i].Total;
                sql_cmnd.Parameters.AddWithValue("@final_Dis_per", SqlDbType.Float).Value = data[0].Final_Dis_per;
                sql_cmnd.Parameters.AddWithValue("@final_Dis_val", SqlDbType.Money).Value = data[0].Final_Dis_Rs;
                sql_cmnd.Parameters.AddWithValue("@final_Igst_per", SqlDbType.Float).Value = data[0].Final_Igst_per;
                sql_cmnd.Parameters.AddWithValue("@final_Igst_val", SqlDbType.Money).Value = data[0].Final_Igst_Rs;
                sql_cmnd.Parameters.AddWithValue("@final_Cgst_per", SqlDbType.Float).Value = data[0].Final_Cgst_per;
                sql_cmnd.Parameters.AddWithValue("@final_Cgst_val", SqlDbType.Money).Value = data[0].Final_Cgst_Rs;
                sql_cmnd.Parameters.AddWithValue("@final_Sgst_per", SqlDbType.Float).Value = data[0].Final_Sgst_per;
                sql_cmnd.Parameters.AddWithValue("@final_Sgst_val", SqlDbType.Money).Value = data[0].Final_Sgst_Rs;
                sql_cmnd.Parameters.AddWithValue("@final_Qty", SqlDbType.Int).Value = data[0].Final_Qty;
                sql_cmnd.Parameters.AddWithValue("@final_Subtotal", SqlDbType.Money).Value = data[0].Final_Sub_Total;
                sql_cmnd.Parameters.AddWithValue("@final_total", SqlDbType.Money).Value = data[0].Final_total;
                sql_cmnd.Parameters.AddWithValue("@Supplier_Acode", SqlDbType.NVarChar).Value = data[i].supplier;
                sql_cmnd.Parameters.AddWithValue("@project", SqlDbType.Int).Value = data[i].project;
                sql_cmnd.Parameters.AddWithValue("@Billto_Acode", SqlDbType.NVarChar).Value = data[i].BillTo;
                sql_cmnd.Parameters.AddWithValue("@ARefNo", SqlDbType.NVarChar).Value = ARefNo;
                sql_cmnd.Parameters.AddWithValue("@po_no", SqlDbType.Int).Value = data[i].PO_No;
                sql_cmnd.Parameters.AddWithValue("@iledger", SqlDbType.Int).Value = data[i].ILedger;
                sql_cmnd.Parameters.AddWithValue("@aledger", SqlDbType.Int).Value = data[i].ALedger;
                sql_cmnd.Parameters.AddWithValue("@goodsissue", SqlDbType.NVarChar).Value = null;
                sql_cmnd.Parameters.AddWithValue("@time", SqlDbType.Time).Value = now.ToLongTimeString();

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
            return Voucher_No;
        }
        public List<PurchaseList> Purchase_List()
        {
            List<PurchaseList> ItemQm = new List<PurchaseList>();
            List<int> vno = new List<int>();
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd2 = "SELECT Voucher_No FROM Purchase GROUP BY Voucher_No HAVING COUNT(*)>0";
            SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
            SqlDataReader dr1 = SqlCmd2.ExecuteReader();
            while (dr1.Read())
            {
                vno.Add(Convert.ToInt32(dr1["Voucher_No"]));
            }
            dr1.Close();
            if(vno.Count == 0)
            {
                Con.Close();
                return ItemQm;
            }
            else
            {
                for (int i = 0; i < vno.Count; i++)
                {
                    string cmd1 = "select Top 1 Invoice_No,Invoice_Date,Voucher_No,Voucher_Date,Supplier_Acode,Project,PO_No from Purchase where Voucher_No = '" + vno[i] + "' ORDER BY Voucher_No Asc;";
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
                            A_code = dr["Supplier_Acode"].ToString(),
                            project = dr["Project"].ToString(),
                            PO_No = dr["PO_No"].ToString()
                        }
                        ) ;
                    }
                    dr.Close();
                }
                Con.Close();
                return ItemQm;
            }
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
            Con.Close();
            return ItemQm;
        }
        public void Edit_and_Delete(List<PurchaseTable> data)
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            for (int k = 0; k <= data.Count - 1; k++)
            {
                string cmd4 = "select Purchase_Qty from Purchase where Voucher_No = '" + data[k].Voucher_No + "' and P_Part_No = '" + data[k].Part_No+"'";
                SqlCommand SqlCmd4 = new SqlCommand(cmd4, Con);
                SqlDataReader dr2 = SqlCmd4.ExecuteReader();
                while (dr2.Read())
                {
                    data[k].I_Qty = (int)dr2["Purchase_Qty"];
                }
                dr2.Close();
                string cmd5 = "update Product_Master set P_Closing_Balance = P_Closing_Balance - '" + data[k].Quantity + "' where P_Part_No = '" + data[k].Part_No + "'";
                SqlCommand SqlCmd5 = new SqlCommand(cmd5, Con);
                SqlCmd5.ExecuteNonQuery();
            }
            string cmd1 = "delete from Purchase where Voucher_No ='" + data[0].Voucher_No + "'";
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
            int ARefNo = 3;
            int j = 0;
            int i = 0;
            int iledger = 1;
            int aledger = 2;
            DateTime now = DateTime.Now;
            while (i < data.Count())
            {
                SqlCommand sql_cmnd = new SqlCommand("[dbo].[EditPurchase]", Con1);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@voucher_no", SqlDbType.NVarChar).Value = data[0].Voucher_No;
                sql_cmnd.Parameters.AddWithValue("@voucher_date", SqlDbType.NVarChar).Value = data[0].Voucher_Date;
                sql_cmnd.Parameters.AddWithValue("@invoice_no", SqlDbType.NVarChar).Value = data[0].Invoice_No;
                sql_cmnd.Parameters.AddWithValue("@invoice_date", data[0].Invoice_Date);
                sql_cmnd.Parameters.AddWithValue("@part_no", SqlDbType.NVarChar).Value = data[i].Part_No;
                sql_cmnd.Parameters.AddWithValue("@qty", SqlDbType.Int).Value = data[i].Quantity;
                sql_cmnd.Parameters.AddWithValue("@price", SqlDbType.Money).Value = data[i].Price;
                sql_cmnd.Parameters.AddWithValue("@Dis_per", SqlDbType.Float).Value = data[i].Dis_per;
                sql_cmnd.Parameters.AddWithValue("@Dis_val", SqlDbType.Money).Value = data[i].Dis_Rs;
                sql_cmnd.Parameters.AddWithValue("@Igst_per", SqlDbType.Float).Value = data[i].Igst_per;
                sql_cmnd.Parameters.AddWithValue("@Igst_val", SqlDbType.Money).Value = data[i].Igst_Rs;
                sql_cmnd.Parameters.AddWithValue("@Cgst_per", SqlDbType.Float).Value = data[i].Cgst_per;
                sql_cmnd.Parameters.AddWithValue("@Cgst_val", SqlDbType.Money).Value = data[i].Cgst_Rs;
                sql_cmnd.Parameters.AddWithValue("@Sgst_per", SqlDbType.Float).Value = data[i].Sgst_per;
                sql_cmnd.Parameters.AddWithValue("@Sgst_val", SqlDbType.Money).Value = data[i].Sgst_Rs;
                sql_cmnd.Parameters.AddWithValue("@subtotal", SqlDbType.Money).Value = data[i].SubTotal;
                sql_cmnd.Parameters.AddWithValue("@total", SqlDbType.Money).Value = data[i].Total;
                sql_cmnd.Parameters.AddWithValue("@final_Dis_per", SqlDbType.Float).Value = data[0].Final_Dis_per;
                sql_cmnd.Parameters.AddWithValue("@final_Dis_val", SqlDbType.Money).Value = data[0].Final_Dis_Rs;
                sql_cmnd.Parameters.AddWithValue("@final_Igst_per", SqlDbType.Float).Value = data[0].Final_Igst_per;
                sql_cmnd.Parameters.AddWithValue("@final_Igst_val", SqlDbType.Money).Value = data[0].Final_Igst_Rs;
                sql_cmnd.Parameters.AddWithValue("@final_Cgst_per", SqlDbType.Float).Value = data[0].Final_Cgst_per;
                sql_cmnd.Parameters.AddWithValue("@final_Cgst_val", SqlDbType.Money).Value = data[0].Final_Cgst_Rs;
                sql_cmnd.Parameters.AddWithValue("@final_Sgst_per", SqlDbType.Float).Value = data[0].Final_Sgst_per;
                sql_cmnd.Parameters.AddWithValue("@final_Sgst_val", SqlDbType.Money).Value = data[0].Final_Sgst_Rs;
                sql_cmnd.Parameters.AddWithValue("@final_Qty", SqlDbType.Int).Value = data[0].Final_Qty;
                sql_cmnd.Parameters.AddWithValue("@final_Subtotal", SqlDbType.Money).Value = data[0].Final_Sub_Total;
                sql_cmnd.Parameters.AddWithValue("@final_total", SqlDbType.Money).Value = data[0].Final_total;
                sql_cmnd.Parameters.AddWithValue("@Supplier_Acode", SqlDbType.NVarChar).Value = data[i].supplier;
                sql_cmnd.Parameters.AddWithValue("@project", SqlDbType.Int).Value = data[i].project;
                sql_cmnd.Parameters.AddWithValue("@Billto_Acode", SqlDbType.NVarChar).Value = data[i].BillTo;
                sql_cmnd.Parameters.AddWithValue("@ARefNo", SqlDbType.NVarChar).Value = ARefNo;
                sql_cmnd.Parameters.AddWithValue("@po_no", SqlDbType.Int).Value = data[i].PO_No;
                sql_cmnd.Parameters.AddWithValue("@iledger", SqlDbType.Int).Value = iledger;
                sql_cmnd.Parameters.AddWithValue("@aledger", SqlDbType.Int).Value = aledger;
                sql_cmnd.Parameters.AddWithValue("@goodsissue", SqlDbType.NVarChar).Value = null;
                sql_cmnd.Parameters.AddWithValue("@time", SqlDbType.Time).Value = now.ToLongTimeString();

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
        public string P_Description(string cPart_No)
        {
            List<GoodsRI> ItemQm = new List<GoodsRI>();
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "select P_Description from Product_Master where P_Part_No = '" + cPart_No + "'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlDataReader dr = SqlCmd1.ExecuteReader();
            while (dr.Read())
            {
                ItemQm.Add(new GoodsRI
                {
                    Description = dr["P_Description"].ToString()
                }
                );
            }
            if (ItemQm.Count() != 0)
            {
                string item1 = string.Join("", ItemQm.Select(m => m.Description));
                Con.Close();
                return item1;
            }
            else
            {
                Con.Close();
                return null;
            }
        }
        public int Add_PO(List<PurchaseTable> data)
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "Update Number_master Set PO_No = PO_No + 1 ";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlCmd1.ExecuteNonQuery();
            string cmd2 = "Select PO_No from Number_master ";
            SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
            SqlDataReader dr = SqlCmd2.ExecuteReader();
            dr.Read();
            int PO_No = dr.GetInt32(0);
            dr.Close();
            Con.Close();
            string ref_date = data[0].Invoice_Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            string PO_date = data[0].PO_Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            SqlConnection Con1 = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con1.Open();
            int i = 0;
            int j = 0;
            int key = 1;
            while (i < data.Count())
            {
                SqlCommand sql_cmnd = new SqlCommand("[dbo].[Add_PO]", Con1);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@po_no", SqlDbType.Int).Value = PO_No;
                sql_cmnd.Parameters.AddWithValue("@po_date", PO_date);
                sql_cmnd.Parameters.AddWithValue("@ref_no", SqlDbType.NVarChar).Value = data[i].Invoice_No;
                sql_cmnd.Parameters.AddWithValue("@ref_date", ref_date);
                sql_cmnd.Parameters.AddWithValue("@part_no", SqlDbType.NVarChar).Value = data[i].Part_No;
                sql_cmnd.Parameters.AddWithValue("@qty", SqlDbType.Int).Value = data[i].Quantity;
                sql_cmnd.Parameters.AddWithValue("@price", SqlDbType.Money).Value = data[i].Price;
                sql_cmnd.Parameters.AddWithValue("@Dis_per", SqlDbType.Float).Value = data[i].Dis_per;
                sql_cmnd.Parameters.AddWithValue("@Dis_val", SqlDbType.Money).Value = data[i].Dis_Rs;
                sql_cmnd.Parameters.AddWithValue("@Igst_per", SqlDbType.Float).Value = data[i].Igst_per;
                sql_cmnd.Parameters.AddWithValue("@Igst_val", SqlDbType.Money).Value = data[i].Igst_Rs;
                sql_cmnd.Parameters.AddWithValue("@Cgst_per", SqlDbType.Float).Value = data[i].Cgst_per;
                sql_cmnd.Parameters.AddWithValue("@Cgst_val", SqlDbType.Money).Value = data[i].Cgst_Rs;
                sql_cmnd.Parameters.AddWithValue("@Sgst_per", SqlDbType.Float).Value = data[i].Sgst_per;
                sql_cmnd.Parameters.AddWithValue("@Sgst_val", SqlDbType.Money).Value = data[i].Sgst_Rs;
                sql_cmnd.Parameters.AddWithValue("@subtotal", SqlDbType.Money).Value = data[i].SubTotal;
                sql_cmnd.Parameters.AddWithValue("@total", SqlDbType.Money).Value = data[i].Total;
                sql_cmnd.Parameters.AddWithValue("@final_Dis_per", SqlDbType.Float).Value = data[0].Final_Dis_per;
                sql_cmnd.Parameters.AddWithValue("@final_Dis_val", SqlDbType.Money).Value = data[0].Final_Dis_Rs;
                sql_cmnd.Parameters.AddWithValue("@final_Igst_per", SqlDbType.Float).Value = data[0].Final_Igst_per;
                sql_cmnd.Parameters.AddWithValue("@final_Igst_val", SqlDbType.Money).Value = data[0].Final_Igst_Rs;
                sql_cmnd.Parameters.AddWithValue("@final_Cgst_per", SqlDbType.Float).Value = data[0].Final_Cgst_per;
                sql_cmnd.Parameters.AddWithValue("@final_Cgst_val", SqlDbType.Money).Value = data[0].Final_Cgst_Rs;
                sql_cmnd.Parameters.AddWithValue("@final_Sgst_per", SqlDbType.Float).Value = data[0].Final_Sgst_per;
                sql_cmnd.Parameters.AddWithValue("@final_Sgst_val", SqlDbType.Money).Value = data[0].Final_Sgst_Rs;
                sql_cmnd.Parameters.AddWithValue("@final_Qty", SqlDbType.Int).Value = data[0].Final_Qty;
                sql_cmnd.Parameters.AddWithValue("@final_Subtotal", SqlDbType.Money).Value = data[0].Final_Sub_Total;
                sql_cmnd.Parameters.AddWithValue("@final_total", SqlDbType.Money).Value = data[0].Final_total;
                sql_cmnd.Parameters.AddWithValue("@Supplier_Acode", SqlDbType.NVarChar).Value = data[i].supplier;
                sql_cmnd.Parameters.AddWithValue("@project", SqlDbType.Int).Value = data[i].project;
                sql_cmnd.Parameters.AddWithValue("@Billto_Acode", SqlDbType.NVarChar).Value = data[i].BillTo;
                if (j == data.Count() - 1)
                {
                    sql_cmnd.Parameters.AddWithValue("@key", SqlDbType.Int).Value = key;
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
            return PO_No;
        }
        public void Edit_PO(List<PurchaseTable> data)
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "delete from Purchase_Order where PO_No = '"+data[0].PO_No+"'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlCmd1.ExecuteNonQuery();
            string cmd2 = "delete from PO_A_Ledger where PO_No = '" + data[0].PO_No + "'";
            SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
            SqlCmd2.ExecuteNonQuery();
            Con.Close();
            string ref_date = data[0].Invoice_Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            string PO_date = data[0].PO_Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            SqlConnection Con1 = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con1.Open();
            int i = 0;
            int j = 0;
            int key = 1;
            while (i < data.Count())
            {
                SqlCommand sql_cmnd = new SqlCommand("[dbo].[Add_PO]", Con1);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@po_no", SqlDbType.Int).Value = data[i].PO_No;
                sql_cmnd.Parameters.AddWithValue("@po_date", PO_date);
                sql_cmnd.Parameters.AddWithValue("@ref_no", SqlDbType.NVarChar).Value = data[i].Invoice_No;
                sql_cmnd.Parameters.AddWithValue("@ref_date", ref_date);
                sql_cmnd.Parameters.AddWithValue("@part_no", SqlDbType.NVarChar).Value = data[i].Part_No;
                sql_cmnd.Parameters.AddWithValue("@qty", SqlDbType.Int).Value = data[i].Quantity;
                sql_cmnd.Parameters.AddWithValue("@price", SqlDbType.Money).Value = data[i].Price;
                sql_cmnd.Parameters.AddWithValue("@Dis_per", SqlDbType.Float).Value = data[i].Dis_per;
                sql_cmnd.Parameters.AddWithValue("@Dis_val", SqlDbType.Money).Value = data[i].Dis_Rs;
                sql_cmnd.Parameters.AddWithValue("@Igst_per", SqlDbType.Float).Value = data[i].Igst_per;
                sql_cmnd.Parameters.AddWithValue("@Igst_val", SqlDbType.Money).Value = data[i].Igst_Rs;
                sql_cmnd.Parameters.AddWithValue("@Cgst_per", SqlDbType.Float).Value = data[i].Cgst_per;
                sql_cmnd.Parameters.AddWithValue("@Cgst_val", SqlDbType.Money).Value = data[i].Cgst_Rs;
                sql_cmnd.Parameters.AddWithValue("@Sgst_per", SqlDbType.Float).Value = data[i].Sgst_per;
                sql_cmnd.Parameters.AddWithValue("@Sgst_val", SqlDbType.Money).Value = data[i].Sgst_Rs;
                sql_cmnd.Parameters.AddWithValue("@subtotal", SqlDbType.Money).Value = data[i].SubTotal;
                sql_cmnd.Parameters.AddWithValue("@total", SqlDbType.Money).Value = data[i].Total;
                sql_cmnd.Parameters.AddWithValue("@final_Dis_per", SqlDbType.Float).Value = data[0].Final_Dis_per;
                sql_cmnd.Parameters.AddWithValue("@final_Dis_val", SqlDbType.Money).Value = data[0].Final_Dis_Rs;
                sql_cmnd.Parameters.AddWithValue("@final_Igst_per", SqlDbType.Float).Value = data[0].Final_Igst_per;
                sql_cmnd.Parameters.AddWithValue("@final_Igst_val", SqlDbType.Money).Value = data[0].Final_Igst_Rs;
                sql_cmnd.Parameters.AddWithValue("@final_Cgst_per", SqlDbType.Float).Value = data[0].Final_Cgst_per;
                sql_cmnd.Parameters.AddWithValue("@final_Cgst_val", SqlDbType.Money).Value = data[0].Final_Cgst_Rs;
                sql_cmnd.Parameters.AddWithValue("@final_Sgst_per", SqlDbType.Float).Value = data[0].Final_Sgst_per;
                sql_cmnd.Parameters.AddWithValue("@final_Sgst_val", SqlDbType.Money).Value = data[0].Final_Sgst_Rs;
                sql_cmnd.Parameters.AddWithValue("@final_Qty", SqlDbType.Int).Value = data[0].Final_Qty;
                sql_cmnd.Parameters.AddWithValue("@final_Subtotal", SqlDbType.Money).Value = data[0].Final_Sub_Total;
                sql_cmnd.Parameters.AddWithValue("@final_total", SqlDbType.Money).Value = data[0].Final_total;
                sql_cmnd.Parameters.AddWithValue("@Supplier_Acode", SqlDbType.NVarChar).Value = data[i].supplier;
                sql_cmnd.Parameters.AddWithValue("@project", SqlDbType.Int).Value = data[i].project;
                sql_cmnd.Parameters.AddWithValue("@Billto_Acode", SqlDbType.NVarChar).Value = data[i].BillTo;
                if (j == data.Count() - 1)
                {
                    sql_cmnd.Parameters.AddWithValue("@key", SqlDbType.Int).Value = key;
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
        public List<PO_List> PO_List()
        {
            List<PO_List> ItemQm = new List<PO_List>();
            List<int> vno = new List<int>();
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd2 = "SELECT PO_No FROM Purchase_Order where PO_No > 0 GROUP BY PO_No HAVING COUNT(*)>0";
            SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
            SqlDataReader dr1 = SqlCmd2.ExecuteReader();
            while (dr1.Read())
            {
                vno.Add(Convert.ToInt32(dr1["PO_No"]));
            }
            dr1.Close();
            if (vno.Count == 0)
            {
                Con.Close();
                return ItemQm;
            }
            else
            {
                for (int i = 0; i < vno.Count; i++)
                {
                    string cmd1 = "select Top 1 PO_No,PO_Date,Ref_No,Ref_Date,Supplier_Acode,BillTo_Acode,Project from Purchase_Order where PO_No = '" + vno[i] + "' ORDER BY PO_No Asc;";
                    SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
                    SqlDataReader dr = SqlCmd1.ExecuteReader();
                    while (dr.Read())
                    {
                        ItemQm.Add(new PO_List
                        {
                            PO_No = (int)dr["PO_No"],
                            PO_Date = dr["PO_Date"].ToString(),
                            Ref_No = dr["Ref_No"].ToString(),
                            Ref_Date = dr["Ref_Date"].ToString(),
                            acode = dr["Supplier_Acode"].ToString(),
                            billto_acode = dr["BillTo_Acode"].ToString(),
                            project_val = (int)dr["Project"]
                        }
                        );
                    }
                    dr.Close();
                }
                Con.Close();
                return ItemQm;
            }
        }
        public void Update_Close_Bal_P(string Part_No, int vtype, int vno)
        {
            SqlConnection Con1 = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con1.Open();
            List<GoodsList> ItemQm = new List<GoodsList>();
            string cmd2 = "Select P_code from Product_Master where P_Part_No = '" + Part_No + "'";
            SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con1);
            SqlDataReader dr = SqlCmd2.ExecuteReader();
            while (dr.Read())
            {
                ItemQm.Add(new GoodsList
                {
                    P_code = dr["P_code"].ToString()
                }
                );
            }
            dr.Close();
            string pcode = string.Join("", ItemQm.Select(m => m.P_code));

            string cmd3 = "Select Purchase_Qty from I_Ledger where Voucher_Type = '" + vtype + "' and P_code = '" + pcode + "' and Voucher_No = '" + vno + "'";
            SqlCommand SqlCmd3 = new SqlCommand(cmd3, Con1);
            SqlDataReader dr1 = SqlCmd3.ExecuteReader();
            int qty = 0;
            while (dr1.Read())
            {
                string i = dr1["Purchase_Qty"].ToString();
                qty = int.Parse(i);
            }
            dr1.Close();

            string cmd1 = "Update Product_Master set P_Closing_Balance = P_Closing_Balance - '" + qty + "' where P_code = '" + pcode + "'";
            string cmd5 = "delete from Purchase where P_code = '" + pcode + "' and Voucher_No = '" + vno + "'";
            string cmd4 = "delete from I_Ledger where P_code = '" + pcode + "' and Voucher_No = '" + vno + "'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con1);
            SqlCmd1.ExecuteNonQuery();
            SqlCommand SqlCmd5 = new SqlCommand(cmd5, Con1);
            SqlCmd5.ExecuteNonQuery();
            SqlCommand SqlCmd4 = new SqlCommand(cmd4, Con1);
            SqlCmd4.ExecuteNonQuery();
            Con1.Close();
        }
        public void Update_Close_Bal_PO(string Part_No, int vtype, int vno)
        {
            SqlConnection Con1 = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con1.Open();
            string cmd3 = "Delete from Purchase_Order where Part_No = '" + Part_No + "' and PO_No = '" + vno + "'";
            SqlCommand SqlCmd3 = new SqlCommand(cmd3, Con1);
            SqlCmd3.ExecuteNonQuery();
            Con1.Close();
        }

    }
    public class Goods_RI
    {
        public int Goods_add(List<GoodsRI> data)
        {
            SqlConnection Con1 = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con1.Open();
            int GR_V_No = 0;
            if (data[0].Index_Type == 1)
            {
                string cmd1 = "Update Number_Master set GReceipt_Voucher_No = GReceipt_Voucher_No + 1";
                string cmd2 = "select GReceipt_Voucher_No from Number_Master";
                SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con1);
                SqlCmd1.ExecuteNonQuery();
                SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con1);
                SqlDataReader dr = SqlCmd2.ExecuteReader();
                dr.Read();
                GR_V_No = dr.GetInt32(0);
                dr.Close();
            }
            else
            {
                string cmd1 = "Update Number_Master set GIssue_Voucher_No = GIssue_Voucher_No + 1";
                string cmd2 = "select GIssue_Voucher_No from Number_Master";
                SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con1);
                SqlCmd1.ExecuteNonQuery();
                SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con1);
                SqlDataReader dr = SqlCmd2.ExecuteReader();
                dr.Read();
                GR_V_No = dr.GetInt32(0);
                dr.Close();
            }
            for (int j = 0; j < data.Count(); j++)
            {
                string cmd3 = "select P_code from Product_Master where P_Part_No = '" + data[j].Part_No + "'";
                SqlCommand SqlCmd3 = new SqlCommand(cmd3, Con1);
                SqlDataReader dr2 = SqlCmd3.ExecuteReader();
                while (dr2.Read())
                {
                    data[j].P_code = dr2["P_code"].ToString();
                }
                data[j].Voucher_No = GR_V_No;
                dr2.Close();
            }
            int i = 0;
            DateTime now = DateTime.Now;
            while (i < data.Count())
            {
                SqlCommand sql_cmnd = new SqlCommand("[dbo].[GoodsRI_Add]", Con1);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@v_type", SqlDbType.Int).Value = data[i].Index_Type;
                sql_cmnd.Parameters.AddWithValue("@voucher_no", SqlDbType.Int).Value = GR_V_No;
                sql_cmnd.Parameters.AddWithValue("@voucher_date", data[i].V_Date);
                sql_cmnd.Parameters.AddWithValue("@ref_no", SqlDbType.Int).Value = data[i].Ref_No;
                sql_cmnd.Parameters.AddWithValue("@ref_date", data[i].R_Date);
                sql_cmnd.Parameters.AddWithValue("@GI", SqlDbType.Int).Value = data[i].GI_Tag;
                sql_cmnd.Parameters.AddWithValue("@Process", SqlDbType.Int).Value = data[i].Process_Tag;
                sql_cmnd.Parameters.AddWithValue("@Project", SqlDbType.Int).Value = data[i].Project;
                sql_cmnd.Parameters.AddWithValue("@Employee", SqlDbType.Int).Value = data[i].Employee;
                sql_cmnd.Parameters.AddWithValue("@note", SqlDbType.NVarChar).Value = data[i].Note;
                sql_cmnd.Parameters.AddWithValue("@part_no", SqlDbType.NVarChar).Value = data[i].Part_No;
                sql_cmnd.Parameters.AddWithValue("@pcode", SqlDbType.NChar).Value = data[i].P_code;
                sql_cmnd.Parameters.AddWithValue("@qty", SqlDbType.Int).Value = data[i].Quantity;
                sql_cmnd.Parameters.AddWithValue("@time", SqlDbType.Time).Value = now.ToLongTimeString();
                sql_cmnd.ExecuteNonQuery();
                if (i == data.Count() - 1)
                {
                    break;
                }
                i++;
            }
            Con1.Close();
            return GR_V_No;
        }
        public List<GoodsRI> Descp_Qty (string part_no)
        {
            List<GoodsRI> ItemQm = new List<GoodsRI>();
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "select P_Closing_Balance,P_Description,P_code from Product_Master where P_Part_No = '" + part_no + "'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlDataReader dr = SqlCmd1.ExecuteReader();
            while (dr.Read())
            {
                ItemQm.Add(new GoodsRI
                {
                    Description = dr["P_Description"].ToString(),
                    Quantity = (int)dr["P_Closing_Balance"],
                    P_code = dr["P_code"].ToString()
                }
                );
            }
            Con.Close();
            return ItemQm;
        }
        public int Goods_Add_json(List<GoodsRI> data)
        {
            SqlConnection Con1 = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con1.Open();
            int GR_V_No = 0;
            int vtype = data[0].Index_Type;
            if (data[0].Index_Type == 1)
            {
                string cmd1 = "Update Number_Master set GReceipt_Voucher_No = GReceipt_Voucher_No + 1";
                string cmd2 = "select GReceipt_Voucher_No from Number_Master";
                SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con1);
                SqlCmd1.ExecuteNonQuery();
                SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con1);
                SqlDataReader dr = SqlCmd2.ExecuteReader();
                dr.Read();
                GR_V_No = dr.GetInt32(0);
                dr.Close();
            }
            else
            {
                string cmd1 = "Update Number_Master set GIssue_Voucher_No = GIssue_Voucher_No + 1";
                string cmd2 = "select GIssue_Voucher_No from Number_Master";
                SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con1);
                SqlCmd1.ExecuteNonQuery();
                SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con1);
                SqlDataReader dr = SqlCmd2.ExecuteReader();
                dr.Read();
                GR_V_No = dr.GetInt32(0);
                dr.Close();
            }
            for (int i = 0; i < data.Count(); i++)
            {
                string cmd3 = "select P_code from Product_Master where P_Part_No = '" + data[i].Part_No + "'";
                SqlCommand SqlCmd3 = new SqlCommand(cmd3, Con1);
                SqlDataReader dr2 = SqlCmd3.ExecuteReader();
                while (dr2.Read())
                {
                    data[i].P_code = dr2["P_code"].ToString();
                }
                data[i].Voucher_No = GR_V_No;
                dr2.Close();
            }
            var json = JsonConvert.SerializeObject(data);
            string js = "{"+'"'+"myrows" +'"'+ ":" + json + "}";
            SqlCommand sql_cmnd = new SqlCommand("[dbo].[Goods_Add_json]", Con1);
            sql_cmnd.CommandType = CommandType.StoredProcedure;
            sql_cmnd.Parameters.AddWithValue("@json", js);
            sql_cmnd.ExecuteReader();
            Con1.Close();
            return GR_V_No;
        }
        public List<GoodsList> Goods_List()
        {
            List<GoodsList> ItemQm = new List<GoodsList>();
            List<int> GR_Vno = new List<int>();
            List<int> GI_Vno = new List<int>();
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd2 = "SELECT Goods_Voucher_No FROM I_Ledger where Voucher_Type = '1' and AccountRefNumber IS NULL GROUP BY Goods_Voucher_No HAVING COUNT(*)>0";
            SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
            SqlDataReader dr1 = SqlCmd2.ExecuteReader();
            while (dr1.Read())
            {
                GR_Vno.Add(Convert.ToInt32(dr1["Goods_Voucher_No"]));
            }
            dr1.Close();
            string cmd3 = "SELECT Goods_Voucher_No FROM I_Ledger where Voucher_Type = '2' and AccountRefNumber IS NULL GROUP BY Goods_Voucher_No HAVING COUNT(*)>0";
            SqlCommand SqlCmd3 = new SqlCommand(cmd3, Con);
            SqlDataReader dr2 = SqlCmd3.ExecuteReader();
            while (dr2.Read())
            {
                GI_Vno.Add(Convert.ToInt32(dr2["Goods_Voucher_No"]));
            }
            dr2.Close();
            int GR_vtype = 1;
            int GI_vtype = 2;
            if(GR_Vno.Count == 0)
            {
                for (int i = 0; i < GI_Vno.Count; i++)
                {
                    string cmd1 = "select Top 1 * from I_Ledger where Voucher_Type = '" + GI_vtype + "' and Goods_Voucher_No = '" + GI_Vno[i] + "' ORDER BY Voucher_Type Asc;";
                    SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
                    SqlDataReader dr = SqlCmd1.ExecuteReader();
                    while (dr.Read())
                    {
                        ItemQm.Add(new GoodsList
                        {
                            Voucher_Type = "Goods Issue",
                            G_Voucher_No = dr["Goods_Voucher_No"].ToString(),
                            G_Voucher_Date = dr["Goods_Voucher_Date"].ToString(),
                            Ref_No = dr["Ref_No"].ToString(),
                            Ref_Date = dr["Ref_Date"].ToString(),
                            GI_Tag = dr["GI_Tag"].ToString(),
                            Process = dr["Process_Tag"].ToString(),
                            Project = dr["Project_Tag"].ToString(),
                            Employee = dr["Employee_Tag"].ToString(),
                            Note = dr["Note"].ToString()
                        }
                        );
                    }
                    dr.Close();
                }
                Con.Close();
                return ItemQm;
            }
            else if (GI_Vno.Count == 0)
            {
                for (int i = 0; i < GR_Vno.Count; i++)
                {
                    string cmd1 = "select Top 1 * from I_Ledger where Voucher_Type = '" + GR_vtype + "' and Goods_Voucher_No = '" + GR_Vno[i] + "' ORDER BY Voucher_Type Asc;";
                    SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
                    SqlDataReader dr = SqlCmd1.ExecuteReader();
                    while (dr.Read())
                    {
                        ItemQm.Add(new GoodsList
                        {
                            Voucher_Type = "Goods Receipt",
                            G_Voucher_No = dr["Goods_Voucher_No"].ToString(),
                            G_Voucher_Date = dr["Goods_Voucher_Date"].ToString(),
                            Ref_No = dr["Ref_No"].ToString(),
                            Ref_Date = dr["Ref_Date"].ToString(),
                            GI_Tag = dr["GI_Tag"].ToString(),
                            Process = dr["Process_Tag"].ToString(),
                            Project = dr["Project_Tag"].ToString(),
                            Employee = dr["Employee_Tag"].ToString(),
                            Note = dr["Note"].ToString()
                        }
                        );
                    }
                    dr.Close();
                }
                Con.Close();
                return ItemQm;
            }
            else
            {
                for (int i = 0; i < GR_Vno.Count; i++)
                {
                    string cmd1 = "select Top 1 * from I_Ledger where Voucher_Type = '" + GR_vtype + "' and Goods_Voucher_No = '" + GR_Vno[i] + "' ORDER BY Voucher_Type Asc;";
                    SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
                    SqlDataReader dr = SqlCmd1.ExecuteReader();
                    while (dr.Read())
                    {
                        ItemQm.Add(new GoodsList
                        {
                            Voucher_Type = "Goods Receipt",
                            G_Voucher_No = dr["Goods_Voucher_No"].ToString(),
                            G_Voucher_Date = dr["Goods_Voucher_Date"].ToString(),
                            Ref_No = dr["Ref_No"].ToString(),
                            Ref_Date = dr["Ref_Date"].ToString(),
                            GI_Tag = dr["GI_Tag"].ToString(),
                            Process = dr["Process_Tag"].ToString(),
                            Project = dr["Project_Tag"].ToString(),
                            Employee = dr["Employee_Tag"].ToString(),
                            Note = dr["Note"].ToString()
                        }
                        );
                    }
                    dr.Close();
                }
                for (int i = 0; i < GI_Vno.Count; i++)
                {
                    string cmd1 = "select Top 1 * from I_Ledger where Voucher_Type = '" + GI_vtype + "' and Goods_Voucher_No = '" + GI_Vno[i] + "' ORDER BY Voucher_Type Asc;";
                    SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
                    SqlDataReader dr = SqlCmd1.ExecuteReader();
                    while (dr.Read())
                    {
                        ItemQm.Add(new GoodsList
                        {
                            Voucher_Type = "Goods Issue",
                            G_Voucher_No = dr["Goods_Voucher_No"].ToString(),
                            G_Voucher_Date = dr["Goods_Voucher_Date"].ToString(),
                            Ref_No = dr["Ref_No"].ToString(),
                            Ref_Date = dr["Ref_Date"].ToString(),
                            GI_Tag = dr["GI_Tag"].ToString(),
                            Process = dr["Process_Tag"].ToString(),
                            Project = dr["Project_Tag"].ToString(),
                            Employee = dr["Employee_Tag"].ToString(),
                            Note = dr["Note"].ToString()
                        }
                        );
                    }
                    dr.Close();
                }
                Con.Close();
                return ItemQm;
            }
        }
        public void Update_Close_Bal(string Part_No, int vtype, int vno)
        {
            SqlConnection Con1 = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con1.Open();
            List<GoodsList> ItemQm = new List<GoodsList>();
            string cmd2 = "Select P_code from Product_Master where P_Part_No = '" + Part_No + "'";
            SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con1);
            SqlDataReader dr = SqlCmd2.ExecuteReader(); 
            while (dr.Read())
            {
                ItemQm.Add(new GoodsList
                {
                    P_code = dr["P_code"].ToString()
                }
                );
            }
            dr.Close ();
            string pcode = string.Join("", ItemQm.Select(m => m.P_code));

            string cmd3 = "Select Purchase_Qty from I_Ledger where Voucher_Type = '"+vtype+"' and P_code = '" + pcode + "' and Goods_Voucher_No = '"+vno+"'";
            SqlCommand SqlCmd3 = new SqlCommand(cmd3, Con1);
            SqlDataReader dr1 = SqlCmd3.ExecuteReader();
            int qty = 0;
            while (dr1.Read())
            {
                string i = dr1["Purchase_Qty"].ToString();
                qty = int.Parse(i);
            }
            dr1.Close();

            if (vtype == 1)
            {
                string cmd1 = "Update Product_Master set P_Closing_Balance = P_Closing_Balance - '"+qty+"' where P_code = '" + pcode + "'";
                string cmd4 = "delete from I_Ledger where Voucher_Type = '" + vtype + "' and P_code = '" + pcode + "' and Goods_Voucher_No = '" + vno + "'";
                SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con1);
                SqlCmd1.ExecuteNonQuery();
                SqlCommand SqlCmd4 = new SqlCommand(cmd4, Con1);
                SqlCmd4.ExecuteNonQuery();
            }
            else
            {
                string cmd1 = "Update Product_Master set P_Closing_Balance = P_Closing_Balance + '" + qty + "' where P_code = '" + pcode + "'";
                string cmd4 = "delete from I_Ledger where Voucher_Type = '" + vtype + "' and P_code = '" + pcode + "' and Goods_Voucher_No = '" + vno + "'";
                SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con1);
                SqlCmd1.ExecuteNonQuery();
                SqlCommand SqlCmd4 = new SqlCommand(cmd4, Con1);
                SqlCmd4.ExecuteNonQuery();
            }
            Con1.Close();
        }
        public void Update_GoodsRI_json(List<GoodsRI> data)
        {
            SqlConnection Con1 = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con1.Open();
            for(int j = 0; j <= data.Count-1; j++)
            {
                string cmd3 = "select Purchase_Qty from I_Ledger where P_code = '" + data[j].P_code + "' and Voucher_Type = '"+data[j].Index_Type+"' and Goods_Voucher_No = '"+data[j].Voucher_No+"'";
                SqlCommand SqlCmd3 = new SqlCommand(cmd3, Con1);
                SqlDataReader dr2 = SqlCmd3.ExecuteReader();
                while (dr2.Read())
                {
                    data[j].I_Qty =(int) dr2["Purchase_Qty"];
                }
                dr2.Close();
                if(data[j].Index_Type == 1)
                {
                    string cmd2 = "update Product_Master set P_Closing_Balance = P_Closing_Balance - '"+data[j].I_Qty+"' where P_Part_No = '"+data[j].Part_No+"'";
                    SqlCommand SqlCmd1 = new SqlCommand(cmd2, Con1);
                    SqlCmd1.ExecuteNonQuery();
                }
                else
                {
                    string cmd2 = "update Product_Master set P_Closing_Balance = P_Closing_Balance + '" + data[j].I_Qty + "' where P_Part_No = '" + data[j].Part_No + "'";
                    SqlCommand SqlCmd1 = new SqlCommand(cmd2, Con1);
                    SqlCmd1.ExecuteNonQuery();
                }
            }
            string cmd = "delete from I_Ledger where Voucher_Type = '"+data[0].Index_Type+"' and Goods_Voucher_No = '"+data[0].Voucher_No+"'";
            SqlCommand Sqlcmd = new SqlCommand(cmd, Con1);
            Sqlcmd.ExecuteNonQuery();
            int i = 0;
            DateTime now = DateTime.Now;
            while (i < data.Count())
            {
                SqlCommand sql_cmnd = new SqlCommand("[dbo].[GoodsRI_Add]", Con1);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@v_type", SqlDbType.Int).Value = data[i].Index_Type;
                sql_cmnd.Parameters.AddWithValue("@voucher_no", SqlDbType.Int).Value = data[i].Voucher_No;
                sql_cmnd.Parameters.AddWithValue("@voucher_date", data[i].V_Date);
                sql_cmnd.Parameters.AddWithValue("@ref_no", SqlDbType.Int).Value = data[i].Ref_No;
                sql_cmnd.Parameters.AddWithValue("@ref_date", data[i].R_Date);
                sql_cmnd.Parameters.AddWithValue("@GI", SqlDbType.Int).Value = data[i].GI_Tag;
                sql_cmnd.Parameters.AddWithValue("@Process", SqlDbType.Int).Value = data[i].Process_Tag;
                sql_cmnd.Parameters.AddWithValue("@Project", SqlDbType.Int).Value = data[i].Project;
                sql_cmnd.Parameters.AddWithValue("@Employee", SqlDbType.Int).Value = data[i].Employee;
                sql_cmnd.Parameters.AddWithValue("@note", SqlDbType.NVarChar).Value = data[i].Note;
                sql_cmnd.Parameters.AddWithValue("@part_no", SqlDbType.NVarChar).Value = data[i].Part_No;
                sql_cmnd.Parameters.AddWithValue("@pcode", SqlDbType.NChar).Value = data[i].P_code;
                sql_cmnd.Parameters.AddWithValue("@qty", SqlDbType.Int).Value = data[i].Quantity;
                sql_cmnd.Parameters.AddWithValue("@time", SqlDbType.Time).Value = now.ToLongTimeString();
                sql_cmnd.ExecuteNonQuery();
                if (i == data.Count() - 1)
                {
                    break;
                }
                i++;
            }
            Con1.Close();
        }
        public List<GoodsRI> PM_list(string package, string value, string partno, string descp)
        {
            List<GoodsRI> ItemQm = new List<GoodsRI>();
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            if(package == null && value == null && partno != null || package == null && value == null && partno == null && descp == null)
            {
                string cmd1 = "SELECT * FROM Product_Master WHERE P_Part_No Like '%" + partno + "%'";
                SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
                SqlDataReader dr = SqlCmd1.ExecuteReader();
                while (dr.Read())
                {
                    string cost = dr["P_Cost"].ToString();
                    string mrp = dr["P_MRP"].ToString();
                    string qty = dr["P_Closing_Balance"].ToString();
                    ItemQm.Add(new GoodsRI
                    {
                        Part_No = dr["P_Part_No"].ToString(),
                        Description = dr["P_Description"].ToString(),
                        P_code = dr["P_code"].ToString(),
                        P_Cost = double.Parse(cost),
                        Package = dr["P_Package"].ToString(),
                        Value = dr["P_Value"].ToString(),
                        P_MRP = double.Parse(mrp),
                        Current_Stock = int.Parse(qty)
                    }
                    );
                }
                Con.Close();
                return ItemQm;
            }
            else if (descp != null)
            {
                string cmd1 = "SELECT * FROM Product_Master WHERE P_Description Like '%" + descp + "%'";
                SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
                SqlDataReader dr = SqlCmd1.ExecuteReader();
                while (dr.Read())
                {
                    string cost = dr["P_Cost"].ToString();
                    string mrp = dr["P_MRP"].ToString();
                    string qty = dr["P_Closing_Balance"].ToString();
                    ItemQm.Add(new GoodsRI
                    {
                        Part_No = dr["P_Part_No"].ToString(),
                        Description = dr["P_Description"].ToString(),
                        P_code = dr["P_code"].ToString(),
                        P_Cost = double.Parse(cost),
                        Package = dr["P_Package"].ToString(),
                        Value = dr["P_Value"].ToString(),
                        P_MRP = double.Parse(mrp),
                        Current_Stock = int.Parse(qty)
                    }
                    );
                }
                Con.Close();
                return ItemQm;
            }
            else if(package != null && value != null && partno == null && descp == null)
            {
                string cmd1 = "SELECT * FROM Product_Master WHERE P_Package LIKE '%" + package + "%' and P_Value Like '%" + value + "%'";
                SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
                SqlDataReader dr = SqlCmd1.ExecuteReader();
                while (dr.Read())
                {
                    string cost = dr["P_Cost"].ToString();
                    string mrp = dr["P_MRP"].ToString();
                    string qty = dr["P_Closing_Balance"].ToString();
                    ItemQm.Add(new GoodsRI
                    {
                        Part_No = dr["P_Part_No"].ToString(),
                        Description = dr["P_Description"].ToString(),
                        P_code = dr["P_code"].ToString(),
                        P_Cost = double.Parse(cost),
                        Package = dr["P_Package"].ToString(),
                        Value = dr["P_Value"].ToString(),
                        P_MRP = double.Parse(mrp),
                        Current_Stock = int.Parse(qty)
                    }
                    );
                }
                Con.Close();
                return ItemQm;
            }
            else if (package != null && value == null && partno == null && descp == null)
            {
                string cmd1 = "SELECT * FROM Product_Master WHERE P_Package LIKE '%" + package + "%'";
                SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
                SqlDataReader dr = SqlCmd1.ExecuteReader();
                while (dr.Read())
                {
                    string cost = dr["P_Cost"].ToString();
                    string mrp = dr["P_MRP"].ToString();
                    string qty = dr["P_Closing_Balance"].ToString();
                    ItemQm.Add(new GoodsRI
                    {
                        Part_No = dr["P_Part_No"].ToString(),
                        Description = dr["P_Description"].ToString(),
                        P_code = dr["P_code"].ToString(),
                        P_Cost = double.Parse(cost),
                        Package = dr["P_Package"].ToString(),
                        Value = dr["P_Value"].ToString(),
                        P_MRP = double.Parse(mrp),
                        Current_Stock = int.Parse(qty)
                    }
                    );
                }
                Con.Close();
                return ItemQm;
            }
            else if (value != null && partno == null && descp == null && package == null)
            {
                string cmd1 = "SELECT * FROM Product_Master WHERE P_Value Like '%" + value + "%'";
                SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
                SqlDataReader dr = SqlCmd1.ExecuteReader();
                while (dr.Read())
                {
                    string cost = dr["P_Cost"].ToString();
                    string mrp = dr["P_MRP"].ToString();
                    string qty = dr["P_Closing_Balance"].ToString();
                    ItemQm.Add(new GoodsRI
                    {
                        Part_No = dr["P_Part_No"].ToString(),
                        Description = dr["P_Description"].ToString(),
                        P_code = dr["P_code"].ToString(),
                        P_Cost = double.Parse(cost),
                        Package = dr["P_Package"].ToString(),
                        Value = dr["P_Value"].ToString(),
                        P_MRP = double.Parse(mrp),
                        Current_Stock = int.Parse(qty)
                    }
                    );
                }
                Con.Close();
                return ItemQm;
            }
            else
            {
                string cmd1 = "SELECT * FROM Product_Master WHERE P_Package LIKE '%" + package + "%' and P_Value Like '%" + value + "%' and P_Part_No Like '%"+partno+"%'";
                SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
                SqlDataReader dr = SqlCmd1.ExecuteReader();
                while (dr.Read())
                {
                    string cost = dr["P_Cost"].ToString();
                    string mrp = dr["P_MRP"].ToString();
                    string qty = dr["P_Closing_Balance"].ToString();
                    ItemQm.Add(new GoodsRI
                    {
                        Part_No = dr["P_Part_No"].ToString(),
                        Description = dr["P_Description"].ToString(),
                        P_code = dr["P_code"].ToString(),
                        P_Cost = double.Parse(cost),
                        Package = dr["P_Package"].ToString(),
                        Value = dr["P_Value"].ToString(),
                        P_MRP = double.Parse(mrp),
                        Current_Stock = int.Parse(qty)
                    }
                    );
                }
                Con.Close();
                return ItemQm;
            }

        }
        public List<GoodsRI> Preview_List(int vtype, int vno)
        {
            List<GoodsRI> ItemQm = new List<GoodsRI>();
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "select P_code,Purchase_Qty from I_Ledger where Voucher_Type = " + vtype + " and Goods_Voucher_No = " + vno + "";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlDataReader dr = SqlCmd1.ExecuteReader();
            while (dr.Read())
            {
                ItemQm.Add(new GoodsRI
                {
                    Quantity = (int)dr["P_Quantity"],
                    P_code = dr["P_code"].ToString()
                }
                );
            }
            dr.Close();
            for (int i = 0; i < ItemQm.Count; i++)
            {
                string cmd2 = "select * from Product_Master where P_code = '"+ItemQm[i].P_code+"'";
                SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
                SqlDataReader dr1 = SqlCmd2.ExecuteReader();
                while (dr1.Read())
                {
                    ItemQm.Add(new GoodsRI
                    {
                        Part_No = dr1["P_Part_No"].ToString(),
                        Description = dr1["P_Description"].ToString()

                    }
                    );
                }
                dr1.Close();
            }
            Con.Close();
            return ItemQm;
        }
    }
    
}