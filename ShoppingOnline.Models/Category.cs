using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ShoppingOnline.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)] //valicaiton 
        [DisplayName("Category Name")]
        public string Name { get; set; }
        [DisplayName("Display Order")]
        [Range(1, 100, ErrorMessage = "Display Order must be between 1-100")]  //valication messages is from 1 to 100 then go to create page
        public int DisplayOrder { get; set; }
    }
}
