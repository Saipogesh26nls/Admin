using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data;
using Newtonsoft.Json;
using System.Data.SqlClient;

using Admin.Models;

namespace Admin.Controllers
{
    public class PurchaseController : Controller
    {
        public ActionResult Purchase_Details()
        {
            SqlConnection _con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            _con.Open();
            if (Table_Data_List.Count == 0)
            {
                SqlDataAdapter _da1 = new SqlDataAdapter("Select * From Account_Master where A_Level<0", _con);
                DataTable _dt1 = new DataTable();
                _da1.Fill(_dt1);
                ViewBag.MfdList = ToSelectList(_dt1, "A_code", "A_Name");
                /*List<SelectListItem> ILedger = new List<SelectListItem>();
                ILedger.Add(new SelectListItem { Text = "Goods-Receipt", Value = "1" });
                ViewBag.ILedger = new SelectList(ILedger, "Value", "Text");
                List<SelectListItem> ALedger = new List<SelectListItem>();
                ALedger.Add(new SelectListItem { Text = "Purchase", Value = "2" });
                ViewBag.ALedger = new SelectList(ALedger, "Value", "Text");
                Table_Data_List.Add(new PurchaseField { Total_Qty = 0 });
                ViewBag.item = Table_Data_List;
                ViewBag.i = 0;*/
                ViewBag.item = Table_Data_List;
                
                return View();
            }
            else
            {
                SqlDataAdapter _da1 = new SqlDataAdapter("Select * From Account_Master where A_Level<0", _con);
                DataTable _dt1 = new DataTable();
                _da1.Fill(_dt1);
                ViewBag.MfdList = ToSelectList(_dt1, "A_code", "A_Name");
                /*List<SelectListItem> ILedger = new List<SelectListItem>();
                ILedger.Add(new SelectListItem { Text = "Goods-Receipt", Value = "1" });
                ViewBag.ILedger = new SelectList(ILedger, "Value", "Text");
                List<SelectListItem> ALedger = new List<SelectListItem>();
                ALedger.Add(new SelectListItem { Text = "Purchase", Value = "2" });
                ViewBag.ALedger = new SelectList(ALedger, "Value", "Text");*/
                ViewBag.item = Table_Data_List;
                ViewBag.i = i;
                return View();
            }
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
        public ActionResult Purchase_GetData(PurchaseField newdata)
        {
            if (newdata.Part_No == null)
            {
                ViewBag.ErrorMessage = "Part_No is not Found";
                return View("Purchase_Details");
            }
            else
            {
                string I_Ledger = "1"; //Goods-Receipt
                string A_Ledger = "2"; //Purchase
                double sub_total = newdata.P_Rate * newdata.P_Qty;
                double discount = (sub_total * newdata.I_Discount) / 100;
                double tax1 = (sub_total * newdata.I_Tax1) / 100;
                double tax2 = (sub_total * newdata.I_Tax2) / 100;
                double total = (sub_total - discount) + tax1 + tax2;
                Qty_List.Add(new Quantity { Qty = newdata.P_Qty, Sub_Total = total});
                Record(newdata.Invoice_No, newdata.Invoice_Date, I_Ledger, A_Ledger, newdata.Part_No, newdata.A_Name, newdata.P_Qty, newdata.P_Rate, newdata.Invoice_Date.ToString(), newdata.I_Discount, newdata.I_Tax1, newdata.I_Tax2, newdata.Reason_Tag);
                Purchase_Details(); //need to change
                i++;
                return View("Purchase_Details");
            }
        }
        static int i = 0;
        static List<Quantity> Qty_List = new List<Quantity>();

        static List<PurchaseField> Table_Data_List = new List<PurchaseField>();
        public List<PurchaseField> Record(string Inv_No, DateTime Inv_Date, string ILedger, string ALedger, string Part_no, string Acc_name, int P_Qty, double P_Rate, string I_Date, double I_Discount, double I_Tax1, double I_Tax2, string Reason_Tag)
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
            /*double Total = (total_S - discount) + tax1 + tax2;*/
            double Total = total_S;
            Table_Data_List.Add(new PurchaseField { Invoice_No = Inv_No.ToUpper(), Invoice_Date = Inv_Date, I_Ledger = ILedger, A_Ledger = ALedger, Part_No = Part_no.ToUpper(), A_Name = Acc_name.ToUpper(), P_Qty = P_Qty, P_Rate = P_Rate, P_Sub_Total = total_S, P_Discount = discount, P_Tax1 = tax1, P_Tax2 = tax2, Total_Qty = total_Q, Total = Total, Inv_Date = I_Date, I_Sub_Total = I_Sub_Total, I_Discount = I_discount, I_Tax1 = I_tax1, I_Tax2 = I_tax2, I_Total = I_Total, Reason_Tag = Reason_Tag });
            return (Table_Data_List);
        }

        public ActionResult Edit()
        {
            return View();
        }
        public ActionResult Delete(string Part_No)
        {
            Table_Data_List.RemoveAll(p => p.Part_No == Part_No);
            if(Table_Data_List.Count == 0)
            {
                i = i - 1;
                Purchase_Details();
                return View("Purchase_Details");
            }
            else
            {
                i = i - 2;
                Purchase_Details();
                return View("Purchase_Details");
            }
        }
        [HttpPost]
        public ActionResult Purchase_Insert(PurchaseField newdata)
        {
                PurchaseInsert purchase = new PurchaseInsert();
                purchase.Add_Data(Table_Data_List);
                ViewBag.Purchase = "Submitted Successfully !!!!";
                Table_Data_List.Clear();
                Purchase_Details(); //need to change
                return View("Purchase_Details", newdata);
        }

        public ActionResult Mfr_to_Addr(PurchaseField name)
        {
            List<string> MFR_Addr = new List<string>();
            PurchaseInsert purchaseInsert = new PurchaseInsert();
            var Mfr_Address = purchaseInsert.Address(name.Address);
            MFR_Addr.Add(Mfr_Address[0]);
            MFR_Addr.Add(Mfr_Address[1]);
            MFR_Addr.Add(Mfr_Address[2]);
            MFR_Addr.Add(Mfr_Address[3]);
            MFR_Addr.Add(Mfr_Address[4]);
            return Json(MFR_Addr, JsonRequestBehavior.AllowGet);
        }
    }
}