namespace LMS_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class last1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Activities", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Courses", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Courses", "Name", c => c.String());
            AlterColumn("dbo.Activities", "Name", c => c.String());
        }
    }
}
