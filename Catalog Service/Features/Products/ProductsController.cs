using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.Features.Products;
[ApiController]
[Route("products")]
public sealed partial class ProductsController(IMediator mediator) : ControllerBase;
