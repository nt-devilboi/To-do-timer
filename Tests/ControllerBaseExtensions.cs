using System.Net;
using BBServer;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace To_do_timer.Tests;

public static class ControllerBaseExtensions
{
    public static void ShouldEquivalentResponse<T>(this ControllerBase controller, Result<T> response,
        HttpStatusCode expectedStatus, Result<T> expectedResponse)
        where T : class
    {
        response.Value.Should().BeEquivalentTo(expectedResponse.Value);
        response.Error.Should().BeEquivalentTo(expectedResponse.Error);
        controller.HttpContext.Response.StatusCode.Should().Be((int)expectedStatus);
    }
}