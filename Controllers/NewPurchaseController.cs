using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Admin.Controllers
{
    public class NewPurchaseController : Controller
    {
        // GET: NewPurchase
        public ActionResult New_Purchase()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Table_Data (List<PurchaseTable> Purchase)
        {
            int Quantity = Purchase[Purchase.Count - 1].final_Qty;
            double Total = Purchase[Purchase.Count - 1].final_Total;
            NewPurchase_Insert purchase = new NewPurchase_Insert();
            purchase.Add_Data(Purchase, Quantity, Total);
            return Json(Purchase); 
        }
        public ActionResult Partno_to_Descp(BOMFields name)
        {
            /*SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "Update Number_master Set Voucher_No = Voucher_No + 1 ";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlCmd1.ExecuteNonQuery();
            string cmd2 = "Select Voucher_No from Number_master ";
            SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
            SqlDataReader dr = SqlCmd2.ExecuteReader();
            dr.Read();
            int Voucher_No = dr.GetInt32(0);
            DateTime date = DateTime.Now;
            string dateWithFormat = date.ToString("dd - MM - yyyy");
            ViewBag.Voucher_No = Voucher_No;
            ViewBag.DateWithFormat = dateWithFormat;
            Con.Close();*/
            NewPurchase_Insert dblogin = new NewPurchase_Insert();
            string Descp = dblogin.SP_Description(name.Part_to_Descp);
            return Json(Descp, JsonRequestBehavior.AllowGet);
        }
    }
}