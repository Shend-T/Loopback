// using backend.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }
}
