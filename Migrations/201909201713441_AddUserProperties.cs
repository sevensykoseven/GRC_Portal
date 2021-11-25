namespace protean.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserProperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "BadgePath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "BadgePath");
        }
    }
}
