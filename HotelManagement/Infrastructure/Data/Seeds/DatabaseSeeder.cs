using HotelManagement.Domain.Models;
using HotelManagement.Infrastructure.Data.Seeds.FakeData;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Infrastructure.Data.Seeds
{
    public static class DatabaseSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            try
            {
                context.Database.Migrate();
            }
            catch
            {
                context.Database.EnsureCreated();
            }

            if (!context.Facilities.Any())
            {
                var facilities = FacilityDataGenerator.Generate(10);
                context.Facilities.AddRange(facilities);
                context.SaveChanges();
            }

            if (!context.Rooms.Any())
            {
                var rooms = RoomDataGenerator.Generate(50);
                context.Rooms.AddRange(rooms);
                context.SaveChanges();

                var facilities = context.Facilities.ToList();
                var roomFacilities = new List<RoomFacility>();
                var rnd = new Random();
                foreach (var room in context.Rooms)
                {
                    var take = rnd.Next(1, Math.Min(4, facilities.Count) + 1);
                    var assigned = facilities.OrderBy(_ => rnd.Next()).Take(take).ToList();
                    foreach (var f in assigned)
                    {
                        roomFacilities.Add(new RoomFacility { RoomId = room.Id, FacilityId = f.Id });
                    }
                }

                if (roomFacilities.Any())
                {
                    context.RoomFacilities.AddRange(roomFacilities);
                    context.SaveChanges();
                }
            }

            // Seed guests
            if (!context.Set<Guest>().Any())
            {
                var guests = FakeData.GuestDataGenerator.Generate(50);
                context.Set<Guest>().AddRange(guests);
                context.SaveChanges();
            }
        }
    }
}
