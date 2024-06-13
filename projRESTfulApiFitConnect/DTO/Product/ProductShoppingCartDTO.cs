namespace projRESTfulApiFitConnect.DTO.Product
{
    public class ProductShoppingCartDTO
    {
        public int shoppingCartId { get; set; }
        public int productId { get; set; }
        public string productName { get; set; } = null!;
        public decimal quantity { get; set; }
        public decimal unitPrice { get; set; }
        public string productImage { get; set; } = null!;
    }
}
