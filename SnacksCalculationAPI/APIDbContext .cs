﻿using Microsoft.EntityFrameworkCore;
using SnacksCalculationAPI.Models;

namespace SnacksCalculationAPI
{
    public class APIDbContext : DbContext
    {
        public APIDbContext(DbContextOptions<APIDbContext> options) : base(options) { }
        public DbSet<UserModel> UserModels { get; set; }


    }
}
