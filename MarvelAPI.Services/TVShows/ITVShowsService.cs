using MarvelAPI.Data.Entities;
using MarvelAPI.Models.TVShows;

namespace MarvelAPI.Services.TVShowsService
{
    public interface ITVShowService
    {
        Task<bool> CreateTVShowAsync(TVShowCreate model);
        Task<IEnumerable<TVShowListItem>> GetAllTVShowsAsync();
        Task<TVShowDetail> GetTVShowByIdAsync(int tvShowId);
        Task<IEnumerable<TVShowListItem>> GetTVShowsByTitleAsync(string tvShowTitle);
        Task<bool> UpdateTVShowAsync(int tvShowId, TVShowUpdate request);
        Task<bool> DeleteTVShowAsync(int tvShowId);
    }
}