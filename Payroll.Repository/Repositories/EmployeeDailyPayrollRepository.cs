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
    public class EmployeeDailyPayrollRepository : Repository<EmployeeDailyPayroll>, IEmployeeDailyPayrollRepository
    {
        public EmployeeDailyPayrollRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
            DbSet = databaseFactory.GetContext().EmployeeDailyPayroll;
        }

        public IList<EmployeeDailyPayroll> GetByDateRange(DateTime dateFrom, DateTime dateTo)
        {
            return Find(p => p.Date >= dateFrom && p.Date < dateTo)
                .OrderByDescending(p => p.Date).ThenBy(p => p.EmployeeId).ToList();
        }
    }
}
