﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Entities
{
    [Table("employee")]
    public class Employee
    {
        public Employee()
        {
            Enabled = true;
        }

        [Key]
        public int EmployeeId { get; set; }
            
        [StringLength(250)]
        public string EmployeeCode { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(50)]
        public string MiddleName { get; set; }
        
        public DateTime BirthDate { get; set; }

        [StringLength(500)]
        public string Picture { get; set; }

        public bool IsActive { get; set; }

        //For clock in/out (biomertrics and rfid)
        public int Privilege { get; set; }
        public bool Enabled { get; set; }
        public bool EnrolledToRfid { get; set; }
        public bool EnrolledToBiometrics { get; set; }

        [NotMapped]
        public string FullName
        {
            get { return String.Format("{0} {1}", FirstName, LastName); }
        }
    }
}
