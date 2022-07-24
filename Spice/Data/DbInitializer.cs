using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Data
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;

        public DbInitializer(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }

                _db.SaveChangesAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error " + ex);
            }

        }
    }
}
