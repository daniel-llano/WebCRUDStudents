using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public enum StudentType { Kinder, Elementary, High, University }
    public class Student
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public StudentType Type { get; set; }
        public bool Enabled { get; set; }
        public DateTime UpdatedOn { get; set; }
        public Student(object[] values)
        {
            Id = Convert.ToInt32(values[0]);
            Name = values[1].ToString();
            Type = (StudentType)Enum.Parse(typeof(StudentType), values[2].ToString());
            Gender = values[3].ToString() == "M" ? "Masculine" : "Feminine";
            Enabled = Convert.ToBoolean(values[4]);
            UpdatedOn = Convert.ToDateTime(values[5]);
        }
        public Student(string name, string gender, StudentType type) {
            Name = name;
            Gender = gender;
            Type = type;
        }
    }
}
