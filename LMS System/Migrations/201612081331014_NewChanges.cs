namespace LMS_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewChanges : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ActivityDocuments", newName: "DocumentActivities");
            DropForeignKey("dbo.Activities", "Activity_Id", "dbo.Activities");
            DropForeignKey("dbo.Documents", "Module_Id", "dbo.Modules");
            DropIndex("dbo.Documents", new[] { "Module_Id" });
            DropIndex("dbo.Activities", new[] { "Activity_Id" });
            DropPrimaryKey("dbo.DocumentActivities");
            AddColumn("dbo.Activities", "ModuleId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.DocumentActivities", new[] { "Document_Id", "Activity_Id" });
            CreateIndex("dbo.Activities", "ModuleId");
            AddForeignKey("dbo.Activities", "ModuleId", "dbo.Modules", "Id", cascadeDelete: true);
            DropColumn("dbo.Documents", "Module_Id");
            DropColumn("dbo.Activities", "Activity_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Activities", "Activity_Id", c => c.Int());
            AddColumn("dbo.Documents", "Module_Id", c => c.Int());
            DropForeignKey("dbo.Activities", "ModuleId", "dbo.Modules");
            DropIndex("dbo.Activities", new[] { "ModuleId" });
            DropPrimaryKey("dbo.DocumentActivities");
            DropColumn("dbo.Activities", "ModuleId");
            AddPrimaryKey("dbo.DocumentActivities", new[] { "Activity_Id", "Document_Id" });
            CreateIndex("dbo.Activities", "Activity_Id");
            CreateIndex("dbo.Documents", "Module_Id");
            AddForeignKey("dbo.Documents", "Module_Id", "dbo.Modules", "Id");
            AddForeignKey("dbo.Activities", "Activity_Id", "dbo.Activities", "Id");
            RenameTable(name: "dbo.DocumentActivities", newName: "ActivityDocuments");
        }
    }
}
