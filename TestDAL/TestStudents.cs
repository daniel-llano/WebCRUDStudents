using Microsoft.VisualStudio.TestTools.UnitTesting;
using DAL;

namespace TestDAL
{
    [TestClass]
    public class TestStudents
    {
        [TestMethod]
        public void TestCreateUpdateDeleteStudents()
        {
            DBConnection.ConnectionString = "Server=(local)\\sqlexpress;Database=Students;Trusted_Connection=True;";

            var newStudent = new Student("Superman", "Male", StudentType.High);
            var result = StudentMapper.Insert(newStudent);
            Assert.AreEqual(true, result);

            var savedStudent = StudentMapper.GetById(newStudent.Id);
            Assert.AreEqual("Superman", savedStudent.Name);

            savedStudent.Name = "Clark Kent";
            StudentMapper.Update(savedStudent);
            var updatedStudent = StudentMapper.GetById(savedStudent.Id);
            Assert.AreEqual("Clark Kent", updatedStudent.Name);

            StudentMapper.Hide(updatedStudent.Id);
            var hiddenStudent = StudentMapper.GetById(updatedStudent.Id);
            Assert.AreEqual(false, hiddenStudent.Enabled);

            StudentMapper.Delete(hiddenStudent.Id);
            var deleteStudent = StudentMapper.GetById(hiddenStudent.Id);
            Assert.AreEqual(null, deleteStudent);
        }

        [TestMethod]
        public void TestReadStudents() {
            DBConnection.ConnectionString = "Server=(local)\\sqlexpress;Database=Students;Trusted_Connection=True;";

            long total = 0;
            long pages = 0;
            var list = StudentMapper.GetAllPaginatedWhere(1, 10, out total, out pages, "");

            Assert.AreNotEqual(0, total);
            Assert.AreNotEqual(0, pages);
            Assert.AreNotEqual(null, list);
            Assert.AreNotEqual(0, list.Count);
        }
    }
}          