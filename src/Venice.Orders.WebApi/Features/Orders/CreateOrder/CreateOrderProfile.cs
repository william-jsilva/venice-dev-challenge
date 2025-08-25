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
        // Mapeamento do WebApi CreateOrderRequest para Application CreateOrderCommand
        CreateMap<CreateOrderRequest, CreateOrderCommand>()
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

        // Mapeamento dos itens do WebApi para Application (resolvendo conflito de namespace)
        CreateMap<OrderItemRequest, Venice.Orders.Application.Dtos.OrderItemRequest>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice));

        // Mapeamento do resultado para resposta da API
        CreateMap<CreateOrderResult, CreateOrderResponse>();
    }
}

