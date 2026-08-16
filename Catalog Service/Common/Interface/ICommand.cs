using MediatR;

namespace Catalog_Service.Common.Interface
{
    public interface ICommand<TResponse> : IRequest<TResponse>
    {
    }
}
