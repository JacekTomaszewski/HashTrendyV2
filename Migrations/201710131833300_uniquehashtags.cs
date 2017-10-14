namespace WebApiHash.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class uniquehashtags : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Hashtags", "HashtagName", c => c.String(maxLength: 200));
            CreateIndex("dbo.Hashtags", "HashtagName", unique: true, name: "HashtagName");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Hashtags", "HashtagName");
            AlterColumn("dbo.Hashtags", "HashtagName", c => c.String());
        }
    }
}
