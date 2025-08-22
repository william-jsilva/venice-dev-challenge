using AutoMapper;
using Venice.Orders.Application.Dtos;
using Venice.Orders.Common.Models;

namespace Venice.Orders.Application.Orders.CreateOrder;

/// <summary>
/// AutoMapper profile for CreateOrder feature
/// </summary>
public class CreateOrderProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateOrderProfile"/> class
    /// </summary>
    public CreateOrderProfile()
    {
        CreateMap<CreateOrderRequest, CreateOrderCommand>();
        
        CreateMap<OrderItemRequest, OrderItemResult>()
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Quantity * src.UnitPrice));
    }
}
