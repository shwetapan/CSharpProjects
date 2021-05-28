using System;

namespace DeveloperTest.Model
{
   public class Email:NotifyPropertyChanged
    {      
        public string Subject { get; set; }
        public string From { get; set; }
        public string Uid { get; set; }
        public DateTime Date { get; set; }      
        
        private string body;
        public string Body
        {
            get => body;
            set
            {
                body = value;
                OnPropertyChanged("Body");
            }
        }

      
    }
}
