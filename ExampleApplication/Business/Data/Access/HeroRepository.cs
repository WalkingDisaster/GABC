using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExampleApplication.Business.Data.Entities;
using ExampleApplication.Business.Data.Framework;
using StackExchange.Redis;

namespace ExampleApplication.Business.Data.Access
{
    public interface IHeroRepository
    {
        Task ResetAsInScorchedEarthKillEverything();
        IEnumerable<Hero> GetAll();
        Hero GetById(string id);
        Task DispatchHeroAsync(string id, string location);
        Task RecallHeroAsync(string id);
        string GetCurrentLocation(string id);
    }

    public class HeroRepository : IHeroRepository
    {
        private readonly IDocumentProvider _documentProvider;
        private readonly ConnectionMultiplexer _connectionMultiplexer;

        public HeroRepository(IDocumentProvider documentProvider, ConnectionMultiplexer connectionMultiplexer)
        {
            _documentProvider = documentProvider;
            _connectionMultiplexer = connectionMultiplexer;
        }

        public IEnumerable<Hero> GetAll()
        {
            return _documentProvider.Query<Hero>();
        }

        public Hero GetById(string id)
        {
            return _documentProvider.Query<Hero>(h => h.Id == id).SingleOrDefault();
        }

        public async Task DispatchHeroAsync(string id, string location)
        {
            var hero = GetById(id);
            if (hero == null)
            {
                throw new Exception("Invalid hero id");
            }
            var db = _connectionMultiplexer.GetDatabase();
            if (db.HashExists("dispatch", id))
            {
                throw new Exception("Hero already dispatched");
            }
            db.HashSet("dispatch", new[] {new HashEntry(id, location)});
            hero.DispatchStatus = DispatchType.Dispatched;
            hero.DispatchLocation = location;
            await _documentProvider.UpdateDocumentAsync(hero);
        }

        public async Task RecallHeroAsync(string id)
        {
            var hero = GetById(id);
            hero.DispatchStatus = DispatchType.Available;
            hero.DispatchLocation = null;
            await _documentProvider.UpdateDocumentAsync(hero);

            var db = _connectionMultiplexer.GetDatabase();
            db.HashDelete("dispatch", id);
        }

        public string GetCurrentLocation(string id)
        {
            var db = _connectionMultiplexer.GetDatabase();
            var locationFromCache = db.HashGet("dispatch", id);
            var result = locationFromCache.HasValue ? locationFromCache.ToString() : null;
            return result;
        }

        public async Task ResetAsInScorchedEarthKillEverything()
        {
            _connectionMultiplexer.GetDatabase().KeyDelete("dispatch");
            await _documentProvider.DestroyDatabaseAsync<Hero>();
            await _documentProvider.CreateDocumentAsync(new Hero
            {
                Name = "Natasha Rominoff",
                AlsoKnownAs = "Black Widow",
                DispatchStatus = DispatchType.Available,
                Types = HeroTypes.SuperSpy | HeroTypes.ExceptionalHuman,
                Weapons = new []
                {
                    new Weapon { Name = "Widow's Bite", Type = "Electric Bracelets" },
                    new Weapon { Type = "Pistol" }
                }
            });
            await _documentProvider.CreateDocumentAsync(new Hero
            {
                Name = "Steve Rogers",
                AlsoKnownAs = "Captain America",
                DispatchStatus = DispatchType.Available,
                Types = HeroTypes.SuperSpy | HeroTypes.GeneticallyModified,
                Weapons = new[]
                {
                    new Weapon { Name = "Cap's Shield", Type = "Shield" }
                }
            });
            await _documentProvider.CreateDocumentAsync(new Hero
            {
                Name = "Bruce Banner",
                AlsoKnownAs = "The Hulk",
                DispatchStatus = DispatchType.Available,
                Types = HeroTypes.GeneticallyModified | HeroTypes.SuperGenius
            });
            await _documentProvider.CreateDocumentAsync(new Hero
            {
                Name = "Tony Stark",
                AlsoKnownAs = "Iron Man",
                DispatchStatus = DispatchType.Available,
                Types = HeroTypes.ExoSuit | HeroTypes.SuperGenius,
                Weapons = new[]
                {
                    new Weapon { Type = "Repulsor Ray" },
                    new Weapon { Type = "EMP Generator" },
                    new Weapon { Type = "Uni-Beam" },
                    new Weapon { Type = "Pulse Bolts" },
                    new Weapon { Type = "Freeze Beam" }
                }
            });
            await _documentProvider.CreateDocumentAsync(new Hero
            {
                Name = "Thor",
                DispatchStatus = DispatchType.Available,
                Types = HeroTypes.Alien,
                Weapons = new[]
                {
                    new Weapon { Name = "Mjölnir", Type = "Hammer" }
                }
            });
            await _documentProvider.CreateDocumentAsync(new Hero
            {
                Name = "Clint Barton",
                AlsoKnownAs = "Hawkeye",
                DispatchStatus = DispatchType.Available,
                Types = HeroTypes.ExceptionalHuman,
                Weapons = new[]
                {
                    new Weapon { Type = "Katana" },
                    new Weapon { Type = "Bow" }
                }
            });
            await _documentProvider.CreateDocumentAsync(new Hero
            {
                Name = "James Rhodes",
                AlsoKnownAs = "War Machine",
                DispatchStatus = DispatchType.Available,
                Types = HeroTypes.ExoSuit,
                Weapons = new[]
                {
                    new Weapon { Type = "Reuplsor Ray" },
                    new Weapon { Type = "Missiles" },
                    new Weapon { Type = "Laser Blasters" }
                }
            });
            await _documentProvider.CreateDocumentAsync(new Hero
            {
                Name = "Pietro Maximoff",
                AlsoKnownAs = "Quicksilver",
                DispatchStatus = DispatchType.Available,
                Types = HeroTypes.Mutant
            });
            await _documentProvider.CreateDocumentAsync(new Hero
            {
                Name = "Wanda Maximoff",
                AlsoKnownAs = "Scarlet Witch",
                DispatchStatus = DispatchType.Available,
                Types = HeroTypes.Mutant
            });
            await _documentProvider.CreateDocumentAsync(new Hero
            {
                Name = "Sam Wilson",
                AlsoKnownAs = "The Falcon",
                DispatchStatus = DispatchType.Available,
                Types = HeroTypes.ExoSuit,
                Weapons = new[]
                {
                    new Weapon { Name = "Talon", Type = "Grappling Hook" },
                    new Weapon { Type = "Submachine Guns (2)" }
                }
            });
            await _documentProvider.CreateDocumentAsync(new Hero
            {
                Name = "Pepper Potts",
                AlsoKnownAs = "Iron Potts",
                DispatchStatus = DispatchType.Available,
                Types = HeroTypes.ExoSuit,
                Weapons = new[]
                {
                    new Weapon { Type = "Repulsor Ray" },
                    new Weapon { Type = "EMP Generator" },
                    new Weapon { Type = "Uni-Beam" },
                    new Weapon { Type = "Pulse Bolts" },
                    new Weapon { Type = "Freeze Beam" }
                }
            });
        }
    }
}