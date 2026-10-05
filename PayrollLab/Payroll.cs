using System;
using System.Collections.Generic;
using System.Linq;

namespace PayrollLab
{
    /// <summary>
    /// Quan ly bang luong tap trung cho cac nhan vien trong mot ky cu the.
    /// </summary>
    public class Payroll
    {
        // Luu List<Employee> de quan ly da hinh; khong dung is/switch nham tuan thu OCP khi them loai nhan vien moi
        private readonly List<Employee> _employees = new();

        /// <summary>Ky tinh luong (vi du: "2026-09").</summary>
        public string Period { get; private set; }

        /// <summary>Danh sach nhan vien trong bang luong (chi doc).</summary>
        public IReadOnlyList<Employee> Employees => _employees.AsReadOnly();

        /// <summary>
        /// Khoi tao bang luong cho mot ky lam viec nhat dinh.
        /// </summary>
        /// <param name="period">Chuoi dinh danh ky luong, khong duoc rong hoac chi chua khoang trang.</param>
        /// <exception cref="ArgumentException">Nem ra khi period rong hoac chi chua khoang trang.</exception>
        public Payroll(string period)
        {
            if (string.IsNullOrWhiteSpace(period))
                throw new ArgumentException(
                    "Ky luong khong duoc rong hoac chi chua khoang trang.", nameof(period));

            Period = period;
        }

        /// <summary>
        /// Them mot nhan vien vao bang luong cua ky hien hanh.
        /// </summary>
        /// <param name="employee">Doi tuong nhan vien can them, khong duoc null.</param>
        /// <exception cref="ArgumentNullException">Nem ra khi employee mang gia tri null.</exception>
        /// <exception cref="ArgumentException">Nem ra khi EmployeeId da ton tai trong bang luong.</exception>
        public void AddEmployee(Employee employee)
        {
            if (employee is null)
                throw new ArgumentNullException(nameof(employee),
                    "Nhan vien khong duoc null.");

            if (_employees.Any(e => e.EmployeeId == employee.EmployeeId))
                throw new ArgumentException(
                    $"Nhan vien voi ma '{employee.EmployeeId}' da ton tai trong bang luong.",
                    nameof(employee));

            _employees.Add(employee);
        }

        /// <summary>
        /// Tim kiem nhan vien trong bang luong theo ma nhan vien.
        /// </summary>
        /// <param name="employeeId">Ma nhan vien can tim.</param>
        /// <returns>Doi tuong nhan vien neu tim thay, nguoc lai tra ve null.</returns>
        public Employee? FindEmployee(string employeeId)
        {
            return _employees.FirstOrDefault(e => e.EmployeeId == employeeId);
        }

        /// <summary>
        /// Tinh tong chi phi chi tra luong cho toan bo nhan vien trong ky.
        /// </summary>
        /// <returns>Tong so tien luong gop cua toan bo bang luong.</returns>
        // Khong static vi phuong thuc phu thuoc vao du lieu cua tung doi tuong ky luong cu the
        public decimal CalculateTotalPayroll()
        {
            return _employees.Sum(e => e.CalculateGrossPay());
        }

        /// <summary>
        /// Tinh tong chi phi chi tra luong theo mot phong ban cu the.
        /// </summary>
        /// <param name="department">Ten phong ban can tong hop luong.</param>
        /// <returns>Tong luong gop cua cac nhan vien thuoc phong ban do.</returns>
        public decimal CalculatePayrollByDepartment(string department)
        {
            return _employees
                .Where(e => e.Department == department)
                .Sum(e => e.CalculateGrossPay());
        }

        /// <summary>
        /// Tim nhan vien co muc thu nhap gop cao nhat trong ky tinh luong.
        /// </summary>
        /// <returns>Doi tuong nhan vien co thu nhap cao nhat, hoac null neu bang luong rong.</returns>
        public Employee? FindHighestPaidEmployee()
        {
            if (_employees.Count == 0)
                return null;

            return _employees.MaxBy(e => e.CalculateGrossPay());
        }

        /// <summary>
        /// Xuat chi tiet bang luong cua toan bo nhan vien va bao cao tong ket ra console.
        /// </summary>
        public void DisplayPayroll()
        {
            Console.WriteLine("+----------------------------------------------+");
            Console.WriteLine($"|       BANG LUONG KY: {Period,-24}|");
            Console.WriteLine("+----------------------------------------------+");

            if (_employees.Count == 0)
            {
                Console.WriteLine("  (Bang luong rong - chua co nhan vien nao.)");
                Console.WriteLine();
                return;
            }

            foreach (var emp in _employees)
            {
                emp.DisplayPayrollInfo();
            }

            Console.WriteLine("-----------------------------------------------");
            Console.WriteLine($"  Tong so nhan vien : {_employees.Count}");
            Console.WriteLine($"  TONG BANG LUONG   : {CalculateTotalPayroll():N0} d");

            var highest = FindHighestPaidEmployee();
            if (highest != null)
            {
                Console.WriteLine($"  Thu nhap cao nhat : {highest.FullName} " +
                                  $"({highest.EmployeeId}) - {highest.CalculateGrossPay():N0} d");
            }

            Console.WriteLine();
        }
    }
}
