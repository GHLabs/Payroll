﻿using Payroll.Common.Extension;
using Payroll.Entities;
using Payroll.Entities.Enums;
using Payroll.Entities.Payroll;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Service.Implementations
{
    public class EmployeePayrollService : IEmployeePayrollService
    {
        private IUnitOfWork _unitOfWork;
        private IEmployeePayrollRepository _employeePayrollRepository;
        private IEmployeeDailyPayrollService _employeeDailyPayrollService;
        private IEmployeePayrollDeductionService _employeePayrollDeductionService;
        private ISettingService _settingService;

        private readonly String PAYROLL_FREQUENCY = "PAYROLL_FREQUENCY";
        private readonly String PAYROLL_WEEK_START = "PAYROLL_WEEK_START";
        private readonly String PAYROLL_WEEK_END = "PAYROLL_WEEK_END";

        public EmployeePayrollService(IUnitOfWork unitOfWork, IEmployeeDailyPayrollService employeeDailyPayrollService, 
            IEmployeePayrollRepository employeeePayrollRepository, ISettingService settingService)
        {
            _unitOfWork = unitOfWork;
            _employeeDailyPayrollService = employeeDailyPayrollService;
            _employeePayrollRepository = employeeePayrollRepository;
            _settingService = settingService;
        }

        public IList<EmployeePayroll> GeneratePayrollNetPayByDateRange(DateTime payrollDate, DateTime dateFrom, DateTime dateTo)
        {
            var employeeDailyPayroll = _employeeDailyPayrollService.GetByDateRange(dateFrom, dateTo);
            var employeePayrollList = new List<EmployeePayroll>();

            //Hold last payroll processed
            EmployeePayroll tempEmployeePayroll = null;

            DateTime today = new DateTime();
            EmployeeDailyPayroll last = employeeDailyPayroll.Last();

            foreach (EmployeeDailyPayroll dailyPayroll in employeeDailyPayroll)
            {
                //If should create new entry
                if (tempEmployeePayroll != null && 
                    (tempEmployeePayroll.EmployeeId != tempEmployeePayroll.EmployeeId))
                {
                    //Save last entry if for different employee
                    _employeePayrollRepository.Add(tempEmployeePayroll);
                    employeePayrollList.Add(tempEmployeePayroll);

                    EmployeePayroll employeePayroll = new EmployeePayroll
                    {
                        EmployeeId = dailyPayroll.EmployeeId,
                        CutOffStartDate = dateFrom,
                        CutOffEndDate = dateTo,
                        PayrollGeneratedDate = today,
                        PayrollDate = payrollDate,
                        TotalNet = dailyPayroll.TotalPay,
                        TaxableIncome = dailyPayroll.TotalPay
                    };

                    tempEmployeePayroll = employeePayroll;

                }
                else
                {
                    //Update last entry
                    tempEmployeePayroll.TotalNet += dailyPayroll.TotalPay;
                }

                //If last iteration save
                if (dailyPayroll.Equals(last))
                {
                    _employeePayrollRepository.Add(tempEmployeePayroll);
                    employeePayrollList.Add(tempEmployeePayroll);
                }
            }

            //Commit
            _unitOfWork.Commit();

            return employeePayrollList;
        }

        public void Update(EmployeePayroll employeePayroll)
        {
            _employeePayrollRepository.Update(employeePayroll);
        }

        public IList<EmployeePayroll> GetForTaxProcessingByEmployee(int employeeId, DateTime payrollDate)
        {
            return _employeePayrollRepository.GetForTaxProcessingByEmployee(employeeId, payrollDate);
        }

        public void GeneratePayroll()
        {
            var frequency = (FrequencyType)Convert.
                ToInt32(_settingService.GetByKey(PAYROLL_FREQUENCY));

            DateTime today = new DateTime();

            DateTime payrollStartDate = today;
            DateTime payrollEndDate = today;

            //TODO more frequency support
            switch (frequency)
            {
                case FrequencyType.Weekly:
                    //Note that the job should always schedule the day after the payroll end date
                    var startOfWeeklyPayroll = (DayOfWeek)Enum.Parse(typeof(DayOfWeek),
                        _settingService.GetByKey(PAYROLL_WEEK_START));
                    var endOfWeekPayroll = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), 
                        _settingService.GetByKey(PAYROLL_WEEK_END));
                    
                    payrollStartDate = today.StartOfWeek(startOfWeeklyPayroll);
                    payrollEndDate = today.StartOfWeek(endOfWeekPayroll);
                    break;
            }

            GeneratePayroll(today, payrollStartDate, payrollEndDate);
        }

        public void GeneratePayroll(DateTime payrollDate, DateTime payrollStartDate, DateTime payrollEndDate)
        {
            //Generate employee payroll and net pay
            var employeePayrollList = GeneratePayrollNetPayByDateRange(payrollDate, payrollStartDate, payrollEndDate);

            //Generate deductions such as SSS, HDMF, Philhealth and TAX
            _employeePayrollDeductionService.GenerateDeductionsByPayroll(payrollDate,
                payrollStartDate, payrollEndDate, employeePayrollList);
        }
    }
}
