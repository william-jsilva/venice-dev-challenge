using Venice.Orders.Application.Orders.GetAllOrders;
using AutoMapper;

namespace Venice.Orders.WebApi.Features.Orders.GetAllOrders;

/// <summary>
/// Profile for mapping between Application and API GetAllOrders response models
/// </summary>
public class GetAllOrdersProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for GetAllOrders feature
    /// </summary>
    public GetAllOrdersProfile()
    {
        CreateMap<GetAllOrdersResult, GetAllOrdersResponse>();
    }
}
