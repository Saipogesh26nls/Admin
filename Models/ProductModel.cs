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
        public string Part_No { get; set; }
        [DisplayName("Description")]
        public string Description { get; set; }
        [DisplayName("Quantity")]
        public string Quantity { get; set; }
        [DisplayName("Part-No")]
        public string Part_No1 { get; set; }
        [DisplayName("Description")]
        public string Description1 { get; set; }
        [DisplayName("Quantity")]
        public string Quantity1 { get; set; }
        public string hello { get; set; }
    }
    public class OrderDetail
    {
        public List<BOMFields> OrderDetails { get; set; }
    }

}