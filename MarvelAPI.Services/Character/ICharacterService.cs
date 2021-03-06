using MarvelAPI.Data.Entities;
using MarvelAPI.Models.Characters;

namespace MarvelAPI.Services.Character
{
    public interface ICharacterService
    {
        Task<bool> CreateCharacterAsync(CharacterCreate model);
        Task<IEnumerable<CharacterListItem>> GetAllCharactersAsync();
        Task<IEnumerable<CharacterAbilities>> GetCharactersByAbilityAsync(string ability);
        Task<IEnumerable<CharacterAliases>> GetCharactersByAliasesAsync(string aliases);
        Task<CharacterDetail> GetCharacterByIdAsync(int id);
        Task<bool> UpdateCharacterAsync(int characterId, CharacterUpdate request);
        Task<bool> DeleteCharacterAsync(int id);
    }
}