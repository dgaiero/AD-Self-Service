using System;
using System.Configuration;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.WsFederation;
using Microsoft.Owin.Security;
using System.DirectoryServices.AccountManagement;

namespace WebApplication1.Controllers
{

    public class AccountController : Controller
    {
        public void SignIn()
        {
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = "/" },
                    WsFederationAuthenticationDefaults.AuthenticationType);

            }
        }

        public void SignOut()
        {
            string callbackUrl = Url.Action("SignOutCallback", "Account", routeValues: null, protocol: Request.Url.Scheme);

            HttpContext.GetOwinContext().Authentication.SignOut(
                new AuthenticationProperties { RedirectUri = callbackUrl },
                WsFederationAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);
        }

        public ActionResult SignOutCallback()
        {
            if (Request.IsAuthenticated)
            {
                // Redirect to home page if the user is authenticated.
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        //POST: ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult changePassword(WebApplication1.Models.userData userInformation)
        {
            if (ModelState.IsValid)
            {
                if (userInformation.username == User.GetsAMAccountID())
                {
                    try
                    {
                        using (var context = new PrincipalContext(ContextType.Domain))
                        {
                            using (var user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, User.GetsAMAccountID()))
                            {
                                user.ChangePassword(userInformation.oldPassword, userInformation.newPassword);
                            }
                        }
                        SendPasswordResetEmail();
                        ViewBag.InformationMessage = "Password Change Successful!";
                        ViewBag.MessageBanner = "alert-success";
                        return View();
                    }
                    catch (System.DirectoryServices.AccountManagement.PasswordException)
                    {
                        ViewBag.InformationMessage = String.Format("Your current password was entered incorrectly");
                        ViewBag.MessageBanner = "alert-info";
                        return View(userInformation);
                    }
                    catch (System.Exception ex)
                    {
                        ViewBag.InformationMessage = String.Format("An error has occured.{0}{1}", Environment.NewLine, ex);
                        ViewBag.MessageBanner = "alert-danger";
                        return View(userInformation);
                    }
                }
                else
                {
                    ViewBag.InformationMessage = "Username Entered Incorrectly.";
                    ViewBag.MessageBanner = "alert-warning";
                    return View(userInformation);
                }
            }
            else
            {
                return View(userInformation);
            }
        }

        public void SendPasswordResetEmail()
        {
            try
            {
                string emailSendUsername = ConfigurationManager.AppSettings["emailUsername"];
                string emailSendPassword = ConfigurationManager.AppSettings["emailPassword"];
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.zoho.com");

                mail.From = new MailAddress(emailSendUsername);
                mail.To.Add(User.GetEmailAddress());
                mail.Subject = "Password Recently Changed";

                mail.IsBodyHtml = true;
                string htmlBody;

                htmlBody = String.Format("Hello {0},<br>Your password was recently changed on <b>AD.FIRE-LIGHT.US</b> with account <b>{1}</b>. If you did not make this change, please <a href=\"mailto:webmaster@fire-light.us\">click here</a>.<br>Thank you,<br>AD", User.Identity.Name,User.GetUPN());

                mail.Body = htmlBody;

                SmtpServer.Port = 587;

                SmtpServer.Credentials = new System.Net.NetworkCredential(emailSendUsername, emailSendPassword);
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}