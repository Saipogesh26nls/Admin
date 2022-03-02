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
            SqlDataAdapter _da1 = new SqlDataAdapter("Select * From Account_Master where A_Level<0", _con);
            DataTable _dt1 = new DataTable();
            _da1.Fill(_dt1);
            ViewBag.MfdList = ToSelectList(_dt1, "A_code", "A_Name");
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
        public ActionResult Table_Data(List<PurchaseTable> Purchase)
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

        public ActionResult PurchaseList()
        {
            NewPurchase_Insert newPurchase_Insert = new NewPurchase_Insert();
            var PM_Data = newPurchase_Insert.Purchase_List();
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            for (int i = 0; i < PM_Data.Count; i++)
            {
                string cmd1 = "select A_Name from Account_Master where A_code = '" + PM_Data[i].A_code + "'";
                SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
                SqlDataReader dr = SqlCmd1.ExecuteReader();
                while (dr.Read())
                {
                    string Mfr = dr["A_Name"].ToString();
                    PM_Data[i].A_code = Mfr;
                }
                dr.Close();
            }
            ViewBag.PL = PM_Data;
            return View(PM_Data);
        }
        static int V_no = 0;
        public ActionResult Edit_Purchase_View(int v_no, DateTime v_date, int inv_no, DateTime inv_date, string a_code)
        {
            New_Purchase newPurchase_Insert = new New_Purchase();
            DataSet PM_Data = newPurchase_Insert.EditPurchase(v_no);
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            SqlDataAdapter _da1 = new SqlDataAdapter("Select * From Account_Master where A_Level<0", Con);
            DataTable _dt1 = new DataTable();
            _da1.Fill(_dt1);
            ViewBag.MfdList = ToSelectList(_dt1, "A_code", "A_Name");
            string cmd1 = "select A_code from Account_Master where A_Name = '" + a_code + "'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlDataReader dr = SqlCmd1.ExecuteReader();
            while (dr.Read())
                {
                    string Mfr = dr["A_code"].ToString();
                    newPurchase_Insert.Supplier = Mfr;
                ViewBag.Mfr = Mfr;
            }
            dr.Close();
            string cmd2 = "select Final_Discount, Final_Tax1, Final_Tax2, Amount from A_Ledger where Voucher_No = '" + v_no + "'";
            SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
            SqlDataReader dr1 = SqlCmd2.ExecuteReader();
            while (dr1.Read())
            {
                string a1 = dr1["Amount"].ToString();
                string a2 = dr1["Final_Discount"].ToString();
                string a3 = dr1["Final_Tax1"].ToString();
                string a4 = dr1["Final_Tax2"].ToString();
                ViewBag.Final_Total = double.Parse(a1);
                ViewBag.Final_Discount = double.Parse(a2);
                ViewBag.Final_Tax1 = double.Parse(a3);
                ViewBag.Final_Tax2 = double.Parse(a4);
            }
            dr1.Close();
            PM_Data.Tables[0].Columns.Add("P_Part_No");
            PM_Data.Tables[0].Columns.Add("P_Description");
            PM_Data.Tables[0].Columns.Add("Discount(%)");
            PM_Data.Tables[0].Columns.Add("Tax1(%)");
            PM_Data.Tables[0].Columns.Add("Tax2(%)");
            List<string> table_data = new List<string>();
            for (int i = 0; i < PM_Data.Tables[0].Rows.Count; i++)
            {
                string Text = PM_Data.Tables[0].Rows[i]["P_code"].ToString();
                NewPurchase_Insert dblogin = new NewPurchase_Insert();
                var Descp = dblogin.Pcode_to_PartNo(Text);
                table_data.Add(Descp[0].P_Part_No);
                table_data.Add(Descp[0].P_Description);
                PM_Data.Tables[0].Rows[i]["P_Part_No"] = Descp[0].P_Part_No;
                PM_Data.Tables[0].Rows[i]["P_Description"] = Descp[0].P_Description;
            }
            newPurchase_Insert.Voucher_No = v_no.ToString();
            newPurchase_Insert.Voucher_Date = v_date;
            newPurchase_Insert.Invoice_No = inv_no.ToString();
            newPurchase_Insert.Invoice_Date = inv_date;
            
            ViewBag.PL = PM_Data.Tables[0];
            V_no = v_no;
            return View(newPurchase_Insert);
        }
        [HttpPost]
        public ActionResult Edited_Table_Data(List<PurchaseTable> Purchase)
        {
            int Quantity = Purchase[Purchase.Count - 1].final_Qty;
            double Total = Purchase[Purchase.Count - 1].final_Sub_Total;
            double Final_Total = Purchase[0].final_total;
            NewPurchase_Insert purchase = new NewPurchase_Insert();
            purchase.Add_Data(Purchase, Quantity, Total, Final_Total);
            return Json(Purchase);
        }
        /*[HttpGet]
        public ActionResult Edit_Purchase_View(string P_code, Admin.Models.New_Purchase data)
        {
            DataSet ds = data.GetAccount(P_code, V_no);
            data.Part_No = ds.Tables[0].Rows[0]["P_code"].ToString();
            data.Quantity = float.Parse(ds.Tables[0].Rows[0]["Purchase_Qty"].ToString());
            data.Price_Per_Unit = float.Parse(ds.Tables[0].Rows[0]["Purchase_Rate"].ToString());
            data.Sub_Total = float.Parse(ds.Tables[0].Rows[0]["Purchase_SubTotal"].ToString());
            data.Discount = float.Parse(ds.Tables[0].Rows[0]["Purchase_Discount"].ToString());
            data.Tax1 = float.Parse(ds.Tables[0].Rows[0]["Purchase_Tax_1"].ToString());
            data.Tax2 = float.Parse(ds.Tables[0].Rows[0]["Purchase_Tax_2"].ToString());
            data.Total = float.Parse(ds.Tables[0].Rows[0]["Purchase_Total"].ToString());

            return View(data);
        }*/
        [HttpGet]
        public ActionResult Edit_Purchase_Row(Admin.Models.New_Purchase data, string P_code)
        {
            /*int _records = data.DataUpdate(P_code, data.Quantity, data.Price_Per_Unit, data.Sub_Total, data.Discount, data.Tax1, data.Tax2, data.Total);
            if (_records > 0)
            {
                return RedirectToAction("Edit_Purchase_View", "NewPurchase");
            }*/
            return RedirectToAction("Edit_Purchase_View", "NewPurchase");
        }

        public ActionResult pcode_to_partno (New_Purchase name)
        {
            NewPurchase_Insert dblogin = new NewPurchase_Insert();
            var Descp = dblogin.Pcode_to_PartNo(name.P_code);
            return Json(Descp, JsonRequestBehavior.AllowGet);
        }
    }
}