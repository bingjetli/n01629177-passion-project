namespace n01629177_passion_project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modifyshopoverpassidtype : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Shops", "ShopOverpassId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Shops", "ShopOverpassId", c => c.Int());
        }
    }
}
