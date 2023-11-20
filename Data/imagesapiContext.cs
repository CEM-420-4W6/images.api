using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using images.api.Models;

namespace images.api.Data
{
    public class imagesapiContext : DbContext
    {
        public imagesapiContext (DbContextOptions<imagesapiContext> options)
            : base(options)
        {
        }

        public DbSet<images.api.Models.Photo> Photo { get; set; } = default!;
    }
}
