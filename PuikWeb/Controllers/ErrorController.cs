using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace PuikWebUI.Controllers;

public class ErrorController : Controller
{
    
    [Route("Error/{statusCode}")]
    public IActionResult HttpStatusCodeHandler(int statusCode)
    {
        var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

        ViewBag.StatusCode = statusCode;
        ViewBag.ErrorMessage = $"Error {statusCode}: {GetErrorMessage(statusCode)}";

        return View("Error");
    }

    [Route("Error")]
    public IActionResult Index()
    {
        var exceptionDetail = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        ViewBag.StatusCode = 500; 
        ViewBag.ErrorMessage = exceptionDetail?.Error.Message ?? "An unexpected error occurred.";


        return View();
    }

    private string GetErrorMessage(int statusCode)
    {
        return new System.Net.Http.HttpResponseMessage((System.Net.HttpStatusCode)statusCode).ReasonPhrase;
    }
}