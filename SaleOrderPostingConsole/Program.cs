using System;
using Microsoft.Extensions.Configuration;
using SaleOrderPostingConsole.Controllers;
using SaleOrderPostingConsole.Models;

namespace SaleOrderPostingConsole
{
    class Program
    {
        static void Main(string[] args)
        {// Set up configuration




            Connectivity connectivity = new Connectivity();
            int bu = connectivity.BuId;
            string server = connectivity.Server;


            switch (connectivity.PostingMethod)
            {
                case "ADD":
                    CreateSaleOrder createSaleOrder = new CreateSaleOrder();
                    _ = createSaleOrder.loadorder_data(bu);
                    break;
                case "EDIT":
                    UpdateSaleOrder updateSaleOrder = new UpdateSaleOrder();
                    _ = updateSaleOrder.loadupdate_data(bu);
                    break;
                case "APPROVE":
                    ApproveSaleOrder approveSaleOrder = new ApproveSaleOrder();
                    _ = approveSaleOrder.loadconfirm_data(bu);
                    break;
                case "CANCEL":
                    CancelSaleOrder cancelSaleOrder = new CancelSaleOrder();
                    _ = cancelSaleOrder.loadcancel_data(bu);
                    break;
                case "HOLD":
                    HoldSaleOrder holdSaleOrder = new HoldSaleOrder();
                    _ = holdSaleOrder.loadhold_data(bu);
                    break;
                default:
                    Console.WriteLine("NO POSTING METHOD FOUND..!!!!!");
                    break;

            }




            Console.WriteLine("********Completed...!**********");


        }
    }
}
