namespace n01629177_passion_project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modifycontraintsshoptable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Shops", "ShopName", c => c.String(nullable: false, maxLength: 128));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Shops", "ShopName", c => c.String(maxLength: 128));
        }
    }
}
