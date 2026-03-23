using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.dbContext
{
    public static class DbSeeder
    {
        public static void SeedData(eShopDbContext context)
        {
            context.Database.Migrate();

            if (!context.Branches.Any())
            {
                context.Branches.AddRange(
                    new Branch { Name = "Kho Tổng Hà Nội", Address = "Hà Nội", BranchType = "WAREHOUSE", IsActive = true },
                    new Branch { Name = "36 Hàng Ngang", Address = "36 Hàng Ngang", BranchType = "STORE", IsActive = true }
                );
                context.SaveChanges();
            }

            if (!context.Suppliers.Any())
            {
                context.Suppliers.AddRange(
                    new Supplier { Name = "Công ty CP Acecook", ContactName = "Anh Tuấn", Phone = "0901234567", Address = "Hà Nội" },
                    new Supplier { Name = "Nhà rang xay Cà phê Trung Nguyên", ContactName = "Chị Lan", Phone = "0987654321", Address = "Hà Nội" }
                );
                context.SaveChanges();
            }

            if (!context.ProductGroups.Any())
            {
                context.ProductGroups.AddRange(
                    new ProductGroup { Name = "Cà phê", Description = "Các loại cà phê hạt và rang xay", IsActive = true },
                    new ProductGroup { Name = "Trà & Thảo mộc", Description = "Trà túi lọc, thảo dược", IsActive = true },
                    new ProductGroup { Name = "Bánh kẹo", Description = "Socola và các loại hạt", IsActive = true }
                );
                context.SaveChanges();
            }

            if (!context.Products.Any())
            {
                var trungNguyenId = context.Suppliers.FirstOrDefault(s => s.Name.Contains("Trung Nguyên"))?.Id;
                var acecookId = context.Suppliers.FirstOrDefault(s => s.Name.Contains("Acecook"))?.Id;

                var cafeGroupId = context.ProductGroups.FirstOrDefault(g => g.Name == "Cà phê")?.Id;
                var teaGroupId = context.ProductGroups.FirstOrDefault(g => g.Name == "Trà & Thảo mộc")?.Id;

                context.Products.AddRange(
                    new Product
                    {
                        Sku = "CF-001",
                        Barcode = "8935137601111",
                        Name = "Cà phê Robusta Nguyên chất",
                        ProductGroupId = cafeGroupId,    
                        SupplierId = trungNguyenId,       
                        BaseUnit = "Gram",             
                        PurchasePrice = 150,         
                        SalePrice = 300,            
                        IsActive = true,           
                        ShowOnPos = true          
                    },
                    new Product
                    {
                        Sku = "AN02",
                        Barcode = "8935137601863",
                        Name = "Actiso (new)",
                        ProductGroupId = teaGroupId,
                        SupplierId = acecookId,
                        BaseUnit = "Box",
                        PurchasePrice = 45000,
                        SalePrice = 65000,
                        IsActive = true,
                        ShowOnPos = true
                    }
                );
                context.SaveChanges();
            }

            if (!context.Inventories.Any())
            {
                var khoTongId = context.Branches.FirstOrDefault(b => b.BranchType == "WAREHOUSE")?.Id ?? 1;
                var haoHaoId = context.Products.FirstOrDefault(p => p.Barcode == "8934561021008")?.Id ?? 1;

                context.Inventories.Add(
                    new Inventory { BranchId = khoTongId, ProductId = haoHaoId, Quantity = 1000 }
                );
                context.SaveChanges();
            }

            if (!context.UnitConversions.Any() && context.Products.Any())
            {
                var cafe = context.Products.FirstOrDefault(p => p.Sku == "CF-001");

                if (cafe != null)
                {
                    context.UnitConversions.AddRange(
                        new UnitConversion { ProductId = cafe.Id, UnitName = "Túi 300gram", ConversionRate = 300, PurchasePrice = 50000, SalePrice = 100000 },
                        new UnitConversion { ProductId = cafe.Id, UnitName = "Túi 500", ConversionRate = 500, PurchasePrice = 50000, SalePrice = 100000 },
                        new UnitConversion { ProductId = cafe.Id, UnitName = "Túi 100", ConversionRate = 100, PurchasePrice = 50000, SalePrice = 100000 },
                        new UnitConversion { ProductId = cafe.Id, UnitName = "Túi 200", ConversionRate = 200, PurchasePrice = 50000, SalePrice = 100000 }
                    );
                    context.SaveChanges();
                }
            }
        }
    }
}
