namespace n01629177_passion_project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modifyitempricerelationship : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApplicationUserBasePriceRecords",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        BasePriceRecord_BasePriceRecordId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.BasePriceRecord_BasePriceRecordId })
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .ForeignKey("dbo.BasePriceRecords", t => t.BasePriceRecord_BasePriceRecordId, cascadeDelete: true)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.BasePriceRecord_BasePriceRecordId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ApplicationUserBasePriceRecords", "BasePriceRecord_BasePriceRecordId", "dbo.BasePriceRecords");
            DropForeignKey("dbo.ApplicationUserBasePriceRecords", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.ApplicationUserBasePriceRecords", new[] { "BasePriceRecord_BasePriceRecordId" });
            DropIndex("dbo.ApplicationUserBasePriceRecords", new[] { "ApplicationUser_Id" });
            DropTable("dbo.ApplicationUserBasePriceRecords");
        }
    }
}
