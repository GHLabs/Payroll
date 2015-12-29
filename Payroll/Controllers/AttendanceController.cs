﻿using System;
using System.Web.Mvc;
using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;

namespace Payroll.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AttendanceController(IAttendanceRepository attendanceRepository, IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork)
        {
            _attendanceRepository = attendanceRepository;
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
        }

        public void ClockIn(string code)
        {
            var employee = _employeeRepository.GetByCode(code);
            if (employee != null)
            {
                var attendance = new Attendance()
                {
                    EmployeeId = employee.EmployeeId,
                    ClockIn = DateTime.Now,
                    ClockOut = null
                };

                _attendanceRepository.Add(attendance);
                _unitOfWork.Commit();
            }
        }


        public void ClockOut(string code)
        {
            var employee = _employeeRepository.GetByCode(code);
            
            if (employee != null)
            {
                var lastClockIn = _attendanceRepository.GetLastAttendance(employee.EmployeeId);

                if (lastClockIn != null)
                {
                    lastClockIn.ClockOut = DateTime.Now;
                    _attendanceRepository.Update(lastClockIn, new[] {"ClockOut"});
                    _unitOfWork.Commit();
                }
                else
                {
                    //employee did not clocked in
                }
            }
        }
    }
}