namespace LMS_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ini4 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Activities", "Activity_Id", "dbo.Activities");
            DropIndex("dbo.Activities", new[] { "Activity_Id" });
            DropColumn("dbo.Activities", "Activity_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Activities", "Activity_Id", c => c.Int());
            CreateIndex("dbo.Activities", "Activity_Id");
            AddForeignKey("dbo.Activities", "Activity_Id", "dbo.Activities", "Id");
        }
    }
}
