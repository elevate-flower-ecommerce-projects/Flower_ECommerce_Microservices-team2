using MediatR;

namespace AuthService.Common.Interface
{
    public interface ICommand<TResponse> : IRequest<TResponse>
    {
    }
}
