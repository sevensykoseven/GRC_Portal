namespace protean.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TestData : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TestObjects",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        Value = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TestObjects");
        }
    }
}
