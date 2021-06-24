using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TechLibrary.Data.Entities
{
    public class ErrorStore
    {
        [Key]
        public int ErrorStoreId { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
        public string Body { get; set; }
    }
}
