using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebCore.Models.Table;

namespace WebCore.Models
{
    public class DwhContext : DbContext
    {
        public DwhContext(DbContextOptions<DwhContext> options)
            : base(options)
        {
           
        }

        public DbSet<User> Users { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
            //optionsBuilder.UseOracle(@"User Id=dwhcore;Password=dwhcore;Data Source=168.138.211.114/dbdwh;");
        }
    }
}
