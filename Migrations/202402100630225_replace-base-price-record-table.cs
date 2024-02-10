namespace n01629177_passion_project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class replacebasepricerecordtable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BasePriceRecords", "ItemId", "dbo.Items");
            DropForeignKey("dbo.BasePriceRecords", "ShopId", "dbo.Shops");
            DropForeignKey("dbo.ApplicationUserBasePriceRecords", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ApplicationUserBasePriceRecords", "BasePriceRecord_BasePriceRecordId", "dbo.BasePriceRecords");
            DropIndex("dbo.BasePriceRecords", new[] { "ItemId" });
            DropIndex("dbo.BasePriceRecords", new[] { "ShopId" });
            DropIndex("dbo.ApplicationUserBasePriceRecords", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.ApplicationUserBasePriceRecords", new[] { "BasePriceRecord_BasePriceRecordId" });
            CreateTable(
                "dbo.Prices",
                c => new
                    {
                        PriceId = c.Int(nullable: false, identity: true),
                        CreationDate = c.DateTime(nullable: false),
                        LastAttestationDate = c.DateTime(nullable: false),
                        Value = c.Single(nullable: false),
                        ItemId = c.Int(nullable: false),
                        ShopId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PriceId)
                .ForeignKey("dbo.Shops", t => t.ShopId, cascadeDelete: true)
                .ForeignKey("dbo.Items", t => t.ItemId, cascadeDelete: true)
                .Index(t => t.ItemId)
                .Index(t => t.ShopId);
            
            CreateTable(
                "dbo.ApplicationUserPrices",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        Price_PriceId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.Price_PriceId })
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .ForeignKey("dbo.Prices", t => t.Price_PriceId, cascadeDelete: true)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.Price_PriceId);
            
            DropTable("dbo.BasePriceRecords");
            DropTable("dbo.ApplicationUserBasePriceRecords");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ApplicationUserBasePriceRecords",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        BasePriceRecord_BasePriceRecordId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.BasePriceRecord_BasePriceRecordId });
            
            CreateTable(
                "dbo.BasePriceRecords",
                c => new
                    {
                        BasePriceRecordId = c.Int(nullable: false, identity: true),
                        BasePriceRecordCreationDate = c.DateTime(nullable: false),
                        BasePriceRecordLastAttestationDate = c.DateTime(nullable: false),
                        BasePrice = c.Single(nullable: false),
                        ItemId = c.Int(nullable: false),
                        ShopId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BasePriceRecordId);
            
            DropForeignKey("dbo.Prices", "ItemId", "dbo.Items");
            DropForeignKey("dbo.ApplicationUserPrices", "Price_PriceId", "dbo.Prices");
            DropForeignKey("dbo.ApplicationUserPrices", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Prices", "ShopId", "dbo.Shops");
            DropIndex("dbo.ApplicationUserPrices", new[] { "Price_PriceId" });
            DropIndex("dbo.ApplicationUserPrices", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Prices", new[] { "ShopId" });
            DropIndex("dbo.Prices", new[] { "ItemId" });
            DropTable("dbo.ApplicationUserPrices");
            DropTable("dbo.Prices");
            CreateIndex("dbo.ApplicationUserBasePriceRecords", "BasePriceRecord_BasePriceRecordId");
            CreateIndex("dbo.ApplicationUserBasePriceRecords", "ApplicationUser_Id");
            CreateIndex("dbo.BasePriceRecords", "ShopId");
            CreateIndex("dbo.BasePriceRecords", "ItemId");
            AddForeignKey("dbo.ApplicationUserBasePriceRecords", "BasePriceRecord_BasePriceRecordId", "dbo.BasePriceRecords", "BasePriceRecordId", cascadeDelete: true);
            AddForeignKey("dbo.ApplicationUserBasePriceRecords", "ApplicationUser_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.BasePriceRecords", "ShopId", "dbo.Shops", "ShopId", cascadeDelete: true);
            AddForeignKey("dbo.BasePriceRecords", "ItemId", "dbo.Items", "ItemId", cascadeDelete: true);
        }
    }
}
