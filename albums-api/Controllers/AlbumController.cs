using albums_api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace albums_api.Controllers
{
    [Route("albums")]
    [ApiController]
    public class AlbumController : ControllerBase
    {
        // GET: api/album
        [HttpGet]
        public IActionResult Get()
        {
            var albums = Album.GetAll();

            return Ok(albums);
        }

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
