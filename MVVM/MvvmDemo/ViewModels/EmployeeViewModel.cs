using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using MvvmDemo.Models;
using MvvmDemo.Commands;
using System.Collections.ObjectModel;
namespace MvvmDemo.ViewModels
{
    public class EmployeeViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged_Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion INotifyPropertyChanged_Implementation

        EmployeeService ObjEmployeeService;
        public EmployeeViewModel()
        {
            ObjEmployeeService = new EmployeeService();
            LoadData();
            CurrentEmployee = new Employee();
            saveCommand = new RelayCommand(Save);
            searchCommand = new RelayCommand(Search);
            updateCommand = new RelayCommand(Update);
            deleteCommand = new RelayCommand(Delete);

        }

        #region DisplayOperation
        private ObservableCollection<Employee> employeesList;
        public ObservableCollection<Employee> EmployeesList
        {
            get { return employeesList; }
            set { employeesList = value;OnPropertyChanged("EmployeesList"); }
        }
        private void LoadData()
        {
            EmployeesList = new ObservableCollection<Employee>( ObjEmployeeService.GetAll());
        }
        #endregion DisplayOperation 

        private Employee currentEmployee;

        public Employee CurrentEmployee
        {
            get { return currentEmployee; }
            set { currentEmployee = value; OnPropertyChanged("CurrentEmployee"); }
        }

        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; OnPropertyChanged("Message"); }
        }

        #region Save_Operation
        private RelayCommand saveCommand;

        public RelayCommand SaveCommand
        {
            get { return saveCommand; }
           
        }

        public void Save()
        {
            try
            {
                var IsSaved = ObjEmployeeService.Add(currentEmployee);
                LoadData();
                if (IsSaved)
                    Message = "Employee Saved";
                else
                    Message = "Save Operation Failed";
            }
            catch(Exception ex)
            {
                Message = ex.Message;
            }
        }
        #endregion


        #region Search_Operation
        private RelayCommand searchCommand;

        public RelayCommand SearchCommand
        {
            get { return searchCommand; }
        }

        public void Search()
        {
            try
            {
                var ObjEmployee = ObjEmployeeService.Search(CurrentEmployee.Id);
                if (ObjEmployee != null)
                {
                    CurrentEmployee.Name = ObjEmployee.Name;
                    CurrentEmployee.Age = ObjEmployee.Age;
                    Message = "Employee found ";
                   
                }
                else
                    Message = "Employee not found ";
            }
            catch (Exception ex)
            {

                Message = ex.Message;
            }
        }
        #endregion

        #region Update_Operation
        private RelayCommand updateCommand;

        public RelayCommand UpdateCommand
        {
            get { return updateCommand; }          
        }

        public void Update()
        {
            try
            {
                var isUpdated = ObjEmployeeService.Update(CurrentEmployee);
                if (isUpdated)
                    Message = "Employee Updated";
                LoadData();
            }
            catch (Exception ex)
            {

                Message = "Upldate Operation Failed";
            }
           
        }
        #endregion

        private RelayCommand deleteCommand;

        public RelayCommand DeleteCommand
        {
            get { return deleteCommand; }
          
        }

        public void Delete()
        {
            try
            {
                var IsDeleted = ObjEmployeeService.Delete(CurrentEmployee.Id);
                if (IsDeleted)
                {
                    Message = "Employee Deleted";
                    LoadData();
                }
                else
                {
                    Message = "Delete Operation Failed";
                }
            }
            catch ( Exception ex)
            {

                Message = ex.Message;
            }
        }
    }

}
