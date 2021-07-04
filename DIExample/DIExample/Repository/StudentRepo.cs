using DIExample.Data;
using DIExample.Infrastructure;
using DIExample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DIExample.Repository
{
    public class StudentRepo : IStudentRepo
    {
        private readonly ApplicationDbContext _context;

        public StudentRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Student> GetAll()
        {
            return _context.Students.ToList();
        }

        public Student GetByID(int id)
        {
            return _context.Students.FirstOrDefault(s => s.Id == id);
        }
    }
}
