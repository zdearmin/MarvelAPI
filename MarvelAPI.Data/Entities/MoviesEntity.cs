using System.ComponentModel.DataAnnotations;

namespace MarvelAPI.Data.Entities
{
    public class MoviesEntity
    {
        public MoviesEntity() {
            Characters = new List<MovieAppearanceEntity>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }

        public int? ReleaseYear { get; set; }

        public virtual IEnumerable<MovieAppearanceEntity> Characters { get; set; }
    }
}