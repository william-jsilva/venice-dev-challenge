using Venice.Orders.Application.Orders.CreateOrder;
using Venice.Orders.Application.Orders.GetOrder;
using Venice.Orders.WebApi.Features.Orders.CreateOrder;
using Venice.Orders.WebApi.Features.Orders.GetOrder;
using Venice.Orders.WebApi.Common;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Venice.Orders.WebApi.Features.Orders;

/// <summary>
/// Controller for managing order operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class OrdersController(IMediator mediator, IMapper mapper) : BaseController
{
    /// <summary>
    /// Create a new order
    /// </summary>
    /// <param name="request">The order creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created order details</returns>
    [HttpPost]
    //[Authorize]
    [ProducesResponseType(typeof(CreateOrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var validator = new CreateOrderRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = mapper.Map<CreateOrderCommand>(request);
        var response = await mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetOrder), new { id = response.Id }, mapper.Map<CreateOrderResponse>(response));
    }

    /// <summary>
    /// Get a specific order by ID
    /// </summary>
    /// <param name="id">The unique identifier of the order</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The order details</returns>
    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(GetOrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetOrder([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetOrderQuery(id);
        var response = await mediator.Send(query, cancellationToken);

        if (response == null)
            return NotFound();

        return Ok(mapper.Map<GetOrderResponse>(response));
    }
}
