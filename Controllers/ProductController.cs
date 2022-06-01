using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Admin.Models;

namespace Admin.Controllers
{
    public class ProductController : Controller
    {
        [HttpGet]
        public ActionResult ProductEntry() // Product entry View
        {
            if (Session["userID"] != null)
            {
                SqlConnection _con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
                _con.Open();
                SqlDataAdapter _da = new SqlDataAdapter("Select * From Product_Master where P_Level>1", _con);
                DataTable _dt = new DataTable();
                _da.Fill(_dt);
                ViewBag.ProductList = ToSelectList(_dt, "P_code", "P_Name");
                SqlDataAdapter _da1 = new SqlDataAdapter("Select * From Account_Master where A_Level<1", _con);
                DataTable _dt1 = new DataTable();
                _da1.Fill(_dt1);
                ViewBag.MfdList = ToSelectList(_dt1, "A_code", "A_Name");
                /*SqlDataAdapter _da2 = new SqlDataAdapter("Select * From Manufacturer_Details", _con);
                DataTable _dt2 = new DataTable();
                _da2.Fill(_dt2);
                ViewBag.RegList = ToSelectList(_dt2, "M_Region", "M_Region");*/
                return View();
            }
            else
            {
                return RedirectToAction("Err", "Login");

            }
        }

        [NonAction]
        public SelectList ToSelectList(DataTable table, string valueField, string textField) //  For making Dropdown list
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
        public ActionResult Add_ProductEntry(ProductModel name) // Adding Data to DB
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "select P_Part_No from Product_Master where P_Part_No = '" + name.P_Part_No + "'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlDataReader dr = SqlCmd1.ExecuteReader();
            string ItemQm = string.Empty;
            while (dr.Read())
            {
                ItemQm = dr["P_Part_No"].ToString();
            }
            if(ItemQm != name.P_Part_No)
            {
                ProductInsert dblogin = new ProductInsert();
                int userid = dblogin.AddData(name.P_Name, name.P_Disp_Name, name.P_Manufacturer, name.Package, name.Value, name.P_Part_No, name.P_Description, name.P_Cost, name.P_MRP, name.P_SP);
                Con.Close();
                return Json(ItemQm);
            }
            else
            {
                Con.Close();
                return Json(ItemQm);
            }
            
        }

        public ActionResult Initial_Screen()
        {
            if (Session["userID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Err", "Login");

            }
        }
        /*[HttpPost]
        public ActionResult bar(ProductModel barcode)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (Bitmap bitMap = new Bitmap(barcode.P_Part_No.Length * 40, 80))
                {
                    using (Graphics graphics = Graphics.FromImage(bitMap))
                    {
                        Font oFont = new Font("IDAutomationHC39M", 16);
                        PointF point = new PointF(2f, 2f);
                        SolidBrush whiteBrush = new SolidBrush(Color.White);
                        graphics.FillRectangle(whiteBrush, 0, 0, bitMap.Width, bitMap.Height);
                        SolidBrush blackBrush = new SolidBrush(Color.Black);
                        graphics.DrawString("*" + barcode + "*", oFont, blackBrush, point);
                    }
                    bitMap.Save(memoryStream, ImageFormat.Jpeg);
                    ViewBag.BarcodeImage = "data:image/png;base64," + Convert.ToBase64String(memoryStream.ToArray());
                }
            }
            ProductEntry();
            return View("ProductEntry",barcode);
        }*/
    }
}