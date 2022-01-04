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
            ViewBag.item = Table_Data_List;
            return View();
        }
        public ActionResult Purchase_GetData(PurchaseField newdata)
        {
            if (newdata.Part_No == null)
            {
                ViewBag.ErrorMessage = "Part_No is not Found";
                return View("Purchase_Details");
            }
            else
            {
                Record(newdata.Invoice_No, newdata.Invoice_Date, newdata.I_Ledger, newdata.A_Ledger, newdata.Part_No, newdata.A_Name, newdata.P_Qty, newdata.P_Rate, newdata.P_Sub_Total, newdata.P_Discount, newdata.P_Tax1, newdata.P_Tax2);
                Purchase_Details();
                return View("Purchase_Details");
            }
        }

        static List<PurchaseField> Table_Data_List = new List<PurchaseField>();
        public static List<PurchaseField> Record(string Inv_No, DateTime Inv_Date, string ILedger, string ALedger, string Part_no, string Acc_name, int P_Qty, double P_Rate, double P_Sub_Total, double P_Discount, double P_Tax1, double P_Tax2)
        {
            Table_Data_List.Add(new PurchaseField { Invoice_No = Inv_No.ToUpper(), Invoice_Date = Inv_Date, I_Ledger = ILedger, A_Ledger = ALedger, Part_No = Part_no.ToUpper(), A_Name = Acc_name.ToUpper(), P_Qty = P_Qty, P_Rate = P_Rate, P_Sub_Total = P_Sub_Total, P_Discount = P_Discount, P_Tax1 = P_Tax1, P_Tax2 = P_Tax2, P_Total = P_Qty*P_Rate }) ;
            int total = Table_Data_List.Sum(x => P_Qty);
            Table_Data_List.Add(new PurchaseField { Total_Qty = total });
            return (Table_Data_List);
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