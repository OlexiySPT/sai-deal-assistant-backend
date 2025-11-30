using AutoMapper;
using MediatR;
#pragma warning disable CS8019
using Microsoft.AspNetCore.Authorization;
#pragma warning restore CS8019
using Microsoft.AspNetCore.Mvc;

namespace Sai.DealAssistant.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
#if DEBUG
#else
	[Authorize]
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
