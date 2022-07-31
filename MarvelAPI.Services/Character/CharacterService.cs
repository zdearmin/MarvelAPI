using System.Net.Sockets;
using MarvelAPI.Data;
using MarvelAPI.Data.Entities;
using MarvelAPI.Models.Characters;
using MarvelAPI.Models.Movies;
using MarvelAPI.Models.Teams;
using MarvelAPI.Models.TVShows;
using Microsoft.EntityFrameworkCore;

namespace MarvelAPI.Services.Character
{
    public class CharacterService : ICharacterService
    {
        private readonly AppDbContext _dbContext;
        public CharacterService (AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> CreateCharacterAsync(CharacterCreate model)
        {
            var character = new CharacterEntity{
                FullName = model.FullName,
                Age = model.Age
            };
            // Verify no duplicates by FullName (formatting for comparison)
            foreach (var c in await _dbContext.Characters.ToListAsync()) {
                if (ReformatName(c.FullName) == ReformatName(character.FullName)) {
                    return false;
                }
            }
            _dbContext.Characters.Add(character);
            var numOfChanges = await _dbContext.SaveChangesAsync();
            return numOfChanges == 1;
        }

        public async Task<IEnumerable<CharacterListItem>> GetAllCharactersAsync()
        {
            var result = await _dbContext.Characters
                .Select(
                    c => new CharacterListItem{
                        Id = c.Id,
                        FullName = c.FullName
                })
                .OrderBy(n => n.FullName)
                .ToListAsync();
            return result;
        }

        public async Task<IEnumerable<CharacterAbilities>> GetCharactersByAbilityAsync(string ability) {
            var result = await _dbContext.Characters
                .Where(
                    o => o.Abilities != null && 
                    o.Abilities.ToLower().Contains(ability.ToLower())
                )
                .Select(
                    c => new CharacterAbilities{
                        Id = c.Id,
                        FullName = c.FullName,
                        Abilities = c.Abilities
                })
                .OrderBy(n => n.FullName)
                .ToListAsync();
            return result;
        }

        public async Task<IEnumerable<CharacterAliases>> GetCharactersByAliasesAsync(string aliases) {
            var result = await _dbContext.Characters
                .Where(
                    o => o.Aliases != null && 
                    o.Aliases.ToLower().Contains(aliases.ToLower())
                )
                .Select(
                    c => new CharacterAliases{
                        Id = c.Id,
                        FullName = c.FullName,
                        Aliases = c.Aliases
                })
                .OrderBy(n => n.FullName)
                .ToListAsync();
            return result;
        }

        public async Task<IEnumerable<CharacterListItem>> GetCharactersByNameAsync(string name) {
            var result = await _dbContext.Characters
                .Where(
                    o => o.FullName != null && 
                    o.FullName.ToLower().Contains(name.ToLower())
                )
                .Select(
                    c => new CharacterListItem{
                        Id = c.Id,
                        FullName = c.FullName
                })
                .OrderBy(n => n.FullName)
                .ToListAsync();
            return result;
        }


        public async Task<CharacterDetail> GetCharacterByIdAsync(int id)
        {
            var characterFound = await _dbContext.Characters
            .Include(x => x.Movies)
            .ThenInclude(y => y.Movie)
            .Include(x => x.TVShows)
            .ThenInclude(y => y.TVShow)
            .Include(x => x.Teams)
            .ThenInclude(y => y.Team)
            .FirstOrDefaultAsync(c => c.Id == id);

            if (characterFound != null)
            {
                return new CharacterDetail
                {
                    Id = characterFound.Id,
                    FullName = characterFound.FullName,
                    Age = characterFound.Age,
                    Location = characterFound.Location,
                    Origin = characterFound.Origin,
                    Abilities = characterFound.Abilities,
                    AbilitiesOrigin = characterFound.AbilitiesOrigin,
                    Aliases = characterFound.Aliases,
                    Status = characterFound.Status,
                    Movies = characterFound.Movies.Select(m => new MovieListItem
                    {
                        Id = m.MovieId,
                        Title = m.Movie.Title
                    })
                    .OrderBy(t => t.Title)
                    .ToList(),
                    TVShows = characterFound.TVShows.Select(tv => new TVShowListItem
                    {
                        Id = tv.TVShowId,
                        Title = tv.TVShow.Title
                    })
                    .OrderBy(t => t.Title)
                    .ToList(),
                    Teams = characterFound.Teams.Select(tm => new TeamListItem
                    {
                        Id = tm.TeamId,
                        Name = tm.Team.Name
                    })
                    .OrderBy(n => n.Name)
                    .ToList(),
                };
            }
            return null;
        }

        public async Task<bool> UpdateCharacterAsync(int characterId, CharacterUpdate request)
        {
            var characterFound = await _dbContext.Characters.FindAsync(characterId);
            
            if (characterFound != null)
            {
                characterFound.FullName = CheckUpdateProperty(characterFound.FullName, request.FullName);
                characterFound.Age = CheckUpdateProperty(characterFound.Age, request.Age);
                characterFound.Location = CheckUpdateProperty(characterFound.Location, request.Location);
                characterFound.Origin = CheckUpdateProperty(characterFound.Origin, request.Origin);
                characterFound.Abilities = CheckUpdateProperty(characterFound.Abilities, request.Abilities);
                characterFound.AbilitiesOrigin = CheckUpdateProperty(characterFound.AbilitiesOrigin, request.AbilitiesOrigin);
                characterFound.Aliases = CheckUpdateProperty(characterFound.Aliases, request.Aliases);
                characterFound.Status = CheckUpdateProperty(characterFound.Status, request.Status);
                var numOfChanges = await _dbContext.SaveChangesAsync();
                return numOfChanges == 1;
            }
            return false;
        }

        public async Task<bool> DeleteCharacterAsync(int id)
        {
            var character = await _dbContext.Characters.FindAsync(id);
            _dbContext.Characters.Remove(character);
            return await _dbContext.SaveChangesAsync() == 1;
        }

        private string ReformatName(string name) {
            var result = String.Concat(name.Split(' ', '-')).ToLower();
            return result;
        }

        private string CheckUpdateProperty(string from, string to) {
            return String.IsNullOrEmpty(to.Trim()) ? from : to.Trim();
        }
    }
}