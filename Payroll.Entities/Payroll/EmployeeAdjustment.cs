﻿using Payroll.Infrastructure.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities.Payroll
{
    [Table("employee_adjustment")]
    public class EmployeeAdjustment : BaseEntity
    {
        [Key]
        public int AdjustmentId { get; set; }

        public int AdjustmentTypeId { get; set; }

        public int EmployeeId { get; set; }

        public DateTime Date { get; set; }

        public decimal Amount { get; set; }

        [StringLength(5000)]
        public string Remarks { get; set; }
    }
}
