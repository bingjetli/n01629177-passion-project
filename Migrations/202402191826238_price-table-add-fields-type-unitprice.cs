namespace n01629177_passion_project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pricetableaddfieldstypeunitprice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Prices", "UnitPrice", c => c.Single(nullable: false));
            AddColumn("dbo.Prices", "Type", c => c.String());
            DropColumn("dbo.Prices", "Value");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Prices", "Value", c => c.Single(nullable: false));
            DropColumn("dbo.Prices", "Type");
            DropColumn("dbo.Prices", "UnitPrice");
        }
    }
}
