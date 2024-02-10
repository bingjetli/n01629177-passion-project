namespace n01629177_passion_project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcontraintsshoptable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Shops", "ShopLatitude", c => c.Single(nullable: false));
            AlterColumn("dbo.Shops", "ShopLongitude", c => c.Single(nullable: false));
            AlterColumn("dbo.Shops", "ShopName", c => c.String(maxLength: 128));
            AlterColumn("dbo.Shops", "ShopAddress", c => c.String(maxLength: 128));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Shops", "ShopAddress", c => c.String());
            AlterColumn("dbo.Shops", "ShopName", c => c.String());
            AlterColumn("dbo.Shops", "ShopLongitude", c => c.Double(nullable: false));
            AlterColumn("dbo.Shops", "ShopLatitude", c => c.Double(nullable: false));
        }
    }
}
