using Bogus;
using HotelManagement.Domain.Enums;
using HotelManagement.Domain.Models;

namespace HotelManagement.Infrastructure.Data.Seeds.FakeData
{
    public static class RoomDataGenerator
    {
        public static List<Room> Generate(int count = 50)
        {
            var faker = new Faker<Room>()
                .RuleFor(r => r.RoomNumber, f => f.IndexFaker)
                .RuleFor(r => r.IsAvailable, f => f.Random.Bool())
                .RuleFor(r => r.CreatedAt, f => f.Date.Past(1, DateTime.UtcNow))
                .RuleFor(r => r.PricePerNight, f => f.Random.Decimal())
                .RuleFor(r => r.UpdatedAt, f => f.Date.Recent(1,DateTime.UtcNow))
                .RuleFor(r => r.ImageUrl, f => f.Internet.Url())
                .RuleFor(r => r.Type, f => f.PickRandom<RoomType>())
                .RuleFor(r => r.IsDeleted, f => f.Random.Bool());
            return faker.Generate(count);
        }
    }
   
}
