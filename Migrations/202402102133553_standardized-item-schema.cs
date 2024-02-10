namespace n01629177_passion_project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class standardizeditemschema : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Items", "Brand", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Items", "Name", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Items", "Variant", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.Items", "ItemBrand");
            DropColumn("dbo.Items", "ItemName");
            DropColumn("dbo.Items", "ItemVariant");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Items", "ItemVariant", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Items", "ItemName", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Items", "ItemBrand", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.Items", "Variant");
            DropColumn("dbo.Items", "Name");
            DropColumn("dbo.Items", "Brand");
        }
    }
}
