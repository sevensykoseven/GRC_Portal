namespace protean.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SalesRepCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "SalesRepCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "SalesRepCode");
        }
    }
}
