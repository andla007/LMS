
namespace LMS_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newone : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Activities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        Activity_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Activities", t => t.Activity_Id)
                .Index(t => t.Activity_Id);
            
            CreateTable(
                "dbo.Documents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        Document_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Documents", t => t.Document_Id)
                .Index(t => t.Document_Id);
            
            CreateTable(
                "dbo.DocumentActivities",
                c => new
                    {
                        Document_Id = c.Int(nullable: false),
                        Activity_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Document_Id, t.Activity_Id })
                .ForeignKey("dbo.Documents", t => t.Document_Id, cascadeDelete: true)
                .ForeignKey("dbo.Activities", t => t.Activity_Id, cascadeDelete: true)
                .Index(t => t.Document_Id)
                .Index(t => t.Activity_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Documents", "Document_Id", "dbo.Documents");
            DropForeignKey("dbo.DocumentActivities", "Activity_Id", "dbo.Activities");
            DropForeignKey("dbo.DocumentActivities", "Document_Id", "dbo.Documents");
            DropForeignKey("dbo.Activities", "Activity_Id", "dbo.Activities");
            DropIndex("dbo.DocumentActivities", new[] { "Activity_Id" });
            DropIndex("dbo.DocumentActivities", new[] { "Document_Id" });
            DropIndex("dbo.Documents", new[] { "Document_Id" });
            DropIndex("dbo.Activities", new[] { "Activity_Id" });
            DropTable("dbo.DocumentActivities");
            DropTable("dbo.Documents");
            DropTable("dbo.Activities");
        }
    }
}
