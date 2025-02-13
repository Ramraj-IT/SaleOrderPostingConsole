using Newtonsoft.Json;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SaleOrderPostingConsole.Controllers
{
    public class Helpers
    {
       
        public string object_to_Json(dynamic table)
        {
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(table, Newtonsoft.Json.Formatting.Indented);
            return JSONString;
        }

        public dynamic json_to_object(dynamic table)
        {
            dynamic myjson_object = JsonConvert.DeserializeObject(table);
            return myjson_object;
        }


    }
}
