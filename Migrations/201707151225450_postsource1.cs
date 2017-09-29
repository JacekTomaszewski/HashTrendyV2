namespace WebApiHash.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class postsource1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Posts", "Discriminator", c => c.String(nullable: false, maxLength: 128));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Posts", "Discriminator");
        }
    }
}
