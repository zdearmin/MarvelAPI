using Microsoft.AspNetCore.Mvc;
using MarvelAPI.Services.MoviesService;
using MarvelAPI.Models.Movies;

namespace MarvelAPI.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly IMoviesService _service;

        public MoviesController(IMoviesService service)
        {
            _service = service;
        }

        [HttpPost]
        [ProducesResponseType(typeof(MoviesCreate), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateMoviesAsync([FromBody] MoviesCreate model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (await _service.CreateMoviesAsync(model))
            {
                return Ok("The movie has been created and added to the database.");
            }
            return BadRequest("Sorry, the movie could not be created.");
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MoviesListItem>),200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllMoviesAsync()
        {
            return Ok(await _service.GetAllMoviesAsync());
        }

        [HttpGet("{moviesId:int}")]
        [ProducesResponseType(typeof(MoviesDetail), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMovieByIdAsync([FromRoute] int moviesId)
        {
            var movie = await _service.GetMovieByIdAsync(moviesId);
            if (movie == default)
            {
                return NotFound("Sorry, the movie you requested could not be found.");
            }
            return Ok(movie);
        }

        [HttpGet("{moviesTitle}")]
        [ProducesResponseType(typeof(MoviesDetail), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMovieByTitleAsync([FromRoute] string moviesTitle)
        {
            var movie = await _service.GetMovieByTitleAsync(moviesTitle);
            return Ok(movie);
        }

        [HttpPut("{moviesId:int}")]
        [ProducesResponseType(typeof(MoviesUpdate), 200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateMoviesAsync([FromRoute] int moviesId, [FromBody] MoviesUpdate request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (await _service.UpdateMoviesAsync(moviesId, request))
            {
                return Ok("The movie has been updated successfully.");
            }
            return BadRequest("Sorry, the movie could not be updated.");
        }

        [HttpDelete("{moviesId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteMoviesAsync([FromRoute] int moviesId)
        {
            return await _service.DeleteMoviesAsync(moviesId) ?
            Ok($"The movie with ID {moviesId} was successfully deleted."):
            BadRequest($"The movie with ID {moviesId} could not be deleted.");
        }
    }
}