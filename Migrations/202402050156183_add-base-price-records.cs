namespace n01629177_passion_project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addbasepricerecords : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BasePriceRecords",
                c => new
                    {
                        BasePriceRecordId = c.Int(nullable: false, identity: true),
                        BasePriceRecordCreationDate = c.DateTime(nullable: false),
                        BasePriceRecordLastAttestationDate = c.DateTime(nullable: false),
                        BasePriceRecordPrice = c.DateTime(nullable: false),
                        ItemId = c.Int(nullable: false),
                        ShopId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BasePriceRecordId)
                .ForeignKey("dbo.Items", t => t.ItemId, cascadeDelete: true)
                .ForeignKey("dbo.Shops", t => t.ShopId, cascadeDelete: true)
                .Index(t => t.ItemId)
                .Index(t => t.ShopId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BasePriceRecords", "ShopId", "dbo.Shops");
            DropForeignKey("dbo.BasePriceRecords", "ItemId", "dbo.Items");
            DropIndex("dbo.BasePriceRecords", new[] { "ShopId" });
            DropIndex("dbo.BasePriceRecords", new[] { "ItemId" });
            DropTable("dbo.BasePriceRecords");
        }
    }
}
