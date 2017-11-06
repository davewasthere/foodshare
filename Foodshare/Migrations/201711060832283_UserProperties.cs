namespace Foodshare.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserProperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Address", c => c.String());
            AddColumn("dbo.AspNetUsers", "IsAgency", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "DateCreated", c => c.DateTime(nullable: false, defaultValueSql: "getDate()"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "DateCreated");
            DropColumn("dbo.AspNetUsers", "IsAgency");
            DropColumn("dbo.AspNetUsers", "Address");
        }
    }
}
