using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Spice.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spice.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<CategoryModel> Category { get; set; }
        public DbSet<SubCategoryModel> SubCategory { get; set; }
        public DbSet<MenuItemModel> MenuItem { get; set; }
        public DbSet<CouponModel> Coupon { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<ShoppingCartModel> ShoppingCart { get; set; }
        public DbSet<OrderHeaderModel> OrderHeader { get; set; }
        public DbSet<OrderDetailsModel> OrderDetails { get; set; }

    }
}
