using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleOrderPostingConsole.Models
{
    public class OrderModel
    {
        public class Parameter
        {

            public int OrderId { get; set; }
            public int DivisionOrderId { get; set; }
            public int BrandID { get; set; }
            public int OrderAmendmentId { get; set; }
            public string WarehouseName { get; set; }
        }

        public class OrderHeader
        {
            public string EntryMode { get; set; }
            public int CompanyCode { get; set; }
            public string CardCode { get; set; }
            public string CardName { get; set; }
            public int SapDocEntry { get; set; }
            public string PoNumber { get; set; }
            public string Destination { get; set; }
            public string Transport { get; set; }
            public int PriceType { get; set; }
            public string SapOrderType { get; set; }
            public string AreaCode { get; set; }
            public string Brand { get; set; }
            public string SubBrand { get; set; }
            public string PoDate { get; set; }
            public string RefNumber { get; set; }
            public string RefDate { get; set; }
            public string OverallDueDate { get; set; }
            public string DocDate { get; set; }
            public string Status { get; set; }
            public int NoOfDetails { get; set; }
            public string Remarks { get; set; }
            public string PoTerms { get; set; }
            public string ShipTerms { get; set; }
            public int SubmittedBy { get; set; }
            public int PreparedBy { get; set; }
            public double ValueBeforeDiscount { get; set; }
            public double DiscountPer { get; set; }
            public double TotalTax { get; set; }
            public int RoundOff { get; set; }
            public double TotalQty { get; set; }
            public double OrderValue { get; set; }
            public string GstIn { get; set; }
            public string DisplayName { get; set; }
            public string TransportMode { get; set; }
            public string ReqColourDetails { get; set; }
            public string UserName { get; set; }
            public string Roles { get; set; }
            public int OrderAmendmentId { get; set; }
            public double ForwardingCharges { get; set; }
            public int ApprovalType { get; set; }
            public string FreightType { get; set; }
            public string OrderReason { get; set; }
            public string TransporterId { get; set; }
            public string TransporterName { get; set; }
            public string ShipToCode { get; set; }
        }

        public class OrderColourData
        {
            public int ProductId { get; set; }
            public string ItemCode { get; set; }
            public string ItemDesc { get; set; }
            public string ColorCode { get; set; }
            public string ColorName { get; set; }
            public double OrderedQty { get; set; }
        }

        public class OrderItemData
        {
            public int RowNum { get; set; }
            public int ProductId { get; set; }
            public string ItemCode { get; set; }
            public string ItemDesc { get; set; }
            public string CatalogCode { get; set; }
            public string CatalogName { get; set; }
            public string HsnCode { get; set; }
            public int UOM { get; set; }
            public double Qty { get; set; }
            public double OfferQty { get; set; }
            public double Rate { get; set; }
            public double MRP { get; set; }
            public string Style { get; set; }
            public string Size { get; set; }
            public string TaxCode { get; set; }
            public double TaxPer { get; set; }
            public double TaxAmount { get; set; }
            public double DisPer { get; set; }
            public int DisAmount { get; set; }
            public double NetAmount { get; set; }
            public string LineRemarks { get; set; }
            public double LineValue { get; set; }
            public DateTime DeliveryDate { get; set; }
            public string OfferCode { get; set; }
        }

        public class OrderEmployeeData
        {
            public int OrderDivisionId { get; set; }
            public string EmployeeId { get; set; }
            public int RoleId { get; set; }
            public string EmployeeName { get; set; }
            public string Designation { get; set; }
            public int OopEmpID { get; set; }
        }

        public class OrderData
        {
            public OrderHeader OrderHeader { get; set; }
            public List<OrderItemData> OrderItemDataList { get; set; }
            public List<OrderColourData> OrderColourDataList { get; set; }
            public List<OrderEmployeeData> OrderEmployeeDataList { get; set; }
        }

        //public class EditOrderData
        //{
        //    public OrderHeader Header { get; set; }
        //    public List<OrderItemData> ItemDetails { get; set; }
        //    public List<OrderColourData> OrderColourDetails { get; set; }
        //    public List<OrderEmployeeData> OrderEmployeeDetails { get; set; }
        //}

        public class ApproveData
        {
            public int SapDocEntry { get; set; }
            public int OrderAmendmentId { get; set; }

        }

        public class CancelData
        {
            public int SapDocEntry { get; set; }
            public int OrderAmendmentID { get; set; }
            public string RevisionReason { get; set; }

        }
        public class HoldData
        {
            public int SapDocEntry { get; set; }
            public int OrderAmendmentId { get; set; }
            public string OrderRemarks { get; set; }

        }
    }
}
