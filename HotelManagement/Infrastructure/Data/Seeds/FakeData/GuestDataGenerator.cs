using Bogus;
using HotelManagement.Domain.Models;

namespace HotelManagement.Infrastructure.Data.Seeds.FakeData
{
    public static class GuestDataGenerator
    {
        public static List<Guest> Generate(int count = 50)
        {
            var faker = new Faker<Guest>()
                .RuleFor(g => g.User.FullName, f => f.Name.FullName())
                .RuleFor(g => g.User.Username, f => f.Internet.UserName())
                .RuleFor(g => g.User.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(g => g.User.Email, f => f.Internet.Email())
                .RuleFor(g => g.User.PasswordHash, f => f.Internet.Password())
                .RuleFor(g => g.IsDeleted, f => f.Random.Bool(0.05f))
                .RuleFor(g => g.CreatedAt, f => f.Date.Past(1, DateTime.UtcNow))
                .RuleFor(g => g.UpdatedAt, f => f.Date.Recent(1, DateTime.UtcNow))
                .RuleFor(g => g.CreatedBy, f => f.Random.Int(1, 5));

            return faker.Generate(count);
        }
    }
}
