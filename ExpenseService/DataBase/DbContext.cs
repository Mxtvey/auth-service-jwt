using ExpenseService.models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseService.DataBase;

public class AppDbContext : DbContext
{
    public DbSet<User> users { get; set; }
    public DbSet<Expense> expenses { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}