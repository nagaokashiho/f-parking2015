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
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<Reservation> Reservations { get; set; }

        public f_parkingContext()
            : base("DefaultConnection")
        {
        }
    }

    public class f_parkingRepository
    {
        private f_parkingContext db = new f_parkingContext();

        public f_parkingRepository()
        {
            //SQLログを出力する
            db.Database.Log = (log) => System.Diagnostics.Debug.WriteLine(log);
        }
    }
}