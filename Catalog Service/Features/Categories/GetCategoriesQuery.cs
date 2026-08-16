using MediatR;

namespace Catalog_Service.Features.Categories;

public sealed record GetCategoriesQuery : IRequest<IEnumerable<CategoryDto>>;
