using System.Net;
using System.Net.Mail;
using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

public class EmailSender : MonoBehaviour
{
    public IEnumerator SendVerificationEmail(string toEmail, string code)
    {
        string fromEmail = "kreatyvicompany@mail.ru";
        string fromPassword = "ALGkiRb4kHFqtgCreuBQ";
        string smtpServer = "smtp.mail.ru";
        int smtpPort = 587;

        string subject = "Код подтверждения";
        string body = "Ваш код подтверждения: " + code;

        bool finished = false;
        string errorMsg = "";

        Task.Run(() =>
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(fromEmail, "Kreatyvi");
                    mail.To.Add(toEmail);
                    mail.Subject = subject;
                    mail.Body = body;

                    using (SmtpClient smtp = new SmtpClient(smtpServer, smtpPort))
                    {
                        smtp.Credentials = new NetworkCredential(fromEmail, fromPassword);
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
            }
            catch (SmtpFailedRecipientsException ex)
            {
                foreach (SmtpFailedRecipientException inner in ex.InnerExceptions)
                    errorMsg += $"❌ Получатель {inner.FailedRecipient}: {inner.Message}\n";
            }
            catch (SmtpException ex)
            {
                errorMsg = $"❌ SMTP ошибка: {ex.StatusCode} - {ex.Message}";
            }
            catch (System.Exception ex)
            {
                errorMsg = $"❌ Общая ошибка: {ex.Message}";
            }
            finally
            {
                finished = true;
            }
        });

        while (!finished)
            yield return null;

        if (!string.IsNullOrEmpty(errorMsg))
            Debug.LogError("Ошибка отправки письма:\n" + errorMsg);
        else
            Debug.Log("✅ Письмо успешно отправлено на " + toEmail);
    }
}
