namespace WebApiHash.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Devices",
                c => new
                    {
                        DeviceId = c.Int(nullable: false, identity: true),
                        DeviceUniqueId = c.String(),
                    })
                .PrimaryKey(t => t.DeviceId);
            
            CreateTable(
                "dbo.Hashtags",
                c => new
                    {
                        HashtagId = c.Int(nullable: false, identity: true),
                        HashtagName = c.String(),
                    })
                .PrimaryKey(t => t.HashtagId);
            
            CreateTable(
                "dbo.HashtagDevices",
                c => new
                    {
                        Hashtag_HashtagId = c.Int(nullable: false),
                        Device_DeviceId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Hashtag_HashtagId, t.Device_DeviceId })
                .ForeignKey("dbo.Hashtags", t => t.Hashtag_HashtagId, cascadeDelete: true)
                .ForeignKey("dbo.Devices", t => t.Device_DeviceId, cascadeDelete: true)
                .Index(t => t.Hashtag_HashtagId)
                .Index(t => t.Device_DeviceId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HashtagDevices", "Device_DeviceId", "dbo.Devices");
            DropForeignKey("dbo.HashtagDevices", "Hashtag_HashtagId", "dbo.Hashtags");
            DropIndex("dbo.HashtagDevices", new[] { "Device_DeviceId" });
            DropIndex("dbo.HashtagDevices", new[] { "Hashtag_HashtagId" });
            DropTable("dbo.HashtagDevices");
            DropTable("dbo.Hashtags");
            DropTable("dbo.Devices");
        }
    }
}
