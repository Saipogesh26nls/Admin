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
        public ActionResult Price_Update(string partno, double? cost, string descp, double? mrp) // Price Update View
        {
            Price_List price_list = new Price_List();
            price_list.Part_No = partno;
            price_list.P_Cost = cost;
            price_list.Description = descp;
            price_list.P_MRP = mrp;
            return View(price_list);
        }
        [HttpPost]
        public void Price_List_Update(Price_List name) // Price Update View
        {
            Price_Update(name.Part_No,name.P_Cost,name.Description,name.P_MRP);
        }
        public ActionResult Price_Insert(PriceFields newdata) // Adding Data to DB
        {
            Price_Updation price_Updation = new Price_Updation();
            int data = price_Updation.AddPrice(newdata.Part_No, newdata.P_Cost, newdata.P_Price_USD, newdata.P_MRP, newdata.P_SP);
            newdata.Reg_success = "Submitted Successfully !!!!";
            return View(newdata);
        }
        public ActionResult Partno_to_Descp(PriceFields name) // conversion of part_no to description
        {
            Price_Updation dblogin = new Price_Updation();
            string Descp = dblogin.Product_Description(name.Part_No);
            return Json(Descp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Price_List()
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
        }
    }
}