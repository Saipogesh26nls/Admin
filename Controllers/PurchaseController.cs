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
            if (Table_Data_List.Count == 0)
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
                List<SelectListItem> Reason_Tag = new List<SelectListItem>();
                Reason_Tag.Add(new SelectListItem { Text = "Adjustment", Value = "Adjustment" });
                Reason_Tag.Add(new SelectListItem { Text = "Scrap", Value = "Scrap" });
                Reason_Tag.Add(new SelectListItem { Text = "Damage", Value = "Damage" });
                Reason_Tag.Add(new SelectListItem { Text = "Goods Return", Value = "Goods Return" });
                ViewBag.Reason_Tag = new SelectList(Reason_Tag, "Value", "Text");
                ViewBag.item = Table_Data_List;
                /*Table_Data_List.Add(new PurchaseField { Total_Qty = 0 });
                ViewBag.item = Table_Data_List;
                ViewBag.i = 0;*/
                return View();
            }
            else
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
                List<SelectListItem> Reason_Tag = new List<SelectListItem>();
                Reason_Tag.Add(new SelectListItem { Text = "Adjustment", Value = "Adjustment" });
                Reason_Tag.Add(new SelectListItem { Text = "Scrap", Value = "Scrap" });
                Reason_Tag.Add(new SelectListItem { Text = "Damage", Value = "Damage" });
                Reason_Tag.Add(new SelectListItem { Text = "Goods Return", Value = "Goods Return" });
                ViewBag.Reason_Tag = new SelectList(Reason_Tag, "Value", "Text");
                ViewBag.item = Table_Data_List;
                ViewBag.i = i;
                return View();
            }
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
                Qty_List.Add(new Quantity { Qty = newdata.P_Qty, Sub_Total = newdata.P_Rate*newdata.P_Qty });
                Record(newdata.Invoice_No, newdata.Invoice_Date, newdata.I_Ledger, newdata.A_Ledger, newdata.Part_No, newdata.A_Name, newdata.P_Qty, newdata.P_Rate, newdata.Invoice_Date.ToString("dd/MM/yyyy"), newdata.I_Discount, newdata.I_Tax1, newdata.I_Tax2, newdata.Reason_Tag);
                Purchase_Details();
                i++;
                return View("Purchase_Details");
            }
        }
        static int i = 0;
        static List<Quantity> Qty_List = new List<Quantity>();

        static List<PurchaseField> Table_Data_List = new List<PurchaseField>();
        public static List<PurchaseField> Record(string Inv_No, DateTime Inv_Date, string ILedger, string ALedger, string Part_no, string Acc_name, int P_Qty, double P_Rate, string I_Date, double I_Discount, double I_Tax1, double I_Tax2, string Reason_Tag)
        {
            //For Individual Table Data Calculation with Edit Field
            double I_Sub_Total = P_Qty * P_Rate;
            double I_discount = (I_Sub_Total * I_Discount) / 100;
            double I_tax1 = (I_Sub_Total * I_Tax1) / 100;
            double I_tax2 = (I_Sub_Total * I_Tax2) / 100;
            double I_Total = (I_Sub_Total - I_discount) + I_tax1 + I_tax2;

            /*//For Individual Table Data Calculation without Edit Field
            double I_Sub_Total = P_Qty * P_Rate;
            double I_discount = (I_Sub_Total * 10) / 100;
            double I_tax1 = (I_Sub_Total * 2.5) / 100;
            double I_tax2 = (I_Sub_Total * 2.5) / 100;
            double I_Total = (I_Sub_Total - I_discount) + I_tax1 + I_tax2;*/

            //For Total Table Calculation
            int total_Q = Qty_List.Sum(x => x.Qty);
            double total_S = Qty_List.Sum(x => x.Sub_Total);
            double discount = (total_S * 10) / 100;
            double tax1 = (total_S * 2.5) / 100;
            double tax2 = (total_S * 2.5) / 100;
            double Total = (total_S - discount) + tax1 + tax2;
            Table_Data_List.Add(new PurchaseField { Invoice_No = Inv_No.ToUpper(), Invoice_Date = Inv_Date, I_Ledger = ILedger, A_Ledger = ALedger, Part_No = Part_no.ToUpper(), A_Name = Acc_name.ToUpper(), P_Qty = P_Qty, P_Rate = P_Rate, P_Sub_Total = total_S, P_Discount = discount, P_Tax1 = tax1, P_Tax2 = tax2, Total_Qty = total_Q, Total = Total, Inv_Date = I_Date, I_Sub_Total = I_Sub_Total, I_Discount = I_discount, I_Tax1 = I_tax1, I_Tax2 = I_tax2, I_Total = I_Total, Reason_Tag = Reason_Tag });
            return (Table_Data_List);
        }
        [HttpPost]
        public ActionResult Purchase_Insert(PurchaseField newdata)
        {
                PurchaseInsert purchase = new PurchaseInsert();
                purchase.Add_Data(Table_Data_List);
                ViewBag.Purchase = "Submitted Successfully !!!!";
                Table_Data_List.Clear();
                Purchase_Details();
                return View("Purchase_Details", newdata);
        }
    }
}