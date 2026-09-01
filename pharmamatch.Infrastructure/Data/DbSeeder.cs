using System;
using System.Collections.Generic;
using System.Linq;
using pharmamatch.Domain.Entities;

namespace pharmamatch.Infrastructure.Data
{
    // =========================================================================
    // كلاس زرع البيانات المبدئية في قاعدة البيانات (Database Seeder)
    // هذا الكلاس يفحص ما إذا كانت قاعدة البيانات فارغة، وإذا كانت فارغة يقوم بتعبئتها
    // ببيانات واقعية تماماً تطابق تصاميم Figma لشاشة المشروعات والتطبيق.
    // =========================================================================
    public static class DbSeeder
    {
        public static void Seed(AppDbContext context)
        {
            // التأكد من إنشاء قاعدة البيانات إن لم تكن موجودة
            context.Database.EnsureCreated();

            // 1. إذا كانت هناك أدوية مخزنة مسبقاً في قاعدة البيانات، لا نكرر الإضافة
            if (context.Medicines.Any())
            {
                return; // إيقاف التمرير لأن البيانات موجودة بالفعل
            }

            // 2. إنشاء الفئات الطبية (Categories)
            var categoryAnalgesic = new Category { CategoryName = "مسكنات", Description = "أدوية مسكنة وخافضة للحرارة" };
            var categoryCold = new Category { CategoryName = "أدوية البرد", Description = "علاجات نزلات البرد والإنفلونزا والاحتقان" };
            var categoryAntibiotic = new Category { CategoryName = "مضادات حيوية", Description = "مضادات حيوية واسعة المجال وموجهة" };
            var categoryDiabetes = new Category { CategoryName = "أدوية السكري", Description = "علاجات وأنسولينات تنظيم سكر الدم" };
            var categoryHypertension = new Category { CategoryName = "أدوية الضغط", Description = "مثبطات وأدوية تنظيم ضغط الدم" };
            var categoryAnticoagulant = new Category { CategoryName = "مضادات التخثر", Description = "أدوية مميعة للدم لمنع التجلط" };

            context.Categories.AddRange(
                categoryAnalgesic, categoryCold, categoryAntibiotic,
                categoryDiabetes, categoryHypertension, categoryAnticoagulant
            );
            context.SaveChanges(); // حفظ الفئات للحصول على Id لها

            // 3. إنشاء المواد الفعالة (Active Ingredients) وربط كل مادة بفئتها
            var ingParacetamolCaffeine = new ActiveIngredient { ScientificName = "باراسيتامول 500ملغ + كافيين 65ملغ", CategoryId = categoryAnalgesic.Id };
            var ingAcetaminophen = new ActiveIngredient { ScientificName = "باراسيتامول وكافيين", CategoryId = categoryAnalgesic.Id };
            var ingParacetamolPseudo = new ActiveIngredient { ScientificName = "باراسيتامول + سودوإيفيدرين", CategoryId = categoryCold.Id };
            var ingAmoxicillin = new ActiveIngredient { ScientificName = "أموكسيسيلين 500ملغ", CategoryId = categoryAntibiotic.Id };
            var ingInsulinGlargine = new ActiveIngredient { ScientificName = "أنسولين جلارجين", CategoryId = categoryDiabetes.Id };
            var ingLisinopril = new ActiveIngredient { ScientificName = "ليسينوبريل 10ملغ", CategoryId = categoryHypertension.Id };
            var ingWarfarin = new ActiveIngredient { ScientificName = "وارفارين 5ملغ", CategoryId = categoryAnticoagulant.Id };
            var ingAspirin = new ActiveIngredient { ScientificName = "أسبرين 81ملغ", CategoryId = categoryAnticoagulant.Id };
            var ingAugmentin = new ActiveIngredient { ScientificName = "أموكسيسيلين وحامض الكلافولانيك", CategoryId = categoryAntibiotic.Id };
            var ingGupisone = new ActiveIngredient { ScientificName = "بريدنيزولون 5ملغ", CategoryId = categoryAnalgesic.Id };
            var ingVentolin = new ActiveIngredient { ScientificName = "سالبوتامول 100ميكروغرام", CategoryId = categoryCold.Id };
            var ingLipitor = new ActiveIngredient { ScientificName = "أتورفاستاتين 20ملغ", CategoryId = categoryHypertension.Id };

            context.ActiveIngredients.AddRange(
                ingParacetamolCaffeine, ingAcetaminophen, ingParacetamolPseudo,
                ingAmoxicillin, ingInsulinGlargine, ingLisinopril, ingWarfarin,
                ingAspirin, ingAugmentin, ingGupisone, ingVentolin, ingLipitor
            );
            context.SaveChanges(); // حفظ المواد الفعالة للحصول على Id لها

            // 4. إنشاء الأدوية الأساسية والفرعية (ProductMedicines)
            // نضمن وجود 24 دواء في قائمة المتابعة (IsWatchlist = true)
            // ونضمن وجود الأدوية المطلوبة في Figma بالدقة ذاتها

            var panadolExtra = new ProductMedicine
            {
                TradeName = "Panadol Extra",
                Price = 15.50m,
                IngredientId = ingParacetamolCaffeine.Id,
                ImagePath = "/images/panadol.png",
                IsWatchlist = true,
                LastUpdated = DateTime.Now.AddMinutes(-15)
            };

            var fefadolExtra = new ProductMedicine
            {
                TradeName = "Fefadol Extra",
                Price = 12.00m,
                IngredientId = ingAcetaminophen.Id,
                ImagePath = "/images/fefadol.png",
                IsWatchlist = true,
                LastUpdated = DateTime.Now.AddHours(-2)
            };

            var adolCold = new ProductMedicine
            {
                TradeName = "Adol Cold",
                Price = 10.00m,
                IngredientId = ingParacetamolPseudo.Id,
                ImagePath = "/images/adol.png",
                IsWatchlist = true,
                LastUpdated = DateTime.Now.AddDays(-1)
            };

            var amoxicillin500 = new ProductMedicine
            {
                TradeName = "Amoxicillin 500mg",
                Price = 25.00m,
                IngredientId = ingAmoxicillin.Id,
                ImagePath = "/images/amoxicillin.png",
                IsWatchlist = true,
                LastUpdated = DateTime.Now.AddMinutes(-15)
            };

            var insulinGlargine = new ProductMedicine
            {
                TradeName = "Insulin Glargine",
                Price = 85.00m,
                IngredientId = ingInsulinGlargine.Id,
                ImagePath = "/images/insulin.png",
                IsWatchlist = true,
                LastUpdated = DateTime.Now.AddHours(-2)
            };

            var lisinopril10 = new ProductMedicine
            {
                TradeName = "Lisinopril 10mg",
                Price = 18.00m,
                IngredientId = ingLisinopril.Id,
                ImagePath = "/images/lisinopril.png",
                IsWatchlist = true,
                LastUpdated = DateTime.Now.AddDays(-1)
            };

            var warfarin5 = new ProductMedicine
            {
                TradeName = "Warfarin 5mg",
                Price = 9.50m,
                IngredientId = ingWarfarin.Id,
                ImagePath = "/images/warfarin.png",
                IsWatchlist = true,
                LastUpdated = DateTime.Now.AddMinutes(-5)
            };

            var aspirinLow = new ProductMedicine
            {
                TradeName = "Aspirin 81mg",
                Price = 8.00m,
                IngredientId = ingAspirin.Id,
                ImagePath = "/images/aspirin.png",
                IsWatchlist = true,
                LastUpdated = DateTime.Now.AddDays(-2)
            };

            context.Medicines.AddRange(
                panadolExtra, fefadolExtra, adolCold, amoxicillin500,
                insulinGlargine, lisinopril10, warfarin5, aspirinLow
            );

            // إضافة باقي الأدوية لإكمال قائمة المتابعة 24 دواء وإكمال العدد الحرج 14 أصناف
            var watchlistStableMedicines = new List<ProductMedicine>();
            for (int i = 1; i <= 13; i++)
            {
                var med = new ProductMedicine
                {
                    TradeName = $"مستحضر متابعة مستقر #{i}",
                    Price = 14.00m + i,
                    IngredientId = ingParacetamolCaffeine.Id,
                    IsWatchlist = true,
                    LastUpdated = DateTime.Now.AddHours(-i)
                };
                watchlistStableMedicines.Add(med);
            }
            context.Medicines.AddRange(watchlistStableMedicines);

            // إضافة أدوية ذات مخزون منخفض/حرج لتحقيق (أصناف حرجة = 14)
            var criticalMedicines = new List<ProductMedicine>();
            for (int i = 1; i <= 10; i++)
            {
                var med = new ProductMedicine
                {
                    TradeName = $"صنف مخزون حرج #{i}",
                    Price = 20.00m + i,
                    IngredientId = ingParacetamolPseudo.Id,
                    IsWatchlist = (i <= 3), // أول 3 منها في قائمة المتابعة لنحصل على (في نقص حاد = 3)
                    LastUpdated = DateTime.Now.AddHours(-i * 3)
                };
                criticalMedicines.Add(med);
            }
            context.Medicines.AddRange(criticalMedicines);

            // أدوية ذات مخزون عالي جداً في الخلفية لنصل بإجمالي الكمية إلى 42,019 بالضبط
            var augmentin = new ProductMedicine { TradeName = "Augmentin Duo", Price = 45.00m, IngredientId = ingAugmentin.Id, IsWatchlist = false };
            var gupisone = new ProductMedicine { TradeName = "Gupisone 5mg", Price = 11.00m, IngredientId = ingGupisone.Id, IsWatchlist = false };
            var ventolin = new ProductMedicine { TradeName = "Ventolin Inhaler", Price = 32.00m, IngredientId = ingVentolin.Id, IsWatchlist = false };
            var lipitor = new ProductMedicine { TradeName = "Lipitor 20mg", Price = 65.00m, IngredientId = ingLipitor.Id, IsWatchlist = false };

            context.Medicines.AddRange(augmentin, gupisone, ventolin, lipitor);
            context.SaveChanges(); // حفظ الأدوية للحصول على أرقام تعريفية (Ids)

            // 5. تعبئة شحنات المخزون (Inventory Batches) لربط الكميات والتواريخ
            var batches = new List<InventoryBatch>
            {
                // الأدوية الرئيسية المحتواه في Figma
                new InventoryBatch { MedicineId = panadolExtra.Id, BatchNumber = "BN-1092-PA", Quantity = 1240, ExpiryDate = new DateTime(2026, 10, 31) },
                new InventoryBatch { MedicineId = fefadolExtra.Id, BatchNumber = "BN-8821-FE", Quantity = 843, ExpiryDate = new DateTime(2026, 1, 31) },
                new InventoryBatch { MedicineId = adolCold.Id, BatchNumber = "BN-4402-AD", Quantity = 42, ExpiryDate = new DateTime(2026, 2, 28) }, // حرج (42 وحدة)
                
                // Amoxicillin 500mg لديه دفعة صالحة ودفعة منتهية الصلاحية مطابقة للتقرير
                new InventoryBatch { MedicineId = amoxicillin500.Id, BatchNumber = "BN-5510-AM", Quantity = 150, ExpiryDate = new DateTime(2026, 8, 30) },
                new InventoryBatch { MedicineId = amoxicillin500.Id, BatchNumber = "SN-98210-AM", Quantity = 450, ExpiryDate = new DateTime(2024, 12, 14) }, // منتهي الصلاحية!

                new InventoryBatch { MedicineId = insulinGlargine.Id, BatchNumber = "BN-4020-IN", Quantity = 120, ExpiryDate = new DateTime(2026, 1, 15) },
                new InventoryBatch { MedicineId = lisinopril10.Id, BatchNumber = "BN-7712-LI", Quantity = 2800, ExpiryDate = new DateTime(2026, 6, 30) },
                new InventoryBatch { MedicineId = warfarin5.Id, BatchNumber = "BN-3390-WA", Quantity = 15, ExpiryDate = new DateTime(2025, 12, 31) }, // حرج (15 وحدة)
                new InventoryBatch { MedicineId = aspirinLow.Id, BatchNumber = "BN-1120-AS", Quantity = 30, ExpiryDate = new DateTime(2026, 5, 20) }, // حرج (30 وحدة)
            };

            // إضافة دفعات للأدوية المستقرة في قائمة المتابعة (13 دواء × 150 = 1,950 وحدة)
            foreach (var med in watchlistStableMedicines)
            {
                batches.Add(new InventoryBatch
                {
                    MedicineId = med.Id,
                    BatchNumber = $"BN-STABLE-{med.Id}",
                    Quantity = 150,
                    ExpiryDate = DateTime.Now.AddMonths(18)
                });
            }

            // إضافة دفعات للأدوية الحرجة الـ 10 الأخرى (مجموع كمياتها = 210 وحدة)
            int[] criticalQuantities = new int[] { 20, 25, 10, 5, 45, 12, 8, 15, 30, 40 };
            for (int i = 0; i < criticalMedicines.Count; i++)
            {
                batches.Add(new InventoryBatch
                {
                    MedicineId = criticalMedicines[i].Id,
                    BatchNumber = $"BN-CRIT-{criticalMedicines[i].Id}",
                    Quantity = criticalQuantities[i],
                    ExpiryDate = DateTime.Now.AddMonths(i % 2 == 0 ? 1 : 12)
                });
            }

            // الكمية المتبقية المطلوبة للوصول لإجمالي 42,019 بالضبط
            // مجموع الكميات أعلاه: 1240+843+42+150+450+120+2800+15+30 + 1950 + 210 = 7,850
            // المتبقي = 42,019 - 7,850 = 34,169
            batches.Add(new InventoryBatch { MedicineId = augmentin.Id, BatchNumber = "BN-BULK-1", Quantity = 12000, ExpiryDate = DateTime.Now.AddYears(2) });
            batches.Add(new InventoryBatch { MedicineId = gupisone.Id, BatchNumber = "BN-BULK-2", Quantity = 11000, ExpiryDate = DateTime.Now.AddYears(2) });
            batches.Add(new InventoryBatch { MedicineId = ventolin.Id, BatchNumber = "BN-BULK-3", Quantity = 6169, ExpiryDate = DateTime.Now.AddYears(2) });
            batches.Add(new InventoryBatch { MedicineId = lipitor.Id, BatchNumber = "BN-BULK-4", Quantity = 5000, ExpiryDate = DateTime.Now.AddYears(2) });

            context.InventoryBatches.AddRange(batches);
            context.SaveChanges(); // حفظ جميع شحنات المخزون في قاعدة البيانات
        }
    }
}
