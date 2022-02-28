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
            New_Purchase new_Purchase = new New_Purchase();
            new_Purchase.Voucher_Date = DateTime.Today;
            new_Purchase.Invoice_Date = DateTime.Today;
          
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
            return View(new_Purchase);
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

        public ActionResult PurchaseList ()
        {
            NewPurchase_Insert newPurchase_Insert = new NewPurchase_Insert();
            var PM_Data = newPurchase_Insert.Purchase_List();
            ViewBag.PL = PM_Data;
            return View(PM_Data);
        }

        public ActionResult Edit_Purchase_View(int v_no, int inv_no)
        {
            NewPurchase_Insert newPurchase_Insert = new NewPurchase_Insert();
            var PM_Data = newPurchase_Insert.EditPurchase(v_no);
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            for(int i=0; i<PM_Data.Count; i++)
            {
                string cmd1 = "select P_Part_No from Product_Master where P_code = '" + PM_Data[i].Part_No + "'";
                SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
                SqlDataReader dr = SqlCmd1.ExecuteReader();
                while (dr.Read())
                {
                    string Part_No = dr["P_Part_No"].ToString();
                    PM_Data[i].Part_No = Part_No;
                }
                dr.Close();
            }
            ViewBag.inv = inv_no;
            ViewBag.PL = PM_Data;
            return View(PM_Data);
        }
        public ActionResult Edit_Purchase_Row(string part_no, double qty, double price, double subtotal, double discount, double tax1, double tax2, double total)
        {
            EditPurchaseValue Data = new EditPurchaseValue();
            Data.Part_No = part_no;
            Data.Quantity = qty;
            Data.Price_Per_Unit = price;
            Data.Sub_Total = subtotal;
            Data.Discount = (subtotal/(discount*10));
            Data.Tax1 = (subtotal/(tax1*10));
            Data.Tax2 = (subtotal/(tax2*10));
            Data.Total = total;

            return View(Data);
        }
    }
}