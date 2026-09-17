using albums_api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

/// <summary>
/// Controller for managing albums.
/// </summary>
namespace albums_api.Controllers
{
    /// <summary>
    /// Handles HTTP requests related to albums.
    /// </summary>
    [Route("albums")]
    [ApiController]
    public class AlbumController : ControllerBase
    {
        /// <summary>
        /// Retrieves all albums from the database.
        /// </summary>
        /// <returns>A list of all albums.</returns>
        // GET: api/album
        [HttpGet]
        public IActionResult Get()
        {
            var albums = Album.GetAll();

            return Ok(albums);
        }

        /// <summary>
        /// Retrieves a specific album by its ID.
        /// </summary>
        /// <param name="id">The ID of the album to retrieve.</param>
        /// <returns>The album with the specified ID, or a 404 Not Found if it doesn't exist.</returns>
        // GET api/<AlbumController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var album = Album.GetById(id);
            if (album == null)
            {
                return NotFound();
            }

            return Ok(album);
        }

        /// <summary>
        /// Retrieves all albums sorted by the specified field.
        /// </summary>
        /// <param name="sortBy">The field to sort by (title, artist, or price).</param>
        /// <returns>A list of albums sorted by the specified field.</returns>
        // GET: albums/sorted?sortBy=title|artist|price
        [HttpGet("sorted")]
        public IActionResult GetSorted([FromQuery] string sortBy = "title")
        {
            var albums = Album.GetAll();

            var sortedAlbums = sortBy.ToLower() switch
            {
                "title" => albums.OrderBy(album => album.Title),
                "artist" => albums.OrderBy(album => album.Artist),
                "price" => albums.OrderBy(album => album.Price),
                _ => null
            };

            if (sortedAlbums == null)
            {
                return BadRequest("sortBy must be title, artist, or price.");
            }

            return Ok(sortedAlbums);
        }

    }

    
}
