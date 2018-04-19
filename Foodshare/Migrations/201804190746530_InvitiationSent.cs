namespace Foodshare.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InvitiationSent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "InvitationSent", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "InvitationSent");
        }
    }
}
