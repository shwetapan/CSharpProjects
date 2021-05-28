using DeveloperTest.Model;
using Limilabs.Client.IMAP;
using Limilabs.Mail;
using Limilabs.Mail.MIME;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace DeveloperTest.Model.Service
{
   public class ImapService
    {
        private List<Imap> serverConnection;
        List<Email> emailHeaderList = new List<Email>();
        string emailAttachmentPath=string.Empty;

        #region Server_Connections
        // Connect to imap
        public async Task<Imap> ConnectImap(Model.UserInfo userInfo)
        {
           
            var serverconnTask = Task.Run(async () =>
            {
                Imap imap = new Imap();

                if (userInfo.EncryptionType == Model.Enum.EncryptionType.Unencrypted)
                {
                    await Task.Run(() =>
                    {
                        imap.SSLConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls;
                        imap.Connect(userInfo.Server, userInfo.Port);
                       
                    });
                    
                }
                else if (userInfo.EncryptionType == Model.Enum.EncryptionType.SSLTLS)
                {
                  

                    await Task.Run(() =>
                    {
                        imap.SSLConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                        imap.ConnectSSL(userInfo.Server, userInfo.Port);
                    });
                }
                else
                {
                    await Task.Run(() =>
                    {
                        imap.SSLConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls11;
                        imap.Connect(userInfo.Server, userInfo.Port);
                    });

                }
                await Task.Run(() =>
                {
                   imap.UseBestLogin(userInfo.UserName, userInfo.Password);
                });
               
                imap.SelectInbox();

                return imap;
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
            serverConnection = new List<Imap>();
            // atleast one connection required for Headers
            Imap imap = await ConnectImap(userInfo);
            serverConnection.Add(imap);
            // Run the others connections 
           await Task.Run(async () =>
            {
                int numOfConnection = 5;
                int connFailure = 0;
               
                for (int i = 0; i < numOfConnection-1;i++)
                {
                    try
                    {
                        Imap imapconnections = await ConnectImap(userInfo);
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

        public Imap AvailableServerConnection()
        {

            int numofimapconnection = serverConnection.Count;
            int i = 0;
            if (i < numofimapconnection)
            {

                Imap imap = serverConnection[i];
                serverConnection.Remove(imap);
                return imap;
            }

            return null;
        }
        #endregion

        #region Email_Header
        public async Task<List<Email>> GetImapEmailHeaders(UserInfo userInfo)
        {

            Imap imap = AvailableServerConnection();
            try
            {
                List<MessageInfo> Infos = new List<MessageInfo>();
                if (imap != null)
                {
                    var uids = await Task.Run(() => imap.Search(Flag.All));


                    Infos = await Task.Run(() => imap.GetMessageInfoByUID(uids));
                    foreach (MessageInfo Info in Infos)
                    {
                        Email email = new Email
                        {
                            Uid = Info.UID.ToString(),
                            Date = Info.Envelope.Date ?? DateTime.MinValue,
                            Subject = Info.Envelope.Subject,
                            From = string.Join(",", Info?.Envelope?.From?.Select(s => s.Address)),

                        };

                        emailHeaderList.Add(email);
                    }
                }
            }

            catch (Exception ex)
            {
                string message = ex.Message;
            }
            finally
            {
                serverConnection.Add(imap);
            }
            return emailHeaderList;

        }

        #endregion

        #region Email_Body

        private void saveAttachmentToDisk(string selectedmailuid, Imap imap)
        {
            List<long> uids = imap.Search(Flag.All);         
                IMail email = new MailBuilder()
                    .CreateFromEml(imap.GetMessageByUID(Convert.ToInt64(selectedmailuid)));

                ReadOnlyCollection<MimeData> attachments = email.ExtractAttachmentsFromInnerMessages();
            if(emailAttachmentPath!=null)
                emailAttachmentPath = Path.Combine(Environment.CurrentDirectory, "EmailFile");


            // save all attachments to disk
            foreach (MimeData mime in attachments)
                {
                    mime.Save(emailAttachmentPath + mime.SafeFileName);
                }
            
            
        }

        private async Task<bool> ReadImapEmailBody(Imap imap, Email email)
        {
            return await Task.Run(() =>
            {
                lock (email)
                {
                    if (string.IsNullOrEmpty(email.Body) && imap != null)
                    {
                        BodyStructure structure = imap.GetBodyStructureByUID(Convert.ToInt64(email.Uid));

                        if (structure?.Text != null)
                        {
                            email.Body = imap.GetTextByUID(structure.Text);
                            if (structure.Attachments.Count>0)
                            {
                                // download attachment from email
                                saveAttachmentToDisk(email.Uid, imap);
                            }
                            return true;
                        }
                    }

                    return false;
                }
            });

        }

        public async Task<bool> GetImapEmailBody(Email email)
        {
            Imap imap = AvailableServerConnection();
            try
            {
                return await ReadImapEmailBody(imap, email);
            }
            finally
            {
                serverConnection.Add(imap);
               
            }
        }

        public async Task<bool> GetImapBodyonDemand(Email email, UserInfo userInfo)
        {
            Imap imap = await ConnectImap(userInfo);
            try
            {
               
                return await ReadImapEmailBody(imap, email);
            }
            finally
            {
                imap.Close();
            }
          
        }

        #endregion     

        #region Dispose_Connection
        // Dispose Connection After Reading Email Header and Body
        internal void DisposeImapConn()
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
