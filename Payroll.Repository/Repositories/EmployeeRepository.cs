﻿using System.Linq;
using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Infrastructure.Implementations;
using System;
using System.Collections.Generic;

namespace Payroll.Repository.Repositories
{
    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(IDatabaseFactory databaseFactory)
            : base (databaseFactory)
        {
            DbSet = databaseFactory.GetContext().Employees;
        }

        public IList<Employee> GetActiveByPaymentFrequency(int PaymentFrequencyId)
        {
            //TODO check the payment frequency id
            return Find(e => e.IsActive).ToList();
        }

        public Employee GetByCode(string code)
        {
            return Find(e => e.EmployeeCode == code).FirstOrDefault();
        }
    }
}
