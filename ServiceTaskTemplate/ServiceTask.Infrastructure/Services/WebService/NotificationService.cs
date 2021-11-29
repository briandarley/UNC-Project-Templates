using System;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using $ext_projectname$.Domain.Models.WebServices.ActiveDirectory;
using $ext_projectname$.Infrastructure.Infrastructure.Extensions;
using $ext_projectname$.Infrastructure.Interfaces.WebServices;
using UNC.Extensions.General;
using UNC.HttpClient.Interfaces;
using UNC.Services;
using UNC.Services.Interfaces.Response;
using UNC.Services.Responses;

namespace $ext_projectname$.Infrastructure.Services.WebService
{
    public class NotificationService:ServiceBase, INotificationService
    {
        private readonly IWebClient _endPoint;
        private readonly Domain.Models.WebServices.Smtp.MessageModel _message;

        public NotificationService(ILogger logger, IWebClient endPoint,
            IApiResources apiResources,
            Domain.Models.WebServices.Smtp.MessageModel message) : base(logger)
        {
            _endPoint = endPoint;

            _endPoint.BaseAddress = apiResources.Resources.Single(c => c.Name.EqualsIgnoreCase("SMTP_MESSAGE")).Address;

            _message = message;
        }

        public async Task<IResponse> SendOwnerNotification(string principalEmail, string alternativeEmail = "")
        {

            try
            {
                LogBeginRequest();
                

                var subject = _message.Subject;
                var from = _message.From;

                var body = _message.Body.Replace("[[USER_EMAIL]]", principalEmail);


                var model = new
                {
                    IsHtml = true,
                    Body = body,
                    Subject = subject,
                    From = from,
                    Recipients = new[] { alternativeEmail ?? principalEmail }

                };

                await _endPoint.Post("emails", model);

                return new SuccessResponse();

            }
            catch (Exception ex)
            {
                return LogException(ex, false);
            }
            finally
            {
                LogEndRequest();
            }

        }

        public async Task<IResponse> SendAdminNotification(AdUserModel adUser)
        {

            try
            {
                LogBeginRequest();


                var primaryEmail = adUser.GetPrimaryEmail();
                
                var adminSubject = _message.AdminSubject.Replace("[[EMAIL]]", primaryEmail).Replace("[[ONYEN]]", adUser.SamAccountName);
                var adminEmail = _message.AdminEmail;

                var from = _message.From;

                var body = _message.Body.Replace("[[USER_EMAIL]]", primaryEmail);

                var model = new
                {
                    IsHtml = true,
                    Body = body,
                    Subject = adminSubject,
                    From = from,
                    Recipients = new[] { adminEmail }

                };

                await _endPoint.Post("emails", model);

                return new SuccessResponse();

            }
            catch (Exception ex)
            {
                return LogException(ex, false);
            }
            finally
            {
                LogEndRequest();
            }

        }
    }
}
