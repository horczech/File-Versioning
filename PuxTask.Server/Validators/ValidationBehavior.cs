using FluentValidation;
using MediatR;

namespace PuxTask.Server.Validators;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IValidator<TRequest>? _validator;

    public ValidationBehavior(IValidator<TRequest>? validator = null) {
        _validator = validator;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
        if (_validator is not null)
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

        return await next();
    }
}