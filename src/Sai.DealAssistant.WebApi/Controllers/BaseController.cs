using AutoMapper;
using MediatR;
#pragma warning disable CS8019
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

#pragma warning restore CS8019
using Microsoft.AspNetCore.Mvc;

namespace Sai.DealAssistant.WebApi.Controllers
{
    [ApiController]
    [EnableCors("AllowFrontend")]
	[Route("api/[controller]")]
#if DEBUG
#else
	//[Authorize]
#endif
	public abstract class BaseController : ControllerBase
	{
		protected BaseController(IMediator mediator, IMapper mapper)
		{
			Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
			Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}

		protected IMediator Mediator { get; }

		protected IMapper Mapper { get; }
	}
}
