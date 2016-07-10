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
        private IEmployeePayrollDeductionService _employeePayrollDeductionService;
        private ISettingService _settingService;
        private IEmployeeInfoService _employeeInfoService;
        private ITotalEmployeeHoursService _totalEmployeeHourService;
        private IEmployeeService _employeeService;
        private ITotalEmployeeHoursService _totalEmployeeHoursService;
        private IEmployeePayrollItemService _employeePayrollItemService;

        private FrequencyType _frequency;

        private readonly String PAYROLL_FREQUENCY = "PAYROLL_FREQUENCY";
        private readonly String PAYROLL_WEEK_START = "PAYROLL_WEEK_START";
        private readonly String PAYROLL_WEEK_END = "PAYROLL_WEEK_END";
        private readonly String PAYROLL_WEEK_RELEASE = "PAYROLL_WEEK_RELEASE";
        private readonly String ALLOWANCE_WEEK_SCHEDULE = "ALLOWANCE_WEEK_SCHEDULE";
        private readonly String ALLOWANCE_DAY_SCHEDULE = "ALLOWANCE_DAY_SCHEDULE";
        private readonly String ALLOWANCE_TOTAL_DAYS = "ALLOWANCE_TOTAL_DAYS";
        private readonly String PAYROLL_TOTAL_HOURS = "PAYROLL_TOTAL_HOURS";

        public EmployeePayrollService(IUnitOfWork unitOfWork, IEmployeePayrollRepository employeeePayrollRepository, ISettingService settingService, IEmployeePayrollDeductionService employeePayrollDeductionService,
            IEmployeeInfoService employeeInfoService, ITotalEmployeeHoursService totalEmployeeHourService, IEmployeeService employeeService, ITotalEmployeeHoursService totalEmployeeHoursService,
            IEmployeePayrollItemService employeePayrollItemService)
        {
            _unitOfWork = unitOfWork;
            _employeePayrollRepository = employeeePayrollRepository;
            _settingService = settingService;
            _employeePayrollDeductionService = employeePayrollDeductionService;
            _employeeInfoService = employeeInfoService;
            _totalEmployeeHourService = totalEmployeeHourService;
            _employeeService = employeeService;
            _totalEmployeeHoursService = totalEmployeeHoursService;
            _employeePayrollItemService = employeePayrollItemService;

            _frequency = (FrequencyType)Convert
                .ToInt32(_settingService.GetByKey(PAYROLL_FREQUENCY));
        }

        public IList<EmployeePayroll> GeneratePayrollGrossPayByDateRange(DateTime payrollDate, DateTime payrollStartDate, DateTime payrollEndDate)
        {
            var employeePayrollItems = _employeePayrollItemService.GetByDateRange(payrollDate, payrollDate);
            var employeePayrollList = new List<EmployeePayroll>();

            if (employeePayrollItems != null && employeePayrollItems.Count() > 0)
            {
                //Hold last payroll processed
                EmployeePayroll tempEmployeePayroll = null;
                DateTime today = DateTime.Now;
                EmployeePayrollItem lastPayrollItem = employeePayrollItems.Last();

                foreach (EmployeePayrollItem item in employeePayrollItems)
                {
                    //If should create new entry
                    if (tempEmployeePayroll == null || (tempEmployeePayroll.Employee.EmployeeId != item.EmployeeId))
                    {
                        if (tempEmployeePayroll != null)
                        {
                            //Save last entry if for different employee
                            _employeePayrollRepository.Add(tempEmployeePayroll);
                            employeePayrollList.Add(tempEmployeePayroll);
                        }
                        Employee employee = _employeeService.GetById(item.EmployeeId);

                        EmployeePayroll employeePayroll = new EmployeePayroll
                        {
                            Employee = employee,
                            CutOffStartDate = payrollStartDate,
                            CutOffEndDate = payrollEndDate,
                            PayrollGeneratedDate = today,
                            PayrollDate = payrollDate,
                            TotalGross = item.TotalAmount,
                            TotalNet = item.TotalAmount,
                            TaxableIncome = item.TotalAmount
                        };

                        tempEmployeePayroll = employeePayroll;
                    }
                    else
                    {
                        //Update last entry
                        tempEmployeePayroll.TotalGross += item.TotalAmount;
                        tempEmployeePayroll.TotalNet += item.TotalAmount;
                        tempEmployeePayroll.TaxableIncome += item.TotalAmount;
                    }

                    //If last iteration save
                    if (item.Equals(tempEmployeePayroll))
                    {
                        _employeePayrollRepository.Add(tempEmployeePayroll);
                        employeePayrollList.Add(tempEmployeePayroll);
                    }
                }

                //Commit
                _unitOfWork.Commit();
            }
            return employeePayrollList;
            /*foreach (EmployeeDailyPayroll dailyPayroll in employeeDailyPayroll)
            {
                //If should create new entry
                if (tempEmployeePayroll == null ||
                        (tempEmployeePayroll.Employee.EmployeeId != dailyPayroll.EmployeeId))
                {
                    if (tempEmployeePayroll != null)
                    {
                        //Save last entry if for different employee
                        _employeePayrollRepository.Add(tempEmployeePayroll);
                        employeePayrollList.Add(tempEmployeePayroll);
                    }
                    Employee employee = _employeeService.GetById(dailyPayroll.EmployeeId);

                    EmployeePayroll employeePayroll = new EmployeePayroll
                    {
                        Employee = employee,
                        CutOffStartDate = dateFrom,
                        CutOffEndDate = dateTo,
                        PayrollGeneratedDate = today,
                        PayrollDate = payrollDate,
                        TotalGross = dailyPayroll.TotalPay,
                        TotalNet = dailyPayroll.TotalPay,
                        TaxableIncome = dailyPayroll.TotalPay
                    };

                    tempEmployeePayroll = employeePayroll;
                    createNewPayrollItem = true;

                }
                else
                {
                    //Update last entry
                    tempEmployeePayroll.TotalGross += dailyPayroll.TotalPay;
                    tempEmployeePayroll.TotalNet += dailyPayroll.TotalPay;
                    tempEmployeePayroll.TaxableIncome += dailyPayroll.TotalPay;

                    if (tempEmployeePayrollItem.RateType != dailyPayroll.RateType)
                    {
                        createNewPayrollItem = true;
                    }
                    else
                    {
                        createNewPayrollItem = false;
                    }
                }

                //Payroll Item
                var totalEmployeeHours = _totalEmployeeHoursService.GetById(dailyPayroll.TotalEmployeeHoursId.Value);
                //If should create new item
                if (createNewPayrollItem)
                {
                    //Save last item
                    if (tempEmployeePayrollItem != null)
                    {
                        _employeePayrollItemService.Add(tempEmployeePayrollItem);
                        tempEmployeePayrollItem = null;
                    }

                    /*tempEmployeePayrollItem = new EmployeePayrollItem
                    {
                        EmployeePayroll = tempEmployeePayroll,
                        RateType = dailyPayroll.RateType,
                        TotalAmount = dailyPayroll.TotalPay,
                        TotalHours = totalEmployeeHours.Hours
                    };

                }
                else
                {
                    //Update last entry
                    tempEmployeePayrollItem.TotalHours += totalEmployeeHours.Hours;
                    tempEmployeePayrollItem.TotalAmount += dailyPayroll.TotalPay;

                }


                //If last iteration save
                if (dailyPayroll.Equals(last))
                {
                    _employeePayrollRepository.Add(tempEmployeePayroll);
                    _employeePayrollItemService.Add(tempEmployeePayrollItem);
                    employeePayrollList.Add(tempEmployeePayroll);
                }
            })*/
        }

        public void Update(EmployeePayroll employeePayroll)
        {
            _employeePayrollRepository.Update(employeePayroll);
        }

        public IList<EmployeePayroll> GetForTaxProcessingByEmployee(int employeeId, DateTime payrollDate)
        {
            return _employeePayrollRepository.GetForTaxProcessingByEmployee(employeeId, payrollDate);
        }

        public DateTime GetNextPayrollStartDate(DateTime? date)
        {
            DateTime? payrollStartDate = _employeePayrollRepository.GetNextPayrollStartDate();
            if (payrollStartDate == null)
            {
                var d = DateTime.Now;
                if (date != null)
                {
                    d = date.Value;
                }
                //TODO more frequency support
                switch (_frequency)
                {
                    case FrequencyType.Weekly:
                        //Note that the job should always schedule the day after the payroll end date
                        var startOfWeeklyPayroll = (DayOfWeek)Enum.Parse(typeof(DayOfWeek),
                            _settingService.GetByKey(PAYROLL_WEEK_START));
                        
                        if (d.DayOfWeek == startOfWeeklyPayroll)
                        {
                            d = d.AddDays(-7);
                        }

                        payrollStartDate = d.StartOfWeek(startOfWeeklyPayroll);

                        break;
                }
            }
            return payrollStartDate.Value;
        }

        public DateTime GetNextPayrollEndDate(DateTime payrollStartDate)
        {
            DateTime payrollEndDate = payrollStartDate;

            //TODO more frequency support
            switch (_frequency)
            {
                case FrequencyType.Weekly:
                    //Note that the job should always schedule the day after the payroll end date
                    var endOfWeekPayroll = (DayOfWeek)Enum.Parse(typeof(DayOfWeek),
                        _settingService.GetByKey(PAYROLL_WEEK_END));

                    payrollEndDate = payrollStartDate.AddDays(7).StartOfWeek(endOfWeekPayroll);

                    break;
            }
        
            return payrollEndDate;
        }

        public DateTime GetNextPayrollReleaseDate(DateTime payrollEndDate)
        {
            DateTime payrollReleaseDate = payrollEndDate;

            //TODO more frequency support
            switch (_frequency)
            {
                case FrequencyType.Weekly:
                    //Note that the job should always schedule the day after the payroll end date
                    var endOfWeekPayroll = (DayOfWeek)Enum.Parse(typeof(DayOfWeek),
                        _settingService.GetByKey(PAYROLL_WEEK_RELEASE));

                    payrollReleaseDate = payrollEndDate.AddDays(1).StartOfWeek(endOfWeekPayroll);

                    break;
            }

            return payrollReleaseDate;
        }

        public void GeneratePayroll(DateTime? date)
        {
            DateTime payrollStartDate = GetNextPayrollStartDate(date);
            DateTime payrollEndDate = GetNextPayrollEndDate(payrollStartDate);
            
            GeneratePayroll(payrollStartDate, payrollEndDate);
        }

        private void DeleteByDateRange(DateTime payrollStartDate, DateTime payrollEndDate)
        {
            //If there's existing payroll within date range, make it inactive
            var existingPayrolls = this.GetByPayrollDateRange(payrollStartDate, payrollEndDate);
            _employeePayrollRepository.DeleteAll(existingPayrolls);

            _unitOfWork.Commit();
        }

        public void GeneratePayroll(DateTime payrollStartDate, DateTime payrollEndDate)
        {
            //Delete existing payrolls
            DeleteByDateRange(payrollStartDate, payrollEndDate);

            var payrollDate = GetNextPayrollReleaseDate(payrollEndDate);
           
            //Generate employee payroll and net pay
            var employeePayrollList = GeneratePayrollGrossPayByDateRange(payrollDate, payrollStartDate, payrollEndDate);

            //Generate Allowance
            ComputeAllowance(payrollStartDate, payrollEndDate, employeePayrollList);

            //Generate deductions such as SSS, HDMF, Philhealth and TAX
            GenerateDeductions(payrollDate,
                payrollStartDate, payrollEndDate, employeePayrollList);
        }


        public IList<EmployeePayroll> GetByDateRange(DateTime dateStart, DateTime dateEnd)
        {
            dateEnd = dateEnd.AddDays(1);
            return _employeePayrollRepository.GetByDateRange(dateStart, dateEnd);
        }

        public void ComputeAllowance(DateTime payrollStartDate, DateTime payrollEndDate, 
            IList<EmployeePayroll> payrollList)
        {
            //Get allowance schedule
            //If 1st, 2nd, 3rd or 4th of the week
            int weekSchedule = Convert.ToInt32(_settingService.GetByKey(ALLOWANCE_WEEK_SCHEDULE));
            //If monday, tuesday ... so on
            DayOfWeek daySchedule = (DayOfWeek)Enum.Parse(typeof(DayOfWeek),
                            _settingService.GetByKey(ALLOWANCE_DAY_SCHEDULE));

            //Get schedule
            var allowanceDateByStartDate = DatetimeExtension
                .GetNthWeekofMonth(payrollStartDate, weekSchedule, daySchedule);

            var allowanceDateByEndDate = DatetimeExtension
               .GetNthWeekofMonth(payrollEndDate, weekSchedule, daySchedule);

            if ((allowanceDateByStartDate >= payrollStartDate && allowanceDateByStartDate < payrollEndDate.AddDays(1)) ||
                    (allowanceDateByEndDate >= payrollStartDate && allowanceDateByEndDate < payrollEndDate.AddDays(1)))
            {

                // Get all active employees
                var employees = _employeeInfoService.GetAllWithAllowance();

                int totalWorkHoursPerDay = Convert.ToInt32(_settingService.GetByKey(PAYROLL_TOTAL_HOURS));

                foreach (EmployeeInfo employee in employees)
                {
                    double totalHours = 0;
                    //Get regular and OT hours per day
                    List<TotalEmployeeHours> employeeTotalHours =
                       new List<TotalEmployeeHours>(_totalEmployeeHourService
                           .GetByTypeAndDateRange(employee.EmployeeId, RateType.Regular, payrollStartDate, payrollEndDate));

                    employeeTotalHours.AddRange(_totalEmployeeHourService
                           .GetByTypeAndDateRange(employee.EmployeeId, RateType.OverTime, payrollStartDate, payrollEndDate));
                    
                    employeeTotalHours = employeeTotalHours.OrderByDescending(e => e.Date).ToList();

                    if (employeeTotalHours != null && employeeTotalHours.Count > 1)
                    {
                        DateTime? tempDate = null;
                        double dayHours = 0;

                        var last = employeeTotalHours.Last();
                        foreach (TotalEmployeeHours employeeHours in employeeTotalHours)
                        {
                            //If different date add dayhours to totalhours and set dat hours to 0
                            if (tempDate != null && tempDate != employeeHours.Date)
                            {
                                totalHours += (dayHours > totalWorkHoursPerDay ?
                                    totalWorkHoursPerDay : dayHours);
                                dayHours = 0;
                            }

                            dayHours = dayHours + employeeHours.Hours;
                            tempDate = employeeHours.Date;

                            //If last iteration
                            if (last.Equals(employeeHours))
                            {
                                totalHours += (dayHours > totalWorkHoursPerDay
                                    ? totalWorkHoursPerDay : dayHours);
                            }
                        }

                        //Compute total allowance
                        var totalDays = Convert.ToInt32(_settingService.GetByKey(ALLOWANCE_TOTAL_DAYS));
                        var totalAllowanceHours = totalDays * totalWorkHoursPerDay;

                        Decimal totalAllowancePerHour = employee.Allowance.Value /
                                ((decimal)totalDays * (decimal)totalWorkHoursPerDay);

                        Decimal totalAllowance = (decimal)totalHours * totalAllowancePerHour;

                        //Update employee payroll
                        EmployeePayroll employeePayroll = payrollList.OfType<EmployeePayroll>()
                              .Where(p => p.EmployeeId == employee.EmployeeId).FirstOrDefault();
                        employeePayroll.TotalAllowance = totalAllowance;
                        employeePayroll.TaxableIncome = decimal.Add(employeePayroll.TaxableIncome, totalAllowance);
                        employeePayroll.TotalGross = decimal.Add(employeePayroll.TotalGross, totalAllowance);
                        employeePayroll.TotalNet = employeePayroll.TotalGross;
                        _unitOfWork.Commit();
                    }
                }
            }
        }

        public DateTime GetNextPayrollStartDate()
        {
            return GetNextPayrollStartDate(null);
        }

        public virtual IEnumerable<string> GetPayrollDates(int months)
        {
            var dates = new List<string>();
            var nextPayrollDate = _employeePayrollRepository.GetNextPayrollStartDate(); //this is actually the last payroll date

            if (nextPayrollDate == null)
            {
                var weekStart = Convert.ToInt32(_settingService.GetByKey(PAYROLL_WEEK_START));
                nextPayrollDate = DateTime.Now.StartOfWeek((DayOfWeek) weekStart);
            }
            //else
            //{
            //    //for some reason it adds 1 day in GetNextPayrollStartDate
            //    nextPayrollDate = nextPayrollDate.Value.AddDays(-1);
            //}
                
            //if (nextPayrollDate == null)
            //{
            //    var weekStart = Convert.ToInt32(_settingService.GetByKey(PAYROLL_WEEK_START));

            //    return new List<string>()
            //    {
            //        String.Format("{0} to {1}", DateTime.Now.ToString("MMMM dd yyyy"), DateTime.Now.StartOfWeek((DayOfWeek) weekStart).ToString("MMMM dd yyyy"))
            //    };
            //}

            var lastPayrollDate = nextPayrollDate.Value; //nextPayrollDate.Value.AddDays(-1);
            var lastPayroll = lastPayrollDate.AddMonths(-months);
           
            while (lastPayrollDate >= lastPayroll)
            {
                var tempDate = new DateTime(lastPayrollDate.Year, lastPayrollDate.Month, lastPayrollDate.Day);
                
                if (_frequency == FrequencyType.Weekly)
                    tempDate = tempDate.AddDays(-7);
                else if (_frequency == FrequencyType.Monthly)
                    tempDate = tempDate.AddMonths(-1);
                else if (_frequency == FrequencyType.Daily)
                    tempDate = tempDate.AddDays(-1);
                else //fallback to monthly
                    tempDate = tempDate.AddMonths(-1);

                var date = String.Format("{0} to {1}", tempDate.ToString("MMMM dd yyyy"), lastPayrollDate.AddDays(-1).ToString("MMMM dd yyyy"));
                lastPayrollDate = tempDate;
                dates.Add(date);
            }

            return dates;
        }

        public void GenerateDeductions(DateTime payrollDate, DateTime payrollStartDate,
            DateTime payrollEndDate, IList<EmployeePayroll> employeePayrolls)
        {
            //If proceed is false return
            if(!_employeePayrollDeductionService
                .proceedDeduction(payrollStartDate, payrollEndDate))
            {
                return;
            }

            foreach (EmployeePayroll payroll in employeePayrolls)
            {
                //Generate deductions such as SSS, HDMF, Philhealth and TAX
                var totalDeductions = 
                    _employeePayrollDeductionService.GenerateDeductionsByPayroll(payroll);

                //Update employeePayroll total deductions and taxable income
                Update(payroll);
                payroll.TaxableIncome = payroll.TotalGross - totalDeductions;
                payroll.TotalDeduction += totalDeductions;

                //Compute Tax
                GenerateTax(payroll);
            }

            try
            {
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                //Print error
            }
        }

        public void GenerateTax(EmployeePayroll payroll)
        {
            //Tax computation
            //Get old payroll for tax computation
            var payrollForTaxProcessing = 
                GetForTaxProcessingByEmployee(payroll.EmployeeId, payroll.PayrollDate);

            decimal totalTaxableIncome = payroll.TaxableIncome;
            foreach (EmployeePayroll employeePayroll in payrollForTaxProcessing)
            {
                totalTaxableIncome += employeePayroll.TaxableIncome;

                //Update Payroll
                Update(employeePayroll);
                employeePayroll.IsTaxed = true;
            }

            //Compute tax
            var employeeInfo = _employeeInfoService.GetByEmployeeId(payroll.EmployeeId);
            var totalTax = _employeePayrollDeductionService
                .ComputeTax(payroll.PayrollId, employeeInfo, totalTaxableIncome);

            //Update payroll for total deductions and total grosss
            payroll.TotalDeduction += totalTax;
            payroll.TotalNet = payroll.TotalGross - payroll.TotalDeduction;
            payroll.IsTaxed = true;
        }

        public IList<EmployeePayroll> GetByPayrollDateRange(DateTime payrollStartDate, DateTime payrollEndDate)
        {
            return _employeePayrollRepository.GetByPayrollDateRange(payrollStartDate, payrollEndDate);
        }
    }
}
