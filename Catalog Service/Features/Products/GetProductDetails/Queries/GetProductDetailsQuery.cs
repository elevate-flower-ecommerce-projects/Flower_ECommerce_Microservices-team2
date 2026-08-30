using Catalog_Service.Common.BaseHandler;
using Catalog_Service.Common.Enums;
using Catalog_Service.Common.ResultPattern;
using Catalog_Service.Features.Products.GetProductDetails.Dto;
using Catalog_Service.Features.Products.Shared;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Features.Products.GetProductDetails.Queries;


public sealed record GetProductDetailsQuery(
    long ProductId,
    long? StoreId = null
) : IRequest<RequestResult<ProductDetailsDto>>;


public sealed class GetProductDetailsQueryHandler(
    BaseRequestParameters baseParameters,
    IStoreAvailabilityResolver availabilityResolver)
    : BaseRequestHandler<GetProductDetailsQuery, RequestResult<ProductDetailsDto>>(baseParameters)
{
    private readonly IStoreAvailabilityResolver _availabilityResolver = availabilityResolver;

    public override async Task<RequestResult<ProductDetailsDto>> Handle(
        GetProductDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var requestAborted = _cancellationTokenCapture.Token;


        var product = await _context.Products
            .AsNoTracking()
            .Where(p => p.Id == request.ProductId)
            .ProjectToType<ProductDetailsProjection>()
            .FirstOrDefaultAsync(requestAborted);

        if (product is null)
        {
            return RequestResult<ProductDetailsDto>.Failure(ErrorCode.ProductNotFound);
        }


        if (product.IsArchived)
        {
            return RequestResult<ProductDetailsDto>.Failure(ErrorCode.ProductNoLongerAvailable);
        }

        var availability = await _availabilityResolver.ResolveAsync(
            product.Id,
            request.StoreId,
            product.Status);

        var details = new ProductDetailsDto(
            Id: product.Id,
            Name: product.Name,
            Description: product.Description,
            Status: product.Status,
            CategoryId: product.CategoryId,
            CategoryName: product.CategoryName,
            Images: product.Images
                .Select((image, index) => image with { IsPrimary = index == 0 })
                .ToList(),
            Pricing: BuildPricing(product, availability),
            Availability: availability.Adapt<ProductAvailabilityDto>(),
            Includes: product.Includes,
            Occasions: product.Occasions);

        return RequestResult<ProductDetailsDto>.Success(details);
    }

    private static ProductPricingDto BuildPricing(
        ProductDetailsProjection product,
        StoreAvailabilityResolution availability)
    {
        var basePrice = availability.PriceOverride ?? product.Price;

        var hasActiveDiscount = product.DiscountPercentage is > 0
            && (product.DiscountStartAt is null || product.DiscountStartAt <= DateTime.UtcNow)
            && (product.DiscountEndAt is null || product.DiscountEndAt >= DateTime.UtcNow);

        var discountedPrice = hasActiveDiscount
            ? Math.Round(basePrice * (1 - (product.DiscountPercentage!.Value / 100m)), 2)
            : (decimal?)null;

        return new ProductPricingDto(
            OriginalPrice: basePrice,
            DiscountPercentage: hasActiveDiscount ? product.DiscountPercentage : null,
            DiscountedPrice: discountedPrice,
            EffectivePrice: discountedPrice ?? basePrice,
            HasActiveDiscount: hasActiveDiscount,
            DiscountStartAt: product.DiscountStartAt,
            DiscountEndAt: product.DiscountEndAt,
            IsStoreScoped: availability.IsStoreResolved);
    }
}
