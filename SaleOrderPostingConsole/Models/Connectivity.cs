using Microsoft.Extensions.Configuration;
using Sha_Chiper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using ConfigurationBuilder = Microsoft.Extensions.Configuration.ConfigurationBuilder;

namespace SaleOrderPostingConsole.Models
{
    public class Connectivity
    {
        public String BaseURL; 
        public String Server;
        public String UserName;
        public String Password;
        public String Database;
        public Int32 Branch;
        public String DbVersion;
        public String SecretKey;
        public String SapUserName;
        public String SapPassword;
        public String SapLicenseServer;
        public String WinAuth;

        public Int32 Series;
        public Int32 BuId;
        public String PostingMethod;
        public Int32 expensecode;

        public Int32 SeriesVT;
        public Int32 SeriesTS;
        public Int32 BranchVT;
        public Int32 BranchTS;
        public Connectivity()
        {
            var config = new  ConfigurationBuilder()
               .SetBasePath(AppContext.BaseDirectory)
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .Build();

            BaseURL = config["appsettings:Baseurl"].ToString();
            SecretKey = config["appsettings:secretkey"].ToString();
            Server = config["appsettings:server"].ToString();
            UserName = config["appsettings:username"].ToString();
            Password = Cipher.Decrypt(config["appsettings:password"].ToString(), SecretKey);
            Database = config["appsettings:database"].ToString();
            WinAuth = config["appsettings:winauth"].ToString(); 
            DbVersion = config["appsettings:dbversion"].ToString(); 
            SapUserName = config["appsettings:sapuserid"].ToString();
            SapPassword = Cipher.Decrypt(config["appsettings:sappassword"].ToString(), SecretKey);
            SapLicenseServer = config["appsettings:saplicense"].ToString();
            Series = Convert.ToInt32(config["appsettings:series"].ToString());
            Branch = Convert.ToInt32(config["appsettings:branch"].ToString());
            BuId = Convert.ToInt32(config["appsettings:buid"].ToString());
            expensecode = Convert.ToInt32(config["appsettings:expensecode"].ToString());
            PostingMethod = config["appsettings:postingmethod"].ToString();
            BranchVT = Convert.ToInt32( config["appsettings:branchvt"]);
            BranchTS = Convert.ToInt32(config["appsettings:branchts"]);
            SeriesVT = Convert.ToInt32(config["appsettings:seriesvt"]);
            SeriesTS = Convert.ToInt32(config["appsettings:seriests"]);
        }
    }
}
