namespace WebApiHash.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nowamigracja2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Posts", "HashtagName", c => c.String());
            DropColumn("dbo.Posts", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Posts", "Name", c => c.String());
            DropColumn("dbo.Posts", "HashtagName");
        }
    }
}
