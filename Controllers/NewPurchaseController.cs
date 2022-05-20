using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Admin.Models;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace Admin.Controllers
{
    public class NewPurchaseController : Controller
    {
        //New Purchase
        [HttpGet]
        public ActionResult New_Purchase(New_Purchase Purchase) // New Purchase Entry View
        {
            /*if (Session["userID"] != null)
            {*/
                New_Purchase new_Purchase = new New_Purchase();
                new_Purchase.Voucher_Date = DateTime.Today;
                new_Purchase.Invoice_Date = DateTime.Today;

                SqlConnection _con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
                _con.Open();
                /*SqlDataAdapter _da = new SqlDataAdapter("Select P_Description From Product_Master where P_Level<0", _con);
                DataTable _dt = new DataTable();
                _da.Fill(_dt);
                ViewBag.ProductList = ToSelectList(_dt, "P_Description", "P_Description");*/
                SqlDataAdapter _da1 = new SqlDataAdapter("Select * From Account_Master where A_Level<0", _con);
                DataTable _dt1 = new DataTable();
                _da1.Fill(_dt1);
                ViewBag.MfdList = ToSelectList(_dt1, "A_code", "A_Name");
                SqlDataAdapter _da4 = new SqlDataAdapter("Select * From Product_Master where P_Level>1", _con);
                DataTable _dt4 = new DataTable();
                _da4.Fill(_dt4);
                ViewBag.ProductList = ToSelectList(_dt4, "P_code", "P_Name");
                SqlDataAdapter _da5 = new SqlDataAdapter("Select * From Account_Master where A_Level<1", _con);
                DataTable _dt5 = new DataTable();
                _da5.Fill(_dt5);
                ViewBag.Mfd = ToSelectList(_dt5, "A_code", "A_Name");
                SqlDataAdapter _da2 = new SqlDataAdapter("Select * From Project_Master", _con);
                DataTable _dt2 = new DataTable();
                _da2.Fill(_dt2);
                ViewBag.Project = ToSelectList(_dt2, "Project_Id", "Project_Name");
                SqlDataAdapter _da3 = new SqlDataAdapter("SELECT PO_No FROM Purchase_Order GROUP BY PO_No HAVING COUNT(*)>0", _con);
                DataTable _dt3 = new DataTable();
                _da3.Fill(_dt3);
                _con.Close();
                ViewBag.POno = ToPOList(_dt3, "PO_No", "PO_No");
                return View(new_Purchase);
            /*}
            else
            {
                return RedirectToAction("Err", "Login");
            }*/
        }
        [NonAction]
        public SelectList ToPOList(DataTable table, string valueField, string textField) // For making Dropdown list
        {
            SqlConnection _con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            _con.Open();
            List<int> vno = new List<int>();
            string cmd = "SELECT PO_No FROM Purchase GROUP BY PO_No HAVING COUNT(*)>0";
            SqlCommand SqlCmd = new SqlCommand(cmd, _con);
            SqlDataReader dr = SqlCmd.ExecuteReader();
            while (dr.Read())
            {
                vno.Add((int)dr["PO_No"]);
            }
            dr.Close();
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (DataRow row in table.Rows)
            {
                list.Add(new SelectListItem()
                {
                    Text = row[textField].ToString(),
                    Value = row[valueField].ToString()
                });
            }
            for(int i = 0; i < list.Count; i++)
            {
                if (vno.Contains(Convert.ToInt32(list[i].Value)))
                {
                    list.Remove(list[i]);
                }
            }
            _con.Close();
            return new SelectList(list, "Value", "Text");
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
        public ActionResult Table_Data(List<PurchaseTable> Purchase) // For Adding Purchase Data to DB
        {
            string Invno = Purchase[0].Invoice_No;
            DateTime Invdate = Purchase[0].Invoice_Date;
            int po_no = Purchase[0].PO_No;
            int project = int.Parse(Purchase[0].project);
            int Quantity = Purchase[0].final_Qty;
            double Total = Purchase[0].final_Sub_Total;
            double Final_Total = Purchase[0].final_total;
            double Final_Discount = Purchase[0].final_Discount;
            double Final_Tax1 = Purchase[0].final_Tax1;
            double Final_Tax2 = Purchase[0].final_Tax2;
            NewPurchase_Insert purchase = new NewPurchase_Insert();
            int v_no = purchase.Add_Data(Purchase, Quantity, Total, Final_Total, Final_Discount, Final_Tax1, Final_Tax2, project, po_no, Invno, Invdate);
            return Json(v_no);
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
        public ActionResult Add_PO_to_Purchase(New_Purchase name)
        {
            if(Convert.ToInt32(name.PO_No) > 0)
            {
                NewPurchase_Insert newPurchase_Insert = new NewPurchase_Insert();
                var data = newPurchase_Insert.Add_PO_to_purchase(name.PO_No);
                return Json(data);
            }
            else
            {
                return Json(name);
            }
            
        } // Add PO to Purchase view
         
        //Purchase List
        [HttpGet]
        public ActionResult PurchaseList() // To show full purchase list view
        {
            /*if (Session["userID"] != null)
            {*/
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
                    string cmd1 = "select Amount from A_Ledger where Voucher_No = '" + PM_Data[i].Voucher_No + "'";
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
                Con.Close();
                return View(PM_Data);
            /*}
            else
            {
                return RedirectToAction("Err", "Login");
            }*/
        }

        //Edit Purchase
        static int V_no = 0;
        public ActionResult Edit_Purchase_View(int v_no, DateTime v_date, string inv_no, DateTime inv_date, string a_code, string data, string po_no) // edit purchase view
        {
            /*if (Session["userID"] != null)
            {*/
                New_Purchase newPurchase_Insert = new New_Purchase();
                PurchaseTable mfr = new PurchaseTable();
                DataSet PM_Data = newPurchase_Insert.EditPurchase(v_no);
                SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
                Con.Open();
                SqlDataAdapter _da1 = new SqlDataAdapter("Select * From Account_Master where A_Level<0", Con);
                DataTable _dt1 = new DataTable();
                _da1.Fill(_dt1);
                ViewBag.MfdList = ToSelectList(_dt1, "A_code", "A_Name");
                SqlDataAdapter _da4 = new SqlDataAdapter("Select * From Product_Master where P_Level>1", Con);
                DataTable _dt4 = new DataTable();
                _da4.Fill(_dt4);
                ViewBag.ProductList = ToSelectList(_dt4, "P_code", "P_Name");
                SqlDataAdapter _da5 = new SqlDataAdapter("Select * From Account_Master where A_Level<1", Con);
                DataTable _dt5 = new DataTable();
                _da5.Fill(_dt5);
                ViewBag.Mfd = ToSelectList(_dt5, "A_code", "A_Name");
                ViewBag.Mfr = a_code;
                SqlDataAdapter _da2 = new SqlDataAdapter("Select * From Project_Master", Con);
                DataTable _dt2 = new DataTable();
                _da2.Fill(_dt2);
                ViewBag.Project = ToSelectList(_dt2, "Project_Id", "Project_Name");
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
                /*List<string> table_data = new List<string>();*/
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
                    /*table_data.Add(Descp[0].P_Part_No);
                    table_data.Add(Descp[0].P_Description);*/
                    PM_Data.Tables[0].Rows[i]["P_Part_No"] = Descp[0].P_Part_No;
                    PM_Data.Tables[0].Rows[i]["P_Description"] = Descp[0].P_Description;
                }
                string cmd3 = "select top 1 Ref_No from I_Ledger where Voucher_NO = '" + v_no + "'";
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
                newPurchase_Insert.Invoice_No = inv_no;
                newPurchase_Insert.Invoice_Date = inv_date;
                newPurchase_Insert.Supplier = a_code;
                newPurchase_Insert.Project = data;
                newPurchase_Insert.PO_No = po_no;

                ViewBag.Project_data = data;
                ViewBag.PO_No = po_no;
                ViewBag.PL = PM_Data.Tables[0];
                V_no = v_no;
                ViewBag.ILedger = 1;
                ViewBag.ALedger = 2;
                ViewBag.Ref_No = ref_no;
                Con.Close();
                return View(newPurchase_Insert);
            /*}
            else
            {
                return RedirectToAction("Err", "Login");
            }*/
        } 
        [HttpPost]
        public ActionResult Edited_Table_Data(List<PurchaseTable> Purchase) // For Edit and Delete the purchase list in DB
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
            int project = Convert.ToInt32(Purchase[0].project);
            int pono = Purchase[0].PO_No;
            NewPurchase_Insert purchase = new NewPurchase_Insert();
            purchase.Edit_and_Delete(Purchase, Final_Quantity, Final_SubTotal, Final_Discount, Final_Tax1, Final_Tax2, Final_Total, pono, project);
            return Json(Purchase);
        }
        public ActionResult Purchase_ED(GoodsRI name) // For Delete individual row from DB
        {
            NewPurchase_Insert dblogin = new NewPurchase_Insert();
            dblogin.Update_Close_Bal_P(name.Part_No, name.Index_Type, name.Voucher_No);
            return Json(name, JsonRequestBehavior.AllowGet);
        }

        //Delete Purchase
        [HttpGet]
        public ActionResult Delete_Purchase_view(int v_no, DateTime v_date, string inv_no, DateTime inv_date, string a_code) // delete purchase view
        {
            /*var roll = Convert.ToInt32(Session["roll"]);
            if (Session["userID"] != null && roll == 1)
            {*/
                New_Purchase newPurchase_Insert = new New_Purchase();
                PurchaseTable mfr = new PurchaseTable();
                DataSet PM_Data = newPurchase_Insert.EditPurchase(v_no);
                SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
                Con.Open();
                SqlDataAdapter _da1 = new SqlDataAdapter("Select * From Account_Master where A_Level<0", Con);
                DataTable _dt1 = new DataTable();
                _da1.Fill(_dt1);
                ViewBag.MfdList = ToSelectList(_dt1, "A_code", "A_Name");
                ViewBag.Mfr = a_code;
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
                /*List<string> table_data = new List<string>();*/
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
                    /*table_data.Add(Descp[0].P_Part_No);
                    table_data.Add(Descp[0].P_Description);*/
                    PM_Data.Tables[0].Rows[i]["P_Part_No"] = Descp[0].P_Part_No;
                    PM_Data.Tables[0].Rows[i]["P_Description"] = Descp[0].P_Description;
                }
                string cmd3 = "select top 1 Ref_No from I_Ledger where Voucher_NO = '" + v_no + "'";
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
                newPurchase_Insert.Invoice_No = inv_no;
                newPurchase_Insert.Invoice_Date = inv_date;

                ViewBag.PL = PM_Data.Tables[0];
                V_no = v_no;
                ViewBag.ILedger = 1;
                ViewBag.ALedger = 2;
                ViewBag.Ref_No = ref_no;
                Con.Close();
                return View(newPurchase_Insert);
            /*}
            else
            {
                return RedirectToAction("Err", "Login");
            }*/
        }
        [HttpPost]
        public ActionResult Add_Deleted_Purchase(List<PurchaseTable> Purchase) // add deleted purchase to DB
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "delete from Purchase where Voucher_No ='" + Purchase[0].Voucher_No + "'";
            string cmd2 = "delete from I_Ledger where Voucher_No ='" + Purchase[0].Voucher_No + "'";
            string cmd3 = "delete from A_Ledger where Voucher_No ='" + Purchase[0].Voucher_No + "'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
            SqlCommand SqlCmd3 = new SqlCommand(cmd3, Con);
            SqlCmd1.ExecuteNonQuery();
            SqlCmd2.ExecuteNonQuery();
            SqlCmd3.ExecuteNonQuery();
            for (int i = 0;i<= Purchase.Count - 1; i++)
            {
                string cmd4 = "Update Product_Master set P_Closing_Balance = P_Closing_Balance - '"+Purchase[i].Quantity+"' where P_code ='" + Purchase[i].Pcode + "'";
                SqlCommand SqlCmd4 = new SqlCommand(cmd4, Con);
                SqlCmd4.ExecuteNonQuery();
            }
            Con.Close();
            return Json(Purchase);
        }

        //New Goods Receipt/Issue
        [HttpGet]
        public ActionResult Goods_Receipt_Issue() // Goods Issue View
        {
            if (Session["userID"] != null)
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
                ViewBag.Employee = ToSelectList(_dt3, "Id", "Employee_Name");
                List<SelectListItem> Index = new List<SelectListItem>();
                Index.Add(new SelectListItem { Text = "Goods-Receipt", Value = "1" });
                Index.Add(new SelectListItem { Text = "Goods-Issue", Value = "2" });
                ViewBag.Index = new SelectList(Index, "Value", "Text");
                SqlDataAdapter _da4 = new SqlDataAdapter("Select * From Product_Master where P_Level>1", _con);
                DataTable _dt4 = new DataTable();
                _da4.Fill(_dt4);
                ViewBag.ProductList = ToSelectList(_dt4, "P_code", "P_Name");
                SqlDataAdapter _da5 = new SqlDataAdapter("Select * From Account_Master where A_Level<1", _con);
                DataTable _dt5 = new DataTable();
                _da5.Fill(_dt5);
                ViewBag.MfdList = ToSelectList(_dt5, "A_code", "A_Name");
                _con.Close();
                return View(Model);
            }
            else
            {
                return RedirectToAction("Err", "Login");
            }
        }
        public ActionResult P_to_D(GoodsRI name) // conversion of part_no to description
        {
            NewPurchase_Insert dblogin = new NewPurchase_Insert();
            string Descp = dblogin.P_Description(name.Part_No);
            return Json(Descp, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Add_Goods(List<GoodsRI> data) // For Adding Goods to DB - Json
        {
            Goods_RI dblogin = new Goods_RI();
            int Vno = dblogin.Goods_add(data);
            return Json(Vno);
        }
        public ActionResult P_to_DQ(GoodsRI name) // conversion of part_no to description, qty
        {
            Goods_RI dblogin = new Goods_RI();
            var Descp = dblogin.Descp_Qty(name.Part_No);
            return Json(Descp, JsonRequestBehavior.AllowGet);
        }

        //Goods RI List
        [HttpGet]
        public ActionResult Goods_Receipt_Issue_List () // Goods RI List View
        {
            if (Session["userID"] != null)
            {
                Goods_RI newPurchase_Insert = new Goods_RI();
                var PM_Data = newPurchase_Insert.Goods_List();
                ViewBag.PL = PM_Data;
                return View(PM_Data);
            }
            else
            {
                return RedirectToAction("Err", "Login");
            }
        }

        //Edit Goods Receipt/Issue 
        [HttpGet]
        public ActionResult Goods_RI_Edit(string v_type, int gv_no, DateTime gv_date, string ref_no, DateTime ref_date, int GI, int process, int project, int employee, string note) // Goods RI Edit View
        {
            int vtype = 0;
            if(v_type == "Goods Receipt")
            {
                vtype = 1;
            }
            else
            {
                vtype = 2;
            }
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
            ViewBag.Employee = ToSelectList(_dt3, "Id", "Employee_Name");
            List<SelectListItem> Index = new List<SelectListItem>();
            Index.Add(new SelectListItem { Text = "Goods-Receipt", Value = "1" });
            Index.Add(new SelectListItem { Text = "Goods-Issue", Value = "2" });
            ViewBag.Index = new SelectList(Index, "Value", "Text");
            SqlDataAdapter _da4 = new SqlDataAdapter("Select * From Product_Master where P_Level>1", _con);
            DataTable _dt4 = new DataTable();
            _da4.Fill(_dt4);
            ViewBag.ProductList = ToSelectList(_dt4, "P_code", "P_Name");
            SqlDataAdapter _da5 = new SqlDataAdapter("Select * From Account_Master where A_Level<1", _con);
            DataTable _dt5 = new DataTable();
            _da5.Fill(_dt5);
            ViewBag.MfdList = ToSelectList(_dt5, "A_code", "A_Name");
            _con.Close();
            GoodsRI goodsRI = new GoodsRI();
            DataSet dataSet = goodsRI.EditGoods(vtype, gv_no);
            dataSet.Tables[0].Columns.Add("P_Part_No");
            dataSet.Tables[0].Columns.Add("P_Description");
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                string Text = dataSet.Tables[0].Rows[i]["P_code"].ToString();
                NewPurchase_Insert dblogin = new NewPurchase_Insert();
                var Descp = dblogin.Pcode_to_PartNo(Text);
                dataSet.Tables[0].Rows[i]["P_Part_No"] = Descp[0].P_Part_No;
                dataSet.Tables[0].Rows[i]["P_Description"] = Descp[0].P_Description;
            }
            goodsRI.Index_Type = vtype;
            goodsRI.Voucher_No = gv_no;
            goodsRI.Voucher_Date = gv_date;
            goodsRI.Ref_No = ref_no.ToString();
            /*goodsRI.Ref_Date = ref_date;*/
            goodsRI.GI_Tag = GI.ToString();
            goodsRI.Process_Tag = process.ToString();
            goodsRI.Project = project.ToString();
            goodsRI.Employee = employee.ToString(); 
            if(note != null)
            {
                goodsRI.Note = note.ToString();
            }
            ViewBag.Goods = dataSet.Tables[0];
            string date = ref_date.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);
            ViewBag.date = date;
            return View(goodsRI);
        }
        [HttpPost]
        public ActionResult Edited_Goods_RI(List<GoodsRI> data) // Adding Edited Goods RI to DB
        {
            Goods_RI goods_RI = new Goods_RI();
            goods_RI.Update_GoodsRI_json(data);
            return Json(data);
        }
        public ActionResult Goods_ED(GoodsRI name) // For Delete individual row from DB
        {
            Goods_RI dblogin = new Goods_RI();
            dblogin.Update_Close_Bal(name.Part_No, name.Index_Type, name.Voucher_No);
            return Json(name, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PM_List(GoodsRI name) // To show full Product Master stocks from DB
        {
            Goods_RI dblogin = new Goods_RI();
            var Descp = dblogin.PM_list(name.Package_letter,name.Value_letter,name.Partno_letter,name.Descp_letter);
            return Json(Descp);
        }

        //Delete Goods Receipt/Issue
        [HttpGet]
        public ActionResult Delete_GoodsRI_View(string v_type, int gv_no, DateTime gv_date, string ref_no, DateTime ref_date, int GI, int process, int project, int employee, string note) // delete GoodsRI view
        {
            var roll = Convert.ToInt32(Session["roll"]);
            if (Session["userID"] != null && roll == 1)
            {
                int vtype = 0;
                if (v_type == "Goods Receipt")
                {
                    vtype = 1;
                }
                else
                {
                    vtype = 2;
                }
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
                List<SelectListItem> Index = new List<SelectListItem>();
                Index.Add(new SelectListItem { Text = "Goods-Receipt", Value = "1" });
                Index.Add(new SelectListItem { Text = "Goods-Issue", Value = "2" });
                ViewBag.Index = new SelectList(Index, "Value", "Text");
                _con.Close();
                GoodsRI goodsRI = new GoodsRI();
                DataSet dataSet = goodsRI.EditGoods(vtype, gv_no);
                dataSet.Tables[0].Columns.Add("P_Part_No");
                dataSet.Tables[0].Columns.Add("P_Description");
                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    string Text = dataSet.Tables[0].Rows[i]["P_code"].ToString();
                    NewPurchase_Insert dblogin = new NewPurchase_Insert();
                    var Descp = dblogin.Pcode_to_PartNo(Text);
                    dataSet.Tables[0].Rows[i]["P_Part_No"] = Descp[0].P_Part_No;
                    dataSet.Tables[0].Rows[i]["P_Description"] = Descp[0].P_Description;
                }
                goodsRI.Index_Type = vtype;
                goodsRI.Voucher_No = gv_no;
                goodsRI.Voucher_Date = gv_date;
                goodsRI.Ref_No = ref_no.ToString();
                /*goodsRI.Ref_Date = ref_date;*/
                goodsRI.GI_Tag = GI.ToString();
                goodsRI.Process_Tag = process.ToString();
                goodsRI.Project = project.ToString();
                goodsRI.Employee = employee.ToString();
                if (note != null)
                {
                    goodsRI.Note = note.ToString();
                }
                ViewBag.Goods = dataSet.Tables[0];
                string date = ref_date.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);
                ViewBag.date = date;
                return View(goodsRI);
            }
            else
            {
                return RedirectToAction("Err", "Login");
            }
        }
        [HttpPost] 
        public ActionResult Add_Deleted_GoodsRI(List<GoodsRI> data) // add deleted GoodsRI to DB
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "delete from I_Ledger where Goods_Voucher_No ='" + data[0].Voucher_No + "' and Voucher_Type = '"+data[0].Index_Type+"'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlCmd1.ExecuteNonQuery();
            if (data[0].Index_Type == 1)
            {
                for (int i = 0; i <= data.Count - 1; i++)
                {
                    string cmd4 = "Update Product_Master set P_Closing_Balance = P_Closing_Balance - '" + data[i].Quantity + "' where P_code ='" + data[i].P_code + "'";
                    SqlCommand SqlCmd4 = new SqlCommand(cmd4, Con);
                    SqlCmd4.ExecuteNonQuery();
                }
            }
            else
            {
                for (int i = 0; i <= data.Count - 1; i++)
                {
                    string cmd4 = "Update Product_Master set P_Closing_Balance = P_Closing_Balance + '" + data[i].Quantity + "' where P_code ='" + data[i].P_code + "'";
                    SqlCommand SqlCmd4 = new SqlCommand(cmd4, Con);
                    SqlCmd4.ExecuteNonQuery();
                }
            }
            Con.Close();
            return Json(data);
        }

        // Print Goods
        public ActionResult Print_Goods(string v_type, int gv_no, DateTime gv_date, string ref_no, DateTime ref_date, int GI, int process, int project, int employee, string note)
        {
            int vtype = 0;
            if (v_type == "Goods Receipt")
            {
                vtype = 1;
            }
            else
            {
                vtype = 2;
            }
            SqlConnection _con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            _con.Open();
            GoodsRI goodsRI = new GoodsRI();
            DataSet dataSet = goodsRI.EditGoods(vtype, gv_no);
            dataSet.Tables[0].Columns.Add("P_Part_No");
            dataSet.Tables[0].Columns.Add("P_Description");
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                string Text = dataSet.Tables[0].Rows[i]["P_code"].ToString();
                NewPurchase_Insert dblogin = new NewPurchase_Insert();
                var Descp = dblogin.Pcode_to_PartNo(Text);
                dataSet.Tables[0].Rows[i]["P_Part_No"] = Descp[0].P_Part_No;
                dataSet.Tables[0].Rows[i]["P_Description"] = Descp[0].P_Description;
            }
            goodsRI.Index_Type = vtype;
            goodsRI.Voucher_No = gv_no;
            goodsRI.Voucher_Date = gv_date;
            goodsRI.Ref_No = ref_no.ToString();
            /*goodsRI.Ref_Date = ref_date;*/
            goodsRI.GI_Tag = GI.ToString();
            goodsRI.Process_Tag = process.ToString();
            goodsRI.Project = project.ToString();
            goodsRI.Employee = employee.ToString();
            if (note != null)
            {
                goodsRI.Note = note.ToString();
            }
            ViewBag.Goods = dataSet.Tables[0];
            string date = ref_date.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);
            ViewBag.date = date;
            return View(goodsRI);
        }
        public ActionResult Print_preview(GoodsRI name)
        {
            Goods_RI dblogin = new Goods_RI();
            var Descp = dblogin.Preview_List(name.Index_Type, name.Voucher_No);
            return Json(Descp);
        }

        // Add Product
        public ActionResult Add_Product(GoodsRI name) // add new products to db
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "select P_Part_No from Product_Master where P_Part_No = '" + name.Add_PartNo + "'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlDataReader dr = SqlCmd1.ExecuteReader();
            string ItemQm = string.Empty;
            while (dr.Read())
            {
                ItemQm = dr["P_Part_No"].ToString();
            }
            if (ItemQm != name.Add_PartNo)
            {
                ProductInsert dblogin = new ProductInsert();
                int userid = dblogin.AddData(name.Add_Group, name.Add_Name, name.Add_Manufacturer,name.Add_Package,name.Add_Value, name.Add_PartNo, name.Add_Description, name.Add_Cost, name.Add_MRP, name.Add_SellPrice);
                Con.Close();
                return Json(ItemQm);
            }
            else
            {
                Con.Close();
                return Json(ItemQm);
            }
            
        }

        // New Purchase Order
        public ActionResult Purchase_Order_View() // PO View
        {
            New_Purchase new_Purchase = new New_Purchase();
            new_Purchase.Purchase_Order_Date = DateTime.Today;
            new_Purchase.Ref_Date = DateTime.Today;
            new_Purchase.Final_Tax1 = 18;

            SqlConnection _con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            _con.Open();
            /*SqlDataAdapter _da = new SqlDataAdapter("Select P_Description From Product_Master where P_Level<0", _con);
            DataTable _dt = new DataTable();
            _da.Fill(_dt);
            ViewBag.ProductList = ToSelectList(_dt, "P_Description", "P_Description");*/
            SqlDataAdapter _da1 = new SqlDataAdapter("Select * From Account_Master where A_Level<0", _con);
            DataTable _dt1 = new DataTable();
            _da1.Fill(_dt1);
            ViewBag.MfdList = ToSelectList(_dt1, "A_code", "A_Name");
            SqlDataAdapter _da4 = new SqlDataAdapter("Select * From Product_Master where P_Level>1", _con);
            DataTable _dt4 = new DataTable();
            _da4.Fill(_dt4);
            ViewBag.ProductList = ToSelectList(_dt4, "P_code", "P_Name");
            SqlDataAdapter _da5 = new SqlDataAdapter("Select * From Account_Master where A_Level<1", _con);
            DataTable _dt5 = new DataTable();
            _da5.Fill(_dt5);
            ViewBag.Mfd = ToSelectList(_dt5, "A_code", "A_Name");
            SqlDataAdapter _da2 = new SqlDataAdapter("Select * From Project_Master", _con);
            DataTable _dt2 = new DataTable();
            _da2.Fill(_dt2);
            ViewBag.Project = ToSelectList(_dt2, "Project_Id", "Project_Name");
            _con.Close();
            return View(new_Purchase);
        }
        public ActionResult Add_PO_to_DB(List<PurchaseTable> Purchase) // add PO to db
        {
            float tax_per = Purchase[0].Tax_Per;
            double tax_amt = Purchase[0].Tax_Total;
            int project = int.Parse(Purchase[0].project);
            int Quantity = Purchase[0].final_Qty;
            double Total = Purchase[0].final_Sub_Total;
            double Final_Total = Purchase[0].final_total;
            NewPurchase_Insert purchase = new NewPurchase_Insert();
            int v_no = purchase.Add_PO(Purchase, Quantity, Total, Final_Total, project, tax_per, tax_amt);
            return Json(v_no);
        }

        // Purchase Order List
        public ActionResult PO_List() // PO list
        {
            NewPurchase_Insert newPurchase_Insert = new NewPurchase_Insert();
            var PM_Data = newPurchase_Insert.PO_List();
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            for (int i = 0; i < PM_Data.Count; i++)
            {
                string cmd1 = "select A_Name from Account_Master where A_code = '" + PM_Data[i].acode + "'";
                SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
                SqlDataReader dr = SqlCmd1.ExecuteReader();
                while (dr.Read())
                {
                    string Mfr = dr["A_Name"].ToString();
                    PM_Data[i].Supplier = Mfr;
                }
                dr.Close();
                string cmd2 = "select A_Name from Account_Master where A_code = '" + PM_Data[i].billto_acode + "'";
                SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
                SqlDataReader dr1 = SqlCmd2.ExecuteReader();
                while (dr1.Read())
                {
                    string Mfr = dr1["A_Name"].ToString();
                    PM_Data[i].BillTo = Mfr;
                }
                dr1.Close();
            }
            ViewBag.PL = PM_Data;
            Con.Close();
            return View(PM_Data);
        }

        // Edit Purchase Order
        public ActionResult PO_to_EditPurchase(int PO_No, DateTime PO_Date, string Ref_No, DateTime Ref_Date, string Supplier, string Billto) // Add PO to Purchase View
        {
            New_Purchase newPurchase_Insert = new New_Purchase();
            PurchaseTable mfr = new PurchaseTable();
            DataSet PM_Data = newPurchase_Insert.Edit_PO_to_Purchase(PO_No);
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            SqlDataAdapter _da1 = new SqlDataAdapter("Select * From Account_Master where A_Level<0", Con);
            DataTable _dt1 = new DataTable();
            _da1.Fill(_dt1);
            ViewBag.MfdList = ToSelectList(_dt1, "A_code", "A_Name");
            SqlDataAdapter _da4 = new SqlDataAdapter("Select * From Product_Master where P_Level>1", Con);
            DataTable _dt4 = new DataTable();
            _da4.Fill(_dt4);
            ViewBag.ProductList = ToSelectList(_dt4, "P_code", "P_Name");
            SqlDataAdapter _da5 = new SqlDataAdapter("Select * From Account_Master where A_Level<1", Con);
            DataTable _dt5 = new DataTable();
            _da5.Fill(_dt5);
            ViewBag.Mfd = ToSelectList(_dt5, "A_code", "A_Name");
            SqlDataAdapter _da2 = new SqlDataAdapter("Select * From Project_Master", Con);
            DataTable _dt2 = new DataTable();
            _da2.Fill(_dt2);
            ViewBag.Project = ToSelectList(_dt2, "Project_Id", "Project_Name");
            PM_Data.Tables[0].Columns.Add("P_Part_No");
            PM_Data.Tables[0].Columns.Add("P_Description");
            /*List<string> table_data = new List<string>();*/
            for (int i = 0; i < PM_Data.Tables[0].Rows.Count; i++)
            {
                string Text = PM_Data.Tables[0].Rows[i]["P_code"].ToString();
                NewPurchase_Insert dblogin = new NewPurchase_Insert();
                var Descp = dblogin.Pcode_to_PartNo(Text);
                PM_Data.Tables[0].Rows[i]["P_Part_No"] = Descp[0].P_Part_No;
                PM_Data.Tables[0].Rows[i]["P_Description"] = Descp[0].P_Description;
            }
            string acode = PM_Data.Tables[0].Rows[0]["A_code"].ToString();
            newPurchase_Insert.Purchase_Order_No = PO_No;
            newPurchase_Insert.Purchase_Order_Date = PO_Date;
            newPurchase_Insert.Ref_Date = Ref_Date;
            newPurchase_Insert.Supplier = Supplier;
            newPurchase_Insert.BillTo = Billto;
            newPurchase_Insert.Final_Tax1 = (double)PM_Data.Tables[0].Rows[0]["PO_Tax_Per"];
            newPurchase_Insert.Project = PM_Data.Tables[0].Rows[0]["Project"].ToString();
            newPurchase_Insert.sup_val = PM_Data.Tables[0].Rows[0]["A_code"].ToString();
            newPurchase_Insert.billval = PM_Data.Tables[0].Rows[0]["BillTo_Acode"].ToString();
            ViewBag.Project_data = PM_Data.Tables[0].Rows[0]["Project"].ToString();
            ViewBag.PL = PM_Data.Tables[0];
            ViewBag.ILedger = 1;
            ViewBag.ALedger = 2;
            ViewBag.Final_Total = PM_Data.Tables[0].Rows[0]["PO_Final_Total"].ToString();
            ViewBag.Final_Tax = PM_Data.Tables[0].Rows[0]["PO_Tax_Total"].ToString(); ;
            Con.Close();
            return View(newPurchase_Insert);
        }
        public ActionResult PO_ED(GoodsRI name) // For Delete individual row from DB
        {
            NewPurchase_Insert dblogin = new NewPurchase_Insert();
            dblogin.Update_Close_Bal_PO(name.Part_No, name.Index_Type, name.Voucher_No);
            return Json(name, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Updated_PO_to_DB (List<PurchaseTable> Purchase)
        {
            int project = int.Parse(Purchase[0].project);
            float taxper = Purchase[0].Tax_Per;
            double taxamt = Purchase[0].Tax_Total;
            string supplier = Purchase[0].supplier;
            string billto = Purchase[0].BillTo;
            int Quantity = Purchase[0].final_Qty;
            double Total = Purchase[0].final_Sub_Total;
            double Final_Total = Purchase[0].final_total;
            string refno = Purchase[0].Invoice_No.ToUpper();
            NewPurchase_Insert purchase = new NewPurchase_Insert();
            purchase.Edit_PO(Purchase, Quantity, Total, Final_Total, project, supplier, refno, billto, taxper, taxamt);
            return Json(Purchase);
        } // update edited PO to db

        // Delete Purchase Order
        public ActionResult PO_Delete(int PO_No, DateTime PO_Date, string Ref_No, DateTime Ref_Date, string Supplier) // PO Delete View
        {
            New_Purchase newPurchase_Insert = new New_Purchase();
            PurchaseTable mfr = new PurchaseTable();
            DataSet PM_Data = newPurchase_Insert.Edit_PO_to_Purchase(PO_No);
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            SqlDataAdapter _da1 = new SqlDataAdapter("Select * From Account_Master where A_Level<0", Con);
            DataTable _dt1 = new DataTable();
            _da1.Fill(_dt1);
            ViewBag.MfdList = ToSelectList(_dt1, "A_code", "A_Name");
            SqlDataAdapter _da4 = new SqlDataAdapter("Select * From Product_Master where P_Level>1", Con);
            DataTable _dt4 = new DataTable();
            _da4.Fill(_dt4);
            ViewBag.ProductList = ToSelectList(_dt4, "P_code", "P_Name");
            SqlDataAdapter _da5 = new SqlDataAdapter("Select * From Account_Master where A_Level<1", Con);
            DataTable _dt5 = new DataTable();
            _da5.Fill(_dt5);
            ViewBag.Mfd = ToSelectList(_dt5, "A_code", "A_Name");
            SqlDataAdapter _da2 = new SqlDataAdapter("Select * From Project_Master", Con);
            DataTable _dt2 = new DataTable();
            _da2.Fill(_dt2);
            ViewBag.Project = ToSelectList(_dt2, "Project_Id", "Project_Name");
            PM_Data.Tables[0].Columns.Add("P_Part_No");
            PM_Data.Tables[0].Columns.Add("P_Description");
            /*List<string> table_data = new List<string>();*/
            for (int i = 0; i < PM_Data.Tables[0].Rows.Count; i++)
            {
                string Text = PM_Data.Tables[0].Rows[i]["P_code"].ToString();
                NewPurchase_Insert dblogin = new NewPurchase_Insert();
                var Descp = dblogin.Pcode_to_PartNo(Text);
                PM_Data.Tables[0].Rows[i]["P_Part_No"] = Descp[0].P_Part_No;
                PM_Data.Tables[0].Rows[i]["P_Description"] = Descp[0].P_Description;
            }
            newPurchase_Insert.Purchase_Order_No = PO_No;
            newPurchase_Insert.Purchase_Order_Date = PO_Date;
            newPurchase_Insert.Ref_Date = Ref_Date;
            newPurchase_Insert.Supplier = PM_Data.Tables[0].Rows[0]["A_code"].ToString();
            newPurchase_Insert.Project = PM_Data.Tables[0].Rows[0]["Project"].ToString();

            ViewBag.Project_data = PM_Data.Tables[0].Rows[0]["Project"].ToString();
            ViewBag.PL = PM_Data.Tables[0];
            ViewBag.ILedger = 1;
            ViewBag.ALedger = 2;
            ViewBag.Final_Total = PM_Data.Tables[0].Rows[0]["PO_Final_Total"].ToString();
            Con.Close();
            return View(newPurchase_Insert);
        }
        [HttpPost]
        public ActionResult Add_Deleted_PO(List<PurchaseTable> data) // add deleted GoodsRI to DB
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "delete from Purchase_Order where PO_No = '"+data[0].PO_No+"'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlCmd1.ExecuteNonQuery();
            Con.Close();
            return Json(data);
        }
    }
}