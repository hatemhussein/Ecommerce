using KASHOP.BL.Service;
using KASHOP.DAL.DTO.Request;
using KASHOP.PL.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _brandService;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public BrandsController(IBrandService brandService, IStringLocalizer<SharedResources> localizer)
        {
            _brandService = brandService;
            _localizer = localizer;
        }

        [HttpPost("")]
        [Authorize]
        public async Task<IActionResult> Create(BrandRequest request)
        {
            var response = await _brandService.CreateBrand(request);

            return Ok(new
            {
                message = _localizer["Success"].Value,
                response
            });
        }

        [HttpGet("")]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var response = await _brandService.GetBrands();
            return Ok(new
            {
                data = response,
                _localizer["Success"].Value
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var brand = await _brandService.GetBrand(b => b.Id == id);
            if (brand == null)
            {
                return NotFound(new { message = _localizer["NotFound"].Value });
            }
            return Ok(brand);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, BrandRequest request)
        {
            var updated = await _brandService.UpdateBrand(id, request);
            if (!updated) return BadRequest();
            return Ok(new { message = _localizer["Success"].Value });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _brandService.DeleteBrand(id);
            if (!deleted)
            {
                return NotFound(new { message = _localizer["NotFound"].Value });
            }
            return Ok(new { message = _localizer["Success"].Value });
        }
    }
}
