﻿﻿using JourneyMentor.Loyalty.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JourneyMentor.Loyalty.Persistence.Context
{
    public class LoyaltyDbContext : DbContext
    {
        public LoyaltyDbContext(DbContextOptions<LoyaltyDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Point> Points { get; set; }
    }
}
