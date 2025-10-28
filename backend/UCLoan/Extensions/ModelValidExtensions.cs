using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace UCLoan.Extensions
{
    public static class ModelValidationExtensions
    {
        public static IServiceCollection AddPortugueseValidationResponses(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var requiredPattern = new Regex(@"^The (.+) field is required\.$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

                    var errors = context.ModelState
                        .Where(kvp => kvp.Value.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors
                                .Select(e =>
                                {
                                    var msg = e.ErrorMessage;
                                    if (string.IsNullOrWhiteSpace(msg) && e.Exception != null)
                                        msg = e.Exception.Message;

                                    if (string.IsNullOrWhiteSpace(msg))
                                        return "Entrada inválida.";

                                    var m = requiredPattern.Match(msg);
                                    if (m.Success)
                                    {
                                        var field = m.Groups[1].Value;
                                        return $"O campo {field} é obrigatório.";
                                    }

                                    if (msg.EndsWith("invalid.", System.StringComparison.OrdinalIgnoreCase))
                                        return msg.Replace("invalid.", "inválido.", System.StringComparison.OrdinalIgnoreCase);

                                    return msg;
                                })
                                .ToArray()
                        );

                    var problem = new ValidationProblemDetails(errors)
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                        Title = "Um ou mais erros de validação ocorreram."
                    };

                    return new BadRequestObjectResult(problem);
                };
            });

            return services;
        }
    }
}