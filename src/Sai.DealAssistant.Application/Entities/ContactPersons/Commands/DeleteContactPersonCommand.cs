using AutoMapper;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.ContactPersons.Dto;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.ContactPersons.Commands;

public class DeleteContactPersonCommand : IRequest<ContactPersonDto>
{
    public DeleteContactPersonCommand(int id) => Id = id;
    public int Id { get; set; }

    public class Handler : IRequestHandler<DeleteContactPersonCommand, ContactPersonDto>
    {
        private readonly ICrudRepository<ContactPerson> _repository;
        private readonly IMapper _mapper;

        public Handler(ICrudRepository<ContactPerson> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ContactPersonDto> Handle(DeleteContactPersonCommand request, CancellationToken cancellationToken)
        {
            ContactPerson? deleted = await _repository.DeleteAsync(request.Id);

            if (deleted == null)
            {
                throw new NotFoundExceptionOverride(nameof(ContactPerson), request.Id);
            }

            return _mapper.Map<ContactPersonDto>(deleted);
        }
    }
}