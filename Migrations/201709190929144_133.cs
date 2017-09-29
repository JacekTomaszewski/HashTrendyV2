namespace WebApiHash.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _133 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Posts", "PostSource", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Posts", "PostSource");
        }
    }
}
