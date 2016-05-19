﻿using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Repository.Repositories
{
    public class EmployeePayrollRepository : Repository<EmployeePayroll>, IEmployeePayrollRepository
    {
        public EmployeePayrollRepository(IDatabaseFactory databaseFactory)
            : base (databaseFactory)
        {
            DbSet = databaseFactory.GetContext().EmployeePayroll;
        }

        public IList<EmployeePayroll> GetByDateRange(DateTime dateStart, DateTime dateEnd)
        {
            return Find(p => p.IsActive && p.PayrollDate >= dateStart && p.PayrollDate < dateEnd)
                .OrderByDescending(p => p.PayrollDate).ToList();
        }

        public IList<EmployeePayroll> GetForTaxProcessingByEmployee(int employeeId, DateTime payrollDate)
        {
            return Find(p => p.IsActive && !p.IsTaxed && p.EmployeeId == employeeId 
                && p.PayrollDate < payrollDate).OrderByDescending( p => p.PayrollDate).ToList();
        }
    }
}
