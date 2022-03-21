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

namespace Admin.Controllers
{
    public class BOMController : Controller
    {
        // GET: BOM
        static int i = 2;
        public ActionResult BOM_Add_Data(BOMFields data) // BOM Add Data View
        {
            return View(data);
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
        [HttpGet]
        public ActionResult BOM_List() // BOM List
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
                Con.Close();
                return View(ItemQm);
            }
        }
    }
}