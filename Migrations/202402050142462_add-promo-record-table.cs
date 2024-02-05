namespace n01629177_passion_project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addpromorecordtable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PromoRecords",
                c => new
                    {
                        PromoRecordId = c.Int(nullable: false, identity: true),
                        PromoRecordCreationDate = c.DateTime(nullable: false),
                        PromoRecordLastAttestationDate = c.DateTime(nullable: false),
                        PromoRecordPrice = c.Single(nullable: false),
                        PromoTypeId = c.Int(nullable: false),
                        ItemId = c.Int(nullable: false),
                        ShopId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PromoRecordId)
                .ForeignKey("dbo.Items", t => t.ItemId, cascadeDelete: true)
                .ForeignKey("dbo.PromoTypes", t => t.PromoTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Shops", t => t.ShopId, cascadeDelete: true)
                .Index(t => t.PromoTypeId)
                .Index(t => t.ItemId)
                .Index(t => t.ShopId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PromoRecords", "ShopId", "dbo.Shops");
            DropForeignKey("dbo.PromoRecords", "PromoTypeId", "dbo.PromoTypes");
            DropForeignKey("dbo.PromoRecords", "ItemId", "dbo.Items");
            DropIndex("dbo.PromoRecords", new[] { "ShopId" });
            DropIndex("dbo.PromoRecords", new[] { "ItemId" });
            DropIndex("dbo.PromoRecords", new[] { "PromoTypeId" });
            DropTable("dbo.PromoRecords");
        }
    }
}
