namespace protean.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using protean.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web.Security;

    internal sealed class Configuration : DbMigrationsConfiguration<protean.Models.MigrationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(protean.Models.MigrationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            var userManager = new UserManager<Models.ApplicationUser>(new UserStore<ApplicationUser>(context));

            // Add Super role
            if (!context.Roles.Any(r => r.Name == "super"))
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var role = new IdentityRole { Name = "super" };
                roleManager.Create(role);
            }

            // Add epicor role
            if (!context.Roles.Any(r => r.Name == "epicor"))
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var role = new IdentityRole { Name = "epicor" };
                roleManager.Create(role);
            }

            // Add customer role
            if (!context.Roles.Any(r => r.Name == "customer"))
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var role = new IdentityRole { Name = "customer" };
                roleManager.Create(role);
            }

            // Add user role
            if (!context.Roles.Any(r => r.Name == "user"))
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var role = new IdentityRole { Name = "user" };
                roleManager.Create(role);
            }

            // Add sales rep role
            if (!context.Roles.Any(r => r.Name == "salesrep"))
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var role = new IdentityRole { Name = "salesrep" };
                roleManager.Create(role);
            }

            // Add rep group principal role
            if (!context.Roles.Any(r => r.Name == "repgroupprincipal"))
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var role = new IdentityRole { Name = "repgroupprincipal" };
                roleManager.Create(role);
            }

            // Add rep group administration role
            if (!context.Roles.Any(r => r.Name == "repgroupadmin"))
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var role = new IdentityRole { Name = "repgroupadmin" };
                roleManager.Create(role);
            }

            // Add grc employee role
            if (!context.Roles.Any(r => r.Name == "grc"))
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var role = new IdentityRole { Name = "grc" };
                roleManager.Create(role);
            }

            // Add grc sales manager role
            if (!context.Roles.Any(r => r.Name == "grcsalesmanager"))
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var role = new IdentityRole { Name = "grcsalesmanager" };
                roleManager.Create(role);
            }

            // Add grc executive role
            if (!context.Roles.Any(r => r.Name == "grcexecutive"))
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var role = new IdentityRole { Name = "grcexecutive" };
                roleManager.Create(role);
            }

            // Add epicor manager role
            if (!context.Roles.Any(r => r.Name == "epicormgmt"))
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var role = new IdentityRole { Name = "epicormgmt" };
                roleManager.Create(role);
            }

            // Add super users
            //if (!context.Users.Any(u => u.UserName == "joseph.dewitt@gmail.com"))
            //{
            //    var user = new ApplicationUser { Email = "joseph.dewitt@gmail.com", UserName = "joseph.dewitt@gmail.com", FirstName = "Joe", LastName = "DeWitt", IsEnabled = true, BadgePath = "/Content/media/users/joedewitt.png", Title = "IT Manager" };

            //    userManager.Create(user, "Letmein6029!");  // Make sure to change this ASAP!!!
            //    userManager.AddToRoles(user.Id, new string[] { "super", "user", "epicor", "grc", "epicormgmt" });
            //}
            //Jasmine 24-10-2021 start
            if (!context.Users.Any(u => u.UserName == "jasmine@indexinfotech.com"))
            {
                var user = new ApplicationUser { Email = "jasmine@indexinfotech.com", UserName = "jasmine@indexinfotech.com", FirstName = "Jasmine", LastName = "Mulla", IsEnabled = true, BadgePath = null, Title = "IT Manager", SalesRepCode = "MID" };

                userManager.Create(user, "Epicor@2020");  // Make sure to change this ASAP!!!
                userManager.AddToRoles(user.Id, new string[] { "super", "user", "epicor", "grc", "epicormgmt", "repgroupadmin", "repgroupprincipal", "salesrep" });
            }
            //Jasmine 24-10-2021 end
            /*if (!context.Users.Any(u => u.UserName == "jdewitt@grandrapidschair.com"))
            {
                var user = new ApplicationUser { Email = "jdewitt@grandrapidschair.com", UserName = "jdewitt@grandrapidschair.com", FirstName = "Joe", LastName = "DeWitt", IsEnabled = true, BadgePath = "/Content/media/users/joedewitt.png", Title = "IT Manager", SalesRepCode = "MID" };

                userManager.Create(user, "Letmein1250!");  // Make sure to change this ASAP!!!
                userManager.AddToRoles(user.Id, new string[] { "super", "user", "epicor", "grc", "epicormgmt", "repgroupadmin", "repgroupprincipal", "salesrep" });
            }*/

            //if (!context.Users.Any(u => u.UserName == "slitzau@grandrapidschair.com"))
            //{
            //    var user = new ApplicationUser { Email = "slitzau@grandrapidschair.com", UserName = "slitzau@grandrapidschair.com", FirstName = "Scott", LastName = "Litzau", IsEnabled = true, BadgePath = "", Title = "ERP Administrator" };

            //    userManager.Create(user, "Letmein1250!");  // Make sure to change this ASAP!!!
            //    userManager.AddToRoles(user.Id, new string[] { "super", "user", "epicor", "grc", "epicormgmt" });
            //}

            // Add test users
            var users = new List<ApplicationUser>
            {
                //new ApplicationUser{Email = "bstiller@itnecessity.com", UserName = "bstiller@itnecessity.com", FirstName = "Ben", LastName = "Stiller", IsEnabled = true, BadgePath = "/Content/media/users/300_13.jpg", Title = "Software Architect"},
                //new ApplicationUser{Email = "lmoss@itnecessity.com", UserName = "lmoss@itnecessity.com", FirstName = "Lisa", LastName = "Moss", IsEnabled = true, BadgePath = "/Content/media/users/300_10.jpg", Title = "QA Manager"},
                //new ApplicationUser{Email = "mpears@itnecessity.com", UserName = "mpears@itnecessity.com", FirstName = "Matt", LastName = "Pears", IsEnabled = true, BadgePath = "/Content/media/users/300_9.jpg", Title = "Procurement Manager"},
                //new ApplicationUser{Email = "jjames@itnecessity.com", UserName = "jjames@itnecessity.com", FirstName = "Jordan", LastName = "James", IsEnabled = true, BadgePath = "/Content/media/users/300_12.jpg", Title = "Customer Service Manager"},
                //new ApplicationUser{Email = "pbrown@itnecessity.com", UserName = "pbrown@itnecessity.com", FirstName = "Peter", LastName = "Brown", IsEnabled = true, BadgePath = "/Content/media/users/100_1.jpg", Title = "Customer Service"},
                //new ApplicationUser{Email = "jmuller@itnecessity.com", UserName = "jmuller@itnecessity.com", FirstName = "Jason", LastName = "Muller", IsEnabled = true, BadgePath = "/Content/media/users/100_5.jpg", Title = "Marketing Manager"},
                //new ApplicationUser{Email = "dhiggins@itnecessity.com", UserName = "dhiggins@itnecessity.com", FirstName = "Devin", LastName = "Higgins", IsEnabled = true, BadgePath = "/Content/media/users/300_15.jpg", Title = "VP of Customer Development"},
                //new ApplicationUser{Email = "alarson@itnecessity.com", UserName = "alarson@itnecessity.com", FirstName = "Ana", LastName = "Larson", IsEnabled = true, BadgePath = null, Title = "Order Entry"},
                //new ApplicationUser{Email = "ldavids@itnecessity.com", UserName = "ldavids@itnecessity.com", FirstName = "Luke", LastName = "Davids", IsEnabled = true, BadgePath = "/Content/media/users/300_22.jpg", Title = "Engineering Manager"},
                //new ApplicationUser{Email = "rhughes@itnecessity.com", UserName = "rhughes@itnecessity.com", FirstName = "Roger", LastName = "Hughes", IsEnabled = false, BadgePath = "/Content/media/users/300_24.jpg", Title = "Accounting Manager"},
                //new ApplicationUser{Email = "khugo@itnecessity.com", UserName = "khugo@itnecessity.com", FirstName = "Karl", LastName = "Hugo", IsEnabled = true, BadgePath = null, Title = "Buyer I"},
                //new ApplicationUser{Email = "dfox@itnecessity.com", UserName = "dfox@itnecessity.com", FirstName = "Diane", LastName = "Fox", IsEnabled = false, BadgePath = null, Title = "Human Resources"},
                //new ApplicationUser{Email = "angela@itnecessity.com", UserName = "angela@itnecessity.com", FirstName = "Angela", LastName = "Budd", IsEnabled = false, BadgePath = null, Title = "Sales Principal", SalesRepCode = "ABU"},
                //new ApplicationUser{Email = "amaxey@itnecessity.com", UserName = "amaxey@itnecessity.com", FirstName = "Ally", LastName = "Maxey", IsEnabled = false, BadgePath = null, Title = "Sales Person", SalesRepCode = "MID"},
                //new ApplicationUser{Email = "mdenney@itnecessity.com", UserName = "mdenney@itnecessity.com", FirstName = "Mike", LastName = "Denney", IsEnabled = false, BadgePath = null, Title = "Sales Principal", SalesRepCode = "MID"},
                new ApplicationUser{Email = "jfrey@grandrapidschair.com", UserName = "jfrey@grandrapidschair.com", FirstName = "Jill", LastName = "Frey", IsEnabled = false, BadgePath = null, Title = "VP of Customer Development", SalesRepCode = "MID"},
                new ApplicationUser{Email = "gbremer@grandrapidschair.com", UserName = "gbremer@grandrapidschair.com", FirstName = "Greg", LastName = "Bremer", IsEnabled = false, BadgePath = null, Title = "VP of Finance", SalesRepCode = "MID"},
                new ApplicationUser{Email = "chaase@grandrapidschair.com", UserName = "chaase@grandrapidschair.com", FirstName = "Carolina", LastName = "Haase", IsEnabled = false, BadgePath = null, Title = "Regional Sales Manager", SalesRepCode = "MID"},
                new ApplicationUser{Email = "rvulpetti@grandrapidschair.com", UserName = "rvulpetti@grandrapidschair.com", FirstName = "Rocky", LastName = "Vulpetti", IsEnabled = false, BadgePath = null, Title = "Regional Sales Manager", SalesRepCode = "MID"},
                new ApplicationUser{Email = "sliebertz@grandrapidschair.com", UserName = "sliebertz@grandrapidschair.com", FirstName = "Sally", LastName = "Liebertz", IsEnabled = false, BadgePath = null, Title = "Customer Sales Manager", SalesRepCode = "MID"},

                new ApplicationUser{Email = "jmccluskey@workscapes.com", UserName = "jmccluskey@workscapes.com", FirstName = "Jillian", LastName = "McCluskey", IsEnabled = true, BadgePath = null, Title = "Sales Rep", SalesRepCode = "JML"},
                new ApplicationUser{Email = "Cheryl.Schmidt@bellsouth.net", UserName = "Cheryl.Schmidt@bellsouth.net", FirstName = "Cheryl", LastName = "Schmidt", IsEnabled = true, BadgePath = null, Title = "Sales Rep", SalesRepCode = "CSCH"},
                //new ApplicationUser{Email = "ap@workscapes.com", UserName = "ap@workscapes.com", FirstName = "Attn", LastName = "Workscapes", IsEnabled = true, BadgePath = null, Title = "Sales Rep", SalesRepCode = "WINC"},
                //new ApplicationUser{Email = "csmith@tjng.com", UserName = "csmith@tjng.com", FirstName = "TJNG", LastName = "Partners, Inc", IsEnabled = true, BadgePath = null, Title = "Sales Rep", SalesRepCode = "TJNG"},
                new ApplicationUser{Email = "mshriver@thebiermangroup.com", UserName = "mshriver@thebiermangroup.com", FirstName = "Meghan", LastName = "Shriver", IsEnabled = true, BadgePath = null, Title = "Sales Rep", SalesRepCode = "MSH"},
                new ApplicationUser{Email = "kbierman@thebiermangroup.com", UserName = "kbierman@thebiermangroup.com", FirstName = "Kara", LastName = "Bierman", IsEnabled = true, BadgePath = null, Title = "Sales Rep", SalesRepCode = "KBI"},
                new ApplicationUser{Email = "drfelder@comcast.net", UserName = "drfelder@comcast.net", FirstName = "Dorrie", LastName = "Felder", IsEnabled = true, BadgePath = null, Title = "Sales Rep", SalesRepCode = "DFE"},
                new ApplicationUser{Email = "jsharkey@thebiermangroup.com", UserName = "jsharkey@thebiermangroup.com", FirstName = "Juliana", LastName = "Sharkey", IsEnabled = true, BadgePath = null, Title = "Sales Rep Principal", SalesRepCode = "JSH"},
                new ApplicationUser{Email = "khudson@thebiermangroup.com", UserName = "khudson@thebiermangroup.com", FirstName = "Kevin", LastName = "Hudson", IsEnabled = true, BadgePath = null, Title = "Sales Rep", SalesRepCode = "KHU"},
                new ApplicationUser{Email = "sfredrickson@thebiermangroup.com", UserName = "sfredrickson@thebiermangroup.com", FirstName = "Sharon", LastName = "Fredrickson", IsEnabled = true, BadgePath = null, Title = "Sales Rep", SalesRepCode = "SFR"},
                new ApplicationUser{Email = "teckes@thebiermangroup.com", UserName = "teckes@thebiermangroup.com", FirstName = "Tom", LastName = "Eckes", IsEnabled = true, BadgePath = null, Title = "Sales Rep", SalesRepCode = "TEC"},
                new ApplicationUser{Email = "mcanovas@thebiermangroup.com", UserName = "mcanovas@thebiermangroup.com", FirstName = "Marie", LastName = "Canovas", IsEnabled = true, BadgePath = null, Title = "Sales Rep", SalesRepCode = "MCA"},
                new ApplicationUser{Email = "rbierman@thebiermangroup.com", UserName = "rbierman@thebiermangroup.com", FirstName = "Randy", LastName = "Bierman", IsEnabled = true, BadgePath = null, Title = "Sales Rep", SalesRepCode = "RBI"},
                new ApplicationUser{Email = "jkennedy@tjng.com", UserName = "jkennedy@tjng.com", FirstName = "Jennifer", LastName = "Kennedy", IsEnabled = true, BadgePath = null, Title = "Sales Rep", SalesRepCode = "JKE"},
                new ApplicationUser{Email = "meg@goodlinesdesign.com", UserName = "meg@goodlinesdesign.com", FirstName = "Meg", LastName = "Krause", IsEnabled = true, BadgePath = null, Title = "Sales Rep", SalesRepCode = "MKR"},
                new ApplicationUser{Email = "marty@goodlinesdesign.com", UserName = "marty@goodlinesdesign.com", FirstName = "Marty", LastName = "Peterson", IsEnabled = true, BadgePath = null, Title = "Sales Rep", SalesRepCode = "MPE"},
                new ApplicationUser{Email = "tina@goodlinesdesign.com", UserName = "tina@goodlinesdesign.com", FirstName = "Tina", LastName = "Danforth", IsEnabled = true, BadgePath = null, Title = "Sales Rep", SalesRepCode = "TDA"},
                new ApplicationUser{Email = "kristin@goodlinesdesign.com", UserName = "kristin@goodlinesdesign.com", FirstName = "Kristin", LastName = "Goodman", IsEnabled = true, BadgePath = null, Title = "Sales Rep Principal", SalesRepCode = "KGO"},
                new ApplicationUser{Email = "shawn@kontract.com", UserName = "shawn@kontract.com", FirstName = "Shawn", LastName = "Gargiulo", IsEnabled = true, BadgePath = null, Title = "Sales Rep Principal", SalesRepCode = "SGA"},
                new ApplicationUser{Email = "angela@novafurnishings.com", UserName = "angela@novafurnishings.com", FirstName = "Angela", LastName = "Budd", IsEnabled = true, BadgePath = null, Title = "Sales Rep Principal", SalesRepCode = "ABU"},
                new ApplicationUser{Email = "jasmine@indexinfotech.com.com", UserName = "jasmine@indexinfotech.com", FirstName = "Jasmine", LastName = "Mulla", IsEnabled = true, BadgePath = null, Title = "IT Manager", SalesRepCode = "MID"},
            };

            foreach (var user in users)
            {
                if (!context.Users.Any(u => u.UserName == user.UserName))
                {
                    userManager.Create(user, Membership.GeneratePassword(12, 1));
                    userManager.AddToRole(user.Id, "user");
                }
            }
        }
    }
}
