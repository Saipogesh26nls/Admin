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
    }
}