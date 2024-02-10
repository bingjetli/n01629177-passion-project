namespace n01629177_passion_project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class itempricerelationship : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BasePriceRecords", "BasePrice", c => c.Single(nullable: false));
            DropColumn("dbo.BasePriceRecords", "BasePriceRecordPrice");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BasePriceRecords", "BasePriceRecordPrice", c => c.DateTime(nullable: false));
            DropColumn("dbo.BasePriceRecords", "BasePrice");
        }
    }
}
