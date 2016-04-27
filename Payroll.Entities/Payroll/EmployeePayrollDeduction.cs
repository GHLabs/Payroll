﻿using Payroll.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Entities
{
    [Table("employee_payroll_deduction")]
    public class EmployeePayrollDeduction : BaseEntity
    {
        [Key]
        public int EmployeePayrollDeductionId { get; set; }

        public int DeductionId { get; set; }

        public DateTime PayrollDate { get; set; }

        public decimal Amount { get; set; }
    }
}
