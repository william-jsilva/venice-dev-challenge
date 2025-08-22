using MediatR;

namespace Venice.Orders.Application.Orders.GetOrder;

/// <summary>
/// Query for getting an order by ID.
/// </summary>
/// <remarks>
/// This query is used to retrieve order details by its ID.
/// </remarks>
public class GetOrderQuery : IRequest<GetOrderResult?>
{
    /// <summary>
    /// Order ID to retrieve
    /// </summary>
    public Guid Id { get; set; }
    
    public GetOrderQuery(Guid id)
    {
        Id = id;
    }
}

