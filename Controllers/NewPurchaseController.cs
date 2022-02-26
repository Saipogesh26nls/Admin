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
        public ActionResult New_Purchase(New_Purchase Purchase)
        {
            Purchase.Invoice_Date = DateTime.UtcNow;
            SqlConnection _con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            _con.Open();
            SqlDataAdapter _da = new SqlDataAdapter("Select P_Description From Product_Master where P_Level<0", _con);
            DataTable _dt = new DataTable();
            _da.Fill(_dt);
            ViewBag.ProductList = ToSelectList(_dt, "P_Description", "P_Description");
            _con.Close();
            NewPurchase_Insert newPurchase_Insert = new NewPurchase_Insert();
            var PM_Data = newPurchase_Insert.Product_Master();
            ViewBag.PM = PM_Data;
            return View();
        }

        [NonAction]
        public SelectList ToSelectList(DataTable table, string valueField, string textField)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (DataRow row in table.Rows)
            {
                list.Add(new SelectListItem()
                {
                    Text = row[textField].ToString(),
                    Value = row[valueField].ToString()
                });
            }
            return new SelectList(list, "Value", "Text");
        }
        [HttpPost]
        public ActionResult Table_Data (List<PurchaseTable> Purchase)
        {
            int Quantity = Purchase[Purchase.Count - 1].final_Qty;
            double Total = Purchase[Purchase.Count - 1].final_Sub_Total;
            double Final_Total = Purchase[0].final_total;
            NewPurchase_Insert purchase = new NewPurchase_Insert();
            purchase.Add_Data(Purchase, Quantity, Total, Final_Total);
            return Json(Purchase); 
        }
        public ActionResult Partno_to_Descp(BOMFields name)
        {
            NewPurchase_Insert dblogin = new NewPurchase_Insert();
            string Descp = dblogin.SP_Description(name.Part_to_Descp);
            return Json(Descp, JsonRequestBehavior.AllowGet);
        }
    }
}