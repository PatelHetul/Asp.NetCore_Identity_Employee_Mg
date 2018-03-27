using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Employee_Mg_Asp.NetCore.Models
{
    public class EmployeeMaster
    {
        [Key]
        public int Employee_Id { get; set; }
        [Display(Name = "Employee Name")]
        [Required(ErrorMessage = "Please Enter Employee Name")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z'\s]*$")]
        [StringLength(100, MinimumLength = 3)]
        public string Employee_Name { get; set; }


        public Nullable<int> Department_Id { get; set; }

        [Required(ErrorMessage = "Please Enter Employee Joining Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> JoiningDate { get; set; }

        [StringLength(200, MinimumLength = 3)]
        public string Address { get; set; }
        public Nullable<int> IsDelete { get; set; }

        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [StringLength(60, MinimumLength = 8)]
        public string Email { get; set; }

        [Display(Name = "Contact No")]
        [StringLength(10, MinimumLength = 10)]
        public string MobileNo { get; set; }
        public string Image { get; set; }

        public virtual DepartmentMaster DepartmentMaster { get; set; }
    }
}
