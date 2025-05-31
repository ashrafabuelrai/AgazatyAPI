


using Agazaty.Shared.Contracts.Email.DTOs;

namespace Agazaty.Shared.Contracts.Email.Service
{
    public interface IEmailService
    {
        Task SendEmail(EmailRequest emailRequest);
    }
}
