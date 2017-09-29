namespace WebApiHash.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedmigrations : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Devices", "Imei");
            CreateTable(
                "dbo.Posts",
                c => new
                    {
                        PostId = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Avatar = c.String(),
                        Username = c.String(),
                        Title = c.String(),
                        ContentDescription = c.String(),
                        ContentImageUrl = c.String(),
                    })
                .PrimaryKey(t => t.PostId);
            
            AlterColumn("dbo.Devices", "DeviceUniqueId", c => c.String(maxLength: 20));
            CreateIndex("dbo.Devices", "DeviceUniqueId", unique: true, name: "Imei");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Devices", "Imei");
            AlterColumn("dbo.Devices", "DeviceUniqueId", c => c.String(maxLength: 11));
            DropTable("dbo.Posts");
            CreateIndex("dbo.Devices", "DeviceUniqueId", unique: true, name: "Imei");
        }
    }
}
