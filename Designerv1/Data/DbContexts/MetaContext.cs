using System;
using Microsoft.EntityFrameworkCore;

namespace Designerv1.Data.DbContexts
{
    public class MetaContext : DbContext
    {
        
        public DbSet<MetaModel> Models { get; set; }

        public MetaContext(DbContextOptions<MetaContext> options) : base(options)
        {
        }

    }
}
