namespace ProductShop.Dtos
{
    using System.Collections.Generic;

    public class UserWithSalesDto
    {
        public UserWithSalesDto()
        {
            SoldProducts = new List<ProductDto>();
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public List<ProductDto> SoldProducts { get; set; }
    }
}
