using Catalog_Service.Common.ResultPattern;
using MediatR;

namespace Catalog_Service.Features.Products;

public sealed record GetProductsQuery(long? CategoryId)
    : IRequest<RequestResult<IEnumerable<ProductSummaryDto>>>;
