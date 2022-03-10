using Admin.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Xml.Linq;
using System;
using System.Web;
using System.Web.Mvc;

namespace Admin.Controllers
{
    public class BOMController : Controller
    {
        // GET: BOM
        static int i = 2;
        public ActionResult BOM_Add_Data(BOMFields data) // BOM Add Data View
        {
            return View(data);
        }
        public ActionResult Order(List<BOM_Table> name) // Add List to DB
        {
            BOM_Insert BOM_SP = new BOM_Insert();
            int BOM = BOM_SP.AddOrderDetails(name);
            return Json(BOM);
        }

        public ActionResult Partno_to_Descp (BOMFields name) // conversion of part_no to description
        {
            BOM_Insert dblogin = new BOM_Insert();
            string Descp = dblogin.SP_Description(name.SP_Part_No);
            return Json(Descp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult P_to_DQ(BOMFields name) // conversion of part_no to description, qty
        {
            BOM_Insert dblogin = new BOM_Insert();
            var Descp = dblogin.Descp_Qty(name.Part_No);
            return Json(Descp, JsonRequestBehavior.AllowGet);
        }
    }
}