﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Mvc;

namespace Admin.Models
{
    public class ProductModel
    {
        [DisplayName("Group")]
        public string P_Name { get; set; }
        [DisplayName("Name")]
        [Required]
        public string P_Disp_Name { get; set; }
        [DisplayName("Manufacturer")]
        public string P_Manufacturer { get; set; }
        [DisplayName("Region")]
        public string P_Region { get; set; }
        [DisplayName("PartNo")]
        public string P_Part_No { get; set; }
        [DisplayName("Description")]
        public string P_Description { get; set; }
        [DisplayName("Cost")]
        public double P_Cost { get; set; }
        [DisplayName("MRP")]
        public double P_MRP { get; set; }
        [DisplayName("SellPrice")]
        public double P_SP { get; set; }
        public string Reg_Success { get; set; }
        public string Package { get; set; }
        public string Value { get; set; }
    }
    public class MFRFields
    {
        [Required]
        [DisplayName("Name")]
        public string M_Name { get; set; }
        [DisplayName("Country")]
        public string M_Country { get; set; }
        [DisplayName("Region")]
        public string M_Region { get; set; }
        [DisplayName("Address")]
        public string M_Address { get; set; }
        [DisplayName("Support No")]
        public string M_Support_No { get; set; }
        [DisplayName("Contact No")]
        public string M_Contact_No { get; set; }
        [DisplayName("Sales No")]
        public string M_Sales_No { get; set; }
        [DisplayName("Website URL")]
        public string M_Website { get; set; }
        [DisplayName("Support E-mail ID")]
        public string M_Support_Email { get; set; }
        [DisplayName("Contact E-mail ID")]
        public string M_Contact_Email { get; set; }
        [DisplayName("Sales E-mail ID")]
        public string M_Sales_Email { get; set; }
        [DisplayName("Payment")]
        public string M_Payment { get; set; }
        public string Regd_Success { get; set; }
    }
    public class GroupFields
    {
        [DisplayName("Group")]
        public string P_Name { get; set; }
        [DisplayName("Name")]
        [Required]
        public string P_Disp_Name { get; set; }
        [DisplayName("Manufacturer")]
        public string P_Manufacturer { get; set; }
        [DisplayName("Region")]
        public string P_Region { get; set; }
        [DisplayName("PartNo")]
        public string P_Part_No { get; set; }
        [DisplayName("Description")]
        public string P_Description { get; set; }
        [DisplayName("Cost")]
        public double P_Cost { get; set; }
        [DisplayName("MRP")]
        public double P_MRP { get; set; }
        [DisplayName("SellPrice")]
        public double P_SP { get; set; }
        public string Regt_Success { get; set; }
    }
    public class BOMFields
    {
        [DisplayName("SP Part-No")]
        [Required]
        public string SP_Part_No { get; set; }
        [DisplayName("SP Description")]
        public string SP_Description { get; set; }
        [DisplayName("Part-No")]
        [Required]
        public string Part_No { get; set; }
        [DisplayName("Description")]
        [Required]
        public string Description { get; set; }
        [DisplayName("Quantity")]
        [Required]
        public string Quantity { get; set; }
        [DisplayName("Part-No")]
        public string Part_No1 { get; set; }
        [DisplayName("Description")]
        public string Description1 { get; set; }
        [DisplayName("Quantity")]
        public string Quantity1 { get; set; }
        public string Part_to_Descp { get; set; }
        public string P_code { get; set; }
        public string BOM_No { get; set; }
        public DateTime BOM_Date { get; set; }
        public DataSet EditBOM(int bomno, string spcode)
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "select BOM_Date, SP_Code, Quantity from BOM where MP_Code = '" + spcode + "' and BOM_No = '" + bomno + "'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(SqlCmd1);
            da.Fill(ds);
            Con.Close();
            return ds;
        }
        public string Add_Name { get; set; }
        public string Add_Group { get; set; }
        public string Add_Manufacturer { get; set; }
        public string Add_Package { get; set; }
        public string Add_Value { get; set; }
        public string Add_PartNo { get; set; }
        public string Add_Description { get; set; }
        public double Add_Cost { get; set; }
        public double Add_MRP { get; set; }
        public double Add_SellPrice { get; set; }
        public string Project { get; set; }
        public string Partno_letter { get; set; }
        public string Package_letter { get; set; }
        public string Value_letter { get; set; }
        public string Descp_letter { get; set; }
        public string BOM_DD { get; set; }

    }
    public class BOM_Table
    {
        public string Part_No { get; set; }
        public string Description { get; set; }
        public string Quantity { get; set; }
        public string SP_Part_No { get; set; }
        public string BOM_Date { get; set; }
    }
    public class BOM_List
    {
        public int BOM_No { get; set; }
        public string BOM_Date { get; set; }
        public string SP_code { get; set; }
        [DisplayName("Product No")]
        public string Part_No { get; set; }
        [DisplayName("Product Name")]
        public string Product_Name { get; set; }
    }
    public class BOMEdit
    {
        public string Part_No { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public string SP_Part_No { get; set; }
        public string BOM_No { get; set; }
        public string BOM_Date { get; set; }
    }
    public class AccountsField
    {
        [DisplayName("Account Name")]
        [Required]
        public string A_Account_Name { get; set; }
        [DisplayName("Group")]
        public string A_Group { get; set; }
        [DisplayName("Door No")]
        public string A_Door_No { get; set; }
        [DisplayName("Street")]
        public string A_Street { get; set; }
        [DisplayName("Area")]
        public string A_Area { get; set; }
        [DisplayName("City")]
        public string A_City { get; set; }
        [DisplayName("State")]
        public string A_State { get; set; }
        [DisplayName("Country")]
        public string A_Country { get; set; }
        [DisplayName("Pincode")]
        public string A_Pincode { get; set; }
        [DisplayName("Contact No")]
        public string A_Contact_No { get; set; }
        [DisplayName("Mobile No")]
        public string A_Mobile_No { get; set; }
        [DisplayName("Email ID")]
        public string A_Email_Id { get; set; }
        [DisplayName("Closing Balance")]
        public double A_Closing_Bal { get; set; }
        [DisplayName("Open Balance")]
        public double A_Open_Bal { get; set; }
        public string Reg_Success { get; set; }
    }
    public class PurchaseField
    {
        [DisplayName("Invoice No")]
        public string Invoice_No { get; set; }
        [DisplayName("Invoice Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Invoice_Date { get; set; } = DateTime.Now;
        [DisplayName("Manufacturer")]
        public string A_Name { get; set; }
        public string Address { get; set; }
        [DisplayName("Part No")]
        public string Part_No { get; set; }
        [DisplayName("ILedger")]
        public string I_Ledger { get; set; }
        [DisplayName("ALedger")]
        public string A_Ledger { get; set; }
        [DisplayName("Quantity")]
        public int P_Qty { get; set; }
        [DisplayName("Purchase Rate")]
        public double P_Rate { get; set; }
        [DisplayName("Purchase Discount")]
        public double P_Discount { get; set; }
        [DisplayName("Purchase Tax1")]
        public double P_Tax1 { get; set; }
        [DisplayName("Purchase Tax2")]
        public double P_Tax2 { get; set; }
        [DisplayName("Purchase Sub-Total")]
        public double P_Sub_Total { get; set; }
        [DisplayName("Purchase Total")]
        public double I_Total { get; set; }
        public int Total_Qty { get; set; }
        public double Total { get; set; }
        public string Inv_Date { get; set; }
        public double I_Sub_Total { get; set; }
        [DisplayName("Discount")]
        public double I_Discount { get; set; }
        [DisplayName("Tax 1")]
        public double I_Tax1 { get; set; }
        [DisplayName("Tax 2")]
        public double I_Tax2 { get; set; }
        [DisplayName("Reason")]
        public string Reason_Tag { get; set; }
        public string A_A_Name { get; set; }
        public string A_Door_No { get; set; }
        public string A_Street { get; set; }
        public string A_Area { get; set; }
        public string A_City { get; set; }
        public string A_State { get; set; }
        public string A_Country { get; set; }
        public string A_Pincode { get; set; }
        public string A_Contact_No { get; set; }
        public string A_Mobile_No { get; set; }
        public string A_Email_Id { get; set; }
        public string A_Code { get; set; }
    }
    public class Quantity
    {
        public int Qty { get; set; }
        public double Sub_Total { get; set; }
    }
    public class PriceFields
    {
        [DisplayName("Part No")]
        public string Part_No { get; set; }
        public string P_Description { get; set; }
        [DisplayName("Cost Price")]
        public double P_Cost { get; set; }
        [DisplayName("Price (USD)")]
        public double P_Price_USD { get; set; }
        [DisplayName("MRP")]
        public double P_MRP { get; set; }
        [DisplayName("Selling Price")]
        public double P_SP { get; set; }
        public string Reg_success { get; set; }
    }
    public class Price_List
    {
        public string P_code { get; set; }
        public string Part_No { get; set; }
        public string Description { get; set; }
        public double P_Cost { get; set; }
        public double P_MRP { get; set; }
        public double P_Price_USD { get; set; }
        public double P_SP { get; set; }
        [DisplayName("Stock")]
        public double Current_Stock { get; set; }
        public double Reg_success { get; set; }
        public string Partno_letter { get; set; }
        public string Package_letter { get; set; }
        public string Value_letter { get; set; }
        public string Descp_letter { get; set; }
        public int Stock { get; set; }

    }
    public class New_Purchase
    {
        [DisplayName("Invoice No")]
        public string Invoice_No { get; set; }
        [DisplayName("Invoice Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Invoice_Date { get; set; } = DateTime.Now;
        [DisplayName("Voucher No")]
        public string Voucher_No { get; set; }
        [DisplayName("Voucher Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Voucher_Date { get; set; }
        [DisplayName("Part No")]
        public string Part_No { get; set; }
        [DisplayName("ILedger")]
        public string I_Ledger { get; set; }
        [DisplayName("ALedger")]
        public string A_Ledger { get; set; }
        [DisplayName("Quantity")]
        public int P_Qty { get; set; }
        [DisplayName("Purchase Rate")]
        public double P_Rate { get; set; }
        [DisplayName("Dis Rs")]
        public double P_Discount { get; set; }
        [DisplayName("Dis %")]
        public double Discount_per { get; set; }
        [DisplayName("Tax Rs")]
        public double P_Tax1 { get; set; }
        [DisplayName("Tax %")]
        public double Tax1_per { get; set; }
        [DisplayName("Tax Rs")]
        public double P_Tax2 { get; set; }
        public int Total_Qty { get; set; }
        [DisplayName("Sub Total")]
        public double Total { get; set; }
        [DisplayName("Total Quantity")]
        public int Quantity { get; set; }
        public double Rate_Per_Unit { get; set; }
        [DisplayName("Sub Total")]
        public double Taxable_Total { get; set; }
        public double Final_Discount_per { get; set; }
        [DisplayName("Discount")]
        public double Final_Discount { get; set; }
        public double Final_Tax1_per { get; set; }
        [DisplayName("Tax1")]
        public double Final_Tax1 { get; set; }
        public double Final_Tax2_per { get; set; }
        [DisplayName("Tax2")]
        public double Final_Tax2 { get; set; }
        [DisplayName("Tax %")]
        public double Tax2_per { get; set; }
        [DisplayName("Total")]
        public double Final_Total { get; set; }
        public string P_code { get; set; }
        public string P_Name { get; set; }
        public string P_Manufacturer { get; set; }
        public string P_Part_No { get; set; }
        public string P_Description { get; set; }
        public string Package { get; set; }
        public string Value { get; set; }
        public string Partno_letter { get; set; }
        public string Package_letter { get; set; }
        public string Value_letter { get; set; }
        public string Descp_letter { get; set; }
        public string Supplier { get; set; }
        public string PO_No { get; set; }
        public double P_Cost { get; set; }
        public int P_Close_Bal { get; set; }
        public DataSet EditPurchase(int id)
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "select * from Purchase where Voucher_No = '" + id + "'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(SqlCmd1);
            da.Fill(ds);
            return ds;
        }
        public DataSet Edit_PO_to_Purchase(int id)
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "select * from Purchase_Order where PO_No = '" + id + "'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(SqlCmd1);
            da.Fill(ds);
            return ds;
        }
        public DataSet GetAccount(string p_code, int v_no)
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd2 = "select P_code, Purchase_Qty, Purchase_Rate, Purchase_Discount, Purchase_Tax_1, Purchase_Tax_2, Purchase_SubTotal, Purchase_Total FROM Purchase where P_code='" + p_code + "' and Voucher_No='" + v_no + "' ";
            SqlCommand SqlCmd2 = new SqlCommand(cmd2, Con);
            DataSet ds = new DataSet();

            SqlDataAdapter da = new SqlDataAdapter(SqlCmd2);
            da.Fill(ds);

            return ds;
        }
        public string Add_Name { get; set; }
        public string Add_Group { get; set; }
        public string Add_Manufacturer { get; set; }
        public string Add_Package { get; set; }
        public string Add_Value { get; set; }
        public string Add_PartNo { get; set; }
        public string Add_Description { get; set; }
        public double Add_Cost { get; set; }
        public double Add_MRP { get; set; }
        public double Add_SellPrice { get; set; }
        public string Project { get; set; }
        [DisplayName("Purchase Order No")]
        public int Purchase_Order_No { get; set; }
        [DisplayName("Purchase Order Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Purchase_Order_Date { get; set; }
        public string Ref_No { get; set; }
        public DateTime Ref_Date { get; set; }
        public string sup_val { get; set; }
        public string BillTo { get; set; }
        public string billval { get; set; }
        public double IGST { get; set; }
        public double CGST { get; set; }
        public double SGST { get; set; }
        public double Total_IGST { get; set; }
        public double Total_CGST { get; set; }
        public double Total_SGST { get; set; }
        public int project_val { get; set; }
    }
    public class PurchaseTable
    {
        public string Pcode { get; set; }
        public string Part_No { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double SubTotal { get; set; }
        public double Total { get; set; }
        public string Invoice_No { get; set; }
        public DateTime Invoice_Date { get; set; }
        public string Voucher_No { get; set; }
        public DateTime Voucher_Date { get; set; }
        public int PO_No { get; set; }
        public DateTime PO_Date { get; set; }
        public DateTime Ref_Date { get; set; }
        public string supplier { get; set; }
        public string sup_name { get; set; }
        public string sup_code { get; set; }
        public int ILedger { get; set; }
        public int ALedger { get; set; }
        public string Ref_No { get; set; }
        public int I_Qty { get; set; }
        public string project { get; set; }
        public string BillTo { get; set; }
        public double Dis_per { get; set; }
        public double Dis_Rs { get; set; }
        public double Igst_per { get; set; }
        public double Igst_Rs { get; set; }
        public double Cgst_per { get; set; }
        public double Cgst_Rs { get; set; }
        public double Sgst_per { get; set; }
        public double Sgst_Rs { get; set; }
        public double Final_Dis_per { get; set; }
        public double Final_Dis_Rs { get; set; }
        public double Final_Igst_per { get; set; }
        public double Final_Igst_Rs { get; set; }
        public double Final_Cgst_per { get; set; }
        public double Final_Cgst_Rs { get; set; }
        public double Final_Sgst_per { get; set; }
        public double Final_Sgst_Rs { get; set; }
        public int Final_Qty { get; set; }
        public double Final_Sub_Total { get; set; }
        public double Final_total { get; set; }
    }
    public class PurchaseList
    {
        [DisplayName("Invoice No")]
        public string Invoice_No { get; set; }
        [DisplayName("Invoice Date")]
        public string Invoice_Date { get; set; }
        [DisplayName("Voucher No")]
        public string Voucher_No { get; set; }
        [DisplayName("Voucher Date")]
        public string Voucher_Date { get; set; }
        public string A_code { get; set; }
        [DisplayName("Supplier")]
        public string A_Name { get; set; }
        [DisplayName("Total")]
        public double Purchase_Total { get; set; }
        public List<string> Ref_No { get; set; }
        public string project { get; set; }
        [DisplayName("PO No")]
        public string PO_No { get; set; }
        public string Project { get; set; }
    }
    public class EditPurchase
    {
        public string Invoice_No { get; set; }
        public string Part_No { get; set; }
        public string Quantity { get; set; }
        public string Price_Per_Unit { get; set; }
        public string Sub_Total { get; set; }
        public string Discount { get; set; }
        public string Tax1 { get; set; }
        public string Tax2 { get; set; }
        public string Total { get; set; }
    }
    public class EditPurchaseValue
    {
        public string Part_No { get; set; }
        public double Quantity { get; set; }
        public double Price_Per_Unit { get; set; }
        public double Sub_Total { get; set; }
        public double Discount { get; set; }
        public double Tax1 { get; set; }
        public double Tax2 { get; set; }
        public double Total { get; set; }

    }
    public class GoodsRI
    {
        public string P_code { get; set; }
        public string Part_No { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public int Voucher_No { get; set; }
        public DateTime Voucher_Date { get; set; }
        public int Index_Type { get; set; }
        public string Ref_No { get; set; }
        public DateTime Ref_Date { get; set; }
        public string GI_Tag { get; set; }
        public string Process_Tag { get; set; }
        public string Project { get; set; }
        public string Employee { get; set; }
        public string Note { get; set; }
        public int v_no { get; set; }
        public string V_Date { get; set; }
        public string R_Date { get; set; }
        public double P_Cost { get; set; }
        public int I_Qty { get; set; }
        public string Package { get; set; }
        public string Value { get; set; }
        public string Partno_letter { get; set; }
        public string Package_letter { get; set; }
        public string Value_letter { get; set; }
        public string Descp_letter { get; set; }
        public DataSet EditGoods(int vtype, int g_vno)
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "select P_code,Purchase_Qty from I_Ledger where Voucher_Type = '" + vtype + "' and Goods_Voucher_No = '" + g_vno + "'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(SqlCmd1);
            da.Fill(ds);
            Con.Close();
            return ds;
        }
        public DataSet SelectIndent(string Indent)
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "select * from Stock_Indent where IndentNo = "+Indent+"";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(SqlCmd1);
            da.Fill(ds);
            Con.Close();
            return ds;
        }
        public int db_qty { get; set; }

        //Add New Product
        public string Add_Name { get; set; }
        public string Add_Group { get; set; }
        public string Add_Manufacturer { get; set; }
        public string Add_Package { get; set; }
        public string Add_Value { get; set; }
        public string Add_PartNo { get; set; }
        public string Add_Description { get; set; }
        public double Add_Cost { get; set; }
        public double Add_MRP { get; set; }
        public double Add_SellPrice { get; set; }
        public double Current_Stock { get; set; }
        public double P_MRP { get; set; }
    }
    public class GoodsList
    {
        public string Voucher_Type { get; set; }
        public string G_Voucher_No { get; set; }
        public string G_Voucher_Date { get; set; }
        public string Ref_No { get; set; }
        public string Ref_Date { get; set; }
        public string GI_Tag { get; set; }
        public string Process { get; set; }
        public string Project { get; set; }
        public string Employee { get; set; }
        public string Note { get; set; }
        public string P_code { get; set; }
    }
    public class PM_list
    {
        public string P_code { get; set; }
        public string Part_No { get; set; }
        public string Description { get; set; }
        public string P_Part_NO { get; set; }
    }
    public class UserModel
    {
        [DisplayName("Employee Name")]
        public string Employee_Name { get; set; }
        [DisplayName("Employee Designation")]
        public string Employee_Designation { get; set; }
        [DisplayName("Employee Id")]
        public string Employee_Id { get; set; }
        [DisplayName("Process Name")]
        public string Process_Name { get; set; }
        [DisplayName("Project Name")]
        public string Project_Name { get; set; }
        public string Company { get; set; }
    }
    public class LoginModel
    {
        public string UserId { get; set; }
        [DisplayName("User Name")]
        [Required(ErrorMessage = "This Field is required")]
        public string UserName { get; set; }
        [DisplayName("Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "This Field is required")]
        public string Password { get; set; }
        public string LoginErr { get; set; }
        public string Roll { get; set; }
        public string Display_name { get; set; }
        public string View { get; set; }
        public string Add { get; set; }
        public string Edit { get; set; }
        public string Delete { get; set; }
        public string Disable { get; set; }
        public string Menu { get; set; }
    }
    public class SignupModel
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Roll { get; set; }
        [DisplayName("Roll")]
        public string Roll_Name { get; set; }
        public string Permission_Detail { get; set; }
        public bool View { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
        public int View_val { get; set; }
        public int Add_val { get; set; }
        public int Edit_val { get; set; }
        public int Delete_val { get; set; }
        public bool Disable { get; set; }
        public string Menu { get; set; }
    }
    public class Createuser
    {
        public string Menu { get; set; }
        public bool View { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
        public bool Disable_Enable { get; set; }
        public string displayname { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public int roll { get; set; }
        public int view_val { get; set; }
        public int add_val { get; set; }
        public int edit_val { get; set; }
        public int delete_val { get; set; }
        public int disable_val { get; set; }
        public int Id { get; set; }
    }
    public class Issue
    {
        public int Vno { get; set; }
        public string IndentNo { get; set; }
        public DateTime IndentDate { get; set; }
        public string Reason { get; set; }
        public string Process { get; set; }
        public string Project { get; set; }
        public string RequestBy { get; set; }
        public string Note { get; set; }
        public string PartNo { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public int Reason_int { get; set; }
        public int Process_int { get; set; }
        public int Project_int { get; set; }
        public int Request_int { get; set; }

    }
    public class Inventory
    {
        public string Project { get; set; }
        public string Process { get; set; }
        public string User { get; set; }
        public string Product { get; set; }
        public string Partno_letter { get; set; }
        public string Package_letter { get; set; }
        public string Value_letter { get; set; }
        public string Descp_letter { get; set; }
    }
    public class Inventory_Table
    {
        public string VoucherNo { get; set; }
        public string Date { get; set; }
        public string PartNo { get; set; }
        public string Description { get; set; }
        public string GoodsReceipt { get; set; }
        public string GoodsIssue { get; set; }
        public string Project { get; set; }
        public string Process { get; set; }
        public string User { get; set; }
        public string pcode { get;set; }
        public int Quantity { get; set; }
        public string VoucherType { get; set; }
    }
    public class Stockstatement
    {
        public string PartNo { get; set; }
        public string Package { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public int Stock { get; set; }
        public double Price { get; set; }
        public double Total { get; set; }
        public string ProductCode { get; set; }
        public string Partno_letter { get; set; }
        public string Package_letter { get; set; }
        public string Value_letter { get; set; }
        public string Descp_letter { get; set; }
    }
    public class Value
    {
        public string partno { get; set; }
        public string package { get; set; }
        public string value { get; set; }   
        public string description { get; set; }
    }
    public class PO_List
    {
        public int PO_No { get; set; }
        public string PO_Date { get; set; }
        public string Ref_No { get; set; }
        public string Ref_Date { get;set; }
        public string acode { get; set; }
        public string Supplier { get; set; }
        public string BillTo { get; set; }
        public string billto_acode { get; set; }
        public int project_val { get; set; }
        public string Project { get; set; }
    }
    public class Material_Index
    {
        public string Part_No { get; set; }
        [DisplayName("Part No")]
        public string Part_No_TB { get; set; }
        [DisplayName("Supplier/Mfr")]
        public string Supplier_TB { get; set; }
        [DisplayName("PO No")]
        public string PO_No_TB { get; set; }
        [DisplayName("PO Date")]
        public string PO_Date_TB { get; set; }
        [DisplayName("PV No")]
        public string PV_No_TB { get; set; }
        [DisplayName("PV Date")]
        public string PV_Date_TB { get; set; }
        [DisplayName("LeadTime (in days)")]
        public string TimeSpan_TB { get; set; }
    }
    public class Workorder
    {
        public int WO_No { get; set; }
        public DateTime WO_Date { get; set; }
        public string Process { get; set; }
        public string Mfr_option { get; set; }
        public string Mfr { get; set; }
        public string Note { get; set; }
        public string PartNo { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public string Add_Name { get; set; }
        public string Add_Group { get; set; }
        public string Add_Manufacturer { get; set; }
        public string Add_Package { get; set; }
        public string Add_Value { get; set; }
        public string Add_PartNo { get; set; }
        public string Add_Description { get; set; }
        public double Add_Cost { get; set; }
        public double Add_MRP { get; set; }
        public double Add_SellPrice { get; set; }
        public double Current_Stock { get; set; }
        public double P_MRP { get; set; }
        public string Package { get; set; }
        public string Value { get; set; }
        public string Partno_letter { get; set; }
        public string Package_letter { get; set; }
        public string Value_letter { get; set; }
        public string Descp_letter { get; set; }
        public string Product { get; set; }
        public string BOM_No { get; set; }
        public DataSet WO_BOM(int bomno)
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["geriahco_db"].ConnectionString);
            Con.Open();
            string cmd1 = "select SP_code,Quantity from BOM where BOM_No = '" + bomno + "'";
            SqlCommand SqlCmd1 = new SqlCommand(cmd1, Con);
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(SqlCmd1);
            da.Fill(ds);
            Con.Close();
            return ds;
        }
    }
    public class WO_List
    {
        [DisplayName("WorkOrder No")]
        public int WO_No { get; set; }
        [DisplayName("WorkOrder Date")]
        public string WO_Date { get; set; }
        public string Product { get; set; }
        public string Process { get; set; }
        [DisplayName("Option")]
        public string Mfr_Option { get; set; }
        [DisplayName("Manufacturer")]
        public string Mfr { get; set; }
        public string Status { get; set; }
        public string Quantity { get; set; }
        public string BOM_No { get; set; }
    }
}