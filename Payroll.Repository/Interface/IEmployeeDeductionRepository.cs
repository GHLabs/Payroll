﻿using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Repository.Interface
{
    public interface IEmployeeDeductionRepository : IRepository<EmployeeDeduction>
    {
        EmployeeDeduction GetByDeductionAndEmployee(int deductionId, int employeeId);
    }
}
