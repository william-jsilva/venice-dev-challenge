using Venice.Orders.Application.Orders.GetOrder;
using AutoMapper;

namespace Venice.Orders.WebApi.Features.Orders.GetOrder;

/// <summary>
/// Profile for mapping between Application and API GetOrder response models
/// </summary>
public class GetOrderProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for GetOrder feature
    /// </summary>
    public GetOrderProfile()
    {
        CreateMap<GetOrderResult, GetOrderResponse>();
    }
}

