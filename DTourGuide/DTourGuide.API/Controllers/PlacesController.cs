using AutoMapper;
using DTourGuide.Application.DTOs;
using DTourGuide.Application.Services;
using DTourGuide.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTourGuide.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlacesController : ControllerBase
    {
        private readonly IPlaceService _service;
        public PlacesController(IPlaceService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlaceListDto>>> Get([FromQuery] Category? category, [FromQuery] string? search)
            => Ok(await _service.GetAsync(category, search));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PlaceDetailDto>> GetById(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            return dto is null ? NotFound() : Ok(dto);
        }

        [HttpPost, Authorize(AuthenticationSchemes = Auth.BasicAuthHandler.SchemeName, Roles = "Admin")]
        public async Task<ActionResult<PlaceDetailDto>> Post(PlaceDetailDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}"), Authorize(AuthenticationSchemes = Auth.BasicAuthHandler.SchemeName, Roles = "Admin")]
        public async Task<ActionResult<PlaceDetailDto>> Put(int id, PlaceDetailDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return updated is null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id:int}"), Authorize(AuthenticationSchemes = Auth.BasicAuthHandler.SchemeName, Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }

        [HttpPost("itinerary")]
        public async Task<ActionResult<IEnumerable<PlaceListDto>>> Itinerary([FromBody] ItineraryRequest req)
            => Ok(await _service.BuildItineraryAsync(req.PlaceIds ?? Array.Empty<int>(), req.StartLat, req.StartLng));

        public class ItineraryRequest
        {
            public IEnumerable<int>? PlaceIds { get; set; }
            public double? StartLat { get; set; }
            public double? StartLng { get; set; }
        }
    }
}
