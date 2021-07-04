using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserRegExample.Models;

namespace UserRegExample.Repo
{
   public interface IEmployee
    {
        IEnumerable<Employee> GetAll();
        Employee GetById(int Id);
    }
}
