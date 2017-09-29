namespace WebApiHash.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class postsource2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Posts", "DirectLinkToStatus", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Posts", "DirectLinkToStatus");
        }
    }
}
