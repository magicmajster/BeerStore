namespace BeerStore.Models
{
    public class ShoppingCartItem
    {
        public int Id { get; set; } // Primary Key
        public int BeerId { get; set; } // Foreign Key
        public Beer Beer { get; set; } // Navigation Property
        public int Quantity { get; set; }
    }
}
