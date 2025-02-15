using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SaleOrderPostingConsole.Models;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using SAPbobsCOM;
using Newtonsoft.Json;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;
using Azure;
using System.Security.Policy;
using static SaleOrderPostingConsole.Models.OrderModel;

namespace SaleOrderPostingConsole.Controllers
{

    public class CreateSaleOrder
    {

        Connectivity con = new Connectivity();

        Helpers helpers = new Helpers();
        String message = "";
        public Company oCompany = null;
        public Documents Ordr = null;
        public UserTable OrdrColour = null;
        private Recordset oRecSet = null;

        private Recordset oRecSetBrn = null;

        private Recordset oRecSet1 = null;

        private Recordset oRecSettax = null;

        private Recordset oRecSetwhscode = null;


        Int32 lRetCode;
        Int32 lErrCode;
        String sErrMsg;
        int RetVal;

        String tempStr;

        int ErrCode;
        String ErrMsg;
        String a;

        public  string loadorder_data(int bucode)
        {

            try
            {
                
                var client = new HttpClient();
                string baseAddress = con.BaseURL;
                string posturl = "api/Order/Post_Order?BUID=" + bucode + "";
                var response =   client.GetAsync(baseAddress + posturl).Result;

                
                string jsonString =   response.Content.ReadAsStringAsync().Result;
                List<OrderData> paramList = JsonConvert.DeserializeObject<List<OrderData>>(jsonString);
 

                foreach (OrderData res in paramList)
                {

                    Post_Order(res);

                }
                return "Success";

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex + "\n");
                return "Fail " + ex.Message;

            }


        }

        private void Post_Order(OrderData objData)
        {

            OrderHeader header = new OrderHeader();
            header = objData.OrderHeader;

            DateTime current_dt = DateTime.Now;
            string inAactiveItem = "";

            //dynamic order_details = objData.order_details;
            //dynamic order_ColourDetails = objData.order_ColourDetails;
            //dynamic order_EmployeeDetails = objData.order_EmployeeDetails;
            //string mode = jsonData.entry_mode;
            //string company_code = jsonData.company_code;
            //string card_code = jsonData.card_code;
            //string card_name = jsonData.card_name;
            //string po_number = jsonData.po_number;
            //string Brand = jsonData.brand;
            //string Destination = jsonData.destination;
            //string Transport = jsonData.transport;
            //string Areacode = jsonData.areacode;
            //string po_date = jsonData.po_date;
            //string GSTIN = jsonData.gSTIN;
            //string ref_number = jsonData.ref_number;
            //string Roles = jsonData.roles;
            //string UserName = jsonData.userName;
            //string SubBrand = jsonData.subBrand;
            //string ref_date = jsonData.ref_date;
            //DateTime? overall_due_date = jsonData.overall_due_date;
            //DateTime? doc_date = jsonData.doc_date;
            //string status = jsonData.status;
            //int noof_details = jsonData.noof_details;
            //string remarks = jsonData.remarks;
            //string po_terms = jsonData.po_terms;
            //string ship_terms = jsonData.ship_terms;
            //string submitted_by = jsonData.submitted_by;
            //string prepared_by = jsonData.prepared_by;
            //double? value_before_discount = jsonData.value_before_discount;
            //double? discount_per = jsonData.discount_per;
            //double? total_tax = jsonData.total_tax;
            //double? round_off = jsonData.round_off;
            //double? total_qty = jsonData.total_qty;
            //double? order_value = jsonData.order_value;
            //string Displayname = jsonData.displayname;
            //string Pricetype = jsonData.u_Pricetype;
            //string refnojson = jsonData.ref_number;
            //string SapOrderType = jsonData.sapOrderType;
            //int orderamendmentid = jsonData.orderAmendmentID;
            //int Approvaltype = jsonData.approvalType;
            //string Transporterid = jsonData.transporterId;
            //string ShiptoCode = jsonData.shiptoCode;
            //string TransporterId = jsonData.transporterId;
            //string TransporterName = jsonData.transporterName;
            //string TransportMode = jsonData.transportMode;

            try
            {

                //con = new Connectivity();
                oCompany = new Company();

                oCompany.Server = con.Server;
                oCompany.CompanyDB = con.Database;

                if (con.DbVersion.ToString() == "2008")
                {
                    oCompany.DbServerType = BoDataServerTypes.dst_MSSQL2008;
                }
                else if (con.DbVersion.ToString() == "2012")
                {
                    oCompany.DbServerType = BoDataServerTypes.dst_MSSQL2012;
                }
                else if (con.DbVersion.ToString() == "2014")
                {
                    oCompany.DbServerType = BoDataServerTypes.dst_MSSQL2014;
                }
                else if (con.DbVersion.ToString() == "2016")
                {
                    oCompany.DbServerType = BoDataServerTypes.dst_MSSQL2016;
                }
                else if (con.DbVersion.ToString() == "2017")
                {
                    oCompany.DbServerType = BoDataServerTypes.dst_MSSQL2017;
                }
                oCompany.DbUserName = con.UserName;
                oCompany.DbPassword = con.Password;
                oCompany.UserName = con.SapUserName;
                oCompany.Password = con.SapPassword;
                oCompany.language = BoSuppLangs.ln_English;
                oCompany.UseTrusted = false;

                oCompany.LicenseServer = con.SapLicenseServer;
                lRetCode = oCompany.Connect();


                if (lRetCode != 0)
                {
                    oCompany.GetLastError(out lErrCode, out sErrMsg);
                    if (oCompany != null)
                    {
                        if (oCompany.Connected) oCompany.Disconnect();
                        Console.WriteLine(lErrCode + " - " + sErrMsg);
                    }

                    /// required error handle

                }
                else
                {

                    message = "";
                    Ordr = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);
                    string Sql1 = "select LineId SBrandId from [@INCM_SBD1]  where U_Name = '" + header.SubBrand + "'";
                    oRecSet = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                    oRecSet.DoQuery(Sql1);
                    int SubbrandId = 0;
                    if (oRecSet.RecordCount > 0)
                    {
                        SubbrandId = oRecSet.Fields.Item("SBrandId").Value;

                        Ordr.UserFields.Fields.Item("U_SubBrand").Value = SubbrandId.ToString();

                    }
                    oRecSetBrn = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                    string Brnch = "Select MltpBrnchs from OADM where MltpBrnchs = 'Y'";
                    oRecSetBrn.DoQuery(Brnch);
                    int cnt = oRecSetBrn.RecordCount;
                    if (oRecSetBrn.RecordCount > 0)
                    {
                        if (header.Brand.ToUpper() == "RAMRAJ LAGNAA" || header.Brand.ToUpper() == "VIVEAGA LAGNAA" || header.Brand.ToUpper() == "RAMRAJ MUHURTH" || header.Brand.ToUpper() == "VIVEAGA MUHURTH")
                        {
                            Ordr.BPL_IDAssignedToInvoice = con.BranchVT;
                            Ordr.Series = con.SeriesVT;
                        }
                        else if (header.Brand.ToUpper() == "RAMYYAM SAREE" || header.Brand.ToUpper() == "RAMYYAM")
                        {
                            Ordr.BPL_IDAssignedToInvoice = con.BranchTS;
                            Ordr.Series = con.SeriesTS;
                        }
                        else
                        {
                            Ordr.BPL_IDAssignedToInvoice = con.Branch;
                            Ordr.Series = con.Series;
                        }

                    }

                    else
                    {
                        //Ordr.BPL_IDAssignedToInvoice = con.Branch;
                        Ordr.Series = con.Series;
                    }

                    string Sql2 = "EXEC [@INSPL_TEAM] '" + header.CardCode + "'";
                    oRecSet.DoQuery(Sql2);
                    if (oRecSet.RecordCount > 0)
                    {
                        Ordr.UserFields.Fields.Item("U_Team1").Value = oRecSet.Fields.Item("U_Team1").Value;
                    }
                    if ((header.CardCode == "C059972") || (header.CardCode == "C061152"))
                    {
                        string Sql3 = "Select top 1  Notes,isnull(U_Pass,'TO PAY')U_Pass,ISNULL(U_Dis1,0) U_Dis1,CardName,CardFName,ISNULL(A.U_GSTIN,'') U_GSTIN,ISNULL(U_LTransport,'') U_Tranport,ISNULL(U_LDestination,'') U_Destination, ISNULL(U_LAreaCode, '') AreaCode,CONVERT(nvarchar(max), isnull(U_LDistance, '10')) Distance,ISNULL(U_LTransporterGST, '') TransporterId,ISNULL(U_LTransporterName, '') TransporterName from OCRD A INNER JOIN CRD1 B ON A.CardCode = B.CardCode where B.Address = '" + header.ShipToCode + "' and a.CardCode = '" + header.CardCode + "'";
                        oRecSet.DoQuery(Sql3);
                    }
                    else
                    {
                        string Sql3 = "Select  Notes,isnull(U_Pass,'TO PAY')U_Pass,ISNULL(U_Dis1,0) U_Dis1,CardName,CardFName,ISNULL(U_GSTIN,'') U_GSTIN,ISNULL(U_Tranport,'') U_Tranport,ISNULL(U_Destination,'') U_Destination,ISNULL(U_AreaCode,'') AreaCode,CONVERT(nvarchar(max), isnull(U_Distance,'10')) Distance,ISNULL(U_TransporterId,'') TransporterId,ISNULL(U_TransporterName,'') TransporterName  from OCRD where CardCode = '" + header.CardCode + "'";
                        oRecSet.DoQuery(Sql3);
                    }
                    if (oRecSet.RecordCount > 0)
                    {
                        string SAPnotess = oRecSet.Fields.Item("Notes").Value;
                        string SAPDestination = oRecSet.Fields.Item("U_Destination").Value;
                        string SAPTranport = oRecSet.Fields.Item("U_Tranport").Value;
                        string SAPGSTIN = oRecSet.Fields.Item("U_GSTIN").Value;
                        string SAPPass = header.TransportMode;
                        string SAPTransporterId = oRecSet.Fields.Item("TransporterId").Value;
                        string SAPTransporterName = oRecSet.Fields.Item("TransporterName").Value;

                        Ordr.UserFields.Fields.Item("U_UsrRole").Value = header.Roles;
                        Ordr.UserFields.Fields.Item("U_Pricetype").Value = header.PriceType;
                        Ordr.UserFields.Fields.Item("U_OOPUsr").Value = header.UserName;
                        Ordr.UserFields.Fields.Item("U_Pass").Value = SAPPass;
                        Ordr.UserFields.Fields.Item("U_Dis1").Value = oRecSet.Fields.Item("U_Dis1").Value;
                        Ordr.UserFields.Fields.Item("U_Arcode").Value = oRecSet.Fields.Item("AreaCode").Value;
                        Ordr.UserFields.Fields.Item("U_Distance").Value = oRecSet.Fields.Item("Distance").Value;
                        Ordr.CardName = oRecSet.Fields.Item("CardName").Value;
                        Ordr.UserFields.Fields.Item("U_GSTIN").Value = (header.GstIn == "" || header.GstIn == null ? SAPGSTIN : header.GstIn);


                        if (con.Database != "RRFLIVE")
                        {
                            Ordr.UserFields.Fields.Item("U_AreaCode").Value = oRecSet.Fields.Item("AreaCode").Value;
                        }
                        else
                        {
                            Ordr.UserFields.Fields.Item("U_areacode").Value = oRecSet.Fields.Item("AreaCode").Value;
                        }



                        if ((con.Database == "RHLLIVE" || con.Database == "RRLIVE" || con.Database == "AKGLIVE" || con.Database == "VTLIVE" ? true : con.Database == "ANTSPRODLIVE"))
                        {

                            Ordr.UserFields.Fields.Item("U_Destination").Value = (header.Destination == "" || header.Destination == null ? SAPDestination : header.Destination);
                            Ordr.UserFields.Fields.Item("U_Trsport").Value = (header.Transport == "" || header.Transport == null ? SAPTranport : header.Transport);
                        }


                        Ordr.UserFields.Fields.Item("U_TransporterId").Value = (header.TransporterId == "" || header.TransporterId == null ? SAPTransporterId : header.TransporterId);
                        Ordr.UserFields.Fields.Item("U_TransporterName").Value = (header.TransporterName == "" || header.TransporterName == null ? SAPTransporterName : header.TransporterName);
                        Ordr.UserFields.Fields.Item("U_Transport").Value = (header.Transport == "" || header.Transport == null ? SAPTranport : header.Transport);
                        Ordr.UserFields.Fields.Item("U_Dsnation").Value = (header.Destination == "" || header.Destination == null ? SAPDestination : header.Destination);


                        Ordr.Comments = SAPnotess.ToString() + " " + header.Remarks.ToUpper().ToString();
                    }
                    if (con.Database == "ANTSPRODLIVE")
                    {
                        Ordr.UserFields.Fields.Item("U_Remarks2").Value = header.DisplayName.ToString();
                    }

                    string Sqlbpcheck = "select * from OCRD where CardCode =  '" + header.CardCode + "' and validFor = 'N'";
                    oRecSet.DoQuery(Sqlbpcheck);
                    if (oRecSet.RecordCount > 0)
                    {
                        string Sqlbpcheckupdate = "update OCRD set validFor = 'Y',frozenFor = 'N' where CardCode = '" + header.CardCode + "'";
                        oRecSet.DoQuery(Sqlbpcheckupdate);
                    }


                    Ordr.CardCode = header.CardCode.ToUpper();
                    Ordr.TaxDate = current_dt;
                    Ordr.UserFields.Fields.Item("U_Brand").Value = header.Brand.ToUpper();
                    Ordr.UserFields.Fields.Item("U_CustRefDt").Value = header.RefNumber.ToString();

                    Ordr.DocDate = Convert.ToDateTime(header.DocDate);

                    Ordr.DocDueDate = Convert.ToDateTime(header.OverallDueDate);

                    //Ordr.NumAtCard = "";
                    Ordr.DocObjectCode = SAPbobsCOM.BoObjectTypes.oOrders;
                    //Ordr.Series = con.seriesORDR;
                    Ordr.DocType = BoDocumentTypes.dDocument_Items;
                    Ordr.HandWritten = BoYesNoEnum.tNO;
                    Ordr.NumAtCard = header.RefNumber.ToString();
                    Ordr.UserFields.Fields.Item("U_WebDocNo").Value = header.PoNumber.ToString();
                    Ordr.UserFields.Fields.Item("U_Noofbun").Value = "1";
                    Ordr.UserFields.Fields.Item("U_OrderBy").Value = "OOP";
                    Ordr.UserFields.Fields.Item("U_OrdType").Value = header.SapOrderType;
                    Ordr.Rounding = BoYesNoEnum.tYES;
                    Ordr.UserFields.Fields.Item("U_ApprovedType").Value = header.ApprovalType.ToString();


                    if (header.ShipToCode != "")
                    {
                        //if ((card_code == "C059972") || (card_code == "C061152"))
                        //{
                        string Sql21 = (" SELECT  ADDRESS  FROM CRD1 WHERE CARDCODE='" + header.CardCode + "' AND ADDRESS='" + header.ShipToCode + "' AND ADRESTYPE='S'");
                        oRecSet = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                        oRecSet.DoQuery(Sql21);
                        if (oRecSet.RecordCount > 0)
                        {
                            Ordr.ShipToCode = header.ShipToCode.ToString();
                        }

                        //Ordr.ShipToCode = ShiptoCode.ToString();
                        // }

                    }

                    foreach (OrderItemData lineDetail in objData.OrderItemDataList)
                    {
                        double qty = (double)lineDetail.Qty;
                        double OfferQty = (double)lineDetail.OfferQty;

                        double rate = (double)lineDetail.Rate;
                        double MRP = (double)lineDetail.MRP;
                        DateTime item_due = (DateTime)lineDetail.DeliveryDate;



                        Ordr.Lines.SetCurrentLine(lineDetail.RowNum);
                        Ordr.Lines.ItemCode = lineDetail.ItemCode.ToUpper();
                        Ordr.Lines.ItemDescription = lineDetail.ItemDesc.ToUpper();
                        Ordr.Lines.DiscountPercent = lineDetail.DisPer;

                        if (con.Database == "ANTSPRODLIVE")
                        {
                            string Sql4 = ("select ItemCode,U_RNAME ItemName,U_Scode SRCode,U_Size,U_Style from OITM WHERE ItemCode = '" + lineDetail.ItemCode + "'");
                            oRecSet = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                            oRecSet.DoQuery(Sql4);
                            if (oRecSet.RecordCount > 0)
                            {
                                Ordr.Lines.UserFields.Fields.Item("U_CatalogCode").Value = oRecSet.Fields.Item("ItemName").Value;
                                Ordr.Lines.UserFields.Fields.Item("U_CatalogName").Value = oRecSet.Fields.Item("ItemName").Value;
                                Ordr.Lines.UserFields.Fields.Item("U_SRCode").Value = oRecSet.Fields.Item("SRCode").Value;
                                Ordr.Lines.UserFields.Fields.Item("U_Size").Value = oRecSet.Fields.Item("U_Size").Value;
                                Ordr.Lines.UserFields.Fields.Item("U_Style").Value = oRecSet.Fields.Item("U_Style").Value;
                            }
                        }
                        if ((con.Database == "RHLLIVE" || con.Database == "RRLIVE" || con.Database == "AKGLIVE" || con.Database == "VTLIVE" || con.Database == "ATCLIVE" || con.Database == "VGLIVE" ? true : con.Database == "RRFLIVE"))
                        {
                            string Sql4 = ("select B.U_CatalgCode ,convert(nvarchar(max), B.U_Remarks) SRCode,A.U_Size,A.U_Style  from [@INS_OPLM] A INNER JOIN [@INS_PLM1] B ON B.DocEntry = A.DocEntry INNER JOIN OITM C ON A.U_ItemCode = C.ItemCode AND ISNULL(U_Lock,'') = 'N' WHERE U_ItemCode = '" + lineDetail.ItemCode + "' AND ISNULL(U_SubBrand,'') = '" + header.SubBrand + "'");
                            oRecSet = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                            oRecSet.DoQuery(Sql4);
                            if (oRecSet.RecordCount > 0)
                            {
                                Ordr.Lines.UserFields.Fields.Item("U_CatalogCode").Value = oRecSet.Fields.Item("U_CatalgCode").Value;
                                Ordr.Lines.UserFields.Fields.Item("U_CatalogName").Value = oRecSet.Fields.Item("U_CatalgCode").Value;
                                Ordr.Lines.UserFields.Fields.Item("U_SRCode").Value = oRecSet.Fields.Item("SRCode").Value;
                                Ordr.Lines.UserFields.Fields.Item("U_Size").Value = oRecSet.Fields.Item("U_Size").Value;
                                Ordr.Lines.UserFields.Fields.Item("U_Style").Value = oRecSet.Fields.Item("U_Style").Value;
                                Ordr.Lines.UserFields.Fields.Item("U_Offer_Code").Value = Convert.ToString(lineDetail.OfferCode);
                                Ordr.Lines.UserFields.Fields.Item("U_FreeQty").Value = Convert.ToString(OfferQty);
                            }
                        }
                        if (OfferQty > 0)
                        {
                            Ordr.Lines.UserFields.Fields.Item("U_FreeQty").Value = Convert.ToString(OfferQty);
                            Ordr.Lines.UserFields.Fields.Item("U_FreeOrdQty").Value = Convert.ToString(OfferQty);
                            Ordr.Lines.UserFields.Fields.Item("U_Freelineamt").Value = Convert.ToString(OfferQty * rate);
                        }
                        Ordr.Lines.UserFields.Fields.Item("U_SalPrice").Value = rate;
                        Ordr.Lines.UserFields.Fields.Item("U_MRP").Value = MRP;
                        Ordr.Lines.UserFields.Fields.Item("U_LineRemarks").Value = lineDetail.LineRemarks.ToUpper().ToString();
                        Ordr.Lines.DiscountPercent = Convert.ToDouble(lineDetail.DisPer);

                        if (!(con.Server == "SAPTEST"))
                        {
                            Ordr.Lines.UserFields.Fields.Item("U_Offer_Code").Value = Convert.ToString(lineDetail.OfferCode);
                        }

                        Ordr.Lines.UserFields.Fields.Item("U_FreeQty").Value = Convert.ToString(OfferQty);


                        Ordr.Lines.UserFields.Fields.Item("U_NOpenQty").Value = qty;
                        if ((con.Database != "RRFLIVE") && (header.Brand.ToUpper() != "VIVEAGHAM FABRIC"))
                        {
                            Ordr.Lines.Quantity = qty;
                            Ordr.Lines.UserFields.Fields.Item("U_NoofPiece").Value = Convert.ToInt32(qty);
                        }
                        else if (lineDetail.UOM != 2)
                        {
                            string Sql5 = "SELECT isnull(CONVERT(Decimal(18,9),  " + qty + ")  * case When charindex('CMS',u_length)> 0  then 1  when cast(t1.u_length as Decimal(16,9))<20 and charindex('CMS',u_length)=0  then   cast(t1.u_length as Decimal(16,9)) ELSE 0 end ,0) as MTRS  FROM OITM T0 LEFT JOIN [@INCM_SZE1] T1 on T0.U_Size=T1.U_Name  where T0.Itemcode= '" + lineDetail.ItemCode + "'  and ISNULL(T0.InvntryUom,'') = 'NOS'";
                            oRecSet1 = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                            oRecSet1.DoQuery(Sql5);
                            if (oRecSet1.RecordCount > 0)
                            {
                                Ordr.Lines.UserFields.Fields.Item("U_NoofPiece").Value = Convert.ToInt32(qty);
                                Ordr.Lines.Quantity = qty;
                                Ordr.Lines.Volume = Convert.ToDouble(oRecSet1.Fields.Item("MTRS").Value);
                            }
                        }
                        else
                        {
                            Ordr.Lines.UserFields.Fields.Item("U_NoofPiece").Value = 1;
                            Ordr.Lines.Quantity = qty;
                            Ordr.Lines.Volume = qty;
                        }


                        if (Ordr.Lines.TreeType.ToString() == "iNotATree")
                        {

                            double priceaftdis = rate - rate * (lineDetail.DisPer == 0 ? 0 : Convert.ToDouble(lineDetail.DisPer) / 100);
                            string Sqltax = "Exec [@INSPL_Sales_TaxRate] '" + header.CardCode + "','" + lineDetail.ItemCode + "','" + priceaftdis + "'";
                            oRecSettax = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                            oRecSettax.DoQuery(Sqltax);
                            if (oRecSettax.RecordCount > 0)
                            {
                                Ordr.Lines.TaxCode = oRecSettax.Fields.Item("TAXRATE").Value;

                            }


                        }


                        Ordr.Lines.UnitPrice = rate;
                        Ordr.Lines.UserFields.Fields.Item("U_HSNCODE").Value = lineDetail.HsnCode;
                        Ordr.Lines.ShipDate = item_due;
                        Ordr.Lines.BaseType = -1;
                        inAactiveItem = lineDetail.ItemCode;
                        Ordr.Lines.Add();
                    }



                    Ordr.UserFields.Fields.Item("U_TotQty").Value = header.TotalQty;




                    RetVal = Ordr.Add();
                    oCompany.GetNewObjectCode(out tempStr);
                    string DocEntry = "";
                    string DocNumber = "";
                    string sap_status = "";
                    DateTime DocDate = DateTime.Now;




                    if (RetVal != 0)
                    {
                        message = "";

                        string Sql = "SELECT DocNum,DocDate,DocEntry FROM ORDR WITH (NOLOCK) WHERE NumAtCard = '" + header.RefNumber.ToString().Replace("'", "''") + "'";
                        oRecSet = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                        oRecSet.DoQuery(Sql);
                        if (oRecSet.RecordCount > 0)
                        {
                            DocEntry = Convert.ToString(oRecSet.Fields.Item("DocEntry").Value);
                            DocNumber = Convert.ToString(oRecSet.Fields.Item("DocNum").Value);
                            DocDate = oRecSet.Fields.Item("DocDate").Value;
                            message = "DocEntry:" + Convert.ToString(DocEntry) + ",DocNum:" + DocNumber + ",DocDate:" + DocDate.ToString("dd-MMM-yyyy");
                            sap_status = "Success_OrderEntry";


                        }

                        else
                        {
                            oCompany.GetLastError(out ErrCode, out ErrMsg);
                            message = message + ",Error Code : " + ErrCode + ", Error Message:" + ErrMsg;
                            Console.WriteLine(message);
                            sap_status = "Error_OrderEntry";
                        }

                        if (oCompany != null)
                        {
                            if (oCompany.Connected) oCompany.Disconnect();

                        }

                    }
                    else
                    {
                        message = "";
                        DocEntry = oCompany.GetNewObjectKey();
                        DocNumber = "";
                        string Sql = "SELECT DocNum,DocDate FROM ORDR WITH (NOLOCK) WHERE DocEntry = '" + DocEntry + "'";
                        oRecSet = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                        oRecSet.DoQuery(Sql);
                        if (oRecSet.RecordCount > 0)
                        {

                            DocNumber = Convert.ToString(oRecSet.Fields.Item("DocNum").Value);
                            DocDate = oRecSet.Fields.Item("DocDate").Value;
                            message = "DocEntry:" + DocEntry + ",DocNum:" + DocNumber + ",DocDate:" + DocDate.ToString("dd-MMM-yyyy");
                            sap_status = "Success_OrderEntry";
                            SaveColourDetails(DocEntry, objData.OrderColourDataList);

                            String Sqlupdatedefalt = "update ORDR set U_ColorRAND= '" + DocEntry + "'  where DocEntry = '" + DocEntry + "'";
                            oRecSet = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                            oRecSet.DoQuery(Sqlupdatedefalt);

                            SaveEmployeeDetails(DocEntry, objData.OrderEmployeeDataList);

                            if (oCompany != null)
                            {
                                if (oCompany.Connected) oCompany.Disconnect();


                            }
                        }

                    }
                    Response response = new Response();
                    response.OrderID = Convert.ToInt32(header.PoNumber);
                    response.BUID = 0;
                    response.Brandid = 0;
                    response.OrderNo = header.PoNumber;
                    response.OrderAmendmentID = header.OrderAmendmentId;
                    response.poid = Convert.ToInt32(header.PoNumber);
                    response.sap_docnum = DocNumber;
                    response.sap_docentry = DocEntry;
                    response.sap_docdate = DocDate.ToString("yyyy-MM-dd");
                    response.sap_Numatcard = header.RefNumber.ToString();
                    response.sap_status = sap_status;
                    response.sap_Error = message;
                    response.Whscode = header.AreaCode;
                    string reqdata = helpers.object_to_Json(response);
                    var content = new StringContent(reqdata.ToString(), Encoding.UTF8, "application/json");
                    string baseAddress = con.BaseURL;
                    string posturl = "api/Order/Update_Resp_Order";
                    var client = new HttpClient();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var Response = client.PostAsync(baseAddress + posturl, content).Result;
                    dynamic Result = helpers.json_to_object(Response.Content.ReadAsStringAsync().Result);
                    Console.WriteLine(message + " / " + Result + "\n");

                }
            }

            catch (Exception e)
            {
                message += e.Message + "," + "Error in Main Json No Record";
                Console.WriteLine(message);
                if (oCompany.Connected) oCompany.Disconnect();




            }
            Console.WriteLine("Order Posted");
        }

        private void SaveColourDetails(string Refdocentry, List<OrderColourData> ColourDetails)
        {

            //dynamic ItemColors = ((IEnumerable<dynamic>)ColourDetails).ToList();
            //var json = JsonConvert.SerializeObject(ItemColors);
            //DataTable dataTable = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));

            try
            {

                string conString = "Data Source=" + con.Server + ",1433;Network Library=DBMSSOCN;TrustServerCertificate=True;Initial Catalog=" + con.Database + ";User ID=" + con.UserName + ";Password=" + con.Password + ";";

                SqlConnection cons = new SqlConnection(conString);

                SqlDataAdapter daDetails = new SqlDataAdapter();
                DataSet dsDetails = new DataSet();
                daDetails.SelectCommand = new SqlCommand();

                if (cons != null && cons.State == ConnectionState.Closed)
                {
                    cons.Open();
                }
                daDetails.SelectCommand.Connection = cons;
                daDetails.SelectCommand.CommandType = CommandType.StoredProcedure;
                daDetails.SelectCommand.CommandText = "[InsertOrdrColourDetails]";
                daDetails.SelectCommand.Parameters.AddWithValue("@docentry", Refdocentry);
                daDetails.SelectCommand.Parameters.AddWithValue("@tblColour", ColourDetails);
                daDetails.SelectCommand.ExecuteNonQuery();
                daDetails.SelectCommand.Dispose();
                cons.Close();




            }
            catch (Exception exception)
            {

                message += exception.Message + "," + "Error in Colour Json";
                if (oCompany.Connected) oCompany.Disconnect();



            }






        }

        private void SaveEmployeeDetails(string Refdocentry, List<OrderEmployeeData> EmployeeDetails)
        {

            //dynamic EmployeeDetail = ((IEnumerable<dynamic>)EmployeeDetails).ToList();
            //var json = JsonConvert.SerializeObject(EmployeeDetail);
            //string jsonrs = JsonConvert.SerializeObject(EmployeeDetail);
            //if (EmployeeDetails > 0)
            //{
            //    DataTable dataTable = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));

            try
            {


                string conString = "Data Source=" + con.Server + ",1433;Network Library=DBMSSOCN;TrustServerCertificate=True;Initial Catalog=" + con.Database + ";User ID=" + con.UserName + ";Password=" + con.Password + ";";

                SqlConnection cons = new SqlConnection(conString);

                SqlDataAdapter daDetails = new SqlDataAdapter();
                DataSet dsDetails = new DataSet();
                daDetails.SelectCommand = new SqlCommand();

                if (cons != null && cons.State == ConnectionState.Closed)
                {
                    cons.Open();
                }
                daDetails.SelectCommand.Connection = cons;
                daDetails.SelectCommand.CommandType = CommandType.StoredProcedure;
                daDetails.SelectCommand.CommandText = "[InsertOrdrEmployeeDetails]";
                daDetails.SelectCommand.Parameters.AddWithValue("@docentry", Refdocentry);
                daDetails.SelectCommand.Parameters.AddWithValue("@tblEmp", EmployeeDetails);
                daDetails.SelectCommand.ExecuteNonQuery();
                daDetails.SelectCommand.Dispose();
                cons.Close();





            }
            catch (Exception exception)
            {

                message += exception.Message + "," + "Error in Employee Json";
                if (oCompany.Connected) oCompany.Disconnect();

            }

            // dataTable.Dispose();
        }


        //List<OrderDetails> SapDetails = new List<OrderDetails>();






    }

}
