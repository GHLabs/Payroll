﻿using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Payroll.Entities.Enums;

namespace Payroll.Repository.Repositories
{
    public class TotalEmployeeHoursRepository : Repository<TotalEmployeeHours>, ITotalEmployeeHoursRepository
    {
        public TotalEmployeeHoursRepository(IDatabaseFactory databaseFactory)
            : base (databaseFactory)
        {
            DbSet = databaseFactory.GetContext().TotalEmployeeHours;
        }

        public TotalEmployeeHours GetByEmployeeDateAndType(int employeeId, DateTime date, RateType type)
        {
            return Find(eh => eh.EmployeeId == employeeId && eh.Date == date && eh.Type == type).First();
        }

        public IList<TotalEmployeeHours> GetByDateRange(DateTime dateFrom, DateTime dateTo)
        {
            return Find(eh => eh.Date >= dateFrom && eh.Date < dateTo)
                .OrderByDescending(eh => eh.Date).OrderBy(eh => eh.EmployeeId).OrderBy(eh => eh.Type).ToList();
        }
    }
}
