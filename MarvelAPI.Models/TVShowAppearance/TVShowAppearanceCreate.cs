using System.ComponentModel.DataAnnotations;

namespace MarvelAPI.Models.TVShowAppearance
{
    public class TVShowAppearanceCreate
    {
        [Required]
        public int CharacterId { get; set; }
        
        [Required]
        public int TVShowId { get; set; }

    }
}