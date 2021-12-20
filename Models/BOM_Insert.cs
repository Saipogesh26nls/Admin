using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using Admin.Models;

namespace Admin.Models
{
    public class BOM_Insert
    {
        public string P_Description(string cSP_Part_No)
        {
            List<BOMFields> ItemQm = new List<BOMFields>();
            DB_Con_Str OCon = new DB_Con_Str();
            string ConString = OCon.DB_Data();
            SqlConnection Con = new SqlConnection(ConString);
            Con.Open();
            string cmd1="select P_Description from Product_Master where P_Part_No = '" + cSP_Part_No+ "'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlDataReader dr = SqlCmd1.ExecuteReader();
            while (dr.Read())
            {
                ItemQm.Add(new BOMFields
                {
                    SP_Description = dr["P_Description"].ToString()
                }
                );
            }
            if (ItemQm.Count()!=0)
            {
                string item = string.Join("", ItemQm.Select(m => m.SP_Description));
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