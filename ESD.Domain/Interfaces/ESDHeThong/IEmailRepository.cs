using System.Collections.Generic;
using ESD.Domain.Models.DAS;
using System.Threading.Tasks;
using ESD.Domain.Models.CustomModels;

namespace ESD.Domain.Interfaces.DAS
{
    public interface IEmailRepository : IBaseRepository<Email>
    {
        Task<EmailResult> SendEmailWithEmailAddress(string body, string title, string emailAddress, string emailType, byte[] attachment = null, string attachmentName = "");

        Task<bool> SendEmailWithListEmail(string body, string title, List<string> lstEmail, string emailType, byte[] attachment = null, string attachmentName = "");

        Task<bool> SendEmailWithListUser(string body, string title, List<int> lstUserId, string emailType, byte[] attachment = null, string attachmentName = "");

        Task<EmailResult> SendEmailWithUser(string body, string title, int userId, string emailType, byte[] attachment = null, string attachmentName = "");

        Task<bool> SendEmailWithAgency(string body, string title, int AgencyID, byte[] attachment = null, string attachmentName = "");
    }
}
