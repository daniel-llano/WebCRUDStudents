using DAL;
using System;
using System.Collections.Generic;
using System.Web.Http;
using WebCRUDStudents.Models;

namespace WebCRUDStudents.Controllers
{
    public class StudentsServiceController : ApiController
    {
        public StudentsResponse GetPaginatedList(DateTime startDate, DateTime endDate, string name, bool isEnabled, string gender, string type, int pageNumber, int pageSize, string sortBy, string sortDirection)
        {
            var listResult = new List<Student>();
            long totalItems = 0;
            long totalPages = 0;

            try
            {
                string filter = string.Empty;
                string joinOperator = string.Empty;

                //filter = " convert(date, updated_on) between '" + startDate.ToString("yyyyMMdd") + "' and '" + endDate.ToString("yyyyMMdd") + "' ";
                filter = " updated_on >= '" + startDate.ToString("yyyy-MM-dd") + "' and updated_on <= '" + endDate.ToString("yyyy-MM-dd") + "' + INTERVAL 1 DAY ";

                if (!string.IsNullOrEmpty(name)) {
                    filter += " and name like '%" + name + "%'";
                }

                if (!string.IsNullOrEmpty(gender))
                {
                    filter += " and gender = '" + gender  + "'";
                }

                joinOperator = (string.IsNullOrEmpty(filter) ? string.Empty : " and ");

                if (!string.IsNullOrEmpty(type))
                {
                    filter += " and type = '" + type + "'";
                }

                listResult = StudentMapper.GetAllPaginatedWhere(pageNumber, pageSize, out totalItems, out totalPages, filter, sortBy, sortDirection, isEnabled);
            }
            catch (Exception ex)
            {
                Logger.Create().Exception(ex);
            }

            return new StudentsResponse { Students = listResult, TotalStudents = totalItems, TotalPages = totalPages };
        }

        public OperationResponse AddNew(Student student) {
            var response = new OperationResponse();
            try
            {
                response.HasError = !StudentMapper.Insert(student);
            }
            catch (Exception ex){
                response.HasError = true;
                response.Message = ex.ToString();
            }

            return response;
        }

        public OperationResponse Update(Student student)
        {
            var response = new OperationResponse();
            try
            {
                response.HasError = !StudentMapper.Update(student);
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.Message = ex.ToString();
            }

            return response;
        }
        [HttpGet]
        public OperationResponse Hide(long id)
        {
            var response = new OperationResponse();
            try
            {
                response.HasError = !StudentMapper.Hide(id);
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.Message = ex.ToString();
            }

            return response;
        }
        [HttpGet]
        public OperationResponse Delete(long id)
        {
            var response = new OperationResponse();
            try
            {
                response.HasError = !StudentMapper.Delete(id);
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.Message = ex.ToString();
            }

            return response;
        }
    }
}
