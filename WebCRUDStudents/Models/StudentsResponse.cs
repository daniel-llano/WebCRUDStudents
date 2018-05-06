using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCRUDStudents.Models
{
    public class StudentsResponse
    {
        public long TotalStudents { get; set; }
        public long TotalPages { get; set; }
        public List<Student> Students { get; set; }
    }
}