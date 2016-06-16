﻿using System.Collections.Generic;
using Payroll.Repository.Models.Payroll;
using Payroll.Service.Interfaces.Model;

namespace Payroll.Models.Payroll
{
    public class PayrollViewModel
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public IPaginationModel Pagination { get; set; }
        public IEnumerable<PayrollDao> Payrolls { get; set; }
    }
}