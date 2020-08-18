namespace HandMade.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTimeToChat : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Chats", "Time", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Chats", "Time");
        }
    }
}
