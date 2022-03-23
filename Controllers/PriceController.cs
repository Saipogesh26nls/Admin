using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Models;
using System.Net;
using System.Web.Script.Serialization;
using System.Data.SqlClient;
using System.Configuration;

namespace Admin.Controllers
{
    public class PriceController : Controller
    {
        public ActionResult Price_List() // PM List View
        {
            return View();
        }
        public ActionResult List_Update(Price_List name)
        {
            List<Price_List> ItemQm = new List<Price_List>();
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "SELECT * FROM Product_Master WHERE P_Description LIKE '%" + name.alphabet + "%'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlDataReader dr = SqlCmd1.ExecuteReader();
            while (dr.Read())
            {
                string cost = dr["P_Cost"].ToString();
                string mrp = dr["P_MRP"].ToString();
                ItemQm.Add(new Price_List
                {
                    Part_No = dr["P_Part_No"].ToString(),
                    Description = dr["P_Description"].ToString(),
                    P_code = dr["P_code"].ToString(),
                    P_Cost = double.Parse(cost),
                    P_MRP = double.Parse(mrp)
                }
                );
            }
            Con.Close();
            return Json(ItemQm);
        } // add DB dat to Price_List View
        public ActionResult Price_Insert(Price_List name) // Adding Data to DB
        {
            Price_Updation price_Updation = new Price_Updation();
            int data = price_Updation.AddPrice(name.Part_No, name.P_Cost, name.P_Price_USD, name.P_MRP, name.P_SP);
            return Json(name);
        }

    }
}