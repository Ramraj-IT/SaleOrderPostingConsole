using Newtonsoft.Json;
using SaleOrderPostingConsole.Models;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static SaleOrderPostingConsole.Models.OrderModel;

namespace SaleOrderPostingConsole.Controllers
{
    public class CancelSaleOrder
    {
        Connectivity con = new Connectivity();

        Helpers helpers = new Helpers();
        String message = "";
        public Company oCompany = null;
        public Documents Ordr = null;
        private Recordset oRecSet = null;
        Int32 lRetCode;
        Int32 lErrCode;
        String sErrMsg;
        int RetVal;

        String tempStr;

        int ErrCode;
        String ErrMsg;
        String a;

        public  string  loadcancel_data(int bucode)

        {

            try
            {

                string baseAddress = con.BaseURL;
                //string baseAddress = api_dt.Rows[0]["api_server_url"].ToString();
                string posturl = "api/Order/Cancel_Order?BUID=" + bucode + ""; //lst_dt.Rows[0]["api_post_url"].ToString();
                var client = new HttpClient();
                var response =   client.GetAsync(baseAddress + posturl).Result;
             

                string jsonString =   response.Content.ReadAsStringAsync().Result;
                List<CancelData> paramList = JsonConvert.DeserializeObject<List<CancelData>>(jsonString);
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //client.Timeout = TimeSpan.FromMinutes(50);
                //var Response = client.GetAsync(baseAddress + posturl).Result;
                //dynamic Result = helpers.json_to_object(Response.Content.ReadAsStringAsync().Result);

                foreach (CancelData res in paramList)
                {

                    Cancel_Order(res);



                }
                return "Success";

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n");
                return "Fail " + ex.Message;

            }


        }


        public void Cancel_Order(CancelData objData)
        {
            var current_dt = DateTime.Now;
            String message = "";
            //SaveJson(objData);
            con = new Connectivity();

            try
            { 
                Int32 DocEntry = objData.SapDocEntry;
                String Reason = objData.RevisionReason;
                string po_number = objData.OrderAmendmentID.ToString();
                string orderamendmentid = objData.OrderAmendmentID.ToString();
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

                }
                else
                {
                    Ordr = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);
                    Ordr.GetByKey(DocEntry);

                    string Sql1 = "select DocStatus,Canceled from [ORDR]  where DocEntry = " + DocEntry;
                    oRecSet = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                    oRecSet.DoQuery(Sql1);
                    if (oRecSet.RecordCount > 0)
                    {
                        string docstatus = oRecSet.Fields.Item("DocStatus").Value;
                        string cancelstatus = oRecSet.Fields.Item("Canceled").Value;
                        if (docstatus == "O")
                        {
                            Ordr.Comments = Reason;
                            Ordr.Update();
                            RetVal = Ordr.Cancel();
                        }
                        else
                        {
                            message = "Order Closed";
                            Response response = new Response();
                            response.OrderID = Convert.ToInt32(0);
                            response.BUID = 0;
                            response.Brandid = 0;
                            response.OrderNo = "0";
                            response.OrderAmendmentID = Convert.ToInt32(orderamendmentid);
                            response.poid = Convert.ToInt32(0);
                            response.sap_docnum = DocEntry.ToString();
                            response.sap_docentry = DocEntry.ToString();
                            response.sap_docdate = current_dt.ToString("yyyy-MM-dd");
                            response.sap_Numatcard = "";
                            response.sap_status = "Order Closed";
                            response.sap_Error = "Order Closed";
                            response.Whscode = "";
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


                    oCompany.GetNewObjectCode(out tempStr);
                    if (RetVal != 0)
                    {
                        message = "";
                        oCompany.GetLastError(out ErrCode, out ErrMsg);

                        message = ErrMsg;
                        Response response = new Response();
                        response.OrderID = Convert.ToInt32(0);
                        response.BUID = 0;
                        response.Brandid = 0;
                        response.OrderNo = "0";
                        response.OrderAmendmentID = Convert.ToInt32(orderamendmentid);
                        response.poid = Convert.ToInt32(0);
                        response.sap_docnum = DocEntry.ToString();
                        response.sap_docentry = DocEntry.ToString();
                        response.sap_docdate = current_dt.ToString("yyyy-MM-dd");
                        response.sap_Numatcard = "";
                        response.sap_status = "Error";
                        response.sap_Error = message;
                        response.Whscode = "";
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

                        Response response = new Response();
                        response.OrderID = Convert.ToInt32(0);
                        response.BUID = 0;
                        response.Brandid = 0;
                        response.OrderNo = "0";
                        response.OrderAmendmentID = Convert.ToInt32(orderamendmentid);
                        response.poid = Convert.ToInt32(0);
                        response.sap_docnum = DocEntry.ToString();
                        response.sap_docentry = DocEntry.ToString();
                        response.sap_docdate = current_dt.ToString("yyyy-MM-dd");
                        response.sap_Numatcard = "";
                        response.sap_status = "Cancelled";
                        response.sap_Error = message;
                        response.Whscode = "";
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

                }
            }
            catch (Exception e)
            {
                message += "Error" + e.Message;
                if (oCompany.Connected) oCompany.Disconnect();
                Console.WriteLine(message);
            }

            Console.WriteLine("Cancel Order");
        }
    }
}
