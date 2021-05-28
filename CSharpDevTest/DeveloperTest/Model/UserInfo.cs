
namespace DeveloperTest.Model
{
   public class UserInfo:NotifyPropertyChanged
    {
        public UserInfo()
        {
            Port = 993;
            Server = "imap.gmail.com";
        }

        private string _server;
        public string Server
        {
            get => _server;
            set
            {
                _server = value;
                OnPropertyChanged("Server");
            }

        }
        
        private int _port;
        public int Port
        {
            get => _port;
            set
            {
                _port = value;
                OnPropertyChanged("Port");
            }
        }
        private string username;

        public string UserName
        {
            get { return username; }
            set { username = value; }
        }

        private string password;

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

      
        public Enum.EncryptionType EncryptionType { get; set; }
        public Enum.ServerType ServerType { get; set; }
    }
}
