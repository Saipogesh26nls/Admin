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
        public ActionResult New_Purchase(New_Purchase Purchase) // New Purchase Entry View
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
        public SelectList ToSelectList(DataTable table, string valueField, string textField) // For making Dropdown list
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
        public ActionResult Table_Data(List<PurchaseTable> Purchase)  // For Adding Purchase Data to DB
        {
            int Quantity = Purchase[Purchase.Count - 1].final_Qty;
            double Total = Purchase[Purchase.Count - 1].final_Sub_Total;
            double Final_Total = Purchase[0].final_total;
            double Final_Discount = Purchase[0].final_Discount;
            double Final_Tax1 = Purchase[0].final_Tax1;
            double Final_Tax2 = Purchase[0].final_Tax2;
            NewPurchase_Insert purchase = new NewPurchase_Insert();
            purchase.Add_Data(Purchase, Quantity, Total, Final_Total, Final_Discount, Final_Tax1, Final_Tax2);
            ViewBag.Purchase = "Submitted Successfully!!!";
            return Json(Purchase);
        }
        public ActionResult Partno_to_Descp(BOMFields name) // conversion of part_no to description
        {
            NewPurchase_Insert dblogin = new NewPurchase_Insert();
            var Descp = dblogin.SP_Description(name.Part_to_Descp);
            if (Descp == null)
            {
                List<string> list = new List<string>();
                list.Add("Please Enter Valid Part No !!!");
                list.Add("");
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(Descp, JsonRequestBehavior.AllowGet);
            }
            
        } 

        public ActionResult PurchaseList() // To show full purchase list view
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
                    PM_Data[i].A_Name = Mfr;
                }
                dr.Close();
            }
            for (int i = 0; i < PM_Data.Count; i++)
            {
                string cmd1 = "select Amount from A_Ledger where A_code = '" + PM_Data[i].A_code + "'";
                SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
                SqlDataReader dr = SqlCmd1.ExecuteReader();
                while (dr.Read())
                {
                    string Total_Amt = dr["Amount"].ToString();
                    PM_Data[i].Purchase_Total = double.Parse(Total_Amt);
                }
                dr.Close();
            }
            ViewBag.PL = PM_Data;
            return View(PM_Data);
        }
        static int V_no = 0;
        public ActionResult Edit_Purchase_View(int v_no, DateTime v_date, int inv_no, DateTime inv_date, string a_code) // edit purchase view
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
            PM_Data.Tables[0].Columns.Add("Ref_No");
            List<string> table_data = new List<string>();
            for (int i = 0; i < PM_Data.Tables[0].Rows.Count; i++)
            {
                string Text = PM_Data.Tables[0].Rows[i]["P_code"].ToString();
                NewPurchase_Insert dblogin = new NewPurchase_Insert();
                var Descp = dblogin.Pcode_to_PartNo(Text);
                /*string cmd3 = "select Ref_No from I_Ledger where Voucher_No = '" + v_no + "'";
                SqlCommand SqlCmd3 = new SqlCommand(cmd3, Con);
                SqlDataReader dr2 = SqlCmd3.ExecuteReader();
                dr2.Read();
                int Ref_No = dr2.GetInt32(0);*/
                table_data.Add(Descp[0].P_Part_No);
                table_data.Add(Descp[0].P_Description);
                PM_Data.Tables[0].Rows[i]["P_Part_No"] = Descp[0].P_Part_No;
                PM_Data.Tables[0].Rows[i]["P_Description"] = Descp[0].P_Description;
            }
            string cmd3 = "select top 1 Ref_No from I_Ledger where Voucher_NO = '" + v_no+"'";
            SqlCommand SqlCmd3 = new SqlCommand(cmd3, Con);
            SqlDataReader dr2 = SqlCmd3.ExecuteReader();
            string ref_no = "";
            while (dr2.Read())
            {
                ref_no = dr2["Ref_No"].ToString();
            }
            dr2.Close();
            newPurchase_Insert.Voucher_No = v_no.ToString();
            newPurchase_Insert.Voucher_Date = v_date;
            newPurchase_Insert.Invoice_No = inv_no.ToString();
            newPurchase_Insert.Invoice_Date = inv_date;
            
            ViewBag.PL = PM_Data.Tables[0];
            V_no = v_no;
            ViewBag.ILedger = 1;
            ViewBag.ALedger = 2;
            ViewBag.Ref_No = ref_no;
            return View(newPurchase_Insert);
        } 
        [HttpPost]
        public ActionResult Edited_Table_Data(List<PurchaseTable> Purchase)  // For Edit and Delete the purchase list in DB
        {
            for(int i = 0;i< Purchase.Count; i++)
            {
                string a = Purchase[i].Invoice_No.Replace("\n","").Replace(" ","");
                string b = Purchase[i].Voucher_No.Replace("\n", "").Replace(" ", "");
                string c = Purchase[i].supplier.Replace("\n", "").Replace(" ", "");
                Purchase[i].Invoice_No = a;
                Purchase[i].Voucher_No = b;
                Purchase[i].supplier = c;
            }
            int Final_Quantity = Purchase[0].final_Qty;
            double Final_SubTotal = Purchase[0].final_Sub_Total;
            double Final_Discount = Purchase[0].final_Discount;
            double Final_Tax1 = Purchase[0].final_Tax1;
            double Final_Tax2 = Purchase[0].final_Tax2;
            double Final_Total = Purchase[0].final_total;
            string Ref_No_str = Purchase[0].Ref_No.Replace("\n", "").Replace(" ", "");
            int Ref_No = int.Parse(Ref_No_str);
            NewPurchase_Insert purchase = new NewPurchase_Insert();
            purchase.Edit_and_Delete(Purchase, Final_Quantity, Final_SubTotal, Final_Discount, Final_Tax1, Final_Tax2, Final_Total, Ref_No);
            return Json(Purchase);
        }
        public ActionResult Goods_Receipt_Issue(int v_no, DateTime v_date) // Goods Issue View
        {
            GoodsRI Model = new GoodsRI();
            Model.Voucher_Date = DateTime.Today;
            Model.Ref_Date = DateTime.Today;
            SqlConnection _con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            _con.Open();
            SqlDataAdapter _da = new SqlDataAdapter("Select * From Project_Master", _con);
            DataTable _dt = new DataTable();
            _da.Fill(_dt);
            ViewBag.Project = ToSelectList(_dt, "Project_Id", "Project_Name");
            SqlDataAdapter _da1 = new SqlDataAdapter("Select * From Process_Tag", _con);
            DataTable _dt1 = new DataTable();
            _da1.Fill(_dt1);
            ViewBag.Process = ToSelectList(_dt1, "Process_Id", "Process_Name");
            SqlDataAdapter _da2 = new SqlDataAdapter("Select * From GI_Tag", _con);
            DataTable _dt2 = new DataTable();
            _da2.Fill(_dt2);
            ViewBag.GI = ToSelectList(_dt2, "GI_Id", "TagName");
            SqlDataAdapter _da3 = new SqlDataAdapter("Select * From Employee_Master", _con);
            DataTable _dt3 = new DataTable();
            _da3.Fill(_dt3);
            ViewBag.Employee = ToSelectList(_dt3, "Employee_Id", "Employee_Name");
            SqlDataAdapter _da4 = new SqlDataAdapter("Select P_code From Purchase where Voucher_No = '"+v_no+"'", _con);
            DataTable _dt4 = new DataTable();
            _da4.Fill(_dt4);
            List<SelectListItem> Partno_list = new List<SelectListItem>();
            for (int i = 0; i < _dt4.Rows.Count; i++)
            {
                var list = _dt4.Rows[i]["P_code"].ToString();
                string cmd4 = "select P_Part_No from Product_Master where P_code = '"+list+"'";
                SqlCommand SqlCmd4 = new SqlCommand(cmd4, _con);
                SqlDataReader dr1 = SqlCmd4.ExecuteReader();
                string partno = "";
                while (dr1.Read())
                {
                    partno = dr1["P_Part_No"].ToString();
                }
                Partno_list.Add(new SelectListItem { Text = partno, Value = partno });
                dr1.Close();
            }
            ViewBag.PartNo = new SelectList(Partno_list, "Value", "Text");
            List<SelectListItem> Index = new List<SelectListItem>();
            Index.Add(new SelectListItem { Text = "Goods-Receipt", Value = "1" });
            Index.Add(new SelectListItem { Text = "Goods-Issue", Value = "2" });
            ViewBag.Index = new SelectList(Index, "Value", "Text");
            string cmd3 = "select top 1 Ref_No from I_Ledger where Voucher_NO = '" + v_no + "'";
            SqlCommand SqlCmd3 = new SqlCommand(cmd3, _con);
            SqlDataReader dr2 = SqlCmd3.ExecuteReader();
            string ref_no = "";
            while (dr2.Read())
            {
                ref_no = dr2["Ref_No"].ToString();
            }
            dr2.Close();
            Model.Voucher_No = v_no;
            Model.Voucher_Date = v_date;
            Model.Ref_No = ref_no;
            _con.Close();
            return View(Model);
        }
    }
}