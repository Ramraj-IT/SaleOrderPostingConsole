using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleOrderPostingConsole.Controllers
{
    public class Response
    {
        public int poid { get; set; }
        public int BUID { get; set; }
        public int OrderID { get; set; }
        public int Brandid { get; set; }
        public string OrderNo { get; set; }
        public string sap_status { get; set; }
        public string sap_docentry { get; set; }
        public string sap_docnum { get; set; }
        public string sap_docdate { get; set; }
        public string sap_Numatcard { get; set; }
        public string sap_Error { get; set; }
        public string Whscode { get; set; }
        public int OrderAmendmentID { get; set; }
    }
}
