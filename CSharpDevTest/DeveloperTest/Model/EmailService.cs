using DeveloperTest.Model.Enum;
using DeveloperTest.Model.Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeveloperTest.Model
{
    public class EmailService
    {
        private ImapService imapService;
        private POP3Service pop3Service;


        public EmailService()
        {
            imapService = new ImapService();
            pop3Service = new POP3Service();

           
        }


        // Download Email Headers
        public async Task<List<Email>> DownloadEmailHeaders(UserInfo userInfo, List<Email> emailHeaderList)
        {                   
            switch (userInfo.ServerType)
            {
                case ServerType.IMAP:
                    {

                        // get 5 different connection to the server
                        await Task.Run(() => imapService.ServerConnections(userInfo));
                        emailHeaderList = await imapService.GetImapEmailHeaders(userInfo);
                        return emailHeaderList;
                    }
                case ServerType.POP3:

                    {
                        await Task.Run(() => pop3Service.ServerConnections(userInfo));
                        emailHeaderList = await pop3Service.GetPop3EmailHeaders(userInfo);
                        return emailHeaderList;
                    }
                default:
                    throw new NotImplementedException("ServerType not available");
            }
        }


        // Download EmailBody on Demand
        public async Task OnDemandEmailBody(Email selectedEmail, UserInfo userInfo)
        {
            switch (userInfo.ServerType)
            {
                case ServerType.IMAP:
                    {
                        await imapService.GetImapBodyonDemand(selectedEmail, userInfo);
                        break;
                    }
                case ServerType.POP3:
                    {
                        await pop3Service.GetPop3BodyonDemand(selectedEmail, userInfo);
                        break;
                    }
                default:
                    throw new NotImplementedException("ServerType not available");                                
            }
        }

        // Downloading Email in Background when Headers are Downloaded
        public async Task DowloadEmailBody(Email email, UserInfo userInfo)
        {
            switch (userInfo.ServerType)
            {
                case ServerType.IMAP:
                    {
                        await imapService.GetImapEmailBody(email);
                        break;
                    }
                case ServerType.POP3:
                    {
                        await pop3Service.GetImapEmailBody(email);
                        break;
                    }
                default:
                    throw new NotImplementedException("ServerType not available");
            }
        }

        // Dispose Connection Based on Services

        internal void Dispose(UserInfo userInfo)
        {
            switch (userInfo.ServerType)
            {
                case ServerType.IMAP:
                    {
                        imapService.DisposeImapConn();
                        break;
                    }
                case ServerType.POP3:
                    {
                        pop3Service.DisposePop3Conn();
                        break;
                    }
                default:
                    throw new NotImplementedException("ServerType not available");
            }

        }

    }
}
