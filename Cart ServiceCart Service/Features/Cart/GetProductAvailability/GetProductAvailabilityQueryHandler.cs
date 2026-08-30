using Cart_ServiceCart_Service.Common.BaseHandler;
using Cart_ServiceCart_Service.Common.ResultPattern;

namespace Cart_ServiceCart_Service.Features.Cart.GetProductAvailability;

public sealed class GetProductAvailabilityQueryHandler(
    IProductCatalogClient productCatalogClient,
    BaseParameters baseParameters)
    : BaseHandler<GetProductAvailabilityQuery, RequestResult<CatalogProduct>>(baseParameters)
{
    private readonly IProductCatalogClient _productCatalogClient = productCatalogClient;

    public override async Task<RequestResult<CatalogProduct>> Handle(
        GetProductAvailabilityQuery request,
        CancellationToken cancellationToken)
    {
        if (request.ProductId <= 0)
            return RequestResult<CatalogProduct>.Failure(ErrorCode.InvalidInput, "Product id must be greater than zero.");

        try
        {
            var lookup = await _productCatalogClient.GetProductAsync(request.ProductId, cancellationToken);

            return lookup.Status switch
            {
                CatalogProductLookupStatus.Found when lookup.Product is { IsActive: true, UnitPrice: >= 0m } product =>
                    RequestResult<CatalogProduct>.Success(product),

                CatalogProductLookupStatus.Found =>
                    RequestResult<CatalogProduct>.Failure(ErrorCode.Conflict, "This product is not currently available."),

                CatalogProductLookupStatus.NotFound =>
                    RequestResult<CatalogProduct>.Failure(ErrorCode.NotFound, "Product was not found."),

                _ => RequestResult<CatalogProduct>.Failure(ErrorCode.ServiceUnavailable, "Product availability could not be verified. Please try again.")
            };
        }
        catch (Exception) when (!cancellationToken.IsCancellationRequested)
        {
            return RequestResult<CatalogProduct>.Failure(ErrorCode.ServiceUnavailable, "Product availability could not be verified. Please try again.");
        }
    }
}
