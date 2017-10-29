namespace Foodshare.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveIsAgencyProperty : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "IsAgency");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "IsAgency", c => c.Boolean(nullable: false));
        }
    }
}
