using FluentValidation;

namespace Catalog_Service.Features.Catalog.GetProductCatalogeList.Query;

public class GetProductCatalogListQueryValidator : AbstractValidator<GetProductCatalogListQuery>
{
    public GetProductCatalogListQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("PageNumber must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("PageSize must be between 1 and 100.");

        RuleFor(x => x.categoryId)
            .GreaterThan(0)
            .When(x => x.categoryId.HasValue)
            .WithMessage("categoryId must be a positive number.");

        RuleFor(x => x.occasionId)
            .GreaterThan(0)
            .When(x => x.occasionId.HasValue)
            .WithMessage("occasionId must be a positive number.");

        RuleFor(x => x.storeId)
            .GreaterThan(0)
            .When(x => x.storeId.HasValue)
            .WithMessage("storeId must be a positive number.");
    }
}