namespace BeerStore.Models
{
    public class Country
    {
        public int Id { get; set; } // Primary key
        public string Name { get; set; } // Country name
        public ICollection<Beer> Beers { get; set; } // Navigation property
    }
}
