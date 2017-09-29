namespace WebApiHash.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nowamigracja : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Posts", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Posts", "Name");
        }
    }
}
