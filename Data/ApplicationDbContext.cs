using Microsoft.EntityFrameworkCore;
using Tickset.Models;

namespace Tickset.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}