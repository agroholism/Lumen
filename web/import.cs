using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

using Lumen;

public static class Main {
    public static void Import(Scope scope, String s) {
        StandartModule.__Kernel__.Set("web", web.Web.Instance);
    }
}

namespace web {
    public class Web : Module {
        public static Web Instance { get; } = new Web();

        private Web() {
            Set("email", new LambdaFun((scope, args) => {
                String fromName = scope["fname"].ToString(scope);
                String fmail = scope["fmail"].ToString(scope);
                String toName = scope["tname"].ToString(scope);
                String toMail = scope["tmail"].ToString(scope);
                String theme = scope["theme"].ToString(scope);
                String mess = scope["mess"].ToString(scope);
                String userName = scope["user"].ToString(scope);
                String password = scope["pass"].ToString(scope);

                MailAddress from = new MailAddress(fmail, fromName);
                MailAddress to = new MailAddress(toMail, toName);

                MailMessage message = new MailMessage(from, to) {
                    Subject = theme,

                    Body = mess
                };

                SmtpClient client = new SmtpClient("smtp.gmail.com", 587) {
                    Credentials = new NetworkCredential(userName, password),
                    EnableSsl = true
                };

                client.Send(message);

                return Const.NULL;
            }) {
                Arguments = new List<FunctionArgument> {
                        new FunctionArgument("fname"),
                        new FunctionArgument("fmail"),
                        new FunctionArgument("tname"),
                        new FunctionArgument("tmail"),
                        new FunctionArgument("theme"),
                        new FunctionArgument("mess"),
                        new FunctionArgument("user"),
                        new FunctionArgument("pass"),
                    }
            });
        }
    }
}
