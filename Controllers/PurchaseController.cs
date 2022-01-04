using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Models;

namespace Admin.Controllers
{
    public class PurchaseController : Controller
    {
        // GET: Purchase
        public ActionResult Purchase_Details()
        {
            List<SelectListItem> ILedger = new List<SelectListItem>();
            ILedger.Add(new SelectListItem { Text = "Goods-Receipt", Value = "Goods-Receipt" });
            ILedger.Add(new SelectListItem { Text = "Goods-Issue", Value = "Goods-Issue" });
            ViewBag.ILedger = new SelectList(ILedger, "Value", "Text");
            List<SelectListItem> ALedger = new List<SelectListItem>();
            ALedger.Add(new SelectListItem { Text = "Sales", Value = "Sales" });
            ALedger.Add(new SelectListItem { Text = "Purchase", Value = "Purchase" });
            ALedger.Add(new SelectListItem { Text = "Receipt", Value = "Receipt" });
            ALedger.Add(new SelectListItem { Text = "Payable", Value = "Payable" });
            ViewBag.ALedger = new SelectList(ALedger, "Value", "Text");
            return View();
        }
        public ActionResult Purchase_GetData()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Purchase_Insert(PurchaseField newdata)
        {
            PurchaseInsert purchase = new PurchaseInsert();
            string Invoice_Date = newdata.Invoice_Date.ToString("dd/MM/yyyy");
            string Voucher_Date = newdata.Voucher_Date.ToString("dd/MM/yyyy");
            var Data = purchase.Add_Data(newdata.Voucher_No, Voucher_Date, newdata.Invoice_No, Invoice_Date, newdata.A_Name, newdata.Part_No, newdata.I_Ledger, newdata.A_Ledger, newdata.P_Qty, newdata.P_Rate, newdata.P_Discount, newdata.P_Tax1, newdata.P_Tax2, newdata.P_Sub_Total, newdata.P_Total);
            ViewBag.Purchase = "Submitted Successfully !!!!";
            ModelState.Clear();
            Purchase_Details();
            return View("Purchase_Details",newdata);
        }
    }
}