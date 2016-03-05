﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities
{
    [Table("attendance")]
    public class Attendance
    {
        [Key]
        public int AttendanceId { get; set; }

        public int EmployeeId { get; set; }
        public DateTime ClockIn { get; set; }
        public DateTime ? ClockOut { get; set; }
        public bool ? IsManuallyEdited { get; set; }
    }
}
