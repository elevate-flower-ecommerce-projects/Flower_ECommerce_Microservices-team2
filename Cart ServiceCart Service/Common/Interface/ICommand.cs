using MediatR;
namespace Cart_ServiceCart_Service.Common.Interface;

public interface ICommand<TResponse> : IRequest<TResponse>
{
}
