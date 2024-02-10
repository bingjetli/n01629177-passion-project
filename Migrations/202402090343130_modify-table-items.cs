namespace n01629177_passion_project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modifytableitems : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Items", "ItemBrand", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Items", "ItemName", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Items", "ItemVariant", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.Items", "ItemWeightGrams");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Items", "ItemWeightGrams", c => c.Int(nullable: false));
            AlterColumn("dbo.Items", "ItemVariant", c => c.String());
            AlterColumn("dbo.Items", "ItemName", c => c.String());
            AlterColumn("dbo.Items", "ItemBrand", c => c.String());
        }
    }
}
