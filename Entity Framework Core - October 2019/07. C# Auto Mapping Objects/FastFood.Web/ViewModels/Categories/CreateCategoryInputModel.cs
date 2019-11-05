namespace FastFood.Web.ViewModels.Categories
{
    using System.ComponentModel.DataAnnotations;

    public class CreateCategoryInputModel
    {
        [Required]
        [MinLength(2), MaxLength(30)]
        public string CategoryName { get; set; }
    }
}
