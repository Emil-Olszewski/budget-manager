using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Controllers.Common;

[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected IMediator Mediator;

    public BaseController(IMediator mediator)
    {
        Mediator = mediator;
    }
}