using Microsoft.EntityFrameworkCore;
using CS_DemoEFCore.Models;
using CS_DemoEFCore.Data;

namespace CS_DemoEFCore
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var context = new ZenDbContext(new DbContextOptions<ZenDbContext>()))
            {
                // Ensure database is created
                // context.Database.EnsureCreated();
                
                // Create a new product
                var product1 = new Product()
                {
                    Name = "Sample Product",
                    Description = "This is a sample product.",
                    Price = 9.99m
                };
                // Add the product to the database
                context.Products.Add(product1);
                var product2 = new Product()
                {
                    Name = "Another Product",
                    Description = "This is another sample product.",
                    Price = 19.99m
                };
                context.Products.Add(product2);
                var product3 = new Product()
                {
                    Name = "Third Product",
                    Description = "This is the third sample product.",
                    Price = 29.99m
                };
                context.Products.Add(product3);
                context.SaveChanges();

                // Retrieve and display all products
                var products = context.Products.ToList();
                foreach (var p in products)
                {
                    Console.WriteLine($"ID: {p.ProductId}, Name: {p.Name}, Description: {p.Description}, Price: {p.Price}");
                }

                Console.WriteLine();
                //Update first product
                var firstProduct = context.Products.FirstOrDefault(p => p.ProductId == 1);
                if (firstProduct != null)
                {
                    firstProduct.Name = "Updated Product";
                    firstProduct.Description = "This is an updated sample product.";
                    firstProduct.Price = 39.99m;
                    context.SaveChanges();
                }
                // Retrieve and display all products
                products = context.Products.ToList();
                foreach (var p in products)
                {
                    Console.WriteLine($"ID: {p.ProductId}, Name: {p.Name}, Description: {p.Description}, Price: {p.Price}");
                }

                Console.WriteLine();

                // Delete second product
                var secondProduct = context.Products.FirstOrDefault(p => p.ProductId == 2);
                if (secondProduct != null)
                {
                    context.Products.Remove(secondProduct);
                    context.SaveChanges();
                }

                // Retrieve and display all products
                products = context.Products.AsNoTracking().ToList();
                foreach (var p in products)
                {
                    Console.WriteLine($"ID: {p.ProductId}, Name: {p.Name}, Description: {p.Description}, Price: {p.Price}");
                }

                Console.WriteLine("Program completed. Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}
