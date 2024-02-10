namespace n01629177_passion_project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modifyshoptable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shops", "ShopName", c => c.String());
            AddColumn("dbo.Shops", "ShopAddress", c => c.String());
            AlterColumn("dbo.Shops", "ShopLatitude", c => c.Double(nullable: false));
            AlterColumn("dbo.Shops", "ShopLongitude", c => c.Double(nullable: false));
            DropColumn("dbo.Shops", "ShopTitle");
            DropColumn("dbo.Shops", "ShopVariant");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Shops", "ShopVariant", c => c.String());
            AddColumn("dbo.Shops", "ShopTitle", c => c.String());
            AlterColumn("dbo.Shops", "ShopLongitude", c => c.Single(nullable: false));
            AlterColumn("dbo.Shops", "ShopLatitude", c => c.Single(nullable: false));
            DropColumn("dbo.Shops", "ShopAddress");
            DropColumn("dbo.Shops", "ShopName");
        }
    }
}
