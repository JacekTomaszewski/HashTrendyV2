using System.Data.Entity.Migrations;
using System.Xml.Linq;

namespace WebApiHash.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class abc : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PostHashtags",
                c => new
                    {
                        Post_PostId = c.Int(nullable: false),
                        Hashtag_HashtagId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Post_PostId, t.Hashtag_HashtagId })
                .ForeignKey("dbo.Posts", t => t.Post_PostId, cascadeDelete: true)
                .ForeignKey("dbo.Hashtags", t => t.Hashtag_HashtagId, cascadeDelete: true)
                .Index(t => t.Post_PostId)
                .Index(t => t.Hashtag_HashtagId);
            
            DropColumn("dbo.Posts", "HashtagName");
            DropColumn("dbo.Posts", "Title");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Posts", "Title", c => c.String());
            AddColumn("dbo.Posts", "HashtagName", c => c.String());
            DropForeignKey("dbo.PostHashtags", "Hashtag_HashtagId", "dbo.Hashtags");
            DropForeignKey("dbo.PostHashtags", "Post_PostId", "dbo.Posts");
            DropIndex("dbo.PostHashtags", new[] { "Hashtag_HashtagId" });
            DropIndex("dbo.PostHashtags", new[] { "Post_PostId" });
            DropTable("dbo.PostHashtags");
        }
    }
}
