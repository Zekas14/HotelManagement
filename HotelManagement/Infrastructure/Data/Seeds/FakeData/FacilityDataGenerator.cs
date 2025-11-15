using Bogus;
using HotelManagement.Domain.Models;

namespace HotelManagement.Infrastructure.Data.Seeds.FakeData
{
    public static class FacilityDataGenerator
    {
        private static readonly string[] DefaultFacilities = new[]
        {
            "Free WiFi",
            "Air Conditioning",
            "Breakfast Included",
            "Swimming Pool",
            "Parking",
            "Gym",
            "Spa",
            "Restaurant",
            "Bar",
            "24-hour Front Desk"
        };

        public static List<Facility> Generate(int count = 10)
        {
            var faker = new Faker<Facility>()
                .RuleFor(f => f.Name, (f, u) => f.PickRandom(DefaultFacilities))
                .RuleFor(g => g.IsDeleted, f => f.Random.Bool(0.05f))
                .RuleFor(g => g.CreatedAt, f => f.Date.Past(1, DateTime.UtcNow))
                .RuleFor(g => g.UpdatedAt, f => f.Date.Recent(1, DateTime.UtcNow))
                .RuleFor(g => g.CreatedBy, f => f.Random.Int(1, 5));

            var list = new List<Facility>();
            while (list.Count < count)
            {
                var facility = faker.Generate();
                if (list.Any(x => x.Name == facility.Name))
                {
                    continue;
                }
                list.Add(facility);
            }

            return list;
        }
    }
}
