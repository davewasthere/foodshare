namespace Foodshare.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsAgency : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "IsAgency", c => c.Boolean(nullable: false, defaultValueSql: "0"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "IsAgency");
        }
    }
}
