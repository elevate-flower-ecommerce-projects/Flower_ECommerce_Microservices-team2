using Catalog_Service.Features.Products.GetProductDetails.Queries;
using FluentValidation;

namespace Catalog_Service.Features.Products.GetProductDetails.Validation;

public sealed class GetProductDetailsQueryValidator : AbstractValidator<GetProductDetailsQuery>
{
    public GetProductDetailsQueryValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("productId must be a positive number.");

        RuleFor(x => x.StoreId)
            .GreaterThan(0)
            .When(x => x.StoreId.HasValue)
            .WithMessage("storeId must be a positive number.");
    }
}
