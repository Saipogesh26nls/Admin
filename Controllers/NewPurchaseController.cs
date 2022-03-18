using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Admin.Models;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Globalization;

namespace Admin.Controllers
{
    public class NewPurchaseController : Controller
    {
        //New Purchase
        [HttpGet]
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
            int Quantity = Purchase[0].final_Qty;
            double Total = Purchase[0].final_Sub_Total;
            double Final_Total = Purchase[0].final_total;
            double Final_Discount = Purchase[0].final_Discount;
            double Final_Tax1 = Purchase[0].final_Tax1;
            double Final_Tax2 = Purchase[0].final_Tax2;
            NewPurchase_Insert purchase = new NewPurchase_Insert();
            int v_no = purchase.Add_Data(Purchase, Quantity, Total, Final_Total, Final_Discount, Final_Tax1, Final_Tax2);
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

        //Purchase List
        [HttpGet]
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
            return View(PM_Data);
        }

        //Edit Purchase
        static int V_no = 0;
        public ActionResult Edit_Purchase_View(int v_no, DateTime v_date, string inv_no, DateTime inv_date, string a_code) // edit purchase view
        {
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
            newPurchase_Insert.Invoice_No = inv_no;
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
            NewPurchase_Insert purchase = new NewPurchase_Insert();
            purchase.Edit_and_Delete(Purchase, Final_Quantity, Final_SubTotal, Final_Discount, Final_Tax1, Final_Tax2, Final_Total);
            return Json(Purchase);
        }

        //Delete Purchase
        [HttpGet]
        public ActionResult Delete_Purchase_view(int v_no, DateTime v_date, string inv_no, DateTime inv_date, string a_code) // delete purchase view
        {
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
            return View(newPurchase_Insert);
        }
        [HttpPost]
        public ActionResult Add_Deleted_Purchase(List<PurchaseTable> Purchase) // add deleted purchase to DB
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "delete from Purchase where Voucher_No ='" + Purchase[0].Voucher_No + "'";
            string cmd2 = "delete from I_Ledger where Voucher_No ='" + Purchase[0].Voucher_No + "'";
            string cmd3 = "delete from A_Ledger where Voucher_No ='" + Purchase[0].Voucher_No + "'";
            string cmd5 = "update Number_Master set Purchase_Voucher_No = Purchase_Voucher_No - 1";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
            SqlCommand SqlCmd3 = new SqlCommand(cmd3, Con);
            SqlCommand SqlCmd5 = new SqlCommand(cmd5, Con);
            SqlCmd1.ExecuteNonQuery();
            SqlCmd2.ExecuteNonQuery();
            SqlCmd3.ExecuteNonQuery();
            SqlCmd5.ExecuteNonQuery();
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
            List<SelectListItem> Index = new List<SelectListItem>();
            Index.Add(new SelectListItem { Text = "Goods-Receipt", Value = "1" });
            Index.Add(new SelectListItem { Text = "Goods-Issue", Value = "2" });
            ViewBag.Index = new SelectList(Index, "Value", "Text");
            _con.Close();
            return View(Model);
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
            /*SqlConnection _con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            _con.Open();
            for(int i=0;i<data.Count();i++)
            {
                string cmd3 = "select P_code from Product_Master where P_Part_No = '" + data[i].Part_No + "'";
                SqlCommand SqlCmd3 = new SqlCommand(cmd3, _con);
                SqlDataReader dr2 = SqlCmd3.ExecuteReader();
                while (dr2.Read())
                {
                    data[i].Part_No = dr2["P_code"].ToString();
                }
                dr2.Close();
            }
            _con.Close();*/
            Goods_RI dblogin = new Goods_RI();
            int Vno = dblogin.Goods_Add_json(data);
            /*var json = JsonConvert.SerializeObject(data);
            var resolveRequest = HttpContext.Request;
            resolveRequest.InputStream.Seek(0, SeekOrigin.Begin);
            string jsonString = new StreamReader(resolveRequest.InputStream).ReadToEnd();
            Goods_RI goods_RI = new Goods_RI();
            goods_RI.Goods_Add_json(jsonString);*/
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
            Goods_RI newPurchase_Insert = new Goods_RI();
            var PM_Data = newPurchase_Insert.Goods_List();
            ViewBag.PL = PM_Data;
            return View(PM_Data);
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
        public ActionResult Edited_Goods_RI() // Adding Edited Goods RI to DB
        {
            /*Goods_RI goods_RI = new Goods_RI();
            goods_RI.Update_GoodsRI_json(data);*/
            var resolveRequest = HttpContext.Request;
            resolveRequest.InputStream.Seek(0, SeekOrigin.Begin);
            string jsonString = new StreamReader(resolveRequest.InputStream).ReadToEnd();
            Goods_RI goods_RI = new Goods_RI();
            goods_RI.Update_GoodsRI_json(jsonString);
            return Json(jsonString);
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
            var Descp = dblogin.PM_list(name.alphabet);
            return Json(Descp);
        }

        //Delete Goods Receipt/Issue
        [HttpGet]
        public ActionResult Delete_GoodsRI_View(string v_type, int gv_no, DateTime gv_date, string ref_no, DateTime ref_date, int GI, int process, int project, int employee, string note) // delete GoodsRI view
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
                string cmd2 = "update Number_Master set GReceipt_Voucher_No = GReceipt_Voucher_No - 1";
                SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
                SqlCmd2.ExecuteNonQuery();
                for (int i = 0; i <= data.Count - 1; i++)
                {
                    string cmd4 = "Update Product_Master set P_Closing_Balance = P_Closing_Balance - '" + data[i].Quantity + "' where P_code ='" + data[i].P_code + "'";
                    SqlCommand SqlCmd4 = new SqlCommand(cmd4, Con);
                    SqlCmd4.ExecuteNonQuery();
                }
            }
            else
            {
                string cmd2 = "update Number_Master set GIssue_Voucher_No = GIssue_Voucher_No - 1";
                SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
                SqlCmd2.ExecuteNonQuery();
                for (int i = 0; i <= data.Count - 1; i++)
                {
                    string cmd4 = "Update Product_Master set P_Closing_Balance = P_Closing_Balance + '" + data[i].Quantity + "' where P_code ='" + data[i].P_code + "'";
                    SqlCommand SqlCmd4 = new SqlCommand(cmd4, Con);
                    SqlCmd4.ExecuteNonQuery();
                }
            }
            return Json(data);
        }
    }
}