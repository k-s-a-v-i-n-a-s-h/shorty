using FluentValidation;

namespace Shorty.Management.Api.Filters;

public class ValidationFilter<T> : IEndpointFilter
{
    private readonly IValidator<T> _validator;

    public ValidationFilter(IValidator<T> validator)
    {
        _validator = validator;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var argument = context.Arguments.OfType<T>().FirstOrDefault()!;

        var validationResult = await _validator.ValidateAsync(argument);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(
                statusCode: StatusCodes.Status422UnprocessableEntity,
                title: "Validation Error",
                detail: "One or more validation errors occurred",
                errors: validationResult.ToDictionary()
            );
        }

        return await next(context);
    }
}