using Catalog_Service.Common.Enums;
using Catalog_Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Catalog_Service.Data.Seeding;

public static class CatalogDataSeeder
{
    public static async Task<Dictionary<string, int>> SeedAsync(IServiceProvider serviceProvider, bool force = false)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CatalogServiceDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<CatalogServiceDbContext>>();
        var summary = new Dictionary<string, int>();

        // 1. Database Migration with Retry Logic
        var maxRetries = 10;
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                Console.WriteLine($"[CatalogDataSeeder] Attempting database migration (Attempt {i + 1}/{maxRetries})...");
                logger.LogInformation("Attempting to apply CatalogServiceDb migrations (Attempt {Attempt}/{MaxRetries})...", i + 1, maxRetries);
                await context.Database.MigrateAsync();
                Console.WriteLine("[CatalogDataSeeder] Database migration completed successfully.");
                logger.LogInformation("CatalogServiceDb database migration completed successfully.");
                break;
            }
            catch (Exception ex)
            {
                if (i == maxRetries - 1)
                {
                    Console.WriteLine($"[CatalogDataSeeder] Migration retry limit reached. Attempting EnsureCreated: {ex.Message}");
                    logger.LogWarning(ex, "Failed to migrate CatalogServiceDb, trying EnsureCreatedAsync.");
                    try { await context.Database.EnsureCreatedAsync(); } catch { /* ignore */ }
                }
                else
                {
                    Console.WriteLine($"[CatalogDataSeeder] Migration failed: {ex.Message}. Retrying in 2 seconds...");
                    await Task.Delay(2000);
                }
            }
        }

        var now = DateTime.UtcNow;

        // ─── 2. Seed Categories (12 Categories) ──────────────────────────────────
        try
        {
            var categoriesToSeed = new List<Category>
            {
                new Category { Id = 1001, Name = "Fresh Bouquets", ImageUrl = "https://images.unsplash.com/photo-1561181286-d3fee7d55364", DisplayOrder = 1, IsDeleted = false, CreatedAt = now },
                new Category { Id = 1002, Name = "Luxury Flower Arrangements", ImageUrl = "https://images.unsplash.com/photo-1526047932273-341f2a7631f9", DisplayOrder = 2, IsDeleted = false, CreatedAt = now },
                new Category { Id = 1003, Name = "Indoor & House Plants", ImageUrl = "https://images.unsplash.com/photo-1485955900006-10f4d324d411", DisplayOrder = 3, IsDeleted = false, CreatedAt = now },
                new Category { Id = 1004, Name = "Velvet & Premium Roses", ImageUrl = "https://images.unsplash.com/photo-1518709268805-4e9042af9f23", DisplayOrder = 4, IsDeleted = false, CreatedAt = now },
                new Category { Id = 1005, Name = "Orchids & Exotic Blooms", ImageUrl = "https://images.unsplash.com/photo-1525310072745-f49212b5ac6d", DisplayOrder = 5, IsDeleted = false, CreatedAt = now },
                new Category { Id = 1006, Name = "Dried & Preserved Florals", ImageUrl = "https://images.unsplash.com/photo-1508610048659-a06b669e3321", DisplayOrder = 6, IsDeleted = false, CreatedAt = now },
                new Category { Id = 1007, Name = "Flower Baskets & Gift Boxes", ImageUrl = "https://images.unsplash.com/photo-1582794543139-8ac9cb0f7b11", DisplayOrder = 7, IsDeleted = false, CreatedAt = now },
                new Category { Id = 1008, Name = "Floral Accessories & Vases", ImageUrl = "https://images.unsplash.com/photo-1612196808214-b7e239e5f6b7", DisplayOrder = 8, IsDeleted = false, CreatedAt = now },
                new Category { Id = 1009, Name = "Bridal & Wedding Collection", ImageUrl = "https://images.unsplash.com/photo-1519741497674-611481863552", DisplayOrder = 9, IsDeleted = false, CreatedAt = now },
                new Category { Id = 1010, Name = "Single Stems & Bunches", ImageUrl = "https://images.unsplash.com/photo-1533616688419-b7a585564566", DisplayOrder = 10, IsDeleted = false, CreatedAt = now },
                new Category { Id = 1011, Name = "Bonsai & Zen Garden", ImageUrl = "https://images.unsplash.com/photo-1512428813834-c702c7702b78", DisplayOrder = 11, IsDeleted = false, CreatedAt = now },
                new Category { Id = 1012, Name = "Chocolates, Cakes & Add-ons", ImageUrl = "https://images.unsplash.com/photo-1549465220-1a8b9238cd48", DisplayOrder = 12, IsDeleted = false, CreatedAt = now }
            };

            var existingCategories = await context.Categories.IgnoreQueryFilters().ToListAsync();
            var existingIds = existingCategories.Select(c => c.Id).ToHashSet();
            var existingNames = existingCategories.Select(c => c.Name).ToHashSet();

            // Un-delete any existing seeded categories
            foreach (var cat in existingCategories.Where(c => c.IsDeleted))
            {
                cat.IsDeleted = false;
            }

            var newCategories = categoriesToSeed
                .Where(c => !existingIds.Contains(c.Id) && !existingNames.Contains(c.Name))
                .ToList();

            if (newCategories.Any())
            {
                context.Categories.AddRange(newCategories);
            }
            await context.SaveChangesAsync();
            summary["Categories"] = await context.Categories.CountAsync();
            Console.WriteLine($"[CatalogDataSeeder] Categories in DB: {summary["Categories"]}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CatalogDataSeeder] ERROR seeding categories: {ex.Message}");
            logger.LogError(ex, "Error seeding categories.");
        }

        // ─── 3. Seed Occasions (16 Occasions) ────────────────────────────────────
        try
        {
            var occasionsToSeed = new List<Occasion>
            {
                new Occasion { Id = 2001, Name = "Birthday", ImageUrl = "https://images.unsplash.com/photo-1513151233558-d860c5398176", IsDeleted = false, CreatedAt = now },
                new Occasion { Id = 2002, Name = "Anniversary & Romance", ImageUrl = "https://images.unsplash.com/photo-1518709268805-4e9042af9f23", IsDeleted = false, CreatedAt = now },
                new Occasion { Id = 2003, Name = "Wedding & Celebration", ImageUrl = "https://images.unsplash.com/photo-1519741497674-611481863552", IsDeleted = false, CreatedAt = now },
                new Occasion { Id = 2004, Name = "Graduation", ImageUrl = "https://images.unsplash.com/photo-1523050854058-8df90110c9f1", IsDeleted = false, CreatedAt = now },
                new Occasion { Id = 2005, Name = "Mother's Day", ImageUrl = "https://images.unsplash.com/photo-1526047932273-341f2a7631f9", IsDeleted = false, CreatedAt = now },
                new Occasion { Id = 2006, Name = "Valentine's Day", ImageUrl = "https://images.unsplash.com/photo-1518199266791-5375a83190b7", IsDeleted = false, CreatedAt = now },
                new Occasion { Id = 2007, Name = "Get Well Soon", ImageUrl = "https://images.unsplash.com/photo-1561181286-d3fee7d55364", IsDeleted = false, CreatedAt = now },
                new Occasion { Id = 2008, Name = "Sympathy & Condolence", ImageUrl = "https://images.unsplash.com/photo-1490750967868-88aa4486c946", IsDeleted = false, CreatedAt = now },
                new Occasion { Id = 2009, Name = "Congratulations", ImageUrl = "https://images.unsplash.com/photo-1530103862676-de8c9debad1d", IsDeleted = false, CreatedAt = now },
                new Occasion { Id = 2010, Name = "Thank You", ImageUrl = "https://images.unsplash.com/photo-1469259943454-aa100abb556a", IsDeleted = false, CreatedAt = now },
                new Occasion { Id = 2011, Name = "New Born & Baby Shower", ImageUrl = "https://images.unsplash.com/photo-1519689680058-324335c77eba", IsDeleted = false, CreatedAt = now },
                new Occasion { Id = 2012, Name = "Housewarming", ImageUrl = "https://images.unsplash.com/photo-1513694203232-719a280e022f", IsDeleted = false, CreatedAt = now },
                new Occasion { Id = 2013, Name = "Corporate & Office Events", ImageUrl = "https://images.unsplash.com/photo-1497366216548-37526070297c", IsDeleted = false, CreatedAt = now },
                new Occasion { Id = 2014, Name = "I'm Sorry & Apology", ImageUrl = "https://images.unsplash.com/photo-1508610048659-a06b669e3321", IsDeleted = false, CreatedAt = now },
                new Occasion { Id = 2015, Name = "Eid & Ramadan Mubarak", ImageUrl = "https://images.unsplash.com/photo-1564507592333-c60657eea523", IsDeleted = false, CreatedAt = now },
                new Occasion { Id = 2016, Name = "Festive Holidays & New Year", ImageUrl = "https://images.unsplash.com/photo-1512389142860-9c449e58a543", IsDeleted = false, CreatedAt = now }
            };

            var existingOccasions = await context.Occasions.IgnoreQueryFilters().ToListAsync();
            var existingIds = existingOccasions.Select(o => o.Id).ToHashSet();
            var existingNames = existingOccasions.Select(o => o.Name).ToHashSet();

            foreach (var occ in existingOccasions.Where(o => o.IsDeleted))
            {
                occ.IsDeleted = false;
            }

            var newOccasions = occasionsToSeed
                .Where(o => !existingIds.Contains(o.Id) && !existingNames.Contains(o.Name))
                .ToList();

            if (newOccasions.Any())
            {
                context.Occasions.AddRange(newOccasions);
            }
            await context.SaveChangesAsync();
            summary["Occasions"] = await context.Occasions.CountAsync();
            Console.WriteLine($"[CatalogDataSeeder] Occasions in DB: {summary["Occasions"]}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CatalogDataSeeder] ERROR seeding occasions: {ex.Message}");
            logger.LogError(ex, "Error seeding occasions.");
        }

        // ─── 4. Seed Products (62 Products across all 12 Categories) ────────────
        try
        {
            var productsToSeed = new List<Product>
            {
                // Category 1001: Fresh Bouquets
                new Product { Id = 3001, Name = "Royal Red Rose Bouquet", Price = 49.99m, DiscountPercentage = 10.00m, DiscountStartAt = now.AddDays(-5), DiscountEndAt = now.AddDays(30), ProductStatus = ProductStatus.Available, Quantity = 50, Description = "A timeless bouquet of 12 long-stemmed premium red roses wrapped in eco-friendly kraft paper.", IsArchived = false, IsDeleted = false, CategoryId = 1001, CreatedAt = now },
                new Product { Id = 3002, Name = "Pastel Dawn Mixed Bouquet", Price = 39.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 35, Description = "Delicate mix of pink carnations, white lilies, and pastel lisianthus paired with fresh eucalyptus.", IsArchived = false, IsDeleted = false, CategoryId = 1001, CreatedAt = now },
                new Product { Id = 3003, Name = "Sunshine Sunflower Delight", Price = 34.99m, DiscountPercentage = 5.00m, DiscountStartAt = now.AddDays(-2), DiscountEndAt = now.AddDays(15), ProductStatus = ProductStatus.Available, Quantity = 25, Description = "Vibrant yellow sunflowers surrounded by solidago and fresh greenery to brighten up any day.", IsArchived = false, IsDeleted = false, CategoryId = 1001, CreatedAt = now },
                new Product { Id = 3025, Name = "Wild Meadow Field Bouquet", Price = 44.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 28, Description = "Lush wildflower-style bouquet featuring blue delphinium, chamomile daisies, and fragrant lavender.", IsArchived = false, IsDeleted = false, CategoryId = 1001, CreatedAt = now },
                new Product { Id = 3026, Name = "Parisian Pink Tulip Symphony", Price = 52.99m, DiscountPercentage = 15.00m, DiscountStartAt = now.AddDays(-3), DiscountEndAt = now.AddDays(20), ProductStatus = ProductStatus.Available, Quantity = 30, Description = "Fresh French pink tulips arranged elegantly with baby eucalyptus in a stylish matte wrap.", IsArchived = false, IsDeleted = false, CategoryId = 1001, CreatedAt = now },
                new Product { Id = 3027, Name = "Lavender Breeze Hand-Tied Bouquet", Price = 37.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 40, Description = "Aromatic purple sea lavender, statice, and white spray roses tied with silk lavender ribbon.", IsArchived = false, IsDeleted = false, CategoryId = 1001, CreatedAt = now },

                // Category 1002: Luxury Flower Arrangements
                new Product { Id = 3004, Name = "Grand Imperial Lily & Rose Box", Price = 89.99m, DiscountPercentage = 15.00m, DiscountStartAt = now.AddDays(-10), DiscountEndAt = now.AddDays(20), ProductStatus = ProductStatus.Available, Quantity = 15, Description = "Opulent arrangement of Casablanca lilies, garden roses, and hydrangeas presented in a signature hatbox.", IsArchived = false, IsDeleted = false, CategoryId = 1002, CreatedAt = now },
                new Product { Id = 3005, Name = "Crystal Glass Peony Symphony", Price = 119.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 10, Description = "Exquisite lush peonies arranged in a hand-cut crystal cylinder vase. Pure elegance for special celebrations.", IsArchived = false, IsDeleted = false, CategoryId = 1002, CreatedAt = now },
                new Product { Id = 3006, Name = "Golden Bloom Deluxe Stand", Price = 149.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 5, Description = "Stunning floor floral stand featuring white orchids, gold-painted foliage, and cascading ivy.", IsArchived = false, IsDeleted = false, CategoryId = 1002, CreatedAt = now },
                new Product { Id = 3028, Name = "Midnight Blue & Gold Velvet Hatbox", Price = 139.99m, DiscountPercentage = 10.00m, DiscountStartAt = now.AddDays(-1), DiscountEndAt = now.AddDays(25), ProductStatus = ProductStatus.Available, Quantity = 8, Description = "Deep sapphire blue dyed roses with gold-dusted ruscus in an embossed navy velvet round box.", IsArchived = false, IsDeleted = false, CategoryId = 1002, CreatedAt = now },
                new Product { Id = 3029, Name = "Cascading White Hydrangea Grandeur", Price = 109.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 12, Description = "Massive heads of cloud-like white hydrangeas with white ranunculus and cascading amaranthus.", IsArchived = false, IsDeleted = false, CategoryId = 1002, CreatedAt = now },
                new Product { Id = 3030, Name = "Opulent Imperial Floral Pedestal", Price = 179.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 4, Description = "Spectacular grand centerpiece with king proteas, cymbidium orchids, and premium garden roses.", IsArchived = false, IsDeleted = false, CategoryId = 1002, CreatedAt = now },

                // Category 1003: Indoor & House Plants
                new Product { Id = 3007, Name = "Monstera Deliciosa Plant", Price = 29.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 40, Description = "Popular Swiss Cheese plant in a white matte ceramic planter. Easy to care for and air-purifying.", IsArchived = false, IsDeleted = false, CategoryId = 1003, CreatedAt = now },
                new Product { Id = 3008, Name = "Peace Lily in Ceramic Pot", Price = 24.99m, DiscountPercentage = 8.00m, DiscountStartAt = now.AddDays(-1), DiscountEndAt = now.AddDays(14), ProductStatus = ProductStatus.Available, Quantity = 30, Description = "Elegant Peace Lily with glossy dark green leaves and graceful white blooms. Great indoor air purifier.", IsArchived = false, IsDeleted = false, CategoryId = 1003, CreatedAt = now },
                new Product { Id = 3009, Name = "Fiddle Leaf Fig Tree", Price = 64.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 12, Description = "Dramatic indoor tree with large violin-shaped leaves. Statement piece for modern living rooms.", IsArchived = false, IsDeleted = false, CategoryId = 1003, CreatedAt = now },
                new Product { Id = 3031, Name = "Golden Pothos Hanging Vine", Price = 19.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 45, Description = "Fast-growing trailing vine with heart-shaped variegated leaves in a bohemian macrame hanging planter.", IsArchived = false, IsDeleted = false, CategoryId = 1003, CreatedAt = now },
                new Product { Id = 3032, Name = "Sansevieria Snake Plant Zeylanica", Price = 27.99m, DiscountPercentage = 5.00m, DiscountStartAt = now.AddDays(-4), DiscountEndAt = now.AddDays(20), ProductStatus = ProductStatus.Available, Quantity = 35, Description = "Ultra low-maintenance architectural plant known for superior oxygen release during nighttime.", IsArchived = false, IsDeleted = false, CategoryId = 1003, CreatedAt = now },
                new Product { Id = 3033, Name = "Calathea Medallion Prayer Plant", Price = 32.99m, DiscountPercentage = 10.00m, DiscountStartAt = now.AddDays(-2), DiscountEndAt = now.AddDays(18), ProductStatus = ProductStatus.Available, Quantity = 22, Description = "Striking patterned foliage with deep purple undersides that gracefully fold upwards at dusk.", IsArchived = false, IsDeleted = false, CategoryId = 1003, CreatedAt = now },

                // Category 1004: Velvet & Premium Roses
                new Product { Id = 3010, Name = "Forever Crimson Eternity Roses", Price = 79.99m, DiscountPercentage = 20.00m, DiscountStartAt = now.AddDays(-7), DiscountEndAt = now.AddDays(14), ProductStatus = ProductStatus.Available, Quantity = 20, Description = "Real roses preserved to last up to a year without water. Arranged in a luxury black velvet box.", IsArchived = false, IsDeleted = false, CategoryId = 1004, CreatedAt = now },
                new Product { Id = 3011, Name = "100 Pure White Stem Roses", Price = 199.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 8, Description = "A breathtaking display of 100 premium Ecuadorian white roses representing pure love and new beginnings.", IsArchived = false, IsDeleted = false, CategoryId = 1004, CreatedAt = now },
                new Product { Id = 3012, Name = "Sweet Pink Velvet Rose Box", Price = 59.99m, DiscountPercentage = 12.00m, DiscountStartAt = now.AddDays(-3), DiscountEndAt = now.AddDays(10), ProductStatus = ProductStatus.Available, Quantity = 18, Description = "Soft blush pink roses closely packed in a circular suede velvet container.", IsArchived = false, IsDeleted = false, CategoryId = 1004, CreatedAt = now },
                new Product { Id = 3034, Name = "Black Velvet Infinity Rose Cloche Dome", Price = 69.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 25, Description = "Single giant preserved enchanted rose resting beneath a clear glass dome with warm LED fairy lights.", IsArchived = false, IsDeleted = false, CategoryId = 1004, CreatedAt = now },
                new Product { Id = 3035, Name = "Regal Purple Preserved Roses Box", Price = 84.99m, DiscountPercentage = 15.00m, DiscountStartAt = now.AddDays(-5), DiscountEndAt = now.AddDays(25), ProductStatus = ProductStatus.Available, Quantity = 14, Description = "Deep royal purple preserved eternity roses in a square embossed matte black container.", IsArchived = false, IsDeleted = false, CategoryId = 1004, CreatedAt = now },
                new Product { Id = 3036, Name = "24K Gold Dipped Long-Stem Rose", Price = 49.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 30, Description = "A genuine freshly picked rose dipped in pure 24 karat gold with certificate of authenticity.", IsArchived = false, IsDeleted = false, CategoryId = 1004, CreatedAt = now },

                // Category 1005: Orchids & Exotic Blooms
                new Product { Id = 3013, Name = "Phalaenopsis White Orchid", Price = 44.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 22, Description = "Double-stemmed snow white Moth Orchid planted in a decorative porcelain vessel.", IsArchived = false, IsDeleted = false, CategoryId = 1005, CreatedAt = now },
                new Product { Id = 3014, Name = "Purple Majesty Double Stem Orchid", Price = 54.99m, DiscountPercentage = 10.00m, DiscountStartAt = now.AddDays(-4), DiscountEndAt = now.AddDays(25), ProductStatus = ProductStatus.Available, Quantity = 14, Description = "Vibrant magenta and purple phalaenopsis orchid with cascading blooms.", IsArchived = false, IsDeleted = false, CategoryId = 1005, CreatedAt = now },
                new Product { Id = 3015, Name = "Hawaiian Bird of Paradise Vase", Price = 69.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 12, Description = "Exotic arrangement featuring Strelitzia (Bird of Paradise), red ginger spikes, and monstera leaves.", IsArchived = false, IsDeleted = false, CategoryId = 1005, CreatedAt = now },
                new Product { Id = 3037, Name = "Blue Vanda Orchid Exotic Planter", Price = 79.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 10, Description = "Rare electric blue Vanda orchid displayed in a suspended glass teardrop terrarium.", IsArchived = false, IsDeleted = false, CategoryId = 1005, CreatedAt = now },
                new Product { Id = 3038, Name = "Cymbidium Orchid Cascade Arrangement", Price = 89.99m, DiscountPercentage = 12.00m, DiscountStartAt = now.AddDays(-2), DiscountEndAt = now.AddDays(20), ProductStatus = ProductStatus.Available, Quantity = 9, Description = "Graceful lime green and burgundy Cymbidium orchids styled with bamboo and river pebbles.", IsArchived = false, IsDeleted = false, CategoryId = 1005, CreatedAt = now },
                new Product { Id = 3039, Name = "Tropical Anthurium & Ginger Centerpiece", Price = 58.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 16, Description = "Glossy red anthurium heart flowers paired with pink ginger stems and tropical monstera leaves.", IsArchived = false, IsDeleted = false, CategoryId = 1005, CreatedAt = now },

                // Category 1006: Dried & Preserved Florals
                new Product { Id = 3016, Name = "Rustic Pampas & Dried Eucalyptus", Price = 39.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 30, Description = "Boho-chic arrangement of fluffy natural pampas grass, preserved eucalyptus, and bunny tails.", IsArchived = false, IsDeleted = false, CategoryId = 1006, CreatedAt = now },
                new Product { Id = 3017, Name = "Lavender Dreams Preserved Dome", Price = 49.99m, DiscountPercentage = 15.00m, DiscountStartAt = now.AddDays(-6), DiscountEndAt = now.AddDays(20), ProductStatus = ProductStatus.Available, Quantity = 15, Description = "Real French lavender and mini roses encapsulated in a fairy-lit glass cloche dome.", IsArchived = false, IsDeleted = false, CategoryId = 1006, CreatedAt = now },
                new Product { Id = 3018, Name = "Autumn Harvest Dried Wreath", Price = 55.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 10, Description = "Handcrafted door wreath made of dried wheat stalks, orange slices, pinecones, and cinnamon sticks.", IsArchived = false, IsDeleted = false, CategoryId = 1006, CreatedAt = now },
                new Product { Id = 3040, Name = "Cotton Cloud & Dried Ruscus Cloche", Price = 42.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 20, Description = "Natural raw cotton bolls, bleached ruscus, and star flowers in a wooden base glass display.", IsArchived = false, IsDeleted = false, CategoryId = 1006, CreatedAt = now },
                new Product { Id = 3041, Name = "Terra Cotta Dried Bunny Tail Bunch", Price = 28.99m, DiscountPercentage = 10.00m, DiscountStartAt = now.AddDays(-3), DiscountEndAt = now.AddDays(15), ProductStatus = ProductStatus.Available, Quantity = 35, Description = "Earth-toned terracotta dyed lagurus (bunny tails) tied in a natural rustic bouquet.", IsArchived = false, IsDeleted = false, CategoryId = 1006, CreatedAt = now },
                new Product { Id = 3042, Name = "Preserved Sakura Cherry Blossom Branch", Price = 65.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 14, Description = "Delicate pink preserved cherry blossoms on natural Japanese hardwood branches.", IsArchived = false, IsDeleted = false, CategoryId = 1006, CreatedAt = now },

                // Category 1007: Flower Baskets & Gift Boxes
                new Product { Id = 3019, Name = "Country Garden Wicker Basket", Price = 54.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 20, Description = "Charming woven rattan basket filled with fresh daisies, spray roses, and baby's breath.", IsArchived = false, IsDeleted = false, CategoryId = 1007, CreatedAt = now },
                new Product { Id = 3020, Name = "Sweet Surprise Flowers & Chocolates Box", Price = 74.99m, DiscountPercentage = 10.00m, DiscountStartAt = now.AddDays(-2), DiscountEndAt = now.AddDays(15), ProductStatus = ProductStatus.Available, Quantity = 16, Description = "Dual-compartment gift box with red roses on top and artisanal pralines tucked beneath.", IsArchived = false, IsDeleted = false, CategoryId = 1007, CreatedAt = now },
                new Product { Id = 3021, Name = "Celebration Champagne & Blossom Hamper", Price = 129.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 7, Description = "Premium gift basket featuring sparkling gourmet cider, French macarons, and pastel blooms.", IsArchived = false, IsDeleted = false, CategoryId = 1007, CreatedAt = now },
                new Product { Id = 3043, Name = "Spa & Lavender Self-Care Gift Basket", Price = 89.99m, DiscountPercentage = 15.00m, DiscountStartAt = now.AddDays(-4), DiscountEndAt = now.AddDays(25), ProductStatus = ProductStatus.Available, Quantity = 15, Description = "Organic lavender essential oils, bath salts, scented soy candle, and fresh mini purple orchids.", IsArchived = false, IsDeleted = false, CategoryId = 1007, CreatedAt = now },
                new Product { Id = 3044, Name = "Luxury Gourmet Fruit & Flower Crate", Price = 95.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 12, Description = "Handcrafted wooden crate loaded with seasonal exotic fruits, grapes, and cheerful yellow blossoms.", IsArchived = false, IsDeleted = false, CategoryId = 1007, CreatedAt = now },
                new Product { Id = 3045, Name = "Artisanal Coffee & Rose Bloom Set", Price = 68.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 18, Description = "Single-origin specialty roasted coffee beans, double-wall ceramic mug, and a mini rose bouquet.", IsArchived = false, IsDeleted = false, CategoryId = 1007, CreatedAt = now },

                // Category 1008: Floral Accessories & Vases
                new Product { Id = 3022, Name = "Handcrafted Artisanal Ceramic Vase", Price = 19.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 50, Description = "Minimalist textured stoneware vase designed to showcase single stems or compact bouquets.", IsArchived = false, IsDeleted = false, CategoryId = 1008, CreatedAt = now },
                new Product { Id = 3023, Name = "Crystal Diamond Cut Flower Vase", Price = 29.99m, DiscountPercentage = 5.00m, DiscountStartAt = now.AddDays(-1), DiscountEndAt = now.AddDays(30), ProductStatus = ProductStatus.Available, Quantity = 40, Description = "Heavy glass vase with light-reflecting diamond facets. Fits medium to large rose bouquets.", IsArchived = false, IsDeleted = false, CategoryId = 1008, CreatedAt = now },
                new Product { Id = 3024, Name = "Plant Care & Nutrient Booster Pack", Price = 12.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 100, Description = "Organic flower food sachets and leaf shine spray for extending cut flower life.", IsArchived = false, IsDeleted = false, CategoryId = 1008, CreatedAt = now },
                new Product { Id = 3046, Name = "Matte Black Nordic Ribbed Vase", Price = 22.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 35, Description = "Scandinavian-style fluted ceramic vase in modern matte anthracite finish.", IsArchived = false, IsDeleted = false, CategoryId = 1008, CreatedAt = now },
                new Product { Id = 3047, Name = "Brass Geometric Centerpiece Stand", Price = 36.99m, DiscountPercentage = 10.00m, DiscountStartAt = now.AddDays(-3), DiscountEndAt = now.AddDays(15), ProductStatus = ProductStatus.Available, Quantity = 25, Description = "Modern brushed brass cage frame for elevated table flower arrangements and candle displays.", IsArchived = false, IsDeleted = false, CategoryId = 1008, CreatedAt = now },
                new Product { Id = 3048, Name = "Professional Florist Shears & Mister Kit", Price = 18.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 60, Description = "High-carbon Japanese steel pruning shears with matching brass water misting bottle.", IsArchived = false, IsDeleted = false, CategoryId = 1008, CreatedAt = now },

                // Category 1009: Bridal & Wedding Collection
                new Product { Id = 3049, Name = "Graceful Cascading Bridal Bouquet", Price = 119.99m, DiscountPercentage = 10.00m, DiscountStartAt = now.AddDays(-5), DiscountEndAt = now.AddDays(30), ProductStatus = ProductStatus.Available, Quantity = 10, Description = "Breathtaking waterfall bouquet of David Austin garden roses, calla lilies, and silver dollar eucalyptus.", IsArchived = false, IsDeleted = false, CategoryId = 1009, CreatedAt = now },
                new Product { Id = 3050, Name = "Groom & Groomsmen Boutonniere Set (4 pcs)", Price = 34.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 25, Description = "Matching white miniature rose boutonnieres with olive leaves and magnetic lapel fasteners.", IsArchived = false, IsDeleted = false, CategoryId = 1009, CreatedAt = now },
                new Product { Id = 3051, Name = "Pure White Wedding Floral Garland (2m)", Price = 179.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 6, Description = "2-meter lush runner crafted with hydrangeas, white spray roses, and Italian ruscus for arches & tables.", IsArchived = false, IsDeleted = false, CategoryId = 1009, CreatedAt = now },
                new Product { Id = 3052, Name = "Blush Peony Bridesmaid Posy", Price = 59.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 15, Description = "Delicate round handheld bouquet of blush Sarah Bernhardt peonies and dusty miller greens.", IsArchived = false, IsDeleted = false, CategoryId = 1009, CreatedAt = now },

                // Category 1010: Single Stems & Bunches
                new Product { Id = 3053, Name = "Fresh Red Explorer Rose Stem", Price = 4.50m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 200, Description = "Single premium long-stem Ecuadorian Explorer red rose with large bloom head.", IsArchived = false, IsDeleted = false, CategoryId = 1010, CreatedAt = now },
                new Product { Id = 3054, Name = "Stem of Pink Stargazer Oriental Lily", Price = 5.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 120, Description = "Fragrant multi-bloom pink oriental lily stem with deep crimson speckles.", IsArchived = false, IsDeleted = false, CategoryId = 1010, CreatedAt = now },
                new Product { Id = 3055, Name = "Single Stem Premium Blue Hydrangea", Price = 7.50m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 80, Description = "Jumbo Dutch blue hydrangea stem with vibrant sky blue florets.", IsArchived = false, IsDeleted = false, CategoryId = 1010, CreatedAt = now },

                // Category 1011: Bonsai & Zen Garden
                new Product { Id = 3056, Name = "Japanese Juniper Bonsai in Glazed Pot", Price = 59.99m, DiscountPercentage = 10.00m, DiscountStartAt = now.AddDays(-2), DiscountEndAt = now.AddDays(20), ProductStatus = ProductStatus.Available, Quantity = 14, Description = "A 5-year-old meticulously pruned Juniper Procumbens in a traditional blue ceramic bonsai tray.", IsArchived = false, IsDeleted = false, CategoryId = 1011, CreatedAt = now },
                new Product { Id = 3057, Name = "Ficus Ginseng Microcarpa Bonsai", Price = 45.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 18, Description = "Distinctive exposed root indoor bonsai tree symbolizing longevity, vitality, and peace.", IsArchived = false, IsDeleted = false, CategoryId = 1011, CreatedAt = now },
                new Product { Id = 3058, Name = "Deluxe Desktop Zen Rock & Sand Garden", Price = 34.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 25, Description = "Meditative zen garden with fine quartz sand, bamboo rake, river stones, and mini air plant.", IsArchived = false, IsDeleted = false, CategoryId = 1011, CreatedAt = now },

                // Category 1012: Chocolates, Cakes & Add-ons
                new Product { Id = 3059, Name = "Belgian Artisan Praline Gift Box (16 pcs)", Price = 24.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 50, Description = "Assorted handcrafted dark, milk, and white Belgian chocolates filled with ganache and hazelnut praline.", IsArchived = false, IsDeleted = false, CategoryId = 1012, CreatedAt = now },
                new Product { Id = 3060, Name = "Red Velvet Mini Celebration Bento Cake", Price = 28.99m, DiscountPercentage = 10.00m, DiscountStartAt = now.AddDays(-1), DiscountEndAt = now.AddDays(10), ProductStatus = ProductStatus.Available, Quantity = 20, Description = "Delicious 4-inch red velvet sponge cake layered with Madagascar vanilla cream cheese frosting.", IsArchived = false, IsDeleted = false, CategoryId = 1012, CreatedAt = now },
                new Product { Id = 3061, Name = "Luxury Handwritten Greeting Card & Wax Seal", Price = 5.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 200, Description = "Heavyweight textured cotton card stock customized with your heartfelt message and gold seal.", IsArchived = false, IsDeleted = false, CategoryId = 1012, CreatedAt = now },
                new Product { Id = 3062, Name = "Fluffy White Teddy Bear with Silk Bow (30cm)", Price = 16.99m, DiscountPercentage = null, ProductStatus = ProductStatus.Available, Quantity = 40, Description = "Ultra-soft premium plush teddy bear holding a miniature red plush heart.", IsArchived = false, IsDeleted = false, CategoryId = 1012, CreatedAt = now }
            };

            var existingProducts = await context.Products.IgnoreQueryFilters().ToListAsync();
            var existingIds = existingProducts.Select(p => p.Id).ToHashSet();

            foreach (var prod in existingProducts.Where(p => p.IsDeleted))
            {
                prod.IsDeleted = false;
            }

            var newProducts = productsToSeed
                .Where(p => !existingIds.Contains(p.Id))
                .ToList();

            if (newProducts.Any())
            {
                context.Products.AddRange(newProducts);
            }
            await context.SaveChangesAsync();
            summary["Products"] = await context.Products.CountAsync();
            Console.WriteLine($"[CatalogDataSeeder] Products in DB: {summary["Products"]}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CatalogDataSeeder] ERROR seeding products: {ex.Message}");
            logger.LogError(ex, "Error seeding products.");
        }

        // ─── 5. Seed Product Images ──────────────────────────────────────────────
        try
        {
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
                { 3024, new[] { "https://images.unsplash.com/photo-1416879595882-3373a0480b5b", "https://images.unsplash.com/photo-1612196808214-b7e239e5f6b7" } },
                { 3025, new[] { "https://images.unsplash.com/photo-1508610048659-a06b669e3321", "https://images.unsplash.com/photo-1561181286-d3fee7d55364" } },
                { 3026, new[] { "https://images.unsplash.com/photo-1520763185298-1b434c919102", "https://images.unsplash.com/photo-1561181286-d3fee7d55364" } },
                { 3027, new[] { "https://images.unsplash.com/photo-1528183429752-a97d0bf99b5a", "https://images.unsplash.com/photo-1561181286-d3fee7d55364" } },
                { 3028, new[] { "https://images.unsplash.com/photo-1518709268805-4e9042af9f23", "https://images.unsplash.com/photo-1526047932273-341f2a7631f9" } },
                { 3029, new[] { "https://images.unsplash.com/photo-1563245372-f21724e3856d", "https://images.unsplash.com/photo-1526047932273-341f2a7631f9" } },
                { 3030, new[] { "https://images.unsplash.com/photo-1519741497674-611481863552", "https://images.unsplash.com/photo-1563245372-f21724e3856d" } },
                { 3031, new[] { "https://images.unsplash.com/photo-1512428813834-c702c7702b78", "https://images.unsplash.com/photo-1485955900006-10f4d324d411" } },
                { 3032, new[] { "https://images.unsplash.com/photo-1509423350716-97f9360b4e09", "https://images.unsplash.com/photo-1485955900006-10f4d324d411" } },
                { 3033, new[] { "https://images.unsplash.com/photo-1614594975525-e45190c55d0b", "https://images.unsplash.com/photo-1485955900006-10f4d324d411" } },
                { 3034, new[] { "https://images.unsplash.com/photo-1518199266791-5375a83190b7", "https://images.unsplash.com/photo-1518709268805-4e9042af9f23" } },
                { 3035, new[] { "https://images.unsplash.com/photo-1562690868-60bbe7293e94", "https://images.unsplash.com/photo-1518709268805-4e9042af9f23" } },
                { 3036, new[] { "https://images.unsplash.com/photo-1518895949257-7621c3c786d7", "https://images.unsplash.com/photo-1518709268805-4e9042af9f23" } },
                { 3037, new[] { "https://images.unsplash.com/photo-1566806764570-5b58c7042571", "https://images.unsplash.com/photo-1525310072745-f49212b5ac6d" } },
                { 3038, new[] { "https://images.unsplash.com/photo-1525310072745-f49212b5ac6d", "https://images.unsplash.com/photo-1566806764570-5b58c7042571" } },
                { 3039, new[] { "https://images.unsplash.com/photo-1533616688419-b7a585564566", "https://images.unsplash.com/photo-1525310072745-f49212b5ac6d" } },
                { 3040, new[] { "https://images.unsplash.com/photo-1508610048659-a06b669e3321", "https://images.unsplash.com/photo-1509316975850-ff9c5deb0cd9" } },
                { 3041, new[] { "https://images.unsplash.com/photo-1513151233558-d860c5398176", "https://images.unsplash.com/photo-1508610048659-a06b669e3321" } },
                { 3042, new[] { "https://images.unsplash.com/photo-1522383225653-ed111181a951", "https://images.unsplash.com/photo-1508610048659-a06b669e3321" } },
                { 3043, new[] { "https://images.unsplash.com/photo-1540555700478-4be289fbecef", "https://images.unsplash.com/photo-1582794543139-8ac9cb0f7b11" } },
                { 3044, new[] { "https://images.unsplash.com/photo-1610832958506-aa56368176cf", "https://images.unsplash.com/photo-1582794543139-8ac9cb0f7b11" } },
                { 3045, new[] { "https://images.unsplash.com/photo-1514432324607-a09d9b4aefdd", "https://images.unsplash.com/photo-1582794543139-8ac9cb0f7b11" } },
                { 3046, new[] { "https://images.unsplash.com/photo-1578749556568-bc2c40e68b61", "https://images.unsplash.com/photo-1612196808214-b7e239e5f6b7" } },
                { 3047, new[] { "https://images.unsplash.com/photo-1581783342308-f792dbdd27c5", "https://images.unsplash.com/photo-1612196808214-b7e239e5f6b7" } },
                { 3048, new[] { "https://images.unsplash.com/photo-1416879595882-3373a0480b5b", "https://images.unsplash.com/photo-1612196808214-b7e239e5f6b7" } },
                { 3049, new[] { "https://images.unsplash.com/photo-1519741497674-611481863552", "https://images.unsplash.com/photo-1563245372-f21724e3856d" } },
                { 3050, new[] { "https://images.unsplash.com/photo-1511285560929-80b456fea0bc", "https://images.unsplash.com/photo-1519741497674-611481863552" } },
                { 3051, new[] { "https://images.unsplash.com/photo-1519225421980-715cb0215aed", "https://images.unsplash.com/photo-1519741497674-611481863552" } },
                { 3052, new[] { "https://images.unsplash.com/photo-1563245372-f21724e3856d", "https://images.unsplash.com/photo-1519741497674-611481863552" } },
                { 3053, new[] { "https://images.unsplash.com/photo-1518709268805-4e9042af9f23", "https://images.unsplash.com/photo-1533616688419-b7a585564566" } },
                { 3054, new[] { "https://images.unsplash.com/photo-1526047932273-341f2a7631f9", "https://images.unsplash.com/photo-1533616688419-b7a585564566" } },
                { 3055, new[] { "https://images.unsplash.com/photo-1563245372-f21724e3856d", "https://images.unsplash.com/photo-1533616688419-b7a585564566" } },
                { 3056, new[] { "https://images.unsplash.com/photo-1512428813834-c702c7702b78", "https://images.unsplash.com/photo-1485955900006-10f4d324d411" } },
                { 3057, new[] { "https://images.unsplash.com/photo-1545241047-6083a3684587", "https://images.unsplash.com/photo-1512428813834-c702c7702b78" } },
                { 3058, new[] { "https://images.unsplash.com/photo-1509423350716-97f9360b4e09", "https://images.unsplash.com/photo-1512428813834-c702c7702b78" } },
                { 3059, new[] { "https://images.unsplash.com/photo-1549465220-1a8b9238cd48", "https://images.unsplash.com/photo-1582794543139-8ac9cb0f7b11" } },
                { 3060, new[] { "https://images.unsplash.com/photo-1578985545062-69928b1d9587", "https://images.unsplash.com/photo-1549465220-1a8b9238cd48" } },
                { 3061, new[] { "https://images.unsplash.com/photo-1513151233558-d860c5398176", "https://images.unsplash.com/photo-1582794543139-8ac9cb0f7b11" } },
                { 3062, new[] { "https://images.unsplash.com/photo-1559454403-b8fb88521f11", "https://images.unsplash.com/photo-1582794543139-8ac9cb0f7b11" } }
            };

            var existingImages = await context.ProductImages.IgnoreQueryFilters().ToListAsync();
            var existingIds = existingImages.Select(pi => pi.Id).ToHashSet();

            foreach (var img in existingImages.Where(i => i.IsDeleted))
            {
                img.IsDeleted = false;
            }

            var productImagesToSeed = new List<ProductImage>();
            long imageIdCounter = 4001;

            foreach (var kvp in imageMappings)
            {
                foreach (var url in kvp.Value)
                {
                    var imgId = imageIdCounter++;
                    if (!existingIds.Contains(imgId))
                    {
                        productImagesToSeed.Add(new ProductImage
                        {
                            Id = imgId,
                            ProductId = kvp.Key,
                            Url = url,
                            IsDeleted = false,
                            CreatedAt = now
                        });
                    }
                }
            }

            if (productImagesToSeed.Any())
            {
                context.ProductImages.AddRange(productImagesToSeed);
            }
            await context.SaveChangesAsync();
            summary["ProductImages"] = await context.ProductImages.CountAsync();
            Console.WriteLine($"[CatalogDataSeeder] ProductImages in DB: {summary["ProductImages"]}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CatalogDataSeeder] ERROR seeding product images: {ex.Message}");
            logger.LogError(ex, "Error seeding product images.");
        }

        // ─── 6. Seed Product Occasion Mappings ───────────────────────────────────
        try
        {
            var productOccasionsToSeed = new List<ProductOccasion>
            {
                // Fresh Bouquets
                new ProductOccasion { ProductId = 3001, OccasionId = 2001 },
                new ProductOccasion { ProductId = 3001, OccasionId = 2002 },
                new ProductOccasion { ProductId = 3001, OccasionId = 2005 },
                new ProductOccasion { ProductId = 3001, OccasionId = 2006 },
                new ProductOccasion { ProductId = 3001, OccasionId = 2014 },
                new ProductOccasion { ProductId = 3002, OccasionId = 2001 },
                new ProductOccasion { ProductId = 3002, OccasionId = 2005 },
                new ProductOccasion { ProductId = 3002, OccasionId = 2007 },
                new ProductOccasion { ProductId = 3002, OccasionId = 2010 },
                new ProductOccasion { ProductId = 3002, OccasionId = 2011 },
                new ProductOccasion { ProductId = 3003, OccasionId = 2001 },
                new ProductOccasion { ProductId = 3003, OccasionId = 2004 },
                new ProductOccasion { ProductId = 3003, OccasionId = 2007 },
                new ProductOccasion { ProductId = 3003, OccasionId = 2009 },
                new ProductOccasion { ProductId = 3003, OccasionId = 2010 },
                new ProductOccasion { ProductId = 3025, OccasionId = 2001 },
                new ProductOccasion { ProductId = 3025, OccasionId = 2007 },
                new ProductOccasion { ProductId = 3025, OccasionId = 2010 },
                new ProductOccasion { ProductId = 3026, OccasionId = 2001 },
                new ProductOccasion { ProductId = 3026, OccasionId = 2002 },
                new ProductOccasion { ProductId = 3026, OccasionId = 2005 },
                new ProductOccasion { ProductId = 3026, OccasionId = 2006 },
                new ProductOccasion { ProductId = 3027, OccasionId = 2007 },
                new ProductOccasion { ProductId = 3027, OccasionId = 2008 },
                new ProductOccasion { ProductId = 3027, OccasionId = 2010 },

                // Luxury Flower Arrangements
                new ProductOccasion { ProductId = 3004, OccasionId = 2002 },
                new ProductOccasion { ProductId = 3004, OccasionId = 2003 },
                new ProductOccasion { ProductId = 3004, OccasionId = 2009 },
                new ProductOccasion { ProductId = 3004, OccasionId = 2013 },
                new ProductOccasion { ProductId = 3005, OccasionId = 2002 },
                new ProductOccasion { ProductId = 3005, OccasionId = 2003 },
                new ProductOccasion { ProductId = 3005, OccasionId = 2005 },
                new ProductOccasion { ProductId = 3005, OccasionId = 2006 },
                new ProductOccasion { ProductId = 3006, OccasionId = 2003 },
                new ProductOccasion { ProductId = 3006, OccasionId = 2009 },
                new ProductOccasion { ProductId = 3006, OccasionId = 2013 },
                new ProductOccasion { ProductId = 3028, OccasionId = 2002 },
                new ProductOccasion { ProductId = 3028, OccasionId = 2003 },
                new ProductOccasion { ProductId = 3028, OccasionId = 2006 },
                new ProductOccasion { ProductId = 3029, OccasionId = 2003 },
                new ProductOccasion { ProductId = 3029, OccasionId = 2008 },
                new ProductOccasion { ProductId = 3029, OccasionId = 2009 },
                new ProductOccasion { ProductId = 3030, OccasionId = 2003 },
                new ProductOccasion { ProductId = 3030, OccasionId = 2009 },
                new ProductOccasion { ProductId = 3030, OccasionId = 2013 },

                // Indoor Plants
                new ProductOccasion { ProductId = 3007, OccasionId = 2009 },
                new ProductOccasion { ProductId = 3007, OccasionId = 2010 },
                new ProductOccasion { ProductId = 3007, OccasionId = 2012 },
                new ProductOccasion { ProductId = 3007, OccasionId = 2013 },
                new ProductOccasion { ProductId = 3008, OccasionId = 2007 },
                new ProductOccasion { ProductId = 3008, OccasionId = 2008 },
                new ProductOccasion { ProductId = 3008, OccasionId = 2010 },
                new ProductOccasion { ProductId = 3008, OccasionId = 2012 },
                new ProductOccasion { ProductId = 3009, OccasionId = 2012 },
                new ProductOccasion { ProductId = 3009, OccasionId = 2013 },
                new ProductOccasion { ProductId = 3031, OccasionId = 2012 },
                new ProductOccasion { ProductId = 3031, OccasionId = 2010 },
                new ProductOccasion { ProductId = 3032, OccasionId = 2012 },
                new ProductOccasion { ProductId = 3032, OccasionId = 2013 },
                new ProductOccasion { ProductId = 3033, OccasionId = 2012 },
                new ProductOccasion { ProductId = 3033, OccasionId = 2001 },

                // Eternity Roses
                new ProductOccasion { ProductId = 3010, OccasionId = 2001 },
                new ProductOccasion { ProductId = 3010, OccasionId = 2002 },
                new ProductOccasion { ProductId = 3010, OccasionId = 2006 },
                new ProductOccasion { ProductId = 3011, OccasionId = 2002 },
                new ProductOccasion { ProductId = 3011, OccasionId = 2003 },
                new ProductOccasion { ProductId = 3011, OccasionId = 2006 },
                new ProductOccasion { ProductId = 3012, OccasionId = 2001 },
                new ProductOccasion { ProductId = 3012, OccasionId = 2002 },
                new ProductOccasion { ProductId = 3012, OccasionId = 2005 },
                new ProductOccasion { ProductId = 3012, OccasionId = 2006 },
                new ProductOccasion { ProductId = 3034, OccasionId = 2002 },
                new ProductOccasion { ProductId = 3034, OccasionId = 2006 },
                new ProductOccasion { ProductId = 3035, OccasionId = 2001 },
                new ProductOccasion { ProductId = 3035, OccasionId = 2002 },
                new ProductOccasion { ProductId = 3036, OccasionId = 2002 },
                new ProductOccasion { ProductId = 3036, OccasionId = 2006 },

                // Orchids
                new ProductOccasion { ProductId = 3013, OccasionId = 2008 },
                new ProductOccasion { ProductId = 3013, OccasionId = 2009 },
                new ProductOccasion { ProductId = 3013, OccasionId = 2010 },
                new ProductOccasion { ProductId = 3013, OccasionId = 2013 },
                new ProductOccasion { ProductId = 3014, OccasionId = 2001 },
                new ProductOccasion { ProductId = 3014, OccasionId = 2005 },
                new ProductOccasion { ProductId = 3014, OccasionId = 2009 },
                new ProductOccasion { ProductId = 3015, OccasionId = 2009 },
                new ProductOccasion { ProductId = 3015, OccasionId = 2012 },
                new ProductOccasion { ProductId = 3015, OccasionId = 2013 },
                new ProductOccasion { ProductId = 3037, OccasionId = 2001 },
                new ProductOccasion { ProductId = 3037, OccasionId = 2009 },
                new ProductOccasion { ProductId = 3038, OccasionId = 2003 },
                new ProductOccasion { ProductId = 3038, OccasionId = 2013 },
                new ProductOccasion { ProductId = 3039, OccasionId = 2009 },
                new ProductOccasion { ProductId = 3039, OccasionId = 2012 },

                // Dried Florals
                new ProductOccasion { ProductId = 3016, OccasionId = 2010 },
                new ProductOccasion { ProductId = 3016, OccasionId = 2012 },
                new ProductOccasion { ProductId = 3017, OccasionId = 2001 },
                new ProductOccasion { ProductId = 3017, OccasionId = 2005 },
                new ProductOccasion { ProductId = 3018, OccasionId = 2012 },
                new ProductOccasion { ProductId = 3018, OccasionId = 2016 },
                new ProductOccasion { ProductId = 3040, OccasionId = 2011 },
                new ProductOccasion { ProductId = 3040, OccasionId = 2012 },
                new ProductOccasion { ProductId = 3041, OccasionId = 2012 },
                new ProductOccasion { ProductId = 3042, OccasionId = 2012 },
                new ProductOccasion { ProductId = 3042, OccasionId = 2013 },

                // Gift Baskets & Hampers
                new ProductOccasion { ProductId = 3019, OccasionId = 2001 },
                new ProductOccasion { ProductId = 3019, OccasionId = 2005 },
                new ProductOccasion { ProductId = 3019, OccasionId = 2007 },
                new ProductOccasion { ProductId = 3019, OccasionId = 2010 },
                new ProductOccasion { ProductId = 3020, OccasionId = 2001 },
                new ProductOccasion { ProductId = 3020, OccasionId = 2002 },
                new ProductOccasion { ProductId = 3020, OccasionId = 2006 },
                new ProductOccasion { ProductId = 3021, OccasionId = 2001 },
                new ProductOccasion { ProductId = 3021, OccasionId = 2003 },
                new ProductOccasion { ProductId = 3021, OccasionId = 2009 },
                new ProductOccasion { ProductId = 3021, OccasionId = 2015 },
                new ProductOccasion { ProductId = 3021, OccasionId = 2016 },
                new ProductOccasion { ProductId = 3043, OccasionId = 2001 },
                new ProductOccasion { ProductId = 3043, OccasionId = 2005 },
                new ProductOccasion { ProductId = 3043, OccasionId = 2007 },
                new ProductOccasion { ProductId = 3044, OccasionId = 2007 },
                new ProductOccasion { ProductId = 3044, OccasionId = 2009 },
                new ProductOccasion { ProductId = 3044, OccasionId = 2015 },
                new ProductOccasion { ProductId = 3045, OccasionId = 2001 },
                new ProductOccasion { ProductId = 3045, OccasionId = 2009 },
                new ProductOccasion { ProductId = 3045, OccasionId = 2010 },

                // Vases & Accessories
                new ProductOccasion { ProductId = 3022, OccasionId = 2012 },
                new ProductOccasion { ProductId = 3023, OccasionId = 2003 },
                new ProductOccasion { ProductId = 3023, OccasionId = 2012 },
                new ProductOccasion { ProductId = 3024, OccasionId = 2010 },
                new ProductOccasion { ProductId = 3046, OccasionId = 2012 },
                new ProductOccasion { ProductId = 3047, OccasionId = 2003 },
                new ProductOccasion { ProductId = 3047, OccasionId = 2012 },
                new ProductOccasion { ProductId = 3048, OccasionId = 2010 },

                // Wedding Collection
                new ProductOccasion { ProductId = 3049, OccasionId = 2003 },
                new ProductOccasion { ProductId = 3050, OccasionId = 2003 },
                new ProductOccasion { ProductId = 3051, OccasionId = 2003 },
                new ProductOccasion { ProductId = 3052, OccasionId = 2003 },

                // Single Stems
                new ProductOccasion { ProductId = 3053, OccasionId = 2002 },
                new ProductOccasion { ProductId = 3053, OccasionId = 2006 },
                new ProductOccasion { ProductId = 3054, OccasionId = 2001 },
                new ProductOccasion { ProductId = 3054, OccasionId = 2007 },
                new ProductOccasion { ProductId = 3055, OccasionId = 2001 },
                new ProductOccasion { ProductId = 3055, OccasionId = 2009 },

                // Bonsai & Zen Garden
                new ProductOccasion { ProductId = 3056, OccasionId = 2009 },
                new ProductOccasion { ProductId = 3056, OccasionId = 2012 },
                new ProductOccasion { ProductId = 3056, OccasionId = 2013 },
                new ProductOccasion { ProductId = 3057, OccasionId = 2010 },
                new ProductOccasion { ProductId = 3057, OccasionId = 2012 },
                new ProductOccasion { ProductId = 3058, OccasionId = 2009 },
                new ProductOccasion { ProductId = 3058, OccasionId = 2013 },

                // Chocolates & Cakes & Add-ons
                new ProductOccasion { ProductId = 3059, OccasionId = 2001 },
                new ProductOccasion { ProductId = 3059, OccasionId = 2002 },
                new ProductOccasion { ProductId = 3059, OccasionId = 2006 },
                new ProductOccasion { ProductId = 3059, OccasionId = 2015 },
                new ProductOccasion { ProductId = 3060, OccasionId = 2001 },
                new ProductOccasion { ProductId = 3060, OccasionId = 2002 },
                new ProductOccasion { ProductId = 3060, OccasionId = 2004 },
                new ProductOccasion { ProductId = 3060, OccasionId = 2009 },
                new ProductOccasion { ProductId = 3061, OccasionId = 2001 },
                new ProductOccasion { ProductId = 3061, OccasionId = 2002 },
                new ProductOccasion { ProductId = 3061, OccasionId = 2010 },
                new ProductOccasion { ProductId = 3062, OccasionId = 2001 },
                new ProductOccasion { ProductId = 3062, OccasionId = 2006 },
                new ProductOccasion { ProductId = 3062, OccasionId = 2011 }
            };

            var existingPOKeys = await context.ProductOccasions
                .Select(po => new { po.ProductId, po.OccasionId })
                .ToListAsync();

            var newPO = productOccasionsToSeed
                .Where(po => !existingPOKeys.Any(e => e.ProductId == po.ProductId && e.OccasionId == po.OccasionId))
                .ToList();

            if (newPO.Any())
            {
                context.ProductOccasions.AddRange(newPO);
            }
            await context.SaveChangesAsync();
            summary["ProductOccasions"] = await context.ProductOccasions.CountAsync();
            Console.WriteLine($"[CatalogDataSeeder] ProductOccasions in DB: {summary["ProductOccasions"]}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CatalogDataSeeder] ERROR seeding product occasions: {ex.Message}");
            logger.LogError(ex, "Error seeding product occasions.");
        }

        // ─── 7. Seed Storefront Sections (8 Configured Sections) ──────────────────
        try
        {
            var sectionsToSeed = new List<Section>
            {
                new Section
                {
                    Id = 5001,
                    Title = "Spring & Summer Collection",
                    Order = 1,
                    Type = SectionType.Banner,
                    IsEnabled = true,
                    IsDeleted = false,
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
                    IsDeleted = false,
                    ContentRefJson = "{\"rule\":\"all\",\"limit\":12}",
                    CreatedAt = now
                },
                new Section
                {
                    Id = 5003,
                    Title = "Celebrate Every Moment",
                    Order = 3,
                    Type = SectionType.OccasionRail,
                    IsEnabled = true,
                    IsDeleted = false,
                    ContentRefJson = "{\"rule\":\"all\",\"limit\":16}",
                    CreatedAt = now
                },
                new Section
                {
                    Id = 5004,
                    Title = "Best Selling Florals",
                    Order = 4,
                    Type = SectionType.ProductRail,
                    IsEnabled = true,
                    IsDeleted = false,
                    ContentRefJson = "{\"rule\":\"bestsellers\",\"limit\":10}",
                    CreatedAt = now
                },
                new Section
                {
                    Id = 5005,
                    Title = "Special Offers & Discounts",
                    Order = 5,
                    Type = SectionType.ProductRail,
                    IsEnabled = true,
                    IsDeleted = false,
                    ContentRefJson = "{\"rule\":\"discounted\",\"limit\":10}",
                    CreatedAt = now
                },
                new Section
                {
                    Id = 5006,
                    Title = "Forever Roses & Preserved Florals",
                    Order = 6,
                    Type = SectionType.ProductRail,
                    IsEnabled = true,
                    IsDeleted = false,
                    ContentRefJson = "{\"rule\":\"category\",\"categoryId\":1004,\"limit\":8}",
                    CreatedAt = now
                },
                new Section
                {
                    Id = 5007,
                    Title = "Air-Purifying Houseplants & Bonsai",
                    Order = 7,
                    Type = SectionType.ProductRail,
                    IsEnabled = true,
                    IsDeleted = false,
                    ContentRefJson = "{\"rule\":\"category\",\"categoryId\":1003,\"limit\":8}",
                    CreatedAt = now
                },
                new Section
                {
                    Id = 5008,
                    Title = "Luxury Wedding & Bridal Highlights",
                    Order = 8,
                    Type = SectionType.Banner,
                    IsEnabled = true,
                    IsDeleted = false,
                    ContentRefJson = "{\"imageUrl\":\"https://images.unsplash.com/photo-1519741497674-611481863552\",\"deepLink\":\"/category/1009\",\"subtitle\":\"Breathtaking custom floral designs for your unforgettable day.\"}",
                    CreatedAt = now
                }
            };

            var existingSections = await context.Sections.IgnoreQueryFilters().ToListAsync();
            var existingIds = existingSections.Select(s => s.Id).ToHashSet();

            foreach (var sec in existingSections.Where(s => s.IsDeleted))
            {
                sec.IsDeleted = false;
            }

            var newSections = sectionsToSeed
                .Where(s => !existingIds.Contains(s.Id))
                .ToList();

            if (newSections.Any())
            {
                context.Sections.AddRange(newSections);
            }
            await context.SaveChangesAsync();
            summary["Sections"] = await context.Sections.CountAsync();
            Console.WriteLine($"[CatalogDataSeeder] Sections in DB: {summary["Sections"]}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CatalogDataSeeder] ERROR seeding sections: {ex.Message}");
            logger.LogError(ex, "Error seeding sections.");
        }

        Console.WriteLine("[CatalogDataSeeder] CatalogServiceDb seeding process completed successfully.");
        logger.LogInformation("CatalogServiceDb seeding process completed successfully.");
        return summary;
    }
}
