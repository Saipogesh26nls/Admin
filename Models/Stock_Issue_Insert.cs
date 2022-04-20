using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Admin.Models
{
    public class Stock_Issue_Insert
    {
        public void StockIssue_add(List<Issue> data)
        {
            SqlConnection Con1 = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con1.Open();
            int i = 0;
            while (i < data.Count())
            {
                SqlCommand sql_cmnd = new SqlCommand("[dbo].[StockIndent]", Con1);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@ref_no", SqlDbType.Int).Value = data[0].IndentNo;
                sql_cmnd.Parameters.AddWithValue("@ref_date", data[i].IndentDate);
                sql_cmnd.Parameters.AddWithValue("@GI", SqlDbType.Int).Value = data[i].Reason_int;
                sql_cmnd.Parameters.AddWithValue("@GI_name", SqlDbType.NVarChar).Value = data[i].Reason;
                sql_cmnd.Parameters.AddWithValue("@Process", SqlDbType.Int).Value = data[i].Process_int;
                sql_cmnd.Parameters.AddWithValue("@Process_name", SqlDbType.NVarChar).Value = data[i].Process;
                sql_cmnd.Parameters.AddWithValue("@Project", SqlDbType.Int).Value = data[i].Project_int;
                sql_cmnd.Parameters.AddWithValue("@Project_name", SqlDbType.NVarChar).Value = data[i].Project;
                sql_cmnd.Parameters.AddWithValue("@Employee", SqlDbType.Int).Value = data[i].Request_int;
                sql_cmnd.Parameters.AddWithValue("@Employee_name", SqlDbType.NVarChar).Value = data[i].RequestBy;
                sql_cmnd.Parameters.AddWithValue("@note", SqlDbType.NVarChar).Value = data[i].Note;
                sql_cmnd.Parameters.AddWithValue("@part_no", SqlDbType.NVarChar).Value = data[i].PartNo;
                sql_cmnd.Parameters.AddWithValue("@descp", SqlDbType.NChar).Value = data[i].Description;
                sql_cmnd.Parameters.AddWithValue("@qty", SqlDbType.Int).Value = data[i].Quantity;
                sql_cmnd.Parameters.AddWithValue("@vno", SqlDbType.Int).Value = 0;
                sql_cmnd.ExecuteNonQuery();
                if (i == data.Count() - 1)
                {
                    break;
                }
                i++;
            }
            Con1.Close();
        }
        public GoodsRI Issue_Descp_Qty(string part_no)
        {
            GoodsRI ItemQm = new GoodsRI();
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "select P_Closing_Balance,P_Description,P_code from Product_Master where P_Part_No = '" + part_no + "'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlDataReader dr = SqlCmd1.ExecuteReader();
            while (dr.Read())
            {
                ItemQm.Description = dr["P_Description"].ToString();
                ItemQm.Quantity = (int)dr["P_Closing_Balance"];
                ItemQm.P_code = dr["P_code"].ToString();
            }
            Con.Close();
            return ItemQm;
        }
        public GoodsRI Issue_Descp(string part_no)
        {
            GoodsRI ItemQm = new GoodsRI();
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "select P_Closing_Balance,P_Description,P_code from Product_Master where P_Part_No = '" + part_no + "'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlDataReader dr = SqlCmd1.ExecuteReader();
            while (dr.Read())
            {
                ItemQm.Description = dr["P_Description"].ToString();
                ItemQm.db_qty = (int)dr["P_Closing_Balance"];
            }
            Con.Close();
            return ItemQm;
        }
        public string partno_to_pcode(string part_no)
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "select P_code from Product_Master where P_Part_No = '" + part_no + "'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlDataReader dr = SqlCmd1.ExecuteReader();
            string pcode = "";
            while (dr.Read())
            {
                pcode = dr["P_code"].ToString();
            }
            Con.Close();
            return pcode;
        }
        public int Goods_add(List<GoodsRI> data, string vdate, string rdate)
        {
            if(data[0].Voucher_No > 0)
            {
                SqlConnection Con1 = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
                Con1.Open();
                for (int j = 0; j < data.Count(); j++)
                {
                    string cmd1 = "Update I_Ledger set Purchase_Qty = Purchase_Qty + "+data[j].Quantity+" where P_code = "+data[j].P_code+" and Goods_Voucher_No = "+data[j].Voucher_No+" and Voucher_Type = "+2+"";
                    string cmd2 = "Update Product_Master set P_Closing_Balance = P_Closing_Balance - "+data[j].Quantity+" where P_code = "+data[j].P_code+"";
                    SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con1);
                    SqlCmd1.ExecuteNonQuery();
                    SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con1);
                    SqlCmd2.ExecuteNonQuery();
                    string cmd3 = "Update Stock_Indent set Quantity = Quantity - "+data[j].Quantity+ " where IndentNo = '" + data[j].Ref_No + "' and PartNo = '" + data[j].Part_No + "' ";
                    SqlCommand SqlCmd3 = new SqlCommand(cmd3, Con1);
                    SqlCmd3.ExecuteNonQuery();
                    string cmd5 = "select Quantity from Stock_Indent where IndentNo = '" + data[j].Ref_No + "' and PartNo = '" + data[j].Part_No + "'";
                    SqlCommand SqlCmd5 = new SqlCommand(cmd5, Con1);
                    SqlDataReader dr2 = SqlCmd5.ExecuteReader();
                    int qty = 0;
                    while (dr2.Read())
                    {
                        qty = (int)dr2["Quantity"];
                    }
                    dr2.Close();
                    if (qty == 0)
                    {
                        string cmd4 = "Delete from Stock_Indent where IndentNo = '" + data[j].Ref_No + "' and PartNo = '" + data[j].Part_No + "'";
                        SqlCommand SqlCmd4 = new SqlCommand(cmd4, Con1);
                        SqlCmd4.ExecuteNonQuery();
                    }
                }
                Con1.Close();
                return data[0].Voucher_No;

            }
            else
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
                while (i < data.Count())
                {
                    SqlCommand sql_cmnd = new SqlCommand("[dbo].[GoodsRI_Add]", Con1);
                    sql_cmnd.CommandType = CommandType.StoredProcedure;
                    sql_cmnd.Parameters.AddWithValue("@v_type", SqlDbType.Int).Value = data[i].Index_Type;
                    sql_cmnd.Parameters.AddWithValue("@voucher_no", SqlDbType.Int).Value = GR_V_No;
                    sql_cmnd.Parameters.AddWithValue("@voucher_date", vdate);
                    sql_cmnd.Parameters.AddWithValue("@ref_no", SqlDbType.NVarChar).Value = data[i].Ref_No;
                    sql_cmnd.Parameters.AddWithValue("@ref_date", rdate);
                    sql_cmnd.Parameters.AddWithValue("@GI", SqlDbType.Int).Value = data[i].GI_Tag;
                    sql_cmnd.Parameters.AddWithValue("@Process", SqlDbType.Int).Value = data[i].Process_Tag;
                    sql_cmnd.Parameters.AddWithValue("@Project", SqlDbType.Int).Value = data[i].Project;
                    sql_cmnd.Parameters.AddWithValue("@Employee", SqlDbType.Int).Value = data[i].Employee;
                    sql_cmnd.Parameters.AddWithValue("@note", SqlDbType.NVarChar).Value = data[i].Note;
                    sql_cmnd.Parameters.AddWithValue("@part_no", SqlDbType.NVarChar).Value = data[i].Part_No;
                    sql_cmnd.Parameters.AddWithValue("@pcode", SqlDbType.NChar).Value = data[i].P_code;
                    sql_cmnd.Parameters.AddWithValue("@qty", SqlDbType.Int).Value = data[i].Quantity;
                    sql_cmnd.ExecuteNonQuery();
                    SqlCommand sql_cmnd1 = new SqlCommand("[dbo].[StockIndent_Edit]", Con1);
                    sql_cmnd1.CommandType = CommandType.StoredProcedure;
                    sql_cmnd1.Parameters.AddWithValue("@ref_no", SqlDbType.NVarChar).Value = data[i].Ref_No;
                    sql_cmnd1.Parameters.AddWithValue("@part_no", SqlDbType.NVarChar).Value = data[i].Part_No;
                    sql_cmnd1.Parameters.AddWithValue("@qty", SqlDbType.Int).Value = data[i].Quantity;
                    sql_cmnd1.Parameters.AddWithValue("@vno", SqlDbType.Int).Value = GR_V_No;
                    sql_cmnd1.ExecuteNonQuery();
                    string cmd3 = "select Quantity from Stock_Indent where IndentNo = '" + data[i].Ref_No + "' and PartNo = '" + data[i].Part_No + "'";
                    SqlCommand SqlCmd3 = new SqlCommand(cmd3, Con1);
                    SqlDataReader dr2 = SqlCmd3.ExecuteReader();
                    int qty = 0;
                    while (dr2.Read())
                    {
                        qty = (int)dr2["Quantity"];
                    }
                    dr2.Close();
                    if (qty == 0)
                    {
                        string cmd1 = "Delete from Stock_Indent where IndentNo = '" + data[i].Ref_No + "' and PartNo = '" + data[i].Part_No + "'";
                        SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con1);
                        SqlCmd1.ExecuteNonQuery();
                    }
                    if (i == data.Count() - 1)
                    {
                        break;
                    }
                    
                    i++;
                }
                Con1.Close();
                return GR_V_No;
            }
        }
        public List<Issue> Indent_List()
        {
            List<Issue> ItemQm = new List<Issue>();
            List<string> vno = new List<string>();
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd2 = "SELECT IndentNo FROM Stock_indent GROUP BY IndentNo HAVING COUNT(*)>0";
            SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
            SqlDataReader dr1 = SqlCmd2.ExecuteReader();
            while (dr1.Read())
            {
                vno.Add(dr1["IndentNo"].ToString());
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
                    string cmd1 = "select Top 1 IndentNo,IndentDate,GI_value,GI_Reason,Process_value,Process,Project_value,Project,Request_value,RequestBy from Stock_Indent where IndentNo = '" + vno[i] + "' ORDER BY IndentNo Asc;";
                    SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
                    SqlDataReader dr = SqlCmd1.ExecuteReader();
                    while (dr.Read())
                    {
                        ItemQm.Add(new Issue
                        {
                            IndentNo = dr["IndentNo"].ToString(),
                            IndentDate = DateTime.Parse( dr["IndentDate"].ToString()),
                            Reason = dr["GI_Reason"].ToString(),
                            Process = dr["Process"].ToString(),
                            Project = dr["Project"].ToString(),
                            RequestBy = dr["RequestBy"].ToString(),
                            Reason_int = (int)dr["GI_value"],
                            Process_int = (int)dr["Process_value"],
                            Project_int = (int)dr["Project_value"],
                            Request_int = (int)dr["Request_value"]
                        }
                        );
                    }
                    dr.Close();
                }
                Con.Close();
                return ItemQm;
            }
        }
    }
}