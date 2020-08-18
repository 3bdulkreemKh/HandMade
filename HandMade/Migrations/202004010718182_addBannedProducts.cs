namespace HandMade.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addBannedProducts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BannedProducts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        Reason = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BannedProducts", "ProductId", "dbo.Products");
            DropIndex("dbo.BannedProducts", new[] { "ProductId" });
            DropTable("dbo.BannedProducts");
        }
    }
}
