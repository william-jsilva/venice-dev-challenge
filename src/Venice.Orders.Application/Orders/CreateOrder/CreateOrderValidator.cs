using FluentValidation;
using Venice.Orders.Application.Dtos;

namespace Venice.Orders.Application.Orders.CreateOrder;

/// <summary>
/// Validator for CreateOrderCommand
/// </summary>
public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateOrderValidator"/> class
    /// </summary>
    public CreateOrderValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Order must have at least one item");

        RuleForEach(x => x.Items)
            .SetValidator(new OrderItemValidator());
    }
}

/// <summary>
/// Validator for OrderItem
/// </summary>
public class OrderItemValidator : AbstractValidator<OrderItemRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrderItemValidator"/> class
    /// </summary>
    public OrderItemValidator()
    {
        RuleFor(x => x.ProductName)
            .NotEmpty()
            .WithMessage("Product name is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0)
            .WithMessage("Unit price must be greater than 0");
    }
}
