using DIExample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DIExample.Infrastructure
{
   public interface IStudentRepo
    {
        List<Student> GetAll();
        Student GetByID(int Id);
    }
}
