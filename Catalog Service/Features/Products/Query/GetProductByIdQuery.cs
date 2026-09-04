using Catalog_Service.Common.ResultPattern;
using Catalog_Service.Features.Products.Dto;
using MediatR;

namespace Catalog_Service.Features.Products.Query;

public sealed record GetProductByIdQuery(long ProductId) : IRequest<RequestResult<ProductDetailDto>>;
