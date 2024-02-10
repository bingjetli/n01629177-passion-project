namespace n01629177_passion_project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnshoptable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shops", "ShopOverpassId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Shops", "ShopOverpassId");
        }
    }
}
