using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DeveloperTest.Model;
using DeveloperTest.Commands;

namespace DeveloperTest.ViewModel
{
   public class MainWindowViewModel: Model.NotifyPropertyChanged
    {
        #region Display_Operation

        private EmailService emailservice;
        private ObservableCollection<Model.Email> _emails;
        private List<Email> EmailList=new List<Email>();
        private bool LoadingHeaders;
        public ObservableCollection<Model.Email> Emails
        {
            get => _emails;
            set
            {
                _emails = value;
                OnPropertyChanged("Emails");
            }
        }

        private Model.Email _selectedEmail;

        public Model.Email SelectedEmail
        {
            get => _selectedEmail;
            set
            {
                _selectedEmail = value;
               
                Task.Run(() => OnDemandBody(SelectedEmail)).ConfigureAwait(false);

                OnPropertyChanged("SelectedEmail");
            }
        }      

        private Model.UserInfo userInfo;
        public Model.UserInfo UserInfo
        {
            get => userInfo;
            set
            {
                userInfo = value;
                OnPropertyChanged("UserInfo");
            }
        }

        public MainWindowViewModel()
        {          
            Emails = new ObservableCollection<Email>();
            UserInfo = new UserInfo();        
            btnStartCommanad = new RelayCommand(Start);
        }
      

        public async Task DownloadEmails()
        {
            if (LoadingHeaders)
                return;
            try
            {
                List<Task> tasks = new List<Task>();
                // Download Email headers                              
                EmailList=await emailservice.DownloadEmailHeaders(UserInfo, EmailList);
               
                foreach (var email in EmailList)
                {
                    if (email != null)
                    {
                        await Task.Run(() =>
                            App.Current.Dispatcher.Invoke(delegate
                         {
                             Emails.Add(email);
                         }));
                        // Download Email Body in background
                        Task emailBoadyTask =Task.Run(async () => await emailservice.DowloadEmailBody(email, UserInfo));
                        tasks.Add(emailBoadyTask);
                    }
                }
                Task taskcompletion = Task.WhenAll(tasks);
                await taskcompletion;

                if (taskcompletion.Status == TaskStatus.RanToCompletion)
                    emailservice.Dispose(userInfo);
                LoadingHeaders = true;
                
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
            finally
            {
                emailservice.Dispose(userInfo);
            }

        }

        // Download email on demand
        public async Task OnDemandBody(Email selectedemail)
        {
            if (selectedemail != null && selectedemail.Body==null)
            {
                foreach (Email email in Emails)
                {
                    if (selectedemail.Uid == email.Uid)
                    {
                        if(email.Body != null)
                        {
                            selectedemail.Body = email.Body;
                            return;
                        }
                        break;
                    }
                   
                }
                await emailservice.OnDemandEmailBody(selectedemail, UserInfo);
            }            
        }

        #endregion

        #region Server_Encryption_Type
        public IEnumerable<Model.Enum.EncryptionType> EncryptionTypes =>
          Enum.GetValues(typeof(Model.Enum.EncryptionType))
              .Cast<Model.Enum.EncryptionType>();

        public IEnumerable<Model.Enum.ServerType> ServerTypes =>
            Enum.GetValues(typeof(Model.Enum.ServerType))
                .Cast<Model.Enum.ServerType>();
        #endregion

        #region Start_Button_Execute

        private RelayCommand btnStartCommanad;

        public ICommand BtnStartCommand
        {
            get
            {
                if (btnStartCommanad == null)
                    btnStartCommanad = new RelayCommand();
                return btnStartCommanad;
            }
            
        }

        //emails will be downloaded when start button clicked
        public void Start()
        {
            try
            {
                emailservice = new EmailService();
                Task.Run(() => DownloadEmails()).ConfigureAwait(false);              
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
            finally
            {
                emailservice.Dispose(userInfo);
            }

        }
        #endregion

    }
}
