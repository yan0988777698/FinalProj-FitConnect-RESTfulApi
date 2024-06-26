using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using projRESTfulApiFitConnect.DTO.Product;
using projRESTfulApiFitConnect.Models;

namespace projRESTfulApiFitConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTrackController : ControllerBase
    {
        private readonly GymContext _db;
        private readonly IWebHostEnvironment _env;
        public ProductTrackController(GymContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        //取得所有追蹤中的商品
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTrackingProduct(int id)
        {
            var trackingProducts = await _db.TproductTracks.Include(x => x.Product).Where(x => x.MemberId == id).ToListAsync();
            List<ProductTrackingDTO> productTrackingDTOs = new List<ProductTrackingDTO>();
            trackingProducts.ForEach(item =>
            {
                ProductTrackingDTO productTrackingDTO = new ProductTrackingDTO
                {
                    productId = item.ProductId,
                    productName = item.Product.ProductName
                };
                productTrackingDTOs.Add(productTrackingDTO);
            });
            return Ok(productTrackingDTOs);
        }


    }
}
