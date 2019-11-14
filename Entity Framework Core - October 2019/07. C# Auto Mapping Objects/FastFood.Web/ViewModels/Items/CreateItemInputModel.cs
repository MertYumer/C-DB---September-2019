namespace FastFood.Web.ViewModels.Items
{
    using System.ComponentModel.DataAnnotations;

    public class CreateItemInputModel
    {
        [Required]
        [MinLength(2), MaxLength(30)]
        public string Name { get; set; }

        [Range(typeof(decimal), "0.50", "100.00")]
        public decimal Price { get; set; }

        public string CategoryName { get; set; }
    }
}
