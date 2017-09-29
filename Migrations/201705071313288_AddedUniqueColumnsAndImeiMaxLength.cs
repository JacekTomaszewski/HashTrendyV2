namespace WebApiHash.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedUniqueColumnsAndImeiMaxLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Hashtags", "HashtagName", c => c.String(maxLength: 450));
            CreateIndex("dbo.Hashtags", "HashtagName", unique: true, name: "Hashtah");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Hashtags", "Hashtah");
            AlterColumn("dbo.Hashtags", "HashtagName", c => c.String());
        }
    }
}
