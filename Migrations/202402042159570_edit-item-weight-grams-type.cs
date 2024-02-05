namespace n01629177_passion_project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edititemweightgramstype : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Items", "ItemWeightGrams", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Items", "ItemWeightGrams");
        }
    }
}
