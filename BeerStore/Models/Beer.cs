namespace BeerStore.Models
{
    public class Beer
    {
        public int Id { get; set; } // Primary key
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }

        // Foreign key for Country
        public int CountryId { get; set; }
        public Country Country { get; set; }
    }
}
