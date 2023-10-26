using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace CqrsExample.Api.Configurations;

public static class ControllerExtensions
{
    public static IActionResult InternalServerError(this ControllerBase controller, object value) =>
        controller.StatusCode((int)HttpStatusCode.InternalServerError, value);
}