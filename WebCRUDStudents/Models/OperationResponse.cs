using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCRUDStudents.Models
{
    public class OperationResponse
    {
        public bool HasError { get; set; }
        public string Message { get; set; }
    }
}