using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using SaleOrderPostingConsole.Models;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static SaleOrderPostingConsole.Models.OrderModel;

namespace SaleOrderPostingConsole.Controllers
{
    public class UpdateSaleOrder
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


        public async Task<String> loadupdate_data(int bucode)

        {

            try
            {

                string baseAddress = con.BaseURL;
                //string baseAddress = api_dt.Rows[0]["api_server_url"].ToString();
                string posturl = "api/Order/Edit_Order?BUID=" + bucode + ""; //lst_dt.Rows[0]["api_post_url"].ToString();
                var client = new HttpClient();

                HttpResponseMessage response = await client.GetAsync(baseAddress + posturl);
                response.EnsureSuccessStatusCode(); // Ensure request success

                string jsonString = await response.Content.ReadAsStringAsync();
                List<OrderData> paramList = JsonConvert.DeserializeObject<List<OrderData>>(jsonString);
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //client.Timeout = TimeSpan.FromMinutes(10);
                //var Response = client.GetAsync(baseAddress + posturl).Result;
                //dynamic Result = helpers.json_to_object(Response.Content.ReadAsStringAsync().Result);

                foreach (OrderData res in paramList)
                {

                    Post_Order_update(res);


                }

                return "Success";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n");

                return "Fail " + ex.Message;
            }


        }

        private void Post_Order_update(OrderData objData)
        {
            DateTime current_dt = DateTime.Now;
            double forwarding_taxrate = 0;
            string forwarding_taxcode = "";

            OrderHeader header = new OrderHeader();
            header = objData.OrderHeader;

            try
            {

                //dynamic jsonData = objData.Header[0];
                //dynamic order_details = objData.order_details;
                //dynamic order_ColourDetails = objData.order_ColourDetails;
                //string mode = jsonData.entry_mode;
                //string company_code = jsonData.company_code;
                //string header.CardCode = jsonData.header.CardCode;
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
                //string Docentry = jsonData.sAPDocEntry;
                //string refnojson = jsonData.ref_number;
                //string orderamendmentid = jsonData.orderAmendmentID;
                //double? forwarding_charges = jsonData.forwarding_charges;
                //string SapOrderType = jsonData.sapOrderType;
                //string FreightType = jsonData.freightType;
                //string OrderReason = jsonData.orderReason;
                //string ShiptoCode = jsonData.shiptoCode;
                //string TransporterId = jsonData.transporterId;
                //string TransporterName = jsonData.transporterName;
                //string TransportMode = jsonData.transportMode;

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
                else
                {
                    oCompany.DbServerType = BoDataServerTypes.dst_MSSQL2012;
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
                    Console.WriteLine(lErrCode + "-" + sErrMsg);

                    if (oCompany != null)
                    {
                        if (oCompany.Connected) oCompany.Disconnect();
                    }

                    //return Content(HttpStatusCode.PreconditionFailed, "Error On SAP Connect \n" + lErrCode.ToString() + " - " + sErrMsg);
                }
                else
                {
                    Ordr = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);
                    oRecSet = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                    Ordr.GetByKey(Convert.ToInt32(header.SapDocEntry));
                    int Cnt = Ordr.Lines.Count;
                    Ordr.Rounding = BoYesNoEnum.tYES;
                    string Confirmed = Ordr.Confirmed.ToString();


                    if (Confirmed == "tNO")
                    {

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
                            string SAPTransporterId = oRecSet.Fields.Item("TransporterId").Value;
                            string SAPTransporterName = oRecSet.Fields.Item("TransporterName").Value;

                            if ((con.Database == "RHLLIVE" || con.Database == "RRLIVE" || con.Database == "AKGLIVE" || con.Database == "VTLIVE" ? true : con.Database == "ANTSPRODLIVE"))
                            {

                                Ordr.UserFields.Fields.Item("U_Destination").Value = (header.Destination == "" || header.Destination == null ? SAPDestination : header.Destination);
                                Ordr.UserFields.Fields.Item("U_Trsport").Value = (header.Transport == "" || header.Transport == null ? SAPTranport : header.Transport);
                            }


                            Ordr.UserFields.Fields.Item("U_TransporterId").Value = (header.TransporterId == "" || header.TransporterId == null ? SAPTransporterId : header.TransporterId);
                            Ordr.UserFields.Fields.Item("U_TransporterName").Value = (header.TransporterName == "" || header.TransporterName == null ? SAPTransporterName : header.TransporterName);
                            Ordr.UserFields.Fields.Item("U_Transport").Value = (header.Transport == "" || header.Transport == null ? SAPTranport : header.Transport);
                            Ordr.UserFields.Fields.Item("U_Dsnation").Value = (header.Destination == "" || header.Destination == null ? SAPDestination : header.Destination);
                            //Ordr.UserFields.Fields.Item("U_Pass").Value = (Destination == "" || Destination == null ? TransportMode : Destination);

                            //
                            if (header.OverallDueDate != "")
                            {
                                Ordr.DocDueDate = Convert.ToDateTime(header.OverallDueDate);
                            }
                            Ordr.Comments = SAPnotess + " " + header.Remarks.ToUpper();
                        }


                        for (int i = 0; i < Cnt; i++)
                        {
                            Ordr.Lines.VisualOrder.Equals(i);
                            Ordr.Lines.Delete();

                        }

                        if (header.ShipToCode != "")
                        {
                            //if ((header.CardCode == "C059972") || (header.CardCode == "C061152"))
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
                        foreach (OrderItemData dtl in objData.OrderItemDataList)
                        {

                            //int row_num = dtl.row_num;
                            //string item_code = dtl.item_code;
                            //string item_desc = dtl.item_desc;
                            //string CatalogCode = dtl.catalogCode;
                            //string CatalogName = dtl.catalogName;
                            //string hsn_code = dtl.hsn_code;
                            //string uom = dtl.uom;
                            double qty = (double)dtl.Qty;
                            double OfferQty = (double)dtl.OfferQty;
                            string qtyy = dtl.Qty.ToString();
                            double rate = (double)dtl.Rate;
                            double MRP = (double)dtl.MRP;
                            //string tax_code = dtl.tax_code;
                            DateTime item_due = (DateTime)dtl.DeliveryDate;
                            //double? tax_per = dtl.tax_per;
                            //double? tax_amount = dtl.tax_amount;
                            //double? dis_per = dtl.dis_per;
                            //double? dis_amount = dtl.dis_amount;
                            //double? nett_amount = dtl.nett_amount;
                            //string line_remarks = dtl.line_remarks;
                            //double? line_value = dtl.line_value;
                            //string style = dtl.style;
                            //string size = dtl.size;
                            //string SAPDocEntry = dtl.sAPDocEntry;
                            //string offer_code = dtl.offer_code;

                            Ordr.Lines.SetCurrentLine(dtl.RowNum);
                            Ordr.Lines.ItemCode = dtl.ItemCode.ToUpper();
                            Ordr.Lines.ItemDescription = dtl.ItemDesc.ToUpper();
                            Ordr.Lines.DiscountPercent = dtl.DisPer;



                            if (con.Database == "ANTSPRODLIVE")
                            {
                                //string Sqlwhscode = ("select DfltWH  from OITM WHERE ItemCode = '" + dtl.item_code + "'");
                                //oRecSetwhscode = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                //oRecSetwhscode.DoQuery(Sqlwhscode);
                                //if (oRecSetwhscode.RecordCount > 0)
                                //{
                                //    Ordr.Lines.WarehouseCode = oRecSetwhscode.Fields.Item("DfltWH").Value;
                                //}

                                string Sql4 = ("select ItemCode,U_RNAME ItemName,U_Scode SRCode,U_Size,U_Style from OITM WHERE ItemCode = '" + dtl.ItemCode + "'");
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
                                string Sql4 = "select B.U_CatalgCode ,convert(nvarchar(max), B.U_Remarks) SRCode,A.U_Size,A.U_Style  from [@INS_OPLM] A INNER JOIN [@INS_PLM1] B ON B.DocEntry = A.DocEntry INNER JOIN OITM C ON A.U_ItemCode = C.ItemCode AND ISNULL(U_Lock,'') = 'N' WHERE U_ItemCode = '" + dtl.ItemCode + "' AND ISNULL(U_SubBrand,'') = '" + header.SubBrand + "'";
                                oRecSet = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                oRecSet.DoQuery(Sql4);
                                if (oRecSet.RecordCount > 0)
                                {
                                    Ordr.Lines.UserFields.Fields.Item("U_CatalogCode").Value = oRecSet.Fields.Item("U_CatalgCode").Value;
                                    Ordr.Lines.UserFields.Fields.Item("U_CatalogName").Value = oRecSet.Fields.Item("U_CatalgCode").Value;
                                    Ordr.Lines.UserFields.Fields.Item("U_SRCode").Value = oRecSet.Fields.Item("SRCode").Value;
                                    Ordr.Lines.UserFields.Fields.Item("U_Size").Value = oRecSet.Fields.Item("U_Size").Value;
                                    Ordr.Lines.UserFields.Fields.Item("U_Style").Value = oRecSet.Fields.Item("U_Style").Value;
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
                            Ordr.Lines.UserFields.Fields.Item("U_LineRemarks").Value = dtl.LineRemarks.ToUpper();
                            Ordr.Lines.DiscountPercent = (dtl.DisPer == 0 ? 0 : Convert.ToDouble(dtl.DisPer));


                            Ordr.Lines.UserFields.Fields.Item("U_Offer_Code").Value = Convert.ToString(dtl.OfferCode);
                            Ordr.Lines.UserFields.Fields.Item("U_FreeQty").Value = Convert.ToString(OfferQty);


                            Ordr.Lines.UserFields.Fields.Item("U_NOpenQty").Value = qty;
                            if ((con.Database != "RRFLIVE") && (header.Brand.ToUpper() != "VIVEAGHAM FABRIC"))
                            {
                                Ordr.Lines.Quantity = qty;
                                Ordr.Lines.UserFields.Fields.Item("U_NoofPiece").Value = Convert.ToInt32(qty);
                            }
                            else if (dtl.UOM != 2)
                            {
                                string Sql5 = "SELECT isnull(CONVERT(Decimal(18,9),  " + qty + ")  * case When charindex('CMS',u_length)> 0  then 1  when cast(t1.u_length as Decimal(16,9))<20 and charindex('CMS',u_length)=0  then   cast(t1.u_length as Decimal(16,9)) ELSE 0 end ,0) as MTRS  FROM OITM T0 LEFT JOIN [@INCM_SZE1] T1 on T0.U_Size=T1.U_Name  where T0.Itemcode= '" + dtl.ItemCode + "'  and ISNULL(T0.InvntryUom,'') = 'NOS'";
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

                                double priceaftdis = rate - rate * (dtl.DisPer == 0 ? 0 : Convert.ToDouble(dtl.DisPer)) / 100;
                                string Sqltax = "Exec [@INSPL_Sales_TaxRate] '" + header.CardCode + "','" + dtl.ItemCode + "','" + priceaftdis + "'";
                                oRecSettax = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                oRecSettax.DoQuery(Sqltax);
                                if (oRecSettax.RecordCount > 0)
                                {
                                    Ordr.Lines.TaxCode = oRecSettax.Fields.Item("TAXRATE").Value;
                                    double rate_tax = Convert.ToDouble(oRecSettax.Fields.Item("rate_tax").Value);
                                    forwarding_taxrate = forwarding_taxrate > rate_tax ? forwarding_taxrate : rate_tax;
                                    forwarding_taxcode = oRecSettax.Fields.Item("taxcode").Value + forwarding_taxrate.ToString();


                                }
                            }




                            Ordr.Lines.UnitPrice = rate;
                            Ordr.Lines.UserFields.Fields.Item("U_HSNCODE").Value = dtl.HsnCode;
                            Ordr.Lines.ShipDate = item_due;
                            Ordr.Lines.BaseType = -1;



                            Ordr.Lines.Add();



                        }

                        if (header.ForwardingCharges != 0)
                        {
                            Ordr.Expenses.ExpenseCode = con.expensecode;
                            Ordr.Expenses.TaxCode = forwarding_taxcode;
                            Ordr.Expenses.LineTotal = Convert.ToDouble(header.ForwardingCharges);
                        }

                        Ordr.UserFields.Fields.Item("U_TotQty").Value = header.TotalQty;
                        Ordr.UserFields.Fields.Item("U_OrdType").Value = header.SapOrderType;
                        if (header.FreightType != null && header.FreightType != "")
                        {
                            Ordr.UserFields.Fields.Item("U_Pass").Value = header.FreightType;
                        }
                        if (header.OrderReason != null && header.OrderReason != "")
                        {
                            Ordr.UserFields.Fields.Item("U_Reason").Value = header.OrderReason;
                        }

                        RetVal = Ordr.Update();

                        oCompany.GetNewObjectCode(out tempStr);
                        string DocEntry = "";
                        string DocNumber = "";
                        string sap_status = "";
                        DateTime DocDate = DateTime.Now;
                        if (RetVal != 0)
                        {

                            message = "";
                            oCompany.GetLastError(out ErrCode, out ErrMsg);
                            Console.WriteLine(ErrCode + "-" + ErrMsg);

                            message = message + ",Error Code : " + ErrCode + ", Error Message:" + ErrMsg;
                            sap_status = "Error_OrderUpdate";




                            //return Content(HttpStatusCode.PreconditionFailed, message);
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
                                message = "";
                                DocNumber = Convert.ToString(oRecSet.Fields.Item("DocNum").Value);
                                DocDate = oRecSet.Fields.Item("DocDate").Value;
                                sap_status = "Success_OrderUpdate";
                                message = "DocEntry:" + DocEntry + ",DocNum:" + DocNumber + ",DocDate:" + DocDate.ToString("dd-MMM-yyyy");
                                UpadteColourDetails(DocEntry, objData.OrderColourDataList);
                                String Sqlupdatedefalt = "update ORDR set U_ColorRAND= '" + DocEntry + "'  where DocEntry = '" + DocEntry + "'";
                                oRecSet = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                oRecSet.DoQuery(Sqlupdatedefalt);

                            }




                        }
                        Response response = new Response();
                        response.OrderID = Convert.ToInt32(header.PoNumber);
                        response.BUID = 0;
                        response.Brandid = 0;
                        response.OrderNo = header.PoNumber;
                        response.OrderAmendmentID = Convert.ToInt32(header.OrderAmendmentId);
                        response.poid = Convert.ToInt32(header.PoNumber);
                        response.sap_docnum = DocNumber;
                        response.sap_docentry = DocEntry;
                        response.sap_docdate = DocDate.ToString("yyyy-MM-dd");
                        response.sap_Numatcard = header.RefNumber.ToString();
                        response.sap_status = sap_status;
                        response.sap_Error = message.ToString();
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


                        if (oCompany != null)
                        {
                            if (oCompany.Connected) oCompany.Disconnect();


                        }

                    }
                    else
                    {
                        string DocEntry = header.SapDocEntry.ToString();
                        string DocNumber = "";
                        string sap_status = "";
                        DateTime DocDate = DateTime.Now;
                        string Sql = "SELECT DocNum,DocDate FROM ORDR WITH (NOLOCK) WHERE DocEntry = '" + DocEntry + "'";
                        oRecSet = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                        oRecSet.DoQuery(Sql);
                        if (oRecSet.RecordCount > 0)
                        {
                            DocNumber = Convert.ToString(oRecSet.Fields.Item("DocNum").Value);
                            DocDate = oRecSet.Fields.Item("DocDate").Value;
                            sap_status = "Success_OrderUpdate";
                            message = "";
                            message = "DocEntry:" + DocEntry + ",DocNum:" + DocNumber + ",DocDate:" + DocDate.ToString("dd-MMM-yyyy");
                            //if(order_ColourDetails != null || order_ColourDetails.Length >0 ) {
                            //    UpadteColourDetails(DocEntry, order_ColourDetails);
                            //}

                            String Sqlupdatedefalt = "update ORDR set U_ColorRAND= '" + DocEntry + "'  where DocEntry = '" + DocEntry + "'";
                            oRecSet = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                            oRecSet.DoQuery(Sqlupdatedefalt);

                        }

                        Response response = new Response();
                        response.OrderID = Convert.ToInt32(header.PoNumber);
                        response.BUID = 0;
                        response.Brandid = 0;
                        response.OrderNo = header.PoNumber;
                        response.OrderAmendmentID = Convert.ToInt32(header.OrderAmendmentId);
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


                        if (oCompany != null)
                        {
                            if (oCompany.Connected) oCompany.Disconnect();


                        }
                    }



                    //return Ok(message);
                }
            }

            catch (Exception e)
            {

                message += e.Message + "," + "Error in Main Json";
                if (oCompany != null)
                {
                    if (oCompany.Connected) oCompany.Disconnect();


                }

                Console.WriteLine(message);
                //return Content(HttpStatusCode.BadRequest, message);

            }
            Console.WriteLine("Order updated");
        }
        private void UpadteColourDetails(string Refdocentry, List<OrderColourData> ColourDetails)
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
                daDetails.SelectCommand.CommandText = "[UpdateOrdrColourDetails]";
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

            // dataTable.Dispose();



            // List<OrderDetails> SapDetails = new List<OrderDetails>();



        }

        //private void UpadteEmployeeDetails(string Refdocentry, List<OrderEmployeeData> EmployeeDetails)
        //{

        //    //dynamic EmployeeDetail = ((IEnumerable<dynamic>)EmployeeDetails).ToList();
        //    //var json = JsonConvert.SerializeObject(EmployeeDetail);
        //    //string jsonrs = JsonConvert.SerializeObject(EmployeeDetail);
        //    //if (jsonrs.ToString().Length > 0)
        //    //{
        //        DataTable dataTable = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));

        //        try
        //        {


        //            string conString = "Data Source=" + con.Server + ",1433;Network Library=DBMSSOCN;TrustServerCertificate=True;Initial Catalog=" + con.Database    + ";User ID=" + con.UserName + ";Password=" + con.Password + ";";

        //            SqlConnection cons = new SqlConnection(conString);

        //            SqlDataAdapter daDetails = new SqlDataAdapter();
        //            DataSet dsDetails = new DataSet();
        //            daDetails.SelectCommand = new SqlCommand();

        //            if (cons != null && cons.State == ConnectionState.Closed)
        //            {
        //                cons.Open();
        //            }
        //            daDetails.SelectCommand.Connection = cons;
        //            daDetails.SelectCommand.CommandType = CommandType.StoredProcedure;
        //            daDetails.SelectCommand.CommandText = "[UpdateOrdrEmployeeDetails]";
        //            daDetails.SelectCommand.Parameters.AddWithValue("@docentry", Refdocentry);
        //            daDetails.SelectCommand.Parameters.AddWithValue("@tblEmp", dataTable);
        //            daDetails.SelectCommand.ExecuteNonQuery();
        //            daDetails.SelectCommand.Dispose();
        //            cons.Close();





        //        }
        //        catch (Exception exception)
        //        {

        //            message += exception.Message + "," + "Error in Employee Json";
        //            if (oCompany.Connected) oCompany.Disconnect();
        //        }


        //    }


        // List<OrderDetails> SapDetails = new List<OrderDetails>();



    }

}
 
