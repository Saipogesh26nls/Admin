using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Admin.Models;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Admin.Controllers
{
    public class ProductController : Controller
    {
        [HttpGet]
        public ActionResult ProductEntry() // Product entry View
        {
            SqlConnection _con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            _con.Open();
            SqlDataAdapter _da = new SqlDataAdapter("Select * From Product_Master where P_Level>1", _con);
            DataTable _dt = new DataTable();
            _da.Fill(_dt);
            ViewBag.ProductList = ToSelectList(_dt,"P_code","P_Name");
            SqlDataAdapter _da1 = new SqlDataAdapter("Select * From Manufacturer_Details", _con);
            DataTable _dt1 = new DataTable();
            _da1.Fill(_dt1);
            ViewBag.MfdList = ToSelectList(_dt1, "M_Id", "M_Name");
            /*SqlDataAdapter _da2 = new SqlDataAdapter("Select * From Manufacturer_Details", _con);
            DataTable _dt2 = new DataTable();
            _da2.Fill(_dt2);
            ViewBag.RegList = ToSelectList(_dt2, "M_Region", "M_Region");*/
            return View();
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
        public ActionResult ProductEntry(ProductModel newuser) // Adding Data to DB
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "select P_Part_No from Product_Master where P_Part_No = '" + newuser.P_Part_No + "'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlDataReader dr = SqlCmd1.ExecuteReader();
            string ItemQm = string.Empty;
            while (dr.Read())
            {
                ItemQm = dr["P_Part_No"].ToString();
            }
            if(ItemQm != newuser.P_Part_No)
            {
                ProductInsert dblogin = new ProductInsert();
                int userid = dblogin.AddData(newuser.P_Name, newuser.P_Disp_Name, newuser.P_Manufacturer, newuser.P_Region, newuser.P_Part_No, newuser.P_Description, newuser.P_Cost, newuser.P_MRP, newuser.P_SP);
                Session["P_Id"] = userid;
                newuser.Reg_Success = "Registered Successfully !!!!";
                ProductEntry();
                return View("ProductEntry", newuser);
            }
            else
            {
                newuser.P_Part_No = "Part No is already exists !!!";
                ProductEntry();
                return View("ProductEntry", newuser);
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
                        *//*SolidBrush whiteBrush = new SolidBrush(Color.White);
                        graphics.FillRectangle(whiteBrush, 0, 0, bitMap.Width, bitMap.Height);*//*
                        SolidBrush blackBrush = new SolidBrush(Color.Black);
                        *//*graphics.DrawString("*" + barcode + "*", oFont, blackBrush, point);*//*
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