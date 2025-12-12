using FluentResults;
using ListingWebApp.Api.Dto.Common;
using ListingWebApp.Common.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ListingWebApp.Api.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this Result<T> result, bool created = false)
    {
        if (result.IsSuccess)
        {
            var payload = new Response<T>("success", result.Value);
            return created
                ? new ObjectResult(payload) { StatusCode = StatusCodes.Status201Created }
                : new OkObjectResult(payload);
        }

        if (result.Errors.Any(e => e is NotFoundError))
            return new NotFoundObjectResult(new Response<string>("error", string.Empty, result.Errors.First().Message));

        return new BadRequestObjectResult(new Response<string>("error", string.Empty,
            string.Join("; ", result.Errors.Select(e => e.Message))));
    }

    public static IActionResult ToActionResult(this Result result, bool created = false)
    {
        if (result.IsSuccess)
            return created ? new StatusCodeResult(StatusCodes.Status201Created) : new NoContentResult();

        if (result.Errors.Any(e => e is NotFoundError))
            return new NotFoundObjectResult(new Response<string>("error", string.Empty, result.Errors.First().Message));

        return new BadRequestObjectResult(new Response<string>("error", string.Empty,
            string.Join("; ", result.Errors.Select(e => e.Message))));
    }
}
