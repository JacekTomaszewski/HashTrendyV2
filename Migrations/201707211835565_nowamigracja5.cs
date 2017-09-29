namespace WebApiHash.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nowamigracja5 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Hashtags", "Hashtah");
            AlterColumn("dbo.Hashtags", "HashtagName", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Hashtags", "HashtagName", c => c.String(maxLength: 450));
            CreateIndex("dbo.Hashtags", "HashtagName", unique: true, name: "Hashtah");
        }
    }
}
