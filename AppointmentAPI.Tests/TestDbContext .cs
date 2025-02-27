using AppointmentManager.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentAPI.Tests
{
    public class TestDbContext
    {
        public static AppointmentDbContext GetTestDbContext()
        {
            var options = new DbContextOptionsBuilder<AppointmentDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppointmentDbContext(options);
        }
    }
}
