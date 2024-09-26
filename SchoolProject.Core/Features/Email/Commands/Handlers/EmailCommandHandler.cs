using MediatR;
using Microsoft.Extensions.Localization;
using SchoolProject.Core.Bases;
using SchoolProject.Core.Features.Email.Commands.Models;
using SchoolProject.Core.Resources;
using SchoolProject.Service.Abstracts;

namespace SchoolProject.Core.Features.Email.Commands.Handlers
{
    public class EmailCommandHandler : ResponseHandler,
        IRequestHandler<SendEmailCommand, Response<string>>
    {
        #region Fields
        private readonly IStringLocalizer<SharedResources> stringLocalizer;
        private readonly IEmailService emailService;
        #endregion

        #region Constructor
        public EmailCommandHandler(IStringLocalizer<SharedResources> stringLocalizer,
            IEmailService emailService) : base(stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
            this.emailService = emailService;
        }
        #endregion

        #region Functions
        public async Task<Response<string>> Handle(SendEmailCommand request, CancellationToken cancellationToken)
        {
            var response = await emailService.SendEmail(request.Email, request.Message, null);
            if (response == "Success") return Success(response);
            return BadRequest<string>(stringLocalizer[SharedResourcesKeys.SendEmailFailed]);
        }
        #endregion
    }
}
