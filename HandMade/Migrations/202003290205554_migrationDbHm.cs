namespace HandMade.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrationDbHm : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Accounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Password = c.String(),
                        FullName = c.String(),
                        PhoneNumber = c.String(),
                        Email = c.String(),
                        Role = c.String(),
                        ImagePath = c.String(),
                        Token = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountId = c.Int(nullable: false),
                        City = c.String(),
                        Provice = c.String(),
                        Country = c.String(),
                        AddressDtails = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.AccountId, cascadeDelete: true)
                .Index(t => t.AccountId);
            
            CreateTable(
                "dbo.Answers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountId = c.Int(),
                        QuestionId = c.Int(),
                        Content = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.AccountId)
                .ForeignKey("dbo.Questions", t => t.QuestionId)
                .Index(t => t.AccountId)
                .Index(t => t.QuestionId);
            
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountId = c.Int(nullable: false),
                        ProductId = c.Int(),
                        Content = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.AccountId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId)
                .Index(t => t.AccountId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountId = c.Int(),
                        Name = c.String(),
                        Price = c.Double(nullable: false),
                        DiscountPercentage = c.Int(),
                        SubCategoryId = c.Int(),
                        Discription = c.String(),
                        SmallDisc = c.String(),
                        IsService = c.Boolean(nullable: false),
                        Stock = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.AccountId)
                .ForeignKey("dbo.SubCategories", t => t.SubCategoryId)
                .Index(t => t.AccountId)
                .Index(t => t.SubCategoryId);
            
            CreateTable(
                "dbo.Pictures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(),
                        Path = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Reviews",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountId = c.Int(nullable: false),
                        ProductId = c.Int(),
                        Rating = c.Int(nullable: false),
                        Content = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.AccountId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId)
                .Index(t => t.AccountId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.SubCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CategoryId = c.Int(nullable: false),
                        SubCategoryName = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountId = c.Int(nullable: false),
                        SellerId = c.Int(),
                        Number = c.String(),
                        ProductTotal = c.Double(nullable: false),
                        ServiceTotal = c.Double(nullable: false),
                        VatTotal = c.Double(nullable: false),
                        Total = c.Double(nullable: false),
                        Status = c.String(),
                        City = c.String(),
                        Provice = c.String(),
                        Country = c.String(),
                        AddressDtails = c.String(),
                        ShippingCompany = c.String(),
                        ShippingNumber = c.String(),
                        Account_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.SellerId)
                .ForeignKey("dbo.Accounts", t => t.Account_Id)
                .Index(t => t.SellerId)
                .Index(t => t.Account_Id);
            
            CreateTable(
                "dbo.OrderLines",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Qyt = c.Int(nullable: false),
                        ProductPrice = c.Double(nullable: false),
                        TotalPrice = c.Double(nullable: false),
                        VatTotal = c.Double(nullable: false),
                        Total = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.OrderId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.BannedAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountId = c.Int(nullable: false),
                        Reason = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.AccountId, cascadeDelete: true)
                .Index(t => t.AccountId);
            
            CreateTable(
                "dbo.Chats",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountId = c.Int(),
                        SendToId = c.Int(),
                        Message = c.String(),
                        IsRead = c.Boolean(nullable: false),
                        DateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.AccountId)
                .ForeignKey("dbo.Accounts", t => t.SendToId)
                .Index(t => t.AccountId)
                .Index(t => t.SendToId);
            
            CreateTable(
                "dbo.Cities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CityName = c.String(nullable: false),
                        RegionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Regions", t => t.RegionId, cascadeDelete: true)
                .Index(t => t.RegionId);
            
            CreateTable(
                "dbo.Regions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RegionName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ShippingCosts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountId = c.Int(nullable: false),
                        City = c.String(),
                        IsInPerson = c.Boolean(nullable: false),
                        IsInCityShipping = c.Boolean(nullable: false),
                        InCityCost = c.Double(nullable: false),
                        IsOutCityShipping = c.Boolean(nullable: false),
                        OutCityCost = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.AccountId, cascadeDelete: true)
                .Index(t => t.AccountId);
            
            CreateTable(
                "dbo.ShoppingCarts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Qty = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.AccountId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.AccountId)
                .Index(t => t.ProductId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ShoppingCarts", "ProductId", "dbo.Products");
            DropForeignKey("dbo.ShoppingCarts", "AccountId", "dbo.Accounts");
            DropForeignKey("dbo.ShippingCosts", "AccountId", "dbo.Accounts");
            DropForeignKey("dbo.Cities", "RegionId", "dbo.Regions");
            DropForeignKey("dbo.Chats", "SendToId", "dbo.Accounts");
            DropForeignKey("dbo.Chats", "AccountId", "dbo.Accounts");
            DropForeignKey("dbo.BannedAccounts", "AccountId", "dbo.Accounts");
            DropForeignKey("dbo.Orders", "Account_Id", "dbo.Accounts");
            DropForeignKey("dbo.OrderLines", "ProductId", "dbo.Products");
            DropForeignKey("dbo.OrderLines", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Orders", "SellerId", "dbo.Accounts");
            DropForeignKey("dbo.Products", "SubCategoryId", "dbo.SubCategories");
            DropForeignKey("dbo.SubCategories", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.Reviews", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Reviews", "AccountId", "dbo.Accounts");
            DropForeignKey("dbo.Questions", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Pictures", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Products", "AccountId", "dbo.Accounts");
            DropForeignKey("dbo.Answers", "QuestionId", "dbo.Questions");
            DropForeignKey("dbo.Questions", "AccountId", "dbo.Accounts");
            DropForeignKey("dbo.Answers", "AccountId", "dbo.Accounts");
            DropForeignKey("dbo.Addresses", "AccountId", "dbo.Accounts");
            DropIndex("dbo.ShoppingCarts", new[] { "ProductId" });
            DropIndex("dbo.ShoppingCarts", new[] { "AccountId" });
            DropIndex("dbo.ShippingCosts", new[] { "AccountId" });
            DropIndex("dbo.Cities", new[] { "RegionId" });
            DropIndex("dbo.Chats", new[] { "SendToId" });
            DropIndex("dbo.Chats", new[] { "AccountId" });
            DropIndex("dbo.BannedAccounts", new[] { "AccountId" });
            DropIndex("dbo.OrderLines", new[] { "ProductId" });
            DropIndex("dbo.OrderLines", new[] { "OrderId" });
            DropIndex("dbo.Orders", new[] { "Account_Id" });
            DropIndex("dbo.Orders", new[] { "SellerId" });
            DropIndex("dbo.SubCategories", new[] { "CategoryId" });
            DropIndex("dbo.Reviews", new[] { "ProductId" });
            DropIndex("dbo.Reviews", new[] { "AccountId" });
            DropIndex("dbo.Pictures", new[] { "ProductId" });
            DropIndex("dbo.Products", new[] { "SubCategoryId" });
            DropIndex("dbo.Products", new[] { "AccountId" });
            DropIndex("dbo.Questions", new[] { "ProductId" });
            DropIndex("dbo.Questions", new[] { "AccountId" });
            DropIndex("dbo.Answers", new[] { "QuestionId" });
            DropIndex("dbo.Answers", new[] { "AccountId" });
            DropIndex("dbo.Addresses", new[] { "AccountId" });
            DropTable("dbo.ShoppingCarts");
            DropTable("dbo.ShippingCosts");
            DropTable("dbo.Regions");
            DropTable("dbo.Cities");
            DropTable("dbo.Chats");
            DropTable("dbo.BannedAccounts");
            DropTable("dbo.OrderLines");
            DropTable("dbo.Orders");
            DropTable("dbo.Categories");
            DropTable("dbo.SubCategories");
            DropTable("dbo.Reviews");
            DropTable("dbo.Pictures");
            DropTable("dbo.Products");
            DropTable("dbo.Questions");
            DropTable("dbo.Answers");
            DropTable("dbo.Addresses");
            DropTable("dbo.Accounts");
        }
    }
}
