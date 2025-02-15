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
    public class ApproveSaleOrder
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

        public String  loadconfirm_data(int bucode)
        {

            try
            {

                string baseAddress = con.BaseURL;
                string posturl = "api/Order/Approve_Order?BUID=" + bucode + "";
                var client = new HttpClient();
                var response =   client.GetAsync(baseAddress + posturl).Result;
                

                string jsonString =   response.Content.ReadAsStringAsync().Result;
                List<ApproveData> paramList = JsonConvert.DeserializeObject<List<ApproveData>>(jsonString);
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //var Response = client.GetAsync(baseAddress + posturl).Result;
                //dynamic Result = helpers.json_to_object(Response.Content.ReadAsStringAsync().Result);

                foreach (ApproveData res in paramList)
                {
                    Confirm_Order(res);
                }

                return "Success";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n");
                return "Error " + ex.Message;
            }


        }

        public void Confirm_Order(ApproveData objData)
        {
            var current_dt = DateTime.Now;
            String message = "";
            string card_code = "";

            con = new Connectivity();

            try
            { 
                Int32 DocEntry = objData.SapDocEntry;
                string po_number = objData.OrderAmendmentId.ToString();
                string orderamendmentid = objData.OrderAmendmentId.ToString();
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
                    Console.WriteLine(lErrCode + " - " + sErrMsg);
                    if (oCompany != null)
                    {
                        if (oCompany.Connected) oCompany.Disconnect();


                    }

                    //return Content(HttpStatusCode.PreconditionFailed, "Error On SAP Connect \n" + lErrCode.ToString() + " - " + sErrMsg);
                }
                else
                {
                    Ordr = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);
                    Ordr.GetByKey(DocEntry);
                    Ordr.Confirmed = BoYesNoEnum.tYES;
                    Ordr.UserFields.Fields.Item("U_AppDate").Value = current_dt.ToString("yyyy-MM-dd");
                    Ordr.UserFields.Fields.Item("U_AppTime").Value = current_dt.ToString("HH:mm:ss");
                    card_code = Ordr.CardCode;
                    RetVal = Ordr.Update();
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
                        try
                        {
                            var client = new HttpClient();
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                            var Response = client.PostAsync(baseAddress + posturl, content).Result;
                            dynamic Result = helpers.json_to_object(Response.Content.ReadAsStringAsync().Result);
                            Console.WriteLine(message);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }

                        if (oCompany != null)
                        {
                            if (oCompany.Connected) oCompany.Disconnect();


                        }
                    }

                    else
                    {
                        Response response = new Response();
                        message = "Confirmed Approval";
                        response.OrderID = Convert.ToInt32(0);
                        response.BUID = 0;
                        response.Brandid = 0;
                        response.OrderNo = "0";
                        response.OrderAmendmentID = Convert.ToInt32(orderamendmentid);
                        response.poid = Convert.ToInt32(0);
                        response.sap_docnum = DocEntry.ToString();
                        response.sap_docentry = DocEntry.ToString();
                        response.sap_docdate = "";
                        response.sap_Numatcard = "";
                        response.sap_status = "Confirmed";
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
                Console.WriteLine(message);
                if (oCompany.Connected) oCompany.Disconnect();

                //return Content(HttpStatusCode.BadRequest, e);
            }

            Console.WriteLine("Confirm Order");
        }

    }
}
