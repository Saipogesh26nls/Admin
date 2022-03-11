using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Admin.Models;
using System.Configuration;
using Newtonsoft.Json;

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
        public int Add_Data(List<PurchaseTable> data, int Qty, double total, double final_total, double final_discount, double final_tax1, double final_tax2)
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
                sql_cmnd.Parameters.AddWithValue("@i_rate", SqlDbType.Money).Value = data[i].Price;
                sql_cmnd.Parameters.AddWithValue("@i_subtotal", SqlDbType.Money).Value = data[i].SubTotal;
                sql_cmnd.Parameters.AddWithValue("@i_discount", SqlDbType.Decimal).Value = data[i].Dis_Rs;
                sql_cmnd.Parameters.AddWithValue("@i_tax1", SqlDbType.Money).Value = data[i].Tax1_Rs;
                sql_cmnd.Parameters.AddWithValue("@i_tax2", SqlDbType.Money).Value = data[i].Tax2_Rs;
                sql_cmnd.Parameters.AddWithValue("@i_total", SqlDbType.Money).Value = data[i].Total;
                sql_cmnd.Parameters.AddWithValue("@Total", SqlDbType.Money).Value = total;
                sql_cmnd.Parameters.AddWithValue("@Total_Qty", SqlDbType.Money).Value = Qty;
                sql_cmnd.Parameters.AddWithValue("@Final_Total", SqlDbType.Money).Value = final_total;
                sql_cmnd.Parameters.AddWithValue("@Final_Discount", SqlDbType.Money).Value = final_discount;
                sql_cmnd.Parameters.AddWithValue("@Final_Tax1", SqlDbType.Money).Value = final_tax1;
                sql_cmnd.Parameters.AddWithValue("@Final_Tax2", SqlDbType.Money).Value = final_tax2;
                sql_cmnd.Parameters.AddWithValue("@acc_name", SqlDbType.NVarChar).Value = data[i].supplier;
                sql_cmnd.Parameters.AddWithValue("@goodsissue", SqlDbType.NVarChar).Value = null;
                sql_cmnd.Parameters.AddWithValue("@ARefNo", SqlDbType.NVarChar).Value = ARefNo;

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
                    string cmd1 = "select Top 1 Invoice_No,Invoice_Date,Voucher_No,Voucher_Date,A_code from Purchase where Voucher_No = '" + vno[i] + "' ORDER BY Voucher_No Asc;";
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
            return ItemQm;
        }
        public void Edit_and_Delete(List<PurchaseTable> data, int final_Qty, double final_subtotal, double final_discount, double final_tax1, double final_tax2, double final_total)
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
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
                sql_cmnd.Parameters.AddWithValue("@i_discount", SqlDbType.Decimal).Value = data[i].Dis_Rs;
                sql_cmnd.Parameters.AddWithValue("@i_tax1", SqlDbType.Money).Value = data[i].Tax1_Rs;
                sql_cmnd.Parameters.AddWithValue("@i_tax2", SqlDbType.Money).Value = data[i].Tax2_Rs;
                sql_cmnd.Parameters.AddWithValue("@i_total", SqlDbType.Money).Value = data[i].Total;
                sql_cmnd.Parameters.AddWithValue("@invoice_no", SqlDbType.NVarChar).Value = data[i].Invoice_No;
                sql_cmnd.Parameters.AddWithValue("@invoice_date", data[i].Invoice_Date);
                sql_cmnd.Parameters.AddWithValue("@voucher_no", SqlDbType.Int).Value = data[i].Voucher_No;
                sql_cmnd.Parameters.AddWithValue("@voucher_date", data[i].Voucher_Date);
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
                sql_cmnd.Parameters.AddWithValue("@ARefNo", SqlDbType.NVarChar).Value = ARefNo;

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
                return null;
            }
        }
    }
    public class Goods_RI
    {
        public int Goods_add(List<GoodsRI> data)
        {
            SqlConnection Con1 = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con1.Open();
            int GR_V_No = 0;
            if (data[0].Index_Type == "1")
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
            int i = 0;
            while (i < data.Count())
            {
                SqlCommand sql_cmnd = new SqlCommand("[dbo].[GoodsReceiptIssue]", Con1);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@index_type", SqlDbType.NVarChar).Value = data[i].Index_Type;
                sql_cmnd.Parameters.AddWithValue("@voucher_no", SqlDbType.Int).Value = GR_V_No;
                sql_cmnd.Parameters.AddWithValue("@voucher_date", data[i].Voucher_Date);
                sql_cmnd.Parameters.AddWithValue("@ref_no", SqlDbType.Int).Value = data[i].Ref_No;
                sql_cmnd.Parameters.AddWithValue("@ref_date", data[i].Ref_Date);
                sql_cmnd.Parameters.AddWithValue("@GI", SqlDbType.Int).Value = data[i].GI_Tag;
                sql_cmnd.Parameters.AddWithValue("@Process", SqlDbType.Int).Value = data[i].Process_Tag;
                sql_cmnd.Parameters.AddWithValue("@Project", SqlDbType.Int).Value = data[i].Project;
                sql_cmnd.Parameters.AddWithValue("@Employee", SqlDbType.Int).Value = data[i].Employee;
                sql_cmnd.Parameters.AddWithValue("@note", SqlDbType.NVarChar).Value = data[i].Note;
                sql_cmnd.Parameters.AddWithValue("@part_no", SqlDbType.NVarChar).Value = data[i].Part_No;
                sql_cmnd.Parameters.AddWithValue("@qty", SqlDbType.Int).Value = data[i].Quantity;
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
            return ItemQm;
        }
        public int json_test_add(List<GoodsRI> data)
        {
            SqlConnection Con1 = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con1.Open();
            int GR_V_No = 0;
            string vtype = data[0].Index_Type;
            if (data[0].Index_Type == "1")
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
                    data[i].Part_No = dr2["P_code"].ToString();
                }
                data[i].v_no = GR_V_No;
                dr2.Close();
            }
            var json = JsonConvert.SerializeObject(data);
            SqlCommand sql_cmnd = new SqlCommand("[dbo].[json_test]", Con1);
            sql_cmnd.CommandType = CommandType.StoredProcedure;
            sql_cmnd.Parameters.AddWithValue("@json", json);
            sql_cmnd.Parameters.AddWithValue("@vtype", vtype);
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
                            Voucher_Type = dr["Voucher_Type"].ToString(),
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
                            Voucher_Type = dr["Voucher_Type"].ToString(),
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
                            Voucher_Type = dr["Voucher_Type"].ToString(),
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
                            Voucher_Type = dr["Voucher_Type"].ToString(),
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
        public void Update_Close_Bal(string Part_No, string vtype)
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
            if (vtype == "1")
            {
                string cmd1 = "Update Product_Master set P_Closing_Balance = P_Closing_Balance - P_Open_Balance where P_code = '" + pcode + "'";
                SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con1);
                SqlCmd1.ExecuteNonQuery();
            }
            else
            {
                string cmd1 = "Update Product_Master set P_Closing_Balance = P_Closing_Balance + P_Open_Balance where P_code = '" + pcode + "'";
                SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con1);
                SqlCmd1.ExecuteNonQuery();
            }
        }
        public void Update_GoodsRI_json(List<GoodsRI> data)
        {
            SqlConnection Con1 = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con1.Open();
            string vtype = data[0].Index_Type;
            int vno = Convert.ToInt32(data[0].Voucher_No);
            string cmd1 = "delete from I_Ledger where Voucher_Type ='" + vtype + "' and Goods_Voucher_No = '"+vno+"'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con1);
            SqlCmd1.ExecuteNonQuery();

            for (int i = 0; i < data.Count(); i++)
            {
                string cmd3 = "select P_code from Product_Master where P_Part_No = '" + data[i].Part_No + "'";
                SqlCommand SqlCmd3 = new SqlCommand(cmd3, Con1);
                SqlDataReader dr2 = SqlCmd3.ExecuteReader();
                while (dr2.Read())
                {
                    data[i].Part_No = dr2["P_code"].ToString();
                }
                data[i].v_no = vno;
                dr2.Close();
                if(vtype == "1")
                {
                    string cmd2 = "Update Product_Master set P_Closing_Balance = P_Closing_Balance - P_Open_Balance where P_code = '"+data[i].Part_No+"'";
                    SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con1);
                    SqlCmd2.ExecuteNonQuery();
                }
                else
                {
                    string cmd2 = "Update Product_Master set P_Closing_Balance = P_Closing_Balance + P_Open_Balance where P_code = '" + data[i].Part_No + "'";
                    SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con1);
                    SqlCmd2.ExecuteNonQuery();
                }
            }
            var json = JsonConvert.SerializeObject(data);
            SqlCommand sql_cmnd = new SqlCommand("[dbo].[json_test]", Con1);
            sql_cmnd.CommandType = CommandType.StoredProcedure;
            sql_cmnd.Parameters.AddWithValue("@json", json);
            sql_cmnd.Parameters.AddWithValue("@vtype", vtype);
            sql_cmnd.ExecuteReader();
            Con1.Close();
        }
    }
}