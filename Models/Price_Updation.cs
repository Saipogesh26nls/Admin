using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Admin.Models
{
    public class Price_Updation
    {
        public void AddPrice(string Part_No, double P_Cost, double P_Price_USD, double P_MRP, double P_SP, int stock)
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            if(P_Cost >= 0)
            {
                string cmd1 = "update Product_Master set P_Cost = '" + P_Cost + "' where P_Part_No = '"+Part_No+"'";
                SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
                SqlCmd1.ExecuteNonQuery();
            }
            if (P_Price_USD >= 0)
            {
                string cmd1 = "update Product_Master set P_Price(USD) = '" + P_Price_USD + "' where P_Part_No = '" + Part_No + "'";
                SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
                SqlCmd1.ExecuteNonQuery();
            }
            if (P_MRP > 0)
            {
                string cmd1 = "update Product_Master set P_MRP = '" + P_MRP + "' where P_Part_No = '" + Part_No + "'";
                SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
                SqlCmd1.ExecuteNonQuery();
            }
            if (P_SP > 0)
            {
                string cmd1 = "update Product_Master set P_SP = '" + P_SP + "' where P_Part_No = '" + Part_No + "'";
                SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
                SqlCmd1.ExecuteNonQuery();
            }

            /*SqlCommand sql_cmnd = new SqlCommand("[dbo].[addPrice]", Con);
            sql_cmnd.CommandType = CommandType.StoredProcedure;
            sql_cmnd.Parameters.AddWithValue("@PartNo", SqlDbType.NVarChar).Value = Part_No.ToUpper();
            sql_cmnd.Parameters.AddWithValue("@p_cost", SqlDbType.Money).Value = P_Cost;
            sql_cmnd.Parameters.AddWithValue("@p_price_USD", SqlDbType.Money).Value = P_Price_USD;
            sql_cmnd.Parameters.AddWithValue("@p_mrp", SqlDbType.Money).Value = P_MRP;
            sql_cmnd.Parameters.AddWithValue("@p_sp", SqlDbType.Money).Value = P_SP;
            sql_cmnd.Parameters.AddWithValue("@stock", SqlDbType.Money).Value = stock;
            int var1;
            var1 = sql_cmnd.ExecuteNonQuery();*/
            Con.Close();
        }
        public List<Inventory_Table> Inventory_List(string partno, string project, string process, string user)
        {
            List<Inventory_Table> ItemQm = new List<Inventory_Table>();
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            if (partno != null)
            {
                string cmd1 = "SELECT P_code FROM Product_Master WHERE P_Part_No = '"+partno+"'";
                SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
                SqlDataReader dr = SqlCmd1.ExecuteReader();
                string pcode = "";
                while (dr.Read())
                {
                    pcode = dr["P_code"].ToString();
                }
                dr.Close();
                string cmd2 = "SELECT * FROM I_Ledger WHERE P_code = '" + pcode + "'";
                SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
                SqlDataReader dr1 = SqlCmd2.ExecuteReader();
                while (dr1.Read())
                {
                    if (dr1["Voucher_Type"].ToString() == "1")
                    {
                        ItemQm.Add(new Inventory_Table
                        {
                            Date = dr1["Goods_Voucher_Date"].ToString(),
                            VoucherNo = dr1["Goods_Voucher_No"].ToString(),
                            Project = dr1["Project_Tag"].ToString(),
                            Process = dr1["Process_Tag"].ToString(),
                            User = dr1["Employee_Tag"].ToString(),
                            Quantity = (int)dr1["Purchase_Qty"],
                            VoucherType = "Goods Receipt"
                        }
                        );
                    }
                    else{
                        ItemQm.Add(new Inventory_Table
                        {
                            Date = dr1["Goods_Voucher_Date"].ToString(),
                            VoucherNo = dr1["Goods_Voucher_No"].ToString(),
                            Project = dr1["Project_Tag"].ToString(),
                            Process = dr1["Process_Tag"].ToString(),
                            User = dr1["Employee_Tag"].ToString(),
                            Quantity = (int)dr1["Purchase_Qty"],
                            VoucherType = "Goods Issue"
                        }
                        );
                    }

                }
                dr1.Close();
                for (int i = 0; i < ItemQm.Count; i++)
                {
                    string cmd3 = "select Project_Name from Project_Master where Project_Id = '" + int.Parse(ItemQm[i].Project) + "'";
                    SqlCommand SqlCmd3 = new SqlCommand(cmd3, Con);
                    SqlDataReader dr2 = SqlCmd3.ExecuteReader();
                    while (dr2.Read())
                    {
                        ItemQm[i].Project = dr2["Project_Name"].ToString();
                    }
                    dr2.Close();
                    string cmd4 = "select Process_Name from Process_Tag where Process_Id = '" + int.Parse(ItemQm[i].Process) + "'";
                    SqlCommand SqlCmd4 = new SqlCommand(cmd4, Con);
                    SqlDataReader dr4 = SqlCmd4.ExecuteReader();
                    while (dr4.Read())
                    {
                        ItemQm[i].Process = dr4["Process_Name"].ToString();
                    }
                    dr4.Close();
                    string cmd5 = "select Employee_Name from Employee_Master where Id = '" + int.Parse(ItemQm[i].User) + "'";
                    SqlCommand SqlCmd5 = new SqlCommand(cmd5, Con);
                    SqlDataReader dr5 = SqlCmd5.ExecuteReader();
                    while (dr5.Read())
                    {
                        ItemQm[i].User = dr5["Employee_Name"].ToString();
                    }
                    dr5.Close();
                }
                Con.Close();
                return ItemQm;
            }
            else if(project != null)
            {
                string cmd1 = "SELECT * FROM I_Ledger WHERE Project_Tag = '" + int.Parse(project) + "'";
                SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
                SqlDataReader dr = SqlCmd1.ExecuteReader();
                while (dr.Read())
                {
                    if (dr["Voucher_Type"].ToString() == "1")
                    {
                        ItemQm.Add(new Inventory_Table
                        {
                            Date = dr["Goods_Voucher_Date"].ToString(),
                            VoucherNo = dr["Goods_Voucher_No"].ToString(),
                            Project = dr["Project_Tag"].ToString(),
                            Process = dr["Process_Tag"].ToString(),
                            User = dr["Employee_Tag"].ToString(),
                            GoodsIssue = "-",
                            pcode = dr["P_code"].ToString(),
                            GoodsReceipt = dr["Purchase_Qty"].ToString()
                        }
                        );
                    }
                    else
                    {
                        ItemQm.Add(new Inventory_Table
                        {
                            Date = dr["Goods_Voucher_Date"].ToString(),
                            VoucherNo = dr["Goods_Voucher_No"].ToString(),
                            Project = dr["Project_Tag"].ToString(),
                            Process = dr["Process_Tag"].ToString(),
                            User = dr["Employee_Tag"].ToString(),
                            GoodsReceipt = "-",
                            pcode = dr["P_code"].ToString(),
                            GoodsIssue = dr["Purchase_Qty"].ToString()
                        }
                        );
                    }
                }
                dr.Close();
                for (int i = 0; i < ItemQm.Count; i++)
                {
                    string cmd2 = "select P_Part_No, P_Description from Product_Master where P_code = '" + ItemQm[i].pcode + "'";
                    SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
                    SqlDataReader dr1 = SqlCmd2.ExecuteReader();
                    while (dr1.Read())
                    {
                        ItemQm[i].PartNo = dr1["P_Part_No"].ToString();
                        ItemQm[i].Description = dr1["P_Description"].ToString();
                    }
                    dr1.Close();
                    string cmd3 = "select Project_Name from Project_Master where Project_Id = '" + int.Parse(ItemQm[i].Project) + "'";
                    SqlCommand SqlCmd3 = new SqlCommand(cmd3, Con);
                    SqlDataReader dr2 = SqlCmd3.ExecuteReader();
                    while (dr2.Read())
                    {
                        ItemQm[i].Project = dr2["Project_Name"].ToString();
                    }
                    dr2.Close();
                    string cmd4 = "select Process_Name from Process_Tag where Process_Id = '" + int.Parse(ItemQm[i].Process) + "'";
                    SqlCommand SqlCmd4 = new SqlCommand(cmd4, Con);
                    SqlDataReader dr4 = SqlCmd4.ExecuteReader();
                    while (dr4.Read())
                    {
                        ItemQm[i].Process = dr4["Process_Name"].ToString();
                    }
                    dr4.Close();
                    string cmd5 = "select Employee_Name from Employee_Master where Id = '" + int.Parse(ItemQm[i].User) + "'";
                    SqlCommand SqlCmd5 = new SqlCommand(cmd5, Con);
                    SqlDataReader dr5 = SqlCmd5.ExecuteReader();
                    while (dr5.Read())
                    {
                        ItemQm[i].User = dr5["Employee_Name"].ToString();
                    }
                    dr5.Close();
                }
                Con.Close();
                return ItemQm;
            }
            else if (process != null)
            {
                string cmd1 = "SELECT * FROM I_Ledger WHERE Process_Tag = '" + int.Parse(process) + "'";
                SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
                SqlDataReader dr = SqlCmd1.ExecuteReader();
                while (dr.Read())
                {
                    if (dr["Voucher_Type"].ToString() == "1")
                    {
                        ItemQm.Add(new Inventory_Table
                        {
                            Date = dr["Goods_Voucher_Date"].ToString(),
                            VoucherNo = dr["Goods_Voucher_No"].ToString(),
                            Project = dr["Project_Tag"].ToString(),
                            Process = dr["Process_Tag"].ToString(),
                            User = dr["Employee_Tag"].ToString(),
                            GoodsIssue = "-",
                            pcode = dr["P_code"].ToString(),
                            GoodsReceipt = dr["Purchase_Qty"].ToString()
                        }
                        );
                    }
                    else
                    {
                        ItemQm.Add(new Inventory_Table
                        {
                            Date = dr["Goods_Voucher_Date"].ToString(),
                            VoucherNo = dr["Goods_Voucher_No"].ToString(),
                            Project = dr["Project_Tag"].ToString(),
                            Process = dr["Process_Tag"].ToString(),
                            User = dr["Employee_Tag"].ToString(),
                            GoodsReceipt = "-",
                            pcode = dr["P_code"].ToString(),
                            GoodsIssue = dr["Purchase_Qty"].ToString()
                        }
                        );
                    }
                }
                dr.Close();
                for (int i = 0; i < ItemQm.Count; i++)
                {
                    string cmd2 = "select P_Part_No, P_Description from Product_Master where P_code = '" + ItemQm[i].pcode + "'";
                    SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
                    SqlDataReader dr1 = SqlCmd2.ExecuteReader();
                    while (dr1.Read())
                    {
                        ItemQm[i].PartNo = dr1["P_Part_No"].ToString();
                        ItemQm[i].Description = dr1["P_Description"].ToString();
                    }
                    dr1.Close();
                    string cmd3 = "select Project_Name from Project_Master where Project_Id = '" + int.Parse(ItemQm[i].Project) + "'";
                    SqlCommand SqlCmd3 = new SqlCommand(cmd3, Con);
                    SqlDataReader dr2 = SqlCmd3.ExecuteReader();
                    while (dr2.Read())
                    {
                        ItemQm[i].Project = dr2["Project_Name"].ToString();
                    }
                    dr2.Close();
                    string cmd4 = "select Process_Name from Process_Tag where Process_Id = '" + int.Parse(ItemQm[i].Process) + "'";
                    SqlCommand SqlCmd4 = new SqlCommand(cmd4, Con);
                    SqlDataReader dr4 = SqlCmd4.ExecuteReader();
                    while (dr4.Read())
                    {
                        ItemQm[i].Process = dr4["Process_Name"].ToString();
                    }
                    dr4.Close();
                    string cmd5 = "select Employee_Name from Employee_Master where Id = '" + int.Parse(ItemQm[i].User) + "'";
                    SqlCommand SqlCmd5 = new SqlCommand(cmd5, Con);
                    SqlDataReader dr5 = SqlCmd5.ExecuteReader();
                    while (dr5.Read())
                    {
                        ItemQm[i].User = dr5["Employee_Name"].ToString();
                    }
                    dr5.Close();
                }
                Con.Close();
                return ItemQm;
            }
            else
            {
                string cmd1 = "SELECT * FROM I_Ledger WHERE Employee_Tag = '" + int.Parse(user) + "'";
                SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
                SqlDataReader dr = SqlCmd1.ExecuteReader();
                while (dr.Read())
                {
                    if (dr["Voucher_Type"].ToString() == "1")
                    {
                        ItemQm.Add(new Inventory_Table
                        {
                            Date = dr["Goods_Voucher_Date"].ToString(),
                            VoucherNo = dr["Goods_Voucher_No"].ToString(),
                            Project = dr["Project_Tag"].ToString(),
                            Process = dr["Process_Tag"].ToString(),
                            User = dr["Employee_Tag"].ToString(),
                            GoodsIssue = "-",
                            pcode = dr["P_code"].ToString(),
                            GoodsReceipt = dr["Purchase_Qty"].ToString()
                        }
                        );
                    }
                    else
                    {
                        ItemQm.Add(new Inventory_Table
                        {
                            Date = dr["Goods_Voucher_Date"].ToString(),
                            VoucherNo = dr["Goods_Voucher_No"].ToString(),
                            Project = dr["Project_Tag"].ToString(),
                            Process = dr["Process_Tag"].ToString(),
                            User = dr["Employee_Tag"].ToString(),
                            GoodsReceipt = "-",
                            pcode = dr["P_code"].ToString(),
                            GoodsIssue = dr["Purchase_Qty"].ToString()
                        }
                        );
                    }
                }
                dr.Close();
                for (int i = 0; i < ItemQm.Count; i++)
                {
                    string cmd2 = "select P_Part_No, P_Description from Product_Master where P_code = '" + ItemQm[i].pcode + "'";
                    SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
                    SqlDataReader dr1 = SqlCmd2.ExecuteReader();
                    while (dr1.Read())
                    {
                        ItemQm[i].PartNo = dr1["P_Part_No"].ToString();
                        ItemQm[i].Description = dr1["P_Description"].ToString();
                    }
                    dr1.Close();
                    string cmd3 = "select Project_Name from Project_Master where Project_Id = '" + int.Parse(ItemQm[i].Project) + "'";
                    SqlCommand SqlCmd3 = new SqlCommand(cmd3, Con);
                    SqlDataReader dr2 = SqlCmd3.ExecuteReader();
                    while (dr2.Read())
                    {
                        ItemQm[i].Project = dr2["Project_Name"].ToString();
                    }
                    dr2.Close();
                    string cmd4 = "select Process_Name from Process_Tag where Process_Id = '" + int.Parse(ItemQm[i].Process) + "'";
                    SqlCommand SqlCmd4 = new SqlCommand(cmd4, Con);
                    SqlDataReader dr4 = SqlCmd4.ExecuteReader();
                    while (dr4.Read())
                    {
                        ItemQm[i].Process = dr4["Process_Name"].ToString();
                    }
                    dr4.Close();
                    string cmd5 = "select Employee_Name from Employee_Master where Id = '" + int.Parse(ItemQm[i].User) + "'";
                    SqlCommand SqlCmd5 = new SqlCommand(cmd5, Con);
                    SqlDataReader dr5 = SqlCmd5.ExecuteReader();
                    while (dr5.Read())
                    {
                        ItemQm[i].User = dr5["Employee_Name"].ToString();
                    }
                    dr5.Close();
                }
                Con.Close();
                return ItemQm;
            }
        }
        public List<Stockstatement> Get_stocks()
        {
            List<Stockstatement> ItemQm = new List<Stockstatement>();
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "SELECT * FROM Product_Master where P_Level <0";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlDataReader dr = SqlCmd1.ExecuteReader();
            while (dr.Read())
            {
                string cost = dr["P_Cost"].ToString();
                string mrp = dr["P_MRP"].ToString();
                string qty = dr["P_Closing_Balance"].ToString();
                ItemQm.Add(new Stockstatement
                {
                    PartNo = dr["P_Part_No"].ToString(),
                    Description = dr["P_Description"].ToString(),
                    Price = double.Parse(cost),
                    Package = dr["P_Package"].ToString(),
                    Value = dr["P_Value"].ToString(),
                    Stock = int.Parse(qty),
                    ProductCode = dr["P_code"].ToString()
                }
                );
            }
            Con.Close();
            return ItemQm;
        }
        public List<Inventory_Table> Receipt_Data(string partno)
        {
            List<Inventory_Table> ItemQm = new List<Inventory_Table>();
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "SELECT P_code FROM Product_Master WHERE P_Part_No = '" + partno + "'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlDataReader dr = SqlCmd1.ExecuteReader();
            string pcode = "";
            while (dr.Read())
            {
                pcode = dr["P_code"].ToString();
            }
            dr.Close();
            string cmd2 = "SELECT * FROM I_Ledger WHERE P_code = '" + pcode + "'";
            SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
            SqlDataReader dr1 = SqlCmd2.ExecuteReader();
            while (dr1.Read())
            {
                if (dr1["Voucher_Type"].ToString() == "1")
                {
                    ItemQm.Add(new Inventory_Table
                    {
                        Date = dr1["Goods_Voucher_Date"].ToString(),
                        VoucherNo = dr1["Goods_Voucher_No"].ToString(),
                        Project = dr1["Project_Tag"].ToString(),
                        Process = dr1["Process_Tag"].ToString(),
                        User = dr1["Employee_Tag"].ToString(),
                        Quantity = (int)dr1["Purchase_Qty"],
                        VoucherType = "Goods Receipt"
                    }
                    );
                }
                else
                {
                    ItemQm.Add(new Inventory_Table
                    {
                        Date = dr1["Goods_Voucher_Date"].ToString(),
                        VoucherNo = dr1["Goods_Voucher_No"].ToString(),
                        Project = dr1["Project_Tag"].ToString(),
                        Process = dr1["Process_Tag"].ToString(),
                        User = dr1["Employee_Tag"].ToString(),
                        Quantity = (int)dr1["Purchase_Qty"],
                        VoucherType = "Goods Issue"
                    }
                    );
                }

            }
            dr1.Close();
            for (int i = 0; i < ItemQm.Count; i++)
            {
                string cmd3 = "select Project_Name from Project_Master where Project_Id = '" + int.Parse(ItemQm[i].Project) + "'";
                SqlCommand SqlCmd3 = new SqlCommand(cmd3, Con);
                SqlDataReader dr2 = SqlCmd3.ExecuteReader();
                while (dr2.Read())
                {
                    ItemQm[i].Project = dr2["Project_Name"].ToString();
                }
                dr2.Close();
                string cmd4 = "select Process_Name from Process_Tag where Process_Id = '" + int.Parse(ItemQm[i].Process) + "'";
                SqlCommand SqlCmd4 = new SqlCommand(cmd4, Con);
                SqlDataReader dr4 = SqlCmd4.ExecuteReader();
                while (dr4.Read())
                {
                    ItemQm[i].Process = dr4["Process_Name"].ToString();
                }
                dr4.Close();
                string cmd5 = "select Employee_Name from Employee_Master where Id = '" + int.Parse(ItemQm[i].User) + "'";
                SqlCommand SqlCmd5 = new SqlCommand(cmd5, Con);
                SqlDataReader dr5 = SqlCmd5.ExecuteReader();
                while (dr5.Read())
                {
                    ItemQm[i].User = dr5["Employee_Name"].ToString();
                }
                dr5.Close();
            }
            Con.Close();
            return ItemQm;
        }
    }
}