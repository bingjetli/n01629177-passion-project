namespace n01629177_passion_project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class standardizedshopschema : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shops", "OverpassId", c => c.Long(nullable: false));
            AddColumn("dbo.Shops", "Latitude", c => c.Single(nullable: false));
            AddColumn("dbo.Shops", "Longitude", c => c.Single(nullable: false));
            AddColumn("dbo.Shops", "Name", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Shops", "Address", c => c.String(maxLength: 128));
            DropColumn("dbo.Shops", "ShopOverpassId");
            DropColumn("dbo.Shops", "ShopLatitude");
            DropColumn("dbo.Shops", "ShopLongitude");
            DropColumn("dbo.Shops", "ShopName");
            DropColumn("dbo.Shops", "ShopAddress");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Shops", "ShopAddress", c => c.String(maxLength: 128));
            AddColumn("dbo.Shops", "ShopName", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Shops", "ShopLongitude", c => c.Single(nullable: false));
            AddColumn("dbo.Shops", "ShopLatitude", c => c.Single(nullable: false));
            AddColumn("dbo.Shops", "ShopOverpassId", c => c.Long(nullable: false));
            DropColumn("dbo.Shops", "Address");
            DropColumn("dbo.Shops", "Name");
            DropColumn("dbo.Shops", "Longitude");
            DropColumn("dbo.Shops", "Latitude");
            DropColumn("dbo.Shops", "OverpassId");
        }
    }
}
