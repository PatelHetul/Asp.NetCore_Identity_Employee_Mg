using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Employee_Mg_Asp.NetCore.Models
{
    public class DepartmentMaster
    {
       
        [Key]
        public int Department_Id { get; set; }
        [Display(Name = "Department Name")]
        [Required(ErrorMessage = "Please Enter Department Name")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z'\s]*$")]
        [StringLength(40, MinimumLength = 2)]
        public string Department_Name { get; set; }
        public Nullable<int> IsDelete { get; set; }
    }
}
