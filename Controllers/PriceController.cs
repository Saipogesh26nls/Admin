using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Models;
using System.Net;
using System.Web.Script.Serialization;

namespace Admin.Controllers
{
    public class PriceController : Controller
    {
        // GET: Price
        public ActionResult Price_Update()
        {
            return View();
        }
        public ActionResult Price_Insert(PriceFields newdata)
        {
            Price_Updation price_Updation = new Price_Updation();
            int data = price_Updation.AddPrice(newdata.Part_No, newdata.P_Cost, newdata.P_Price_USD, newdata.P_MRP, newdata.P_SP);
            newdata.Reg_success = "Submitted Successfully !!!!";
            Price_Update();
            return View("Price_Update",newdata);
        }
        public ActionResult Partno_to_Descp(PriceFields name)
        {
            Price_Updation dblogin = new Price_Updation();
            string Descp = dblogin.Product_Description(name.Part_No);
            return Json(Descp, JsonRequestBehavior.AllowGet);
        }
    }
}