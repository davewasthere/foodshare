namespace Foodshare.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Deleted : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Donations", "IsDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Donations", "IsDeleted");
        }
    }
}
