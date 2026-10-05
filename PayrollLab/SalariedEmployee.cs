using System;

namespace PayrollLab
{
    /// <summary>
    /// Dai dien cho nhan vien huong luong co dinh theo thang va phu cap trach nhiem.
    /// </summary>
    public class SalariedEmployee : Employee
    {
        /// <summary>Muc luong cung co dinh hang thang.</summary>
        public decimal MonthlySalary { get; private set; }

        /// <summary>Tien phu cap trach nhiem theo vi tri cong viec.</summary>
        public decimal ResponsibilityAllowance { get; private set; }

        /// <summary>
        /// Khoi tao nhan vien huong luong co dinh voi day du thong tin va phu cap.
        /// </summary>
        /// <param name="id">Ma nhan vien, khong duoc rong.</param>
        /// <param name="name">Ho ten nhan vien, khong duoc rong.</param>
        /// <param name="department">Phong ban, khong duoc rong.</param>
        /// <param name="monthlySalary">Muc luong thang, phai lon hon hoac bang 0.</param>
        /// <param name="responsibilityAllowance">Phu cap trach nhiem, phai lon hon hoac bang 0.</param>
        /// <exception cref="ArgumentException">Nem ra khi chuoi thong tin rong hoac khoang trang.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Nem ra khi luong hoac phu cap am.</exception>
        // Constructor uy quyen: chuyen tiep thong tin dinh danh len constructor lop cha
        public SalariedEmployee(string id, string name, string department,
                                decimal monthlySalary, decimal responsibilityAllowance)
            : base(id, name, department)
        {
            ValidateNonNegative(monthlySalary, nameof(monthlySalary));
            ValidateNonNegative(responsibilityAllowance, nameof(responsibilityAllowance));

            MonthlySalary = monthlySalary;
            ResponsibilityAllowance = responsibilityAllowance;
        }

        /// <summary>
        /// Khoi tao nhan vien huong luong co dinh voi phong ban mac dinh va khong co phu cap.
        /// </summary>
        /// <param name="id">Ma nhan vien, khong duoc rong.</param>
        /// <param name="name">Ho ten nhan vien, khong duoc rong.</param>
        /// <param name="monthlySalary">Muc luong thang, phai lon hon hoac bang 0.</param>
        /// <exception cref="ArgumentException">Nem ra khi chuoi thong tin rong hoac khoang trang.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Nem ra khi luong am.</exception>
        // Constructor uy quyen: goi constructor 5 tham so voi phu cap bang 0 va phong ban mac dinh
        public SalariedEmployee(string id, string name, decimal monthlySalary)
            : this(id, name, "Unassigned", monthlySalary, 0m)
        {
        }

        /// <summary>
        /// Tinh tong thu nhap gop gom luong thang, phu cap trach nhiem va thuong.
        /// </summary>
        /// <returns>Tong thu nhap gop trong thang.</returns>
        // Ghi de (override): tinh luong co dinh, duoc trinh thuc thi CLR dispatch luc chay
        public override decimal CalculateGrossPay()
        {
            return MonthlySalary + ResponsibilityAllowance + MonthlyBonus;
        }

        /// <summary>
        /// Lay phan loai nhan vien luong co dinh.
        /// </summary>
        /// <returns>Chuoi "Salaried".</returns>
        // Ghi de (override): tra ve ten loai nhan su theo co che da hinh luc chay
        public override string GetEmployeeType()
        {
            return "Salaried";
        }

        /// <summary>
        /// Hien thi thong tin chi tiet bang luong cua nhan vien huong luong co dinh.
        /// </summary>
        // Ghi de (override): tai su dung hien thi tu lop cha va bo sung chi tiet luong co dinh
        public override void DisplayPayrollInfo()
        {
            base.DisplayPayrollInfo();
            Console.WriteLine($"  Luong thang : {MonthlySalary:N0} d");
            Console.WriteLine($"  Phu cap     : {ResponsibilityAllowance:N0} d");
            Console.WriteLine($"  TONG THU NHAP: {CalculateGrossPay():N0} d");
            Console.WriteLine();
        }
    }
}
