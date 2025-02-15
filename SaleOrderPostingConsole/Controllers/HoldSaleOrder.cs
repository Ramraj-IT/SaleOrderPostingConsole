using Newtonsoft.Json;
using SaleOrderPostingConsole.Models;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static SaleOrderPostingConsole.Models.OrderModel;

namespace SaleOrderPostingConsole.Controllers
{
    public class HoldSaleOrder
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

        public  string  loadhold_data(int bucode) 
        {
            try
            {
                string baseAddress = con.BaseURL; 
                string posturl = "api/Order/Hold_Order?BUID=" + bucode + ""; //lst_dt.Rows[0]["api_post_url"].ToString();
                var client = new HttpClient();
                var response =   client.GetAsync(baseAddress + posturl).Result;
               // Ensure request success

                string jsonString =   response.Content.ReadAsStringAsync().Result;
                List<HoldData> paramList = JsonConvert.DeserializeObject<List<HoldData>>(jsonString);
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //var Response = client.GetAsync(baseAddress + posturl).Result;
                //dynamic Result = helpers.json_to_object(Response.Content.ReadAsStringAsync().Result);

                foreach (HoldData res in paramList)
                {  
                    Hold_Order(res); 

                }
                return "Success";

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n");
                return "Fail " + ex.Message;

            }


        }
        public void Hold_Order(dynamic objData)
        {

            var current_dt = DateTime.Now;
            String message = "";
            string card_code = "";
            con = new Connectivity();

            try
            {
                dynamic jsonData = objData;
                Int32 DocEntry = jsonData.SAPDocEntry;
                string po_number = jsonData.po_number;
                string orderamendmentid = jsonData.OrderAmendmentID;
                string remarks = jsonData.OrderRemarks;
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
                    Ordr.UserFields.Fields.Item("U_OrdRemarks").Value = remarks;
                    RetVal = Ordr.Update();
                    oCompany.GetNewObjectCode(out tempStr);
                    card_code = Ordr.CardCode;
                    if (RetVal != 0)
                    {
                        oCompany.GetLastError(out ErrCode, out ErrMsg);

                        oRecSet = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);



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
                        response.sap_status = "Hold";
                        response.sap_Error = remarks;
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
                //return Content(HttpStatusCode.BadRequest, e);
            }
            Console.WriteLine("Hold Order");
        }
    }
}
