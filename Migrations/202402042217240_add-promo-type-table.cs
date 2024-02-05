namespace n01629177_passion_project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addpromotypetable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PromoTypes",
                c => new
                    {
                        PromoTypeId = c.Int(nullable: false, identity: true),
                        PromoTypeName = c.String(),
                    })
                .PrimaryKey(t => t.PromoTypeId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PromoTypes");
        }
    }
}
