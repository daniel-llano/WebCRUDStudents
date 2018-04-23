using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class StudentMapper 
    {
        public static bool Insert(Student student)
        {
            bool res = true;
            DBConnection.CreateConnection(DBType.SQLServer);
            DBParameter name = new DBParameter("name", TypeOfValue.STRING, student.Name);
            DBParameter type = new DBParameter("type", TypeOfValue.STRING, student.Type.ToString());
            DBParameter gender = new DBParameter("gender", TypeOfValue.CHAR, student.Gender == "Masculine" ? "M" : "F");
            DBParameter[] pars = new DBParameter[] { name, type, gender };
            long id = DBConnection.TheConnection.ExecStoredProcAdd("addStudent", pars);
            if (id == -1)
                res = false;
            else
                student.Id = id;
            return res;
        }
        public static bool Update(Student student)
        {
            bool res = true;
            DBConnection.CreateConnection(DBType.SQLServer);
            DBParameter id = new DBParameter("id", TypeOfValue.INTEGER, student.Id);
            DBParameter name = new DBParameter("name", TypeOfValue.STRING, student.Name);
            DBParameter type = new DBParameter("type", TypeOfValue.STRING, student.Type.ToString());
            DBParameter gender = new DBParameter("gender", TypeOfValue.CHAR, student.Gender == "Masculine" ? "M" : "F");
            DBParameter enabled = new DBParameter("enabled", TypeOfValue.BOOLEAN, student.Enabled);
            DBParameter[] pars = new DBParameter[] { id, name, type, gender, enabled };
            res = DBConnection.TheConnection.ExecStoredProc("updStudent", pars);
            return res;
        }
        public static bool Hide(long id)
        {
            bool res = true;
            DBConnection.CreateConnection(DBType.SQLServer);
            DBParameter pid = new DBParameter("id", TypeOfValue.INTEGER, id);
            DBParameter[] pars = new DBParameter[] { pid };
            res = DBConnection.TheConnection.ExecStoredProc("hideStudent", pars);
            return res;
        }
        public static bool Delete(long id)
        {
            bool res = true;
            DBConnection.CreateConnection(DBType.SQLServer);
            DBParameter pid = new DBParameter("id", TypeOfValue.INTEGER, id);
            DBParameter[] pars = new DBParameter[] { pid };
            res = DBConnection.TheConnection.ExecStoredProc("delStudent", pars);
            return res;
        }
        public static Student GetById(long id)
        {
            Student res = null;
            DBConnection.CreateConnection(DBType.SQLServer);
            DBParameter pid = new DBParameter("id", TypeOfValue.INTEGER, id);
            DBParameter[] pars = new DBParameter[] { pid };
            IList<object[]> list = DBConnection.TheConnection.ExecStoredProcQuery("selStudentById", pars);
            if (list != null && list.Count > 0)
                res = new Student(list[0]);
            return res;
        }
        public static List<Student> GetAllPaginatedWhere(long currentPage, long itemsPerPage, out long totalItems, out long totalPages, string filteredBy, string sortByField = "updated_on", string sortDirection = "DESC", bool isEnabled = true, params DBParameter[] parameters)
        {
            List<Student> res = null;
            DBConnection.CreateConnection(DBType.SQLServer);
            DBParameter offset = new DBParameter("OFFSET", TypeOfValue.INTEGER, currentPage);
            DBParameter limit = new DBParameter("LIMIT", TypeOfValue.INTEGER, itemsPerPage);
            DBParameter count = new DBParameter("COUNT", TypeOfValue.INTEGER, 0, ParameterType.OUT);
            DBParameter pags = new DBParameter("TPAGS", TypeOfValue.INTEGER, 0, ParameterType.OUT);
            DBParameter field = new DBParameter("field", TypeOfValue.STRING, sortByField);
            DBParameter condition = new DBParameter("cond", TypeOfValue.STRING, filteredBy);
            DBParameter sort = new DBParameter("sort", TypeOfValue.STRING, sortDirection);
            DBParameter enabled = new DBParameter("enabled", TypeOfValue.BOOLEAN, isEnabled);
            DBParameter[] pars = new DBParameter[] { offset, limit, count, pags, field, condition, sort, enabled };
            List<object[]> lista = DBConnection.TheConnection.ExecStoredProcQuery("selPaginatedStudentsWhere", pars);
            if (lista != null && lista.Count > 0)
            {
                res = new List<Student>();
                foreach (object[] fila in lista)
                    res.Add(new Student(fila));
            }
            totalItems = Convert.ToInt64(count.Value);
            totalPages = Convert.ToInt64(pags.Value);
            return res;
        }
    }
}
