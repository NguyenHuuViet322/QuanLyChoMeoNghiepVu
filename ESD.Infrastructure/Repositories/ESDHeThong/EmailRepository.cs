using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.CustomModels;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;
using ESD.Utility;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class EmailRepository : DasBaseRepository<Email>, IEmailRepository
    {
        public EmailRepository(ESDContext repositoryContext)
            : base(repositoryContext)
        {

        }
        //public async Task<RepositoryResult> Create(Email model)
        //{
        //    await dasRepo.Email.InsertAsync(model);
        //    await dasRepo.SaveAync();
        //    return new RepositoryResultSuccess();
        //}

        private async Task<EmailResult> SendEmail(string body, string title, string toEmail, byte[] attachment = null, string attachmentName = "")
        {
            var result = new EmailResult();
            try
            {
                //var userEmail = ConfigUtils.GetKeyValue("EmailConfigs", "EmailUsername");
                //string passwordEmail = ConfigUtils.GetKeyValue("EmailConfigs", "EmailPassword");

                //var mailMessage = new MailMessage(userEmail, toEmail, title, body);
                //mailMessage.IsBodyHtml = true;

                //var netCred = new NetworkCredential(userEmail, passwordEmail);
                //var smtpClient = new SmtpClient("smtp.gmail.com", 587);
                //smtpClient.EnableSsl = true;
                //smtpClient.UseDefaultCredentials = false;
                //smtpClient.Credentials = netCred;
                //smtpClient.Timeout = 200000;
                //await smtpClient.SendMailAsync(mailMessage);
                //result.IsSuccess = true;

                await Task.Run(() =>
                {
                    var userEmail = ConfigUtils.GetKeyValue("EmailConfigs", "EmailUsername");
                    string passwordEmail = ConfigUtils.GetKeyValue("EmailConfigs", "EmailPassword");

                    var mailMessage = new MailMessage(userEmail, toEmail, title, body);
                    mailMessage.IsBodyHtml = true;

                    //Add attachment
                    if (attachment != null)
                    {
                        var fileName = string.IsNullOrWhiteSpace(attachmentName) ? "FileDefault" : attachmentName;
                        var stream = new MemoryStream(attachment);
                        mailMessage.Attachments.Add(new Attachment(stream, fileName));
                    }
                    var netCred = new NetworkCredential(userEmail, passwordEmail);
                    var smtpClient = new SmtpClient("smtp.gmail.com", 587);
                    smtpClient.EnableSsl = true;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = netCred;
                    smtpClient.Timeout = 200000;
                    smtpClient.Send(mailMessage);
                    result.IsSuccess = true;
                });

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.Trace = ex.StackTrace;
            }

            return result;
        }

        public async Task<EmailResult> SendEmailWithEmailAddress(string body, string title, string emailAddress, string emailType, byte[] attachment = null, string attachmentName = "")
        {
            //// Validate email, body ở tầng service
            //if (string.IsNullOrEmpty(body) || string.IsNullOrEmpty(title) || string.IsNullOrEmpty(emailAddress))
            //{
            //    return new RepositoryResultError("Nhập thiếu thông tin");
            //}

            //if (!StringUltils.IsValidEmail(emailAddress))
            //{
            //    return new RepositoryResultError("Sai định dạng email");
            //}

            var resultSend = await SendEmail(body, title, emailAddress, attachment, attachmentName);
            if (resultSend.IsSuccess)
            {
                var modelEmail = new Email
                {
                    Content = body,
                    EmailType = emailType,
                    FromEmail = ConfigUtils.GetKeyValue("EmailConfigs", "EmailUsername"),
                    Title = title,
                    ToEmail = emailAddress
                };
                await InsertAsync(modelEmail);
                await DasContext.SaveChangesAsync();
            }

            return resultSend;
        }

        public async Task<bool> SendEmailWithListEmail(string body, string title, List<string> lstEmail, string emailType, byte[] attachment = null, string attachmentName = "")
        {
            //// Validate email, body ở tầng service
            //if (string.IsNullOrEmpty(body) || string.IsNullOrEmpty(title) || lstEmail == null || lstEmail.Count == 0)
            //{
            //    return new RepositoryResultError();
            //}

            try
            {
                bool sendSuccess = false;
                for (int i = 0; i < lstEmail.Count; i++)
                {
                    var emailAddress = lstEmail[i];
                    var resultSend = await SendEmailWithEmailAddress(body, title, emailAddress, emailType, attachment, attachmentName);

                    if (!resultSend.IsSuccess)
                    {
                        continue;
                    }

                    sendSuccess = true;
                }

                return sendSuccess;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public async Task<bool> SendEmailWithListUser(string body, string title, List<int> lstUserId, string emailType, byte[] attachment = null, string attachmentName = "")
        {
            //// Validate email, body ở tầng service
            //if (string.IsNullOrEmpty(body) || string.IsNullOrEmpty(title) || lstUserId == null || lstUserId.Count == 0)
            //{
            //    return new RepositoryResultError();
            //}
            var lstEmail = new List<String>();
            for (int i = 0; i < lstUserId.Count; i++)
            {
                var email = await DasContext.User.Where(u => u.ID == lstUserId[i]).Select(u => u.Email).FirstOrDefaultAsync();
                lstEmail.Add(email);
            }
            return await SendEmailWithListEmail(body, title, lstEmail, emailType, attachment, attachmentName);
        }

        public async Task<EmailResult> SendEmailWithUser(string body, string title, int userId, string emailType, byte[] attachment = null, string attachmentName = "")
        {
            //// Validate email, body ở tầng service
            //if (string.IsNullOrEmpty(body) || string.IsNullOrEmpty(title))
            //{
            //    return new RepositoryResultError();
            //}

            var strEmail = await DasContext.User.Where(u => u.ID == userId).Select(u => u.Email).FirstOrDefaultAsync();
            return await SendEmailWithEmailAddress(body, title, strEmail, emailType, attachment, attachmentName);
        }

        public async Task<bool> SendEmailWithAgency(string body, string title, int AgencyID, byte[] attachment = null, string attachmentName = "")
        {
            //// Validate email, body ở tầng service
            //if (string.IsNullOrEmpty(body) || string.IsNullOrEmpty(title))
            //{
            //    return new RepositoryResultError();
            //}

            var lstEmail = await DasContext.User.Where(x => x.IDAgency == AgencyID).Select(x => x.Email).ToListAsync();
            if (lstEmail == null)
            {
                return false;
            }
            return await SendEmailWithListEmail(body, title, lstEmail, "Agencys", attachment, attachmentName);
        }
    }
}