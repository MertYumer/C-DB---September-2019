namespace FastFood.Web.MappingConfiguration
{
    using AutoMapper;
    using ViewModels.Categories;
    using Models;
    using ViewModels.Employees;
    using ViewModels.Positions;
    using ViewModels.Items;
    using ViewModels.Orders;

    public class FastFoodProfile : Profile
    {
        public FastFoodProfile()
        {
            //Positions
            this.CreateMap<CreatePositionInputModel, Position>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.PositionName));

            this.CreateMap<Position, PositionsAllViewModel>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.Name));

            //Employees
            this.CreateMap<Position, RegisterEmployeeViewModel>()
                .ForMember(x => x.PositionName, y => y.MapFrom(p => p.Name));

            this.CreateMap<RegisterEmployeeInputModel, Employee>();

            this.CreateMap<Employee, EmployeesAllViewModel>()
                .ForMember(x => x.Position, y => y.MapFrom(s => s.Position.Name));

            //Categories
            this.CreateMap<CreateCategoryInputModel, Category>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.CategoryName));

            this.CreateMap<Category, CategoryAllViewModel>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.Name));

            //Items
            this.CreateMap<Category, CreateItemViewModel>()
                .ForMember(x => x.CategoryId, y => y.MapFrom(c => c.Id));

            this.CreateMap<CreateItemInputModel, Item>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.Name));

            this.CreateMap<Item, ItemsAllViewModels>()
                .ForMember(x => x.Category, y => y.MapFrom(s => s.Category.Name));

            //Orders
            this.CreateMap<CreateOrderInputModel, Order>()
                .ForMember(x => x.Customer, y => y.MapFrom(c => c.Customer));

            this.CreateMap<Order, OrderAllViewModel>()
                .ForMember(x => x.OrderId, y => y.MapFrom(s => s.Id))
                .ForMember(x => x.Employee, y => y.MapFrom(o => o.Employee.Name))
                .ForMember(x => x.DateTime, y => y.MapFrom(o => o.DateTime.ToString("g")));
        }
    }
}
