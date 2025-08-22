using Venice.Orders.Application.Orders.CreateOrder;
using Venice.Orders.Application.Dtos;
using AutoMapper;

namespace Venice.Orders.WebApi.Features.Orders.CreateOrder;

/// <summary>
/// Profile for mapping between Application and API CreateOrder response and request models
/// </summary>
public class CreateOrderProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for CreateOrder feature
    /// </summary>
    public CreateOrderProfile()
    {
        CreateMap<CreateOrderRequest, CreateOrderCommand>()
            .ForCtorParam("request", opt => opt.MapFrom(src => new Venice.Orders.Application.Dtos.CreateOrderRequest
            {
                CustomerId = src.CustomerId,
                Items = src.Items.Select(item => new Venice.Orders.Application.Dtos.OrderItemRequest
                {
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList()
            }));

        CreateMap<CreateOrderResult, CreateOrderResponse>();
    }
}

