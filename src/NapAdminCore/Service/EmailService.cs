namespace NapAdminCore.Service;

using System.Text.RegularExpressions;
using Fanex.MailDeliver.Model;
using Helpers;

public static class EmailService
{
    public static void SendEmail(string mailTo, string mailCc, string mailBcc, string subject, string body)
    {
        if (!IsValidEmailList(mailTo, true) || !IsValidEmailList(mailCc) || !IsValidEmailList(mailBcc))
        {
            return;
        }

        var mailItem = new MailRecord
        {
            MailTo = mailTo,
            MailCc = mailCc,
            MailBcc = mailBcc,
            Subject = subject,
            Title = string.IsNullOrWhiteSpace(AppSettings.EmailSenderName)
                ? string.Empty
                : AppSettings.EmailSenderName,
            Body = body
        };

        using var client = new Fanex.MailDeliver.MailClient.API.MailDeliver();
        client.SendMail(mailItem);
    }

    public static bool IsValidEmailList(string emailList, bool hasCheckEmpty = false)
    {
        var isValid = true;

        if (string.IsNullOrWhiteSpace(emailList))
        {
            if (hasCheckEmpty)
            {
                isValid = false;
            }
        }
        else
        {
            var arrEmail = emailList.Split(';');

            if (arrEmail.Any(email => !IsValidEmail(email)))
            {
                return false;
            }
        }

        return isValid;
    }

    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        var regex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
        return regex.IsMatch(email);
    }
}
