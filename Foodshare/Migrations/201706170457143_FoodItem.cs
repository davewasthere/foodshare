namespace Foodshare.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FoodItem : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FoodItems",
                c => new
                    {
                        FoodItemId = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        Location = c.String(),
                        AvailableFrom = c.DateTime(nullable: false),
                        AvailableTo = c.DateTime(nullable: false),
                        SupplierId = c.Int(nullable: false),
                        Contact = c.String(),
                        Phone = c.String(),
                        ImageUrl = c.String(),
                    })
                .PrimaryKey(t => t.FoodItemId)
                .ForeignKey("dbo.Suppliers", t => t.SupplierId, cascadeDelete: true)
                .Index(t => t.SupplierId);
            
            AddColumn("dbo.Suppliers", "Guid", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FoodItems", "SupplierId", "dbo.Suppliers");
            DropIndex("dbo.FoodItems", new[] { "SupplierId" });
            DropColumn("dbo.Suppliers", "Guid");
            DropTable("dbo.FoodItems");
        }
    }
}
