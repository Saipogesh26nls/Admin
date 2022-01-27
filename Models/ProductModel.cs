using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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
        public DateTime Invoice_Date { get; set; }
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
}