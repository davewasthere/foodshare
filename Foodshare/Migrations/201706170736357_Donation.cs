namespace Foodshare.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Donation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FoodItems", "ClaimedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.FoodItems", "DonatedById", "dbo.AspNetUsers");
            DropIndex("dbo.FoodItems", new[] { "DonatedById" });
            DropIndex("dbo.FoodItems", new[] { "ClaimedById" });
            CreateTable(
                "dbo.Donations",
                c => new
                    {
                        DonationId = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        Location = c.String(),
                        AvailableFrom = c.DateTime(nullable: false),
                        AvailableTo = c.DateTime(nullable: false),
                        DonatedById = c.String(maxLength: 128),
                        Contact = c.String(),
                        Phone = c.String(),
                        ImageUrl = c.String(),
                        ClaimedById = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.DonationId)
                .ForeignKey("dbo.AspNetUsers", t => t.ClaimedById)
                .ForeignKey("dbo.AspNetUsers", t => t.DonatedById)
                .Index(t => t.DonatedById)
                .Index(t => t.ClaimedById);
            
            DropTable("dbo.FoodItems");
        }
        
        public override void Down()
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
                        DonatedById = c.String(maxLength: 128),
                        Contact = c.String(),
                        Phone = c.String(),
                        ImageUrl = c.String(),
                        ClaimedById = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.FoodItemId);
            
            DropForeignKey("dbo.Donations", "DonatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Donations", "ClaimedById", "dbo.AspNetUsers");
            DropIndex("dbo.Donations", new[] { "ClaimedById" });
            DropIndex("dbo.Donations", new[] { "DonatedById" });
            DropTable("dbo.Donations");
            CreateIndex("dbo.FoodItems", "ClaimedById");
            CreateIndex("dbo.FoodItems", "DonatedById");
            AddForeignKey("dbo.FoodItems", "DonatedById", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.FoodItems", "ClaimedById", "dbo.AspNetUsers", "Id");
        }
    }
}
