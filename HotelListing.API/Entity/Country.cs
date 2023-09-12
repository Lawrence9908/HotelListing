namespace HotelListing.API.Entity
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        //One to Many Relationship
        public virtual IList<Hotel> Hotels { get; set; }
    }
}