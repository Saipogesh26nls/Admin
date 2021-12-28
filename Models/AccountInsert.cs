using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using Admin.Models;
using System.Configuration;

namespace Admin.Models
{
    public class AccountInsert
    {
        public int Add_Data(string cA_Account_Name, string cA_Group, string cA_Door_No, string cA_Street, string cA_Area, string cA_City, string cA_State, string cA_Country, string cA_Pincode, string cA_Contact_No,string cA_Mobile_No, string cA_Email_Id, double cA_Closing_Bal, double cA_Open_Bal)
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();

            SqlCommand sql_cmnd = new SqlCommand("[dbo].[addAccount]", Con);
            sql_cmnd.CommandType = CommandType.StoredProcedure;
            sql_cmnd.Parameters.AddWithValue("@Aname", SqlDbType.NVarChar).Value = cA_Account_Name;
            sql_cmnd.Parameters.AddWithValue("@AGroup", SqlDbType.NVarChar).Value = cA_Group;
            sql_cmnd.Parameters.AddWithValue("@ADoor_no", SqlDbType.NVarChar).Value = cA_Door_No;
            sql_cmnd.Parameters.AddWithValue("@AStreet", SqlDbType.NVarChar).Value = cA_Street;
            sql_cmnd.Parameters.AddWithValue("@AArea", SqlDbType.NVarChar).Value = cA_Area;
            sql_cmnd.Parameters.AddWithValue("@ACity", SqlDbType.NVarChar).Value = cA_City;
            sql_cmnd.Parameters.AddWithValue("@AState", SqlDbType.NVarChar).Value = cA_State;
            sql_cmnd.Parameters.AddWithValue("@ACountry", SqlDbType.NVarChar).Value = cA_Country;
            sql_cmnd.Parameters.AddWithValue("@Pincode", SqlDbType.NVarChar).Value = cA_Pincode;
            sql_cmnd.Parameters.AddWithValue("@Contact_no", SqlDbType.NVarChar).Value = cA_Contact_No;
            sql_cmnd.Parameters.AddWithValue("@Mobile_no", SqlDbType.NVarChar).Value = cA_Mobile_No;
            sql_cmnd.Parameters.AddWithValue("@Email_id", SqlDbType.NVarChar).Value = cA_Email_Id;
            sql_cmnd.Parameters.AddWithValue("@AClosing", SqlDbType.NVarChar).Value = cA_Closing_Bal;
            sql_cmnd.Parameters.AddWithValue("@AOpen", SqlDbType.NVarChar).Value = cA_Open_Bal;

            string cmd1 = "update Account_Master set A_Name = UPPER(A_Name)";
            string cmd2 = "update Address_Details set Door_No = UPPER(Door_No),Street = UPPER(Street),Area = UPPER(Area),City = UPPER(City),State = UPPER(State),Country = UPPER(Country),Pincode = UPPER(Pincode),Contact_No = UPPER(Contact_No),Mobile_No = UPPER(Mobile_No),Email_Id = UPPER(Email_Id)";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
            int var1, var2, var3;
            var1 = sql_cmnd.ExecuteNonQuery();
            var2 = SqlCmd1.ExecuteNonQuery();
            var3 = SqlCmd2.ExecuteNonQuery();

            return var1;
        }
    }
}