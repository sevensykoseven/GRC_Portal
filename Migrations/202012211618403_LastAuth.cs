namespace protean.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LastAuth : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "LastAuth", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "LastAuth");
        }
    }
}
