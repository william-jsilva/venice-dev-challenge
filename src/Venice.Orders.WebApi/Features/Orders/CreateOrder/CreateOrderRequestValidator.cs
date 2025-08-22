using FluentValidation;

namespace Venice.Orders.WebApi.Features.Orders.CreateOrder;

/// <summary>
/// Validator for CreateOrderRequest that defines validation rules for order creation.
/// </summary>
public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    /// <summary>
    /// Initializes a new instance of the CreateOrderRequestValidator with defined validation rules.
    /// </summary>
    public CreateOrderRequestValidator()
    {
        RuleFor(request => request.CustomerId)
            .NotEmpty().WithMessage("CustomerId must not be empty.");

        RuleFor(request => request.Items)
            .NotEmpty().WithMessage("Order must have at least one item.");

        RuleForEach(request => request.Items)
            .SetValidator(new OrderItemRequestValidator());
    }
}

/// <summary>
/// Validator for OrderItemRequest
/// </summary>
public class OrderItemRequestValidator : AbstractValidator<OrderItemRequest>
{
    /// <summary>
    /// Initializes a new instance of the OrderItemRequestValidator with defined validation rules.
    /// </summary>
    public OrderItemRequestValidator()
    {
        RuleFor(item => item.ProductName)
            .NotEmpty().WithMessage("Product name is required.");

        RuleFor(item => item.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0.");

        RuleFor(item => item.UnitPrice)
            .GreaterThan(0).WithMessage("Unit price must be greater than 0.");
    }
}

