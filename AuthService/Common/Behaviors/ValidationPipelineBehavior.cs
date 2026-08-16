using AuthService.Common.Exceptions;
using FluentValidation;
using MediatR;
using System.Text;

namespace AuthService.Common.Behaviors
{
    public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                var validationResults = await Task.WhenAll(
                    _validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

                if (validationResults.Any(x => !x.IsValid))
                {
                    var errors = new StringBuilder();
                    foreach (var validationResult in validationResults)
                    {
                        if (!validationResult.IsValid)
                        {
                            errors.AppendLine(string.Join(Environment.NewLine, validationResult.Errors.Select(x => x.ErrorMessage)));
                        }
                    }
                    throw new RequestValidationException(errors.ToString());
                }
                return await next();
            }

            return await next();
        }
    }
}
