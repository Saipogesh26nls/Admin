using Admin.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace Admin.Controllers
{
    public class BOMController : Controller
    {
        // GET: BOM
        public ActionResult BOM_Add_Data()
        {
            if(_list.Count == 0)
            {
                List<BOMFields> itemQm = new List<BOMFields>();
                itemQm.Add(new BOMFields { Part_No1 = "0", Description1 = "0", Quantity1 = "0" });
                ViewBag.item = itemQm;
                return View();
            }
            else
            {
                ViewBag.item = _list;
                return View();
            }
        }

        [HttpPost]
        public ActionResult Main(BOMFields new_data)
        {
            BOM_Insert dblogin = new BOM_Insert();
            string values = dblogin.P_Description(new_data.SP_Part_No);
            if(values == null)
            {
                ViewBag.ErrorMessage = "Part_No is not Found";
                return View("BOM_Add_Data"); 
            }
            else
            {
                Record(new_data.Part_No, new_data.Description, new_data.Quantity, values);
                BOM_Add_Data();
                return View("BOM_Add_Data");
            }
            
        }

        static List<BOMFields> _list = new List<BOMFields>();
        public static List<BOMFields> Record(string tbl_part_no, string tbl_Descp, string tbl_Quan, string SP_Descp)
        {
            _list.Add(new BOMFields { Part_No1 = tbl_part_no, Description1 = tbl_Descp, Quantity1 = tbl_Quan, SP_Description = SP_Descp });
            return (_list);
        }
        public ActionResult Order(BOMFields orderDetail)
        {
            AddOrderDetails(_list);
            orderDetail.Reg_Success = "Registered Successfully !!!!";
            ModelState.Clear();
            return View("BOM_Add_Data", orderDetail);
        }
        [HttpPost]
        public void AddOrderDetails(List<BOMFields> orderDetail)
        {
            int i = 1;
            while (i<=orderDetail.Count())
            {
                if(i== orderDetail.Count())
                {
                    break;
                }
                DB_Con_Str OCon = new DB_Con_Str();
                string ConString = OCon.DB_Data();
                SqlConnection Con = new SqlConnection(ConString);
                SqlCommand sql_cmnd = new SqlCommand("[dbo].[BOM_Prod]", Con);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@part_no", SqlDbType.NVarChar).Value = orderDetail[i].Part_No1.ToUpper();
                sql_cmnd.Parameters.AddWithValue("@descp", SqlDbType.NVarChar).Value = orderDetail[i].Description1.ToUpper();
                sql_cmnd.Parameters.AddWithValue("@mp_descp", SqlDbType.NVarChar).Value = orderDetail[i].SP_Description.ToUpper();
                sql_cmnd.Parameters.AddWithValue("@quantity", SqlDbType.NVarChar).Value = orderDetail[i].Quantity1.ToUpper();
                Con.Open();
                sql_cmnd.ExecuteNonQuery();
                Con.Close();
                i++;
            }
            
        }
    }
}