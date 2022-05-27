using Admin.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Xml.Linq;
using System;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Globalization;

namespace Admin.Controllers
{
    public class BOMController : Controller
    {
        // Add New BOM
        public ActionResult BOM_Add_Data(BOMFields data) // BOM Add Data View
        {
            if (Session["userID"] != null)
            {
                data.BOM_Date = DateTime.Today;
                SqlConnection _con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
                _con.Open();
                SqlDataAdapter _da4 = new SqlDataAdapter("Select * From Product_Master where P_Level>1", _con);
                DataTable _dt4 = new DataTable();
                _da4.Fill(_dt4);
                ViewBag.ProductList = ToSelectList(_dt4, "P_code", "P_Name");
                SqlDataAdapter _da1 = new SqlDataAdapter("Select * From Account_Master where A_Level<0", _con);
                DataTable _dt1 = new DataTable();
                _da1.Fill(_dt1);
                ViewBag.MfdList = ToSelectList(_dt1, "A_code", "A_Name");
                _con.Close();
                return View(data);
            }
            else
            {
                return RedirectToAction("Err", "Login");
            }
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
        public ActionResult Order(List<BOM_Table> name) // Add List to DB
        {
            BOM_Insert BOM_SP = new BOM_Insert();
            int BOM = BOM_SP.AddOrderDetails(name);
            return Json(BOM);
        }
        public ActionResult Partno_to_Descp (BOMFields name) // conversion of part_no to description
        {
            BOM_Insert dblogin = new BOM_Insert();
            string Descp = dblogin.SP_Description(name.SP_Part_No);
            return Json(Descp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult P_to_DQ(BOMFields name) // conversion of part_no to description, qty
        {
            BOM_Insert dblogin = new BOM_Insert();
            var Descp = dblogin.Descp_Qty(name.Part_No);
            return Json(Descp, JsonRequestBehavior.AllowGet);
        }

        // BOM List
        [HttpGet]
        public ActionResult BOM_List() // BOM List View
        {
            if (Session["userID"] != null)
            {
                List<BOM_List> ItemQm = new List<BOM_List>();
                List<int> bom_no = new List<int>();
                SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
                Con.Open();
                string cmd2 = "SELECT BOM_No FROM BOM GROUP BY BOM_No HAVING COUNT(*)>0";
                SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
                SqlDataReader dr1 = SqlCmd2.ExecuteReader();
                while (dr1.Read())
                {
                    bom_no.Add(Convert.ToInt32(dr1["BOM_No"]));
                }
                dr1.Close();
                if (bom_no.Count == 0)
                {
                    Con.Close();
                    return View(ItemQm);
                }
                else
                {
                    for (int i = 0; i < bom_no.Count; i++)
                    {
                        string cmd1 = "select Top 1 BOM_No, BOM_Date, MP_Code from BOM where BOM_No = '" + bom_no[i] + "' ORDER BY BOM_No Asc;";
                        SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
                        SqlDataReader dr = SqlCmd1.ExecuteReader();
                        while (dr.Read())
                        {
                            ItemQm.Add(new BOM_List
                            {
                                BOM_No = (int)dr["BOM_No"],
                                BOM_Date = dr["BOM_Date"].ToString(),
                                SP_code = dr["MP_Code"].ToString()
                            }
                            );
                        }
                        dr.Close();
                    }
                    for (int i = 0; i < bom_no.Count; i++)
                    {
                        string cmd1 = "select P_Part_No,P_Name from Product_Master where P_code = '" + ItemQm[i].SP_code + "'";
                        SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
                        SqlDataReader dr = SqlCmd1.ExecuteReader();
                        while (dr.Read())
                        {
                            for(int j = 0; j < ItemQm.Count; j++)
                            {
                                ItemQm[i].Part_No = dr["P_Part_No"].ToString();
                                ItemQm[i].Product_Name = dr["P_Name"].ToString();
                            }
                        }
                        dr.Close();
                    }
                    Con.Close();
                    return View(ItemQm);
                }
            }
            else
            {
                return RedirectToAction("Err", "Login");
            }
        }

        // Edit BOM
        [HttpGet]
        public ActionResult BOM_Edit_Delete(int BOM_No, DateTime bomdate, string spcode) // BOM Edit Delete View
        {
            if (Session["userID"] != null)
            {
                SqlConnection _con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
                _con.Open();
                SqlDataAdapter _da4 = new SqlDataAdapter("Select * From Product_Master where P_Level>1", _con);
                DataTable _dt4 = new DataTable();
                _da4.Fill(_dt4);
                ViewBag.ProductList = ToSelectList(_dt4, "P_code", "P_Name");
                SqlDataAdapter _da1 = new SqlDataAdapter("Select * From Account_Master where A_Level<0", _con);
                DataTable _dt1 = new DataTable();
                _da1.Fill(_dt1);
                ViewBag.MfdList = ToSelectList(_dt1, "A_code", "A_Name");
                _con.Close();
                BOMFields bOMFields = new BOMFields();
                DataSet ds = bOMFields.EditBOM(BOM_No, spcode);
                ds.Tables[0].Columns.Add("P_Part_No");
                ds.Tables[0].Columns.Add("P_Description");
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    string Text = ds.Tables[0].Rows[i]["SP_Code"].ToString();
                    NewPurchase_Insert dblogin = new NewPurchase_Insert();
                    var Descp = dblogin.Pcode_to_PartNo(Text);
                    ds.Tables[0].Rows[i]["P_Part_No"] = Descp[0].P_Part_No;
                    ds.Tables[0].Rows[i]["P_Description"] = Descp[0].P_Description;
                }
                NewPurchase_Insert dblogin1 = new NewPurchase_Insert();
                var sp_partno = dblogin1.Pcode_to_PartNo(spcode);
                bOMFields.BOM_No = BOM_No.ToString();
                bOMFields.SP_Part_No = sp_partno[0].P_Part_No;
                bOMFields.BOM_Date = bomdate;
                ViewBag.Goods = ds.Tables[0];
                return View(bOMFields);
            }
            else
            {
                return RedirectToAction("Err", "Login");
            }
        }
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
                int userid = dblogin.AddData(name.Add_Group, name.Add_Name, name.Add_Manufacturer, name.Add_Package, name.Add_Value, name.Add_PartNo, name.Add_Description, name.Add_Cost, name.Add_MRP, name.Add_SellPrice);
                Con.Close();
                return Json(ItemQm);
            }
            else
            {
                Con.Close();
                return Json(ItemQm);
            }

        }
        public ActionResult BOMEdit(List<BOMEdit> name) // add edited BOM to db
        {
            DateTime v = DateTime.Parse(name[0].BOM_Date);
            string bom_date = v.ToString("yyyy-MM-dd");
            BOM_Insert BOM_SP = new BOM_Insert();
            int BOM = BOM_SP.EditOrder(name, bom_date);
            return Json(BOM);
        } 
        public ActionResult Delete_BOM_row(BOMEdit name) // remove deleted row from db
        {
            BOM_Insert bOM_Insert = new BOM_Insert();
            bOM_Insert.Update_BOM_Row(name.Part_No, name.BOM_No);
            return Json(name);
        }

        // Delete BOM
        public ActionResult Delete_BOM_View(int BOM_No, DateTime bomdate, string spcode)
        {
            var roll = Convert.ToInt32(Session["roll"]);
            if (Session["userID"] != null && roll == 1)
            {
                SqlConnection _con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
                _con.Open();
                SqlDataAdapter _da4 = new SqlDataAdapter("Select * From Product_Master where P_Level>1", _con);
                DataTable _dt4 = new DataTable();
                _da4.Fill(_dt4);
                ViewBag.ProductList = ToSelectList(_dt4, "P_code", "P_Name");
                SqlDataAdapter _da1 = new SqlDataAdapter("Select * From Account_Master where A_Level<0", _con);
                DataTable _dt1 = new DataTable();
                _da1.Fill(_dt1);
                ViewBag.MfdList = ToSelectList(_dt1, "A_code", "A_Name");
                _con.Close();
                BOMFields bOMFields = new BOMFields();
                DataSet ds = bOMFields.EditBOM(BOM_No, spcode);
                ds.Tables[0].Columns.Add("P_Part_No");
                ds.Tables[0].Columns.Add("P_Description");
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    string Text = ds.Tables[0].Rows[i]["SP_Code"].ToString();
                    NewPurchase_Insert dblogin = new NewPurchase_Insert();
                    var Descp = dblogin.Pcode_to_PartNo(Text);
                    ds.Tables[0].Rows[i]["P_Part_No"] = Descp[0].P_Part_No;
                    ds.Tables[0].Rows[i]["P_Description"] = Descp[0].P_Description;
                }
                NewPurchase_Insert dblogin1 = new NewPurchase_Insert();
                var sp_partno = dblogin1.Pcode_to_PartNo(spcode);
                bOMFields.BOM_No = BOM_No.ToString();
                bOMFields.SP_Part_No = sp_partno[0].P_Part_No;
                bOMFields.BOM_Date = bomdate;
                ViewBag.Goods = ds.Tables[0];
                return View(bOMFields);
            }
            else
            {
                return RedirectToAction("Err", "Login");
            }
        } // Delete BOM View
        public ActionResult Remove_BOM(List<BOMEdit> data)
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "delete from BOM where BOM_No = '" + data[0].BOM_No + "'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlCmd1.ExecuteNonQuery();
            Con.Close();
            return Json(data);
        }
    }
}