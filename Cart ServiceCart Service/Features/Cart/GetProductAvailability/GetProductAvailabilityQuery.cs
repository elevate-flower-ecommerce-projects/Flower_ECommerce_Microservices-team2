using Cart_ServiceCart_Service.Common.ResultPattern;
using MediatR;

namespace Cart_ServiceCart_Service.Features.Cart.GetProductAvailability;

public sealed record GetProductAvailabilityQuery(long ProductId)
    : IRequest<RequestResult<CatalogProduct>>;
