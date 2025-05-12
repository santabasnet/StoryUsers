using System.ComponentModel.DataAnnotations;

namespace StoryUserApi.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Vendor { get; set; }
    }
}
