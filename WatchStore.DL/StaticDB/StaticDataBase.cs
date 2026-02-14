using WatchStore.Core.Models;

namespace WatchStore.DL.StaticDataBase
{
    internal static class StaticDb
    {
        public static Guid RichClientId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        public static Guid PoorClientId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        public static Guid RolexId = Guid.Parse("33333333-3333-3333-3333-333333333333"); 
        public static Guid CasioId = Guid.Parse("44444444-4444-4444-4444-444444444444"); 
        public static Guid SoldOmegaId = Guid.Parse("55555555-5555-5555-5555-555555555555"); 

        public static List<Client> Clients { get; set; } = new List<Client>();
        public static List<Watch> Watches { get; set; } = new List<Watch>();

        static StaticDb()
        {
            Watches.Add(new Watch
            {
                Brand = "Rolex",
                Model = "Submariner",
                Price = 12500,
                CaseDiameterMm = 41,
                ProductionYear = 2022,
                OwnerId = null
            });

            Watches.Add(new Watch
            {
                Brand = "Casio",
                Model = "G-Shock",
                Price = 150,
                CaseDiameterMm = 45,
                ProductionYear = 2023,
                OwnerId = null
            });

            Watches.Add(new Watch
            {
                Brand = "Omega",
                Model = "Speedmaster",
                Price = 7200,
                CaseDiameterMm = 42,
                ProductionYear = 2021,
                OwnerId = RichClientId
            });

            Clients.Add(new Client
            {
                Id=RichClientId,
                Name = "Ivan Petrov",
                PhoneNumber = "0888123456",
                Email = "ivan@example.com",
                Address = "Sofia, str. Vitosha 15",
                Balance = 5000
            });

            Clients.Add(new Client
            {
                Id=PoorClientId,
                Name = "Maria Georgieva",
                PhoneNumber = "0899987654",
                Email = "maria@test.com",
                Address = "Plovdiv, str. Glavna 3",
                Balance = 15000
            });
        }
    }
}