namespace n01629177_passion_project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class itemtableaddfieldspricetypedefaultquantity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Items", "PriceType", c => c.String(nullable: false, maxLength: 16));
            AddColumn("dbo.Items", "DefaultQuantity", c => c.Int(nullable: false));
            AddColumn("dbo.Prices", "Value", c => c.Single(nullable: false));
            DropColumn("dbo.Prices", "UnitPrice");
            DropColumn("dbo.Prices", "Type");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Prices", "Type", c => c.String());
            AddColumn("dbo.Prices", "UnitPrice", c => c.Single(nullable: false));
            DropColumn("dbo.Prices", "Value");
            DropColumn("dbo.Items", "DefaultQuantity");
            DropColumn("dbo.Items", "PriceType");
        }
    }
}
