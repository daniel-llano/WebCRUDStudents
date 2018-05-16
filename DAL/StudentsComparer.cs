using System.Collections.Generic;

namespace DAL
{
    public class StudentsComparer : IEqualityComparer<Student>
    {
        public bool Equals(Student x, Student y)
        {
            return x.Gender.Equals(y.Gender) && x.Name.Equals(y.Name) && x.Type.Equals(y.Type) && x.UpdatedOn.Equals(y.UpdatedOn);
        }

        public int GetHashCode(Student obj)
        {
            return obj.GetHashCode();
        }
    }
}
