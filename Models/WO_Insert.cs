using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Admin.Models
{
    public class WO_Insert
    {
        public int Add_WO(List<Workorder> data, int id)
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "Update Number_master Set WO_No = WO_No + 1 ";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlCmd1.ExecuteNonQuery();
            string cmd2 = "Select WO_No from Number_master ";
            SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
            SqlDataReader dr = SqlCmd2.ExecuteReader();
            dr.Read();
            int WO_No = dr.GetInt32(0);
            dr.Close();
            Con.Close();

            SqlConnection Con1 = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con1.Open();
            int i = 0;
            DateTime now = DateTime.Now;
            while (i < data.Count())
            {
                SqlCommand sql_cmnd = new SqlCommand("[dbo].[add_WO]", Con1);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@wo_no", SqlDbType.NVarChar).Value = WO_No;
                sql_cmnd.Parameters.AddWithValue("@wo_date", SqlDbType.Date).Value = data[i].WO_Date;
                sql_cmnd.Parameters.AddWithValue("@partno", SqlDbType.NVarChar).Value = data[i].PartNo;
                sql_cmnd.Parameters.AddWithValue("@process", SqlDbType.Int).Value = data[i].Process;
                sql_cmnd.Parameters.AddWithValue("@qty", SqlDbType.Int).Value = data[i].Quantity;
                sql_cmnd.Parameters.AddWithValue("@mfropt", SqlDbType.NVarChar).Value = data[i].Mfr_option;
                sql_cmnd.Parameters.AddWithValue("@mfr",SqlDbType.NVarChar).Value=data[i].Mfr;
                sql_cmnd.Parameters.AddWithValue("@note", SqlDbType.NVarChar).Value = data[i].Note;
                sql_cmnd.Parameters.AddWithValue("@time", SqlDbType.Time).Value = now.ToLongTimeString();
                sql_cmnd.Parameters.AddWithValue("@employee", SqlDbType.Int).Value = id;
                sql_cmnd.ExecuteNonQuery();
                if (i == data.Count() - 1)
                {
                    break;
                }
                i++;
            }
            Con1.Close();
            return WO_No;
        }
        public List<GoodsRI> Descp_Qty(string part_no)
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
                    P_code = dr["P_code"].ToString()
                }
                );
            }
            Con.Close();
            return ItemQm;
        }
        public List<WO_List> WO_List()
        {
            List<WO_List> ItemQm = new List<WO_List>();
            List<int> vno = new List<int>();
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd2 = "SELECT WO_No FROM Work_Order GROUP BY WO_No HAVING COUNT(*)>0";
            SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
            SqlDataReader dr1 = SqlCmd2.ExecuteReader();
            while (dr1.Read())
            {
                vno.Add(Convert.ToInt32(dr1["WO_No"]));
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
                    string cmd1 = "select Top 1 * from Work_Order where WO_No = '" + vno[i] + "' ORDER BY WO_No Asc;";
                    SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
                    SqlDataReader dr = SqlCmd1.ExecuteReader();
                    while (dr.Read())
                    {
                        ItemQm.Add(new WO_List
                        {
                            WO_No = (int)dr["WO_No"],
                            WO_Date = dr["WO_Date"].ToString(),
                            Product = dr["P_code"].ToString(),
                            Process = dr["Process"].ToString(),
                            Mfr_Option = dr["Mfr_Option"].ToString(),
                            Mfr = dr["Mfr"].ToString()
                        }
                        );
                    }
                    dr.Close();
                }
                for (int i = 0; i < ItemQm.Count; i++)
                {
                    string cmd1 = "select A_Name from Account_Master where A_code = '" + ItemQm[i].Mfr + "'";
                    SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
                    SqlDataReader dr = SqlCmd1.ExecuteReader();
                    while (dr.Read())
                    {
                        string Mfr = dr["A_Name"].ToString();
                        ItemQm[i].Mfr = Mfr;
                    }
                    dr.Close();
                }
                for (int i = 0; i < ItemQm.Count; i++)
                {
                    string cmd1 = "select P_Name from Product_Master where P_code = '" + ItemQm[i].Product + "'";
                    SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
                    SqlDataReader dr = SqlCmd1.ExecuteReader();
                    while (dr.Read())
                    {
                        ItemQm[i].Product = dr["P_Name"].ToString();
                    }
                    dr.Close();
                }
                for (int i = 0; i < ItemQm.Count; i++)
                {
                    string cmd1 = "select Process_Name from Process_Tag where Process_Id = " + ItemQm[i].Process + "";
                    SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
                    SqlDataReader dr = SqlCmd1.ExecuteReader();
                    while (dr.Read())
                    {
                        ItemQm[i].Process = dr["Process_Name"].ToString();
                    }
                    dr.Close();
                }
                Con.Close();
                return ItemQm;
            }
        }

    }
}