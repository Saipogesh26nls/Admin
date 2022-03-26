using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Models;

namespace Admin.Controllers
{
    public class UserController : Controller
    {
        // Employee
        public ActionResult Add_New_Employee() //Employee View
        {
            return View();
        }
        public ActionResult Employee_to_DB(UserModel name) //Add New Employee to DB
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "select Employee_Name from Employee_Master where Employee_Id = '"+name.Employee_Id+"'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlDataReader dr = SqlCmd1.ExecuteReader();
            string ItemQm = string.Empty;
            while (dr.Read())
            {
                ItemQm = dr["Employee_Name"].ToString();
            }
            dr.Close();
            if (ItemQm != name.Employee_Name)
            {
                SqlCommand sql_cmnd = new SqlCommand("[dbo].[Add_Employee]", Con);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@emp_name", SqlDbType.NVarChar).Value = name.Employee_Name;
                sql_cmnd.Parameters.AddWithValue("@emp_designation", SqlDbType.NVarChar).Value = name.Employee_Designation;
                sql_cmnd.Parameters.AddWithValue("@emp_id", SqlDbType.NVarChar).Value = name.Employee_Id;
                sql_cmnd.ExecuteNonQuery();
                return Json(ItemQm);
            }
            else
            {
                return Json(ItemQm);
            }
        }
        // Project
        public ActionResult Add_New_Project() //Project View
        {
            return View();
        }
        public ActionResult Project_to_DB(UserModel name) //Add New Project to DB
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "select Project_Name from Project_Master where Project_Name = '" + name.Project_Name + "'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlDataReader dr = SqlCmd1.ExecuteReader();
            string ItemQm = string.Empty;
            while (dr.Read())
            {
                ItemQm = dr["Project_Name"].ToString();
            }
            dr.Close();
            if (ItemQm != name.Project_Name)
            {
                SqlCommand sql_cmnd = new SqlCommand("[dbo].[Add_Project]", Con);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@project_name", SqlDbType.NVarChar).Value = name.Project_Name;
                sql_cmnd.Parameters.AddWithValue("@company", SqlDbType.NVarChar).Value = name.Company;
                sql_cmnd.ExecuteNonQuery();
                return Json(ItemQm);
            }
            else
            {
                return Json(ItemQm);
            }
        }
        //Process
        public ActionResult Add_New_Process() //Process View
        {
            return View();
        }
        public ActionResult Process_to_DB(UserModel name) //Add New Process to DB
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "select Process_Name from Process_Tag where Process_Name = '" + name.Process_Name + "'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlDataReader dr = SqlCmd1.ExecuteReader();
            string ItemQm = string.Empty;
            while (dr.Read())
            {
                ItemQm = dr["Process_Name"].ToString();
            }
            dr.Close();
            if (ItemQm != name.Process_Name)
            {
                SqlCommand sql_cmnd = new SqlCommand("[dbo].[Add_Process]", Con);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@process_name", SqlDbType.NVarChar).Value = name.Process_Name;
                sql_cmnd.ExecuteNonQuery();
                return Json(ItemQm);
            }
            else
            {
                return Json(ItemQm);
            }
        }
    }
}