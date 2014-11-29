using System;
using System.Net;
using System.Security.Principal;

namespace JsConsole.Services
{
    public class PermissionService
    {
        public static bool IsUserAdministrator()
        {
            bool isAdmin;
            try
            {
                var user = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException ex)
            {
                isAdmin = false;
            }
            catch (Exception ex)
            {
                isAdmin = false;
            }
            return isAdmin;
        }

        public static string GetUrl(int port)
        {
            var host = "";
            if (IsUserAdministrator())
            {
                var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                if (ipHostInfo != null)
                    host = ipHostInfo.HostName;
            }
            else
            {
                host = "localhost";
            }
            return string.Format("http://{0}:{1}", host, port);
        }
    }
}