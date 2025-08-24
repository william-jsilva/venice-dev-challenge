using MediatR;

namespace Venice.Orders.Application.Orders.GetAllOrders;

public record GetAllOrdersQuery : IRequest<IEnumerable<GetAllOrdersResult>>;
