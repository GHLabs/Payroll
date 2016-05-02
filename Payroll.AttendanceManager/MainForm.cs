﻿using System.Linq;
using System.Windows.Forms;
using Payroll.Repository.Interface;
using Payroll.Repository.Repositories;
using System;

namespace AttendanceManager
{
    public partial class MainForm : Form
    {

        public IMachineRepository _machineRepository;

        public MainForm()
        {
            InitializeComponent();

            _machineRepository = new MachineRepository(Program._databaseFactory);
            var machines = _machineRepository.Find(x => x.IsActive).ToList();

            gvMachine.AutoGenerateColumns = false;
            gvMachine.DataSource = machines;
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            var machineId = gvMachine.CurrentRow.Cells[0].Value;
            var ipAddress = gvMachine.CurrentRow.Cells[1].Value;

            var newForm = new MachineForm();
            newForm._ipAddress = ipAddress.ToString();
            newForm._machineNumber = Convert.ToInt32(machineId);
            newForm.Show();
        }
    }
}
