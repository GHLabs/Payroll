﻿using System.Configuration;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Payroll.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base(ConnectionString)
        {

        }

        static string ConnectionString
        {

            get
            {
                string cs = "";
                switch (ConfigurationManager.AppSettings["DatabaseType"])
                {
                    case "MsSql":
                        cs = "ConnectionString.MsSql";
                        break;
                    case "MySql":
                        cs = "ConnectionString.MySql";
                        break;
                }

                return cs;
            }
        }
    }
}