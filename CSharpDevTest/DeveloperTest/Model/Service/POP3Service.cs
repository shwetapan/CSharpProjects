using DeveloperTest.Model;
using Limilabs.Client.IMAP;
using Limilabs.Client.POP3;
using Limilabs.Mail;
using Limilabs.Mail.MIME;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperTest.Model.Service
{
   public class POP3Service
    {
        private List<Pop3> serverConnection;
        List<Email> emailHeaderList = new List<Email>();

        #region Server_Connections
        // Connect to POP3
        public async Task<Pop3> ConnectPop3(Model.UserInfo userInfo)
        {
            var serverconnTask = Task.Run(() =>
            {
                Pop3 pop3 = new Pop3();

                if (userInfo.EncryptionType == Model.Enum.EncryptionType.Unencrypted)
                    pop3.Connect(userInfo.Server, userInfo.Port);
                else if (userInfo.EncryptionType == Model.Enum.EncryptionType.SSLTLS)
                    pop3.ConnectSSL(userInfo.Server, userInfo.Port);
                else
                {
                    pop3.Connect(userInfo.Server, userInfo.Port);
                    pop3.StartTLS();
                }
                pop3.UseBestLogin(userInfo.UserName, userInfo.Password);
                return pop3;
            });
            var completedTask = await Task.WhenAny(serverconnTask);
            if (completedTask == serverconnTask)
                return await serverconnTask;
            else
                throw new TimeoutException("server Connection timed out");
        }


        // create five different connection to server
        public async Task ServerConnections(Model.UserInfo userInfo)
        {
            // List To store server connections
            serverConnection = new List<Pop3>();
            // atleast one connection required for Headers
            Pop3 pop3 = await ConnectPop3(userInfo);
            serverConnection.Add(pop3);
            // Run the others connections 
            await Task.Run(async () =>
            {
                int numOfConnection = 5;
                int connFailure = 0;

                for (int i = 0; i < numOfConnection - 1; i++)
                {
                    try
                    {
                        Pop3 imapconnections = await ConnectPop3(userInfo);
                        serverConnection.Add(imapconnections);
                    }
                    catch (Exception)
                    {
                        connFailure++;
                        break;
                    }
                }
            });

        }

        internal async Task<bool> GetImapEmailBody(Email email)
        {
            Pop3 pop3 = AvailableServerConnection();
            try
            {
                return await ReadPop3EmailBody(pop3, email);
            }
            finally
            {
                serverConnection.Add(pop3);
                pop3.Close();
            }
        }

        public Pop3 AvailableServerConnection()
        {

            int numofimapconnection = serverConnection.Count;
            int i = 0;
            if (i < numofimapconnection)
            {

                Pop3 pop3 = serverConnection[i];
                serverConnection.Remove(pop3);
                return pop3;
            }

            return null;
        }

        #endregion

        #region Email_Header

        public async Task<List<Email>> GetPop3EmailHeaders(Model.UserInfo userInfo)
        {
            Pop3 pop3 = await ConnectPop3(userInfo);

            try
            {
                if (pop3 != null)
                {
                    List<string> uids = pop3.GetAll();
                    MailBuilder builder = new MailBuilder();
                    foreach (string uid in uids)
                    {
                        var headers = pop3.GetHeadersByUID(uid);
                        IMail mail = builder.CreateFromEml(headers);

                        Email email = new Email
                        {
                            Uid = uid,
                            Date = mail.Date ?? DateTime.MinValue,
                            Subject = mail.Subject,
                            From = string.Join(",", mail.From?.Select(s => s.Address)),
                        };
                        emailHeaderList.Add(email);
                    }
                }
                return emailHeaderList;
            }
            finally
            {
                serverConnection.Add(pop3);
            }

        }

        #endregion

        #region Email_Body    

        private async Task<bool> ReadPOP3EmailBody( Email email, UserInfo userInfo)
        {
            Pop3 pop3 = await ConnectPop3(userInfo);
            var eml = await Task.Run(() => pop3.GetMessageByUID(email.Uid));
            MailBuilder mailbuilder = new MailBuilder();
            IMail mail = mailbuilder.CreateFromEml(eml);
            email.Body = mail.Text;
            return true;

        }

        private void saveAttachmentToDisk(string selectedmailuid, Pop3 pop3)
        {
            List<string> uids = pop3.GetAll();

            foreach (string uid in uids)
            {
                if (uid.ToString() == selectedmailuid.ToString())
                {
                    var eml = pop3.GetMessageByUID(uid);
                    IMail email = new MailBuilder()
                        .CreateFromEml(eml);

                    ReadOnlyCollection<MimeData> attachments = email.ExtractAttachmentsFromInnerMessages();
                    string emailAttachmentPath = Path.Combine(Environment.CurrentDirectory, "LocalData");
                    // save all attachments to disk
                    foreach (MimeData mime in attachments)
                    {
                        mime.Save(emailAttachmentPath + mime.SafeFileName);
                    }
                }
            }
        }

        private async Task<bool> ReadPop3EmailBody(Pop3 pop3, Model.Email email)
        {
            return await Task.Run(() =>
            {
                lock (email)
                {
                    if (string.IsNullOrEmpty(email.Body))
                    {
                        var msg = pop3.GetMessageByUID(email.Uid);
                        MailBuilder builder = new MailBuilder();
                        IMail mail = builder.CreateFromEml(msg);
                        email.Body = mail.Text;
                        if (mail.Attachments != null)
                        {
                            // show attachment in email and when user clicks the attachment it should download
                            saveAttachmentToDisk(email.Uid, pop3);
                        }
                        return true;
                    }
                    return false;
                }
            });

        }

        internal async Task<bool> GetPop3BodyonDemand(Email selectedEmail, UserInfo userInfo)
        {
            Pop3 pop3 = await ConnectPop3(userInfo);
            return await ReadPop3EmailBody(pop3, selectedEmail);
        }

        public Task<bool> GetPOP3EmailBody(Email email)
        {
            return Task.FromResult(false);
        }

        #endregion

        #region Dispose_Connection

        //Dispose Server connections after reading Emails 
        internal void DisposePop3Conn()
        { 
            if (serverConnection != null)
            {
                foreach (var item in serverConnection)
                {
                    item.Dispose();
                }

            }
        }
        #endregion

    }
}
