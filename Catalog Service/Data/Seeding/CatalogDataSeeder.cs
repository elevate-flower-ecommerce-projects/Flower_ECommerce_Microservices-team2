using Catalog_Service.Common.Enums;
using Catalog_Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Catalog_Service.Data.Seeding;

public static class CatalogDataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CatalogServiceDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<CatalogServiceDbContext>>();

        // 1. Database Migration with Retry Logic
        var maxRetries = 10;
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                logger.LogInformation("Attempting to apply CatalogServiceDb migrations (Attempt {Attempt}/{MaxRetries})...", i + 1, maxRetries);
                await context.Database.MigrateAsync();
                logger.LogInformation("CatalogServiceDb database migration completed successfully.");
                break;
            }
            catch (Exception ex)
            {
                if (i == maxRetries - 1)
                {
                    logger.LogError(ex, "Failed to migrate CatalogServiceDb database after {MaxRetries} attempts.", maxRetries);
                    throw;
                }
                logger.LogWarning("CatalogServiceDb database migration failed: {Message}. Retrying in 3 seconds...", ex.Message);
                await Task.Delay(3000);
            }
        }

        // 2. Check if data is already seeded
        try
        {
            if (await context.Categories.AnyAsync() || await context.Products.AnyAsync())
            {
                logger.LogInformation("CatalogServiceDb already contains seed data. Skipping seeding process.");
                return;
            }

            logger.LogInformation("Seeding CatalogServiceDb with initial Categories, Occasions, Products, Images, Occasion Mappings, and Storefront Sections...");

            var now = DateTime.UtcNow;

            // ─── 3. Seed Categories ──────────────────────────────────────────────────
            var categories = new List<Category>
            {
                new Category
                {
                    Id = 1001,
                    Name = "Fresh Bouquets",
                    ImageUrl = "https://images.unsplash.com/photo-1561181286-d3fee7d55364",
                    DisplayOrder = 1,
                    CreatedAt = now
                },
                new Category
                {
                    Id = 1002,
                    Name = "Luxury Flower Arrangements",
                    ImageUrl = "https://images.unsplash.com/photo-1526047932273-341f2a7631f9",
                    DisplayOrder = 2,
                    CreatedAt = now
                },
                new Category
                {
                    Id = 1003,
                    Name = "Indoor & House Plants",
                    ImageUrl = "https://images.unsplash.com/photo-1485955900006-10f4d324d411",
                    DisplayOrder = 3,
                    CreatedAt = now
                },
                new Category
                {
                    Id = 1004,
                    Name = "Velvet & Premium Roses",
                    ImageUrl = "https://images.unsplash.com/photo-1518709268805-4e9042af9f23",
                    DisplayOrder = 4,
                    CreatedAt = now
                },
                new Category
                {
                    Id = 1005,
                    Name = "Orchids & Exotic Blooms",
                    ImageUrl = "https://images.unsplash.com/photo-1525310072745-f49212b5ac6d",
                    DisplayOrder = 5,
                    CreatedAt = now
                },
                new Category
                {
                    Id = 1006,
                    Name = "Dried & Preserved Florals",
                    ImageUrl = "https://images.unsplash.com/photo-1508610048659-a06b669e3321",
                    DisplayOrder = 6,
                    CreatedAt = now
                },
                new Category
                {
                    Id = 1007,
                    Name = "Flower Baskets & Gift Boxes",
                    ImageUrl = "https://images.unsplash.com/photo-1582794543139-8ac9cb0f7b11",
                    DisplayOrder = 7,
                    CreatedAt = now
                },
                new Category
                {
                    Id = 1008,
                    Name = "Floral Accessories & Vases",
                    ImageUrl = "https://images.unsplash.com/photo-1612196808214-b7e239e5f6b7",
                    DisplayOrder = 8,
                    CreatedAt = now
                }
            };
            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();

            // ─── 4. Seed Occasions ───────────────────────────────────────────────────
            var occasions = new List<Occasion>
            {
                new Occasion
                {
                    Id = 2001,
                    Name = "Birthday",
                    ImageUrl = "https://images.unsplash.com/photo-1513151233558-d860c5398176",
                    CreatedAt = now
                },
                new Occasion
                {
                    Id = 2002,
                    Name = "Anniversary & Romance",
                    ImageUrl = "https://images.unsplash.com/photo-1518709268805-4e9042af9f23",
                    CreatedAt = now
                },
                new Occasion
                {
                    Id = 2003,
                    Name = "Wedding & Celebration",
                    ImageUrl = "https://images.unsplash.com/photo-1519741497674-611481863552",
                    CreatedAt = now
                },
                new Occasion
                {
                    Id = 2004,
                    Name = "Graduation",
                    ImageUrl = "https://images.unsplash.com/photo-1523050854058-8df90110c9f1",
                    CreatedAt = now
                },
                new Occasion
                {
                    Id = 2005,
                    Name = "Mother's Day",
                    ImageUrl = "https://images.unsplash.com/photo-1526047932273-341f2a7631f9",
                    CreatedAt = now
                },
                new Occasion
                {
                    Id = 2006,
                    Name = "Valentine's Day",
                    ImageUrl = "https://images.unsplash.com/photo-1518199266791-5375a83190b7",
                    CreatedAt = now
                },
                new Occasion
                {
                    Id = 2007,
                    Name = "Get Well Soon",
                    ImageUrl = "https://images.unsplash.com/photo-1561181286-d3fee7d55364",
                    CreatedAt = now
                },
                new Occasion
                {
                    Id = 2008,
                    Name = "Sympathy & Condolence",
                    ImageUrl = "https://images.unsplash.com/photo-1490750967868-88aa4486c946",
                    CreatedAt = now
                },
                new Occasion
                {
                    Id = 2009,
                    Name = "Congratulations",
                    ImageUrl = "https://images.unsplash.com/photo-1530103862676-de8c9debad1d",
                    CreatedAt = now
                },
                new Occasion
                {
                    Id = 2010,
                    Name = "Thank You",
                    ImageUrl = "https://images.unsplash.com/photo-1469259943454-aa100abb556a",
                    CreatedAt = now
                }
            };
            context.Occasions.AddRange(occasions);
            await context.SaveChangesAsync();

            // ─── 5. Seed Products ────────────────────────────────────────────────────
            var products = new List<Product>
            {
                // Fresh Bouquets (Category 1001)
                new Product
                {
                    Id = 3001,
                    Name = "Royal Red Rose Bouquet",
                    Price = 49.99m,
                    DiscountPercentage = 10.00m,
                    DiscountStartAt = now.AddDays(-5),
                    DiscountEndAt = now.AddDays(30),
                    ProductStatus = ProductStatus.Available,
                    Quantity = 50,
                    Description = "A timeless bouquet of 12 long-stemmed premium red roses wrapped in eco-friendly kraft paper.",
                    IsArchived = false,
                    CategoryId = 1001,
                    CreatedAt = now
                },
                new Product
                {
                    Id = 3002,
                    Name = "Pastel Dawn Mixed Bouquet",
                    Price = 39.99m,
                    DiscountPercentage = null,
                    ProductStatus = ProductStatus.Available,
                    Quantity = 35,
                    Description = "Delicate mix of pink carnations, white lilies, and pastel lisianthus paired with fresh eucalyptus.",
                    IsArchived = false,
                    CategoryId = 1001,
                    CreatedAt = now
                },
                new Product
                {
                    Id = 3003,
                    Name = "Sunshine Sunflower Delight",
                    Price = 34.99m,
                    DiscountPercentage = 5.00m,
                    DiscountStartAt = now.AddDays(-2),
                    DiscountEndAt = now.AddDays(15),
                    ProductStatus = ProductStatus.Available,
                    Quantity = 25,
                    Description = "Vibrant yellow sunflowers surrounded by solidago and fresh greenery to brighten up any day.",
                    IsArchived = false,
                    CategoryId = 1001,
                    CreatedAt = now
                },

                // Luxury Flower Arrangements (Category 1002)
                new Product
                {
                    Id = 3004,
                    Name = "Grand Imperial Lily & Rose Box",
                    Price = 89.99m,
                    DiscountPercentage = 15.00m,
                    DiscountStartAt = now.AddDays(-10),
                    DiscountEndAt = now.AddDays(20),
                    ProductStatus = ProductStatus.Available,
                    Quantity = 15,
                    Description = "Opulent arrangement of Casablanca lilies, garden roses, and hydrangeas presented in a signature hatbox.",
                    IsArchived = false,
                    CategoryId = 1002,
                    CreatedAt = now
                },
                new Product
                {
                    Id = 3005,
                    Name = "Crystal Glass Peony Symphony",
                    Price = 119.99m,
                    DiscountPercentage = null,
                    ProductStatus = ProductStatus.Available,
                    Quantity = 10,
                    Description = "Exquisite lush peonies arranged in a hand-cut crystal cylinder vase. Pure elegance for special celebrations.",
                    IsArchived = false,
                    CategoryId = 1002,
                    CreatedAt = now
                },
                new Product
                {
                    Id = 3006,
                    Name = "Golden Bloom Deluxe Stand",
                    Price = 149.99m,
                    DiscountPercentage = null,
                    ProductStatus = ProductStatus.Available,
                    Quantity = 5,
                    Description = "Stunning floor floral stand featuring white orchids, gold-painted foliage, and cascading ivy.",
                    IsArchived = false,
                    CategoryId = 1002,
                    CreatedAt = now
                },

                // Indoor & House Plants (Category 1003)
                new Product
                {
                    Id = 3007,
                    Name = "Monstera Deliciosa Plant",
                    Price = 29.99m,
                    DiscountPercentage = null,
                    ProductStatus = ProductStatus.Available,
                    Quantity = 40,
                    Description = "Popular Swiss Cheese plant in a white matte ceramic planter. Easy to care for and air-purifying.",
                    IsArchived = false,
                    CategoryId = 1003,
                    CreatedAt = now
                },
                new Product
                {
                    Id = 3008,
                    Name = "Peace Lily in Ceramic Pot",
                    Price = 24.99m,
                    DiscountPercentage = 8.00m,
                    DiscountStartAt = now.AddDays(-1),
                    DiscountEndAt = now.AddDays(14),
                    ProductStatus = ProductStatus.Available,
                    Quantity = 30,
                    Description = "Elegant Peace Lily with glossy dark green leaves and graceful white blooms. Great indoor air purifier.",
                    IsArchived = false,
                    CategoryId = 1003,
                    CreatedAt = now
                },
                new Product
                {
                    Id = 3009,
                    Name = "Fiddle Leaf Fig Tree",
                    Price = 64.99m,
                    DiscountPercentage = null,
                    ProductStatus = ProductStatus.OutOfStock,
                    Quantity = 0,
                    Description = "Dramatic indoor tree with large violin-shaped leaves. Statement piece for modern living rooms.",
                    IsArchived = false,
                    CategoryId = 1003,
                    CreatedAt = now
                },

                // Velvet & Premium Roses (Category 1004)
                new Product
                {
                    Id = 3010,
                    Name = "Forever Crimson Eternity Roses",
                    Price = 79.99m,
                    DiscountPercentage = 20.00m,
                    DiscountStartAt = now.AddDays(-7),
                    DiscountEndAt = now.AddDays(14),
                    ProductStatus = ProductStatus.Available,
                    Quantity = 20,
                    Description = "Real roses preserved to last up to a year without water. Arranged in a luxury black velvet box.",
                    IsArchived = false,
                    CategoryId = 1004,
                    CreatedAt = now
                },
                new Product
                {
                    Id = 3011,
                    Name = "100 Pure White Stem Roses",
                    Price = 199.99m,
                    DiscountPercentage = null,
                    ProductStatus = ProductStatus.Available,
                    Quantity = 8,
                    Description = "A breathtaking display of 100 premium Ecuadorian white roses representing pure love and new beginnings.",
                    IsArchived = false,
                    CategoryId = 1004,
                    CreatedAt = now
                },
                new Product
                {
                    Id = 3012,
                    Name = "Sweet Pink Velvet Rose Box",
                    Price = 59.99m,
                    DiscountPercentage = 12.00m,
                    DiscountStartAt = now.AddDays(-3),
                    DiscountEndAt = now.AddDays(10),
                    ProductStatus = ProductStatus.Available,
                    Quantity = 18,
                    Description = "Soft blush pink roses closely packed in a circular suede velvet container.",
                    IsArchived = false,
                    CategoryId = 1004,
                    CreatedAt = now
                },

                // Orchids & Exotic Blooms (Category 1005)
                new Product
                {
                    Id = 3013,
                    Name = "Phalaenopsis White Orchid",
                    Price = 44.99m,
                    DiscountPercentage = null,
                    ProductStatus = ProductStatus.Available,
                    Quantity = 22,
                    Description = "Double-stemmed snow white Moth Orchid planted in a decorative porcelain vessel.",
                    IsArchived = false,
                    CategoryId = 1005,
                    CreatedAt = now
                },
                new Product
                {
                    Id = 3014,
                    Name = "Purple Majesty Double Stem Orchid",
                    Price = 54.99m,
                    DiscountPercentage = 10.00m,
                    DiscountStartAt = now.AddDays(-4),
                    DiscountEndAt = now.AddDays(25),
                    ProductStatus = ProductStatus.Available,
                    Quantity = 14,
                    Description = "Vibrant magenta and purple phalaenopsis orchid with cascading blooms.",
                    IsArchived = false,
                    CategoryId = 1005,
                    CreatedAt = now
                },
                new Product
                {
                    Id = 3015,
                    Name = "Hawaiian Bird of Paradise Vase",
                    Price = 69.99m,
                    DiscountPercentage = null,
                    ProductStatus = ProductStatus.Available,
                    Quantity = 12,
                    Description = "Exotic arrangement featuring Strelitzia (Bird of Paradise), red ginger spikes, and monstera leaves.",
                    IsArchived = false,
                    CategoryId = 1005,
                    CreatedAt = now
                },

                // Dried & Preserved Florals (Category 1006)
                new Product
                {
                    Id = 3016,
                    Name = "Rustic Pampas & Dried Eucalyptus",
                    Price = 39.99m,
                    DiscountPercentage = null,
                    ProductStatus = ProductStatus.Available,
                    Quantity = 30,
                    Description = "Boho-chic arrangement of fluffy natural pampas grass, preserved eucalyptus, and bunny tails.",
                    IsArchived = false,
                    CategoryId = 1006,
                    CreatedAt = now
                },
                new Product
                {
                    Id = 3017,
                    Name = "Lavender Dreams Preserved Dome",
                    Price = 49.99m,
                    DiscountPercentage = 15.00m,
                    DiscountStartAt = now.AddDays(-6),
                    DiscountEndAt = now.AddDays(20),
                    ProductStatus = ProductStatus.Available,
                    Quantity = 15,
                    Description = "Real French lavender and mini roses encapsulated in a fairy-lit glass cloche dome.",
                    IsArchived = false,
                    CategoryId = 1006,
                    CreatedAt = now
                },
                new Product
                {
                    Id = 3018,
                    Name = "Autumn Harvest Dried Wreath",
                    Price = 55.99m,
                    DiscountPercentage = null,
                    ProductStatus = ProductStatus.Available,
                    Quantity = 10,
                    Description = "Handcrafted door wreath made of dried wheat stalks, orange slices, pinecones, and cinnamon sticks.",
                    IsArchived = false,
                    CategoryId = 1006,
                    CreatedAt = now
                },

                // Flower Baskets & Gift Boxes (Category 1007)
                new Product
                {
                    Id = 3019,
                    Name = "Country Garden Wicker Basket",
                    Price = 54.99m,
                    DiscountPercentage = null,
                    ProductStatus = ProductStatus.Available,
                    Quantity = 20,
                    Description = "Charming woven rattan basket filled with fresh daisies, spray roses, and baby's breath.",
                    IsArchived = false,
                    CategoryId = 1007,
                    CreatedAt = now
                },
                new Product
                {
                    Id = 3020,
                    Name = "Sweet Surprise Flowers & Chocolates Box",
                    Price = 74.99m,
                    DiscountPercentage = 10.00m,
                    DiscountStartAt = now.AddDays(-2),
                    DiscountEndAt = now.AddDays(15),
                    ProductStatus = ProductStatus.Available,
                    Quantity = 16,
                    Description = "Dual-compartment gift box with red roses on top and artisanal pralines tucked beneath.",
                    IsArchived = false,
                    CategoryId = 1007,
                    CreatedAt = now
                },
                new Product
                {
                    Id = 3021,
                    Name = "Celebration Champagne & Blossom Hamper",
                    Price = 129.99m,
                    DiscountPercentage = null,
                    ProductStatus = ProductStatus.Available,
                    Quantity = 7,
                    Description = "Premium gift basket featuring non-alcoholic sparkling cider, gourmet macaroons, and pastel blooms.",
                    IsArchived = false,
                    CategoryId = 1007,
                    CreatedAt = now
                },

                // Floral Accessories & Vases (Category 1008)
                new Product
                {
                    Id = 3022,
                    Name = "Handcrafted Artisanal Ceramic Vase",
                    Price = 19.99m,
                    DiscountPercentage = null,
                    ProductStatus = ProductStatus.Available,
                    Quantity = 50,
                    Description = "Minimalist textured stoneware vase designed to showcase single stems or compact bouquets.",
                    IsArchived = false,
                    CategoryId = 1008,
                    CreatedAt = now
                },
                new Product
                {
                    Id = 3023,
                    Name = "Crystal Diamond Cut Flower Vase",
                    Price = 29.99m,
                    DiscountPercentage = 5.00m,
                    DiscountStartAt = now.AddDays(-1),
                    DiscountEndAt = now.AddDays(30),
                    ProductStatus = ProductStatus.Available,
                    Quantity = 40,
                    Description = "Heavy glass vase with light-reflecting diamond facets. Fits medium to large rose bouquets.",
                    IsArchived = false,
                    CategoryId = 1008,
                    CreatedAt = now
                },
                new Product
                {
                    Id = 3024,
                    Name = "Plant Care & Nutrient Booster Pack",
                    Price = 12.99m,
                    DiscountPercentage = null,
                    ProductStatus = ProductStatus.Available,
                    Quantity = 100,
                    Description = "Organic flower food sachets and leaf shine spray for extending cut flower life.",
                    IsArchived = false,
                    CategoryId = 1008,
                    CreatedAt = now
                }
            };
            context.Products.AddRange(products);
            await context.SaveChangesAsync();

            // ─── 6. Seed Product Images ───────────────────────────────────────────────
            var productImages = new List<ProductImage>();
            long imageIdCounter = 4001;

            var imageMappings = new Dictionary<long, string[]>
            {
                { 3001, new[] { "https://images.unsplash.com/photo-1518709268805-4e9042af9f23", "https://images.unsplash.com/photo-1561181286-d3fee7d55364" } },
                { 3002, new[] { "https://images.unsplash.com/photo-1526047932273-341f2a7631f9", "https://images.unsplash.com/photo-1582794543139-8ac9cb0f7b11" } },
                { 3003, new[] { "https://images.unsplash.com/photo-1597848212624-a19eb35e2651", "https://images.unsplash.com/photo-1561181286-d3fee7d55364" } },
                { 3004, new[] { "https://images.unsplash.com/photo-1526047932273-341f2a7631f9", "https://images.unsplash.com/photo-1582794543139-8ac9cb0f7b11" } },
                { 3005, new[] { "https://images.unsplash.com/photo-1563245372-f21724e3856d", "https://images.unsplash.com/photo-1526047932273-341f2a7631f9" } },
                { 3006, new[] { "https://images.unsplash.com/photo-1519741497674-611481863552", "https://images.unsplash.com/photo-1525310072745-f49212b5ac6d" } },
                { 3007, new[] { "https://images.unsplash.com/photo-1485955900006-10f4d324d411", "https://images.unsplash.com/photo-1614594975525-e45190c55d0b" } },
                { 3008, new[] { "https://images.unsplash.com/photo-1593691509543-c55fb32e7355", "https://images.unsplash.com/photo-1485955900006-10f4d324d411" } },
                { 3009, new[] { "https://images.unsplash.com/photo-1545241047-6083a3684587", "https://images.unsplash.com/photo-1485955900006-10f4d324d411" } },
                { 3010, new[] { "https://images.unsplash.com/photo-1518709268805-4e9042af9f23", "https://images.unsplash.com/photo-1518199266791-5375a83190b7" } },
                { 3011, new[] { "https://images.unsplash.com/photo-1533616688419-b7a585564566", "https://images.unsplash.com/photo-1518709268805-4e9042af9f23" } },
                { 3012, new[] { "https://images.unsplash.com/photo-1562690868-60bbe7293e94", "https://images.unsplash.com/photo-1518709268805-4e9042af9f23" } },
                { 3013, new[] { "https://images.unsplash.com/photo-1525310072745-f49212b5ac6d", "https://images.unsplash.com/photo-1566806764570-5b58c7042571" } },
                { 3014, new[] { "https://images.unsplash.com/photo-1566806764570-5b58c7042571", "https://images.unsplash.com/photo-1525310072745-f49212b5ac6d" } },
                { 3015, new[] { "https://images.unsplash.com/photo-1508610048659-a06b669e3321", "https://images.unsplash.com/photo-1525310072745-f49212b5ac6d" } },
                { 3016, new[] { "https://images.unsplash.com/photo-1508610048659-a06b669e3321", "https://images.unsplash.com/photo-1513151233558-d860c5398176" } },
                { 3017, new[] { "https://images.unsplash.com/photo-1469259943454-aa100abb556a", "https://images.unsplash.com/photo-1508610048659-a06b669e3321" } },
                { 3018, new[] { "https://images.unsplash.com/photo-1509316975850-ff9c5deb0cd9", "https://images.unsplash.com/photo-1508610048659-a06b669e3321" } },
                { 3019, new[] { "https://images.unsplash.com/photo-1582794543139-8ac9cb0f7b11", "https://images.unsplash.com/photo-1561181286-d3fee7d55364" } },
                { 3020, new[] { "https://images.unsplash.com/photo-1549465220-1a8b9238cd48", "https://images.unsplash.com/photo-1582794543139-8ac9cb0f7b11" } },
                { 3021, new[] { "https://images.unsplash.com/photo-1513151233558-d860c5398176", "https://images.unsplash.com/photo-1582794543139-8ac9cb0f7b11" } },
                { 3022, new[] { "https://images.unsplash.com/photo-1612196808214-b7e239e5f6b7", "https://images.unsplash.com/photo-1581783342308-f792dbdd27c5" } },
                { 3023, new[] { "https://images.unsplash.com/photo-1581783342308-f792dbdd27c5", "https://images.unsplash.com/photo-1612196808214-b7e239e5f6b7" } },
                { 3024, new[] { "https://images.unsplash.com/photo-1416879595882-3373a0480b5b", "https://images.unsplash.com/photo-1612196808214-b7e239e5f6b7" } }
            };

            foreach (var kvp in imageMappings)
            {
                foreach (var url in kvp.Value)
                {
                    productImages.Add(new ProductImage
                    {
                        Id = imageIdCounter++,
                        ProductId = kvp.Key,
                        Url = url,
                        CreatedAt = now
                    });
                }
            }
            context.ProductImages.AddRange(productImages);
            await context.SaveChangesAsync();

            // ─── 7. Seed Product Occasion Mappings ───────────────────────────────────
            var productOccasions = new List<ProductOccasion>
            {
                // Royal Red Rose Bouquet -> Birthday, Romance, Valentine's, Mother's Day
                new ProductOccasion { ProductId = 3001, OccasionId = 2001 },
                new ProductOccasion { ProductId = 3001, OccasionId = 2002 },
                new ProductOccasion { ProductId = 3001, OccasionId = 2005 },
                new ProductOccasion { ProductId = 3001, OccasionId = 2006 },

                // Pastel Dawn -> Birthday, Mother's Day, Thank You, Get Well
                new ProductOccasion { ProductId = 3002, OccasionId = 2001 },
                new ProductOccasion { ProductId = 3002, OccasionId = 2005 },
                new ProductOccasion { ProductId = 3002, OccasionId = 2007 },
                new ProductOccasion { ProductId = 3002, OccasionId = 2010 },

                // Sunshine Sunflower -> Graduation, Congratulations, Get Well
                new ProductOccasion { ProductId = 3003, OccasionId = 2004 },
                new ProductOccasion { ProductId = 3003, OccasionId = 2007 },
                new ProductOccasion { ProductId = 3003, OccasionId = 2009 },

                // Grand Imperial Lily -> Wedding, Anniversary, Congratulations
                new ProductOccasion { ProductId = 3004, OccasionId = 2002 },
                new ProductOccasion { ProductId = 3004, OccasionId = 2003 },
                new ProductOccasion { ProductId = 3004, OccasionId = 2009 },

                // Peony Symphony -> Wedding, Romance, Anniversary
                new ProductOccasion { ProductId = 3005, OccasionId = 2002 },
                new ProductOccasion { ProductId = 3005, OccasionId = 2003 },

                // Golden Bloom Stand -> Wedding, Congratulations
                new ProductOccasion { ProductId = 3006, OccasionId = 2003 },
                new ProductOccasion { ProductId = 3006, OccasionId = 2009 },

                // Eternity Roses -> Romance, Valentine's Day, Anniversary
                new ProductOccasion { ProductId = 3010, OccasionId = 2002 },
                new ProductOccasion { ProductId = 3010, OccasionId = 2006 },

                // 100 White Roses -> Wedding, Anniversary, Romance
                new ProductOccasion { ProductId = 3011, OccasionId = 2002 },
                new ProductOccasion { ProductId = 3011, OccasionId = 2003 },

                // Pink Velvet Box -> Birthday, Mother's Day, Romance
                new ProductOccasion { ProductId = 3012, OccasionId = 2001 },
                new ProductOccasion { ProductId = 3012, OccasionId = 2002 },
                new ProductOccasion { ProductId = 3012, OccasionId = 2005 },

                // White Orchid -> Sympathy, Thank You, Congratulations
                new ProductOccasion { ProductId = 3013, OccasionId = 2008 },
                new ProductOccasion { ProductId = 3013, OccasionId = 2009 },
                new ProductOccasion { ProductId = 3013, OccasionId = 2010 },

                // Gift Boxes & Hamps -> Birthday, Congratulations, Thank You
                new ProductOccasion { ProductId = 3020, OccasionId = 2001 },
                new ProductOccasion { ProductId = 3020, OccasionId = 2006 },
                new ProductOccasion { ProductId = 3021, OccasionId = 2003 },
                new ProductOccasion { ProductId = 3021, OccasionId = 2009 }
            };
            context.ProductOccasions.AddRange(productOccasions);
            await context.SaveChangesAsync();

            // ─── 8. Seed Storefront Sections ─────────────────────────────────────────
            var sections = new List<Section>
            {
                new Section
                {
                    Id = 5001,
                    Title = "Spring & Summer Collection",
                    Order = 1,
                    Type = SectionType.Banner,
                    IsEnabled = true,
                    ContentRefJson = "{\"imageUrl\":\"https://images.unsplash.com/photo-1526047932273-341f2a7631f9\",\"deepLink\":\"/category/1001\",\"subtitle\":\"Fresh, hand-picked blooms delivered straight to your door.\"}",
                    CreatedAt = now
                },
                new Section
                {
                    Id = 5002,
                    Title = "Shop by Category",
                    Order = 2,
                    Type = SectionType.CategoryRail,
                    IsEnabled = true,
                    ContentRefJson = "{\"rule\":\"all\",\"limit\":8}",
                    CreatedAt = now
                },
                new Section
                {
                    Id = 5003,
                    Title = "Celebrate Every Moment",
                    Order = 3,
                    Type = SectionType.OccasionRail,
                    IsEnabled = true,
                    ContentRefJson = "{\"rule\":\"all\",\"limit\":10}",
                    CreatedAt = now
                },
                new Section
                {
                    Id = 5004,
                    Title = "Best Selling Florals",
                    Order = 4,
                    Type = SectionType.ProductRail,
                    IsEnabled = true,
                    ContentRefJson = "{\"rule\":\"bestsellers\",\"limit\":6}",
                    CreatedAt = now
                },
                new Section
                {
                    Id = 5005,
                    Title = "Special Offers & Discounts",
                    Order = 5,
                    Type = SectionType.ProductRail,
                    IsEnabled = true,
                    ContentRefJson = "{\"rule\":\"discounted\",\"limit\":6}",
                    CreatedAt = now
                }
            };
            context.Sections.AddRange(sections);
            await context.SaveChangesAsync();

            logger.LogInformation("CatalogServiceDb successfully seeded with {Categories} categories, {Occasions} occasions, {Products} products, {Images} images, {OccasionMappings} occasion links, and {Sections} storefront sections.",
                categories.Count, occasions.Count, products.Count, productImages.Count, productOccasions.Count, sections.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding CatalogServiceDb database.");
        }
    }
}
