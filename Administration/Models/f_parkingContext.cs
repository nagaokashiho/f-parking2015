using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Administration.Models
{
    class f_parkingContext : DbContext
    {
        public virtual DbSet<WhatsNew> WhatsNews { get; set; }

        public f_parkingContext()
            : base("DefaultConnection")
        {
        }
    }
}