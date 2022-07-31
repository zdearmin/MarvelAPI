using MarvelAPI.Data;
using MarvelAPI.Data.Entities;
using MarvelAPI.Models.TVShows;
using MarvelAPI.Models.TVShowAppearance;
using MarvelAPI.Models.Characters;
using Microsoft.EntityFrameworkCore;

namespace MarvelAPI.Services.TVShowsService
{
    public class TVShowService : ITVShowService
    {
        private readonly AppDbContext _dbContext;
        public TVShowService (AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> CreateTVShowAsync (TVShowCreate model)
        {
            var tvShowCreate = new TVShowsEntity
            {
                Title = model.Title,
                ReleaseYear = model.ReleaseYear,
                Seasons = model.Seasons
            };

            foreach (var tvS in await _dbContext.TVShows.ToListAsync())
            {
                if (ReformatTitle(tvS) == ReformatTitle(tvShowCreate))
                {
                    return false;
                }
            }

            _dbContext.TVShows.Add(tvShowCreate);
            var numberOfChanges = await _dbContext.SaveChangesAsync();
            return numberOfChanges == 1;
        }

        public async Task<IEnumerable<TVShowListItem>> GetAllTVShowsAsync()
        {
            var tvShowList = await _dbContext.TVShows.Select(tvS => new TVShowListItem
            {
                Id = tvS.Id,
                Title = tvS.Title
            })
            .OrderBy(t => t.Title)
            .ToListAsync();
            return tvShowList;
        }

        public async Task<TVShowDetail> GetTVShowByIdAsync(int tvShowId)
        {
            var tvShowFound = await _dbContext.TVShows
            .Include(x => x.Characters)
            .ThenInclude(y => y.Character)
            .FirstOrDefaultAsync(tv => tv.Id == tvShowId);

            if (tvShowFound != null)
            {
                return new TVShowDetail
                {
                    Id = tvShowFound.Id,
                    Title = tvShowFound.Title,
                    ReleaseYear = (int)tvShowFound.ReleaseYear,
                    Seasons = (int)tvShowFound.Seasons,
                    Characters = tvShowFound.Characters.Select(c => new CharacterListItem
                    {
                        Id = c.CharacterId,
                        FullName = c.Character.FullName
                    })
                    .OrderBy(n => n.FullName)
                    .ToList()
                };
            }
            return null;
        }

        public async Task<IEnumerable<TVShowListItem>> GetTVShowsByTitleAsync(string tvShowTitle)
        {
            var tvShowsFound = await _dbContext.TVShows
            .Where(
                t => t.Title != null &&
                t.Title.ToLower().Contains(tvShowTitle.ToLower())
            )
            .Select(
                tv => new TVShowListItem
                {
                    Id = tv.Id,
                    Title = tv.Title
                })
            .OrderBy(t => t.Title)
            .ToListAsync();
            return tvShowsFound;
        }

        public async Task<bool> UpdateTVShowAsync(int tvShowId, TVShowUpdate request)
        {
            var tvShowFound = await _dbContext.TVShows.FindAsync(tvShowId);

            if (tvShowFound is null)
            {
                return false;
            }

            var tvShowUpdate = new TVShowsEntity
            {
                Title = request.Title
            };

            foreach (var tvS in await _dbContext.TVShows.ToListAsync())
            {
                if (ReformatTitle(tvS) == ReformatTitle(tvShowUpdate))
                {
                    return false;
                }
            }

            tvShowFound.Title = request.Title;
            tvShowFound.ReleaseYear = request.ReleaseYear;
            tvShowFound.Seasons = request.Seasons;
            var numberOfChanges = await _dbContext.SaveChangesAsync();
            return numberOfChanges == 1;
        }

        public async Task<bool> DeleteTVShowAsync(int tvShowId)
        {
            var tvShowDelete = await _dbContext.TVShows.FindAsync(tvShowId);

            if (tvShowDelete is null) 
            {
                return false;
            }
            
            _dbContext.TVShows.Remove(tvShowDelete);
            return await _dbContext.SaveChangesAsync() == 1;
        }

        private string ReformatTitle(TVShowsEntity tvShow)
        {
            var tvShowReform = String.Concat(tvShow.Title.Split(' ', '-')).ToLower();
            return tvShowReform;
        }
    }
}