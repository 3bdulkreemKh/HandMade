namespace HandMade.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrationDbHm2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "OrderDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "OrderDate");
        }
    }
}
