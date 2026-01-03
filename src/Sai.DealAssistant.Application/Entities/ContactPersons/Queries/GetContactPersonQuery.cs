using AutoMapper;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.ContactPersons.Dto;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.ContactPersons.Queries;

public class GetContactPersonQuery : IRequest<ContactPersonDto>
{
    public GetContactPersonQuery(int id) => Id = id;
    public int Id { get; }

    public class Handler : IRequestHandler<GetContactPersonQuery, ContactPersonDto>
    {
        private readonly IReadRepository<ContactPerson> _repository;
        private readonly IMapper _mapper;

        public Handler(IReadRepository<ContactPerson> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ContactPersonDto> Handle(GetContactPersonQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repository.FirstOrDefaultAsync(p => p.Id == request.Id);
            if (entity == null)
            {
                throw new NotFoundExceptionOverride(nameof(ContactPerson), request.Id);
            }

            return _mapper.Map<ContactPersonDto>(entity);
        }
    }
}