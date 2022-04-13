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
            if (Session["userID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Err","Login");
            }
        }
        public ActionResult List_Update(Price_List name)  // add DB dat to Price_List View
        {
            Goods_RI dblogin = new Goods_RI();
            var Descp = dblogin.PM_list(name.Package_letter, name.Value_letter, name.Partno_letter, name.Descp_letter);
            return Json(Descp);
        }
        public ActionResult Price_Insert(Price_List name) // Adding Data to DB
        {
            Price_Updation price_Updation = new Price_Updation();
            int data = price_Updation.AddPrice(name.Part_No, name.P_Cost, name.P_Price_USD, name.P_MRP, name.P_SP, name.Stock);
            return Json(name);
        }

    }
}