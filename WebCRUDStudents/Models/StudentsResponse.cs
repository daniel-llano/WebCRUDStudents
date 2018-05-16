using DAL;
using System.Collections.Generic;

namespace WebCRUDStudents.Models
{
    public class StudentsResponse
    {
        public long TotalStudents { get; set; }
        public long TotalPages { get; set; }
        public List<Student> Students { get; set; }
    }
}