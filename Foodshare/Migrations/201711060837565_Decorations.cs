namespace Foodshare.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Decorations : DbMigration
    {
        public override void Up()
        {
            Sql("UPDATE dbo.Donations SET Title = 'Title' WHERE Title IS NULL");
            Sql("UPDATE dbo.Donations SET Description = 'Description' WHERE Description IS NULL");

            AlterColumn("dbo.Donations", "Title", c => c.String(nullable: false));
            AlterColumn("dbo.Donations", "Description", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Donations", "Description", c => c.String());
            AlterColumn("dbo.Donations", "Title", c => c.String());
        }
    }
}
