using System;

namespace PayrollLab
{
    /// <summary>
    /// Dai dien cho nhan vien kinh doanh huong luong co ban va hoa hong doanh so.
    /// </summary>
    public class SalesEmployee : Employee
    {
        private const decimal MinCommissionRate = 0m;
        private const decimal MaxCommissionRate = 0.3m;

        /// <summary>Muc luong co ban hang thang.</summary>
        public decimal BaseSalary { get; private set; }

        /// <summary>Doanh so ban hang dat duoc trong thang.</summary>
        public decimal SalesRevenue { get; private set; }

        /// <summary>Ty le hoa hong tren doanh so ban hang (tu 0 den 0.3).</summary>
        public decimal CommissionRate { get; private set; }

        /// <summary>
        /// Khoi tao nhan vien kinh doanh voi day du luong cung, doanh so va ty le hoa hong.
        /// </summary>
        /// <param name="id">Ma nhan vien, khong duoc rong.</param>
        /// <param name="name">Ho ten nhan vien, khong duoc rong.</param>
        /// <param name="department">Phong ban, khong duoc rong.</param>
        /// <param name="baseSalary">Luong co ban, phai lon hon hoac bang 0.</param>
        /// <param name="salesRevenue">Doanh so, phai lon hon hoac bang 0.</param>
        /// <param name="commissionRate">Ty le hoa hong, phai nam trong khoang [0, 0.3].</param>
        /// <exception cref="ArgumentException">Nem ra khi chuoi thong tin rong hoac khoang trang.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Nem ra khi luong/doanh so am hoac ty le hoa hong ngoai [0, 0.3].</exception>
        // Constructor uy quyen: chuyen tiep thong tin dinh danh len constructor lop cha
        public SalesEmployee(string id, string name, string department,
                             decimal baseSalary, decimal salesRevenue, decimal commissionRate)
            : base(id, name, department)
        {
            ValidateNonNegative(baseSalary, nameof(baseSalary));
            ValidateNonNegative(salesRevenue, nameof(salesRevenue));
            ValidateCommissionRate(commissionRate);

            BaseSalary = baseSalary;
            SalesRevenue = salesRevenue;
            CommissionRate = commissionRate;
        }

        /// <summary>
        /// Khoi tao nhan vien kinh doanh voi phong ban mac dinh va doanh so khoi diem bang 0.
        /// </summary>
        /// <param name="id">Ma nhan vien, khong duoc rong.</param>
        /// <param name="name">Ho ten nhan vien, khong duoc rong.</param>
        /// <param name="baseSalary">Luong co ban, phai lon hon hoac bang 0.</param>
        /// <param name="commissionRate">Ty le hoa hong, phai nam trong khoang [0, 0.3].</param>
        /// <exception cref="ArgumentException">Nem ra khi chuoi thong tin rong hoac khoang trang.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Nem ra khi luong am hoac ty le hoa hong ngoai [0, 0.3].</exception>
        // Constructor uy quyen: goi constructor 6 tham so voi doanh so bang 0 va phong ban mac dinh
        public SalesEmployee(string id, string name, decimal baseSalary, decimal commissionRate)
            : this(id, name, "Unassigned", baseSalary, 0m, commissionRate)
        {
        }

        /// <summary>
        /// Tinh tong thu nhap gop gom luong co ban, tien hoa hong doanh so va thuong thang.
        /// </summary>
        /// <returns>Tong thu nhap gop trong thang.</returns>
        // Ghi de (override): tinh luong kinh doanh theo hoa hong, duoc chon luc chay theo doi tuong thuc te
        public override decimal CalculateGrossPay()
        {
            return BaseSalary + SalesRevenue * CommissionRate + MonthlyBonus;
        }

        /// <summary>
        /// Lay phan loai nhan vien kinh doanh.
        /// </summary>
        /// <returns>Chuoi "Sales".</returns>
        // Ghi de (override): tra ve ten loai nhan su theo co che da hinh luc chay
        public override string GetEmployeeType()
        {
            return "Sales";
        }

        /// <summary>
        /// Hien thi thong tin bang luong cua nhan vien kinh doanh kem chi tiet hoa hong.
        /// </summary>
        // Ghi de (override): tai su dung hien thi tu lop cha va in them chi tiet doanh so, hoa hong
        public override void DisplayPayrollInfo()
        {
            base.DisplayPayrollInfo();
            Console.WriteLine($"  Luong co ban: {BaseSalary:N0} d");
            Console.WriteLine($"  Doanh so    : {SalesRevenue:N0} d");
            Console.WriteLine($"  Hoa hong    : {CommissionRate:P0} ({SalesRevenue * CommissionRate:N0} d)");
            Console.WriteLine($"  TONG THU NHAP: {CalculateGrossPay():N0} d");
            Console.WriteLine();
        }

        /// <summary>
        /// Cap nhat doanh so ban hang moi dat duoc trong ky tinh luong.
        /// </summary>
        /// <param name="newRevenue">Doanh so moi, phai lon hon hoac bang 0.</param>
        /// <exception cref="ArgumentOutOfRangeException">Nem ra khi newRevenue nho hon 0.</exception>
        public void UpdateSalesRevenue(decimal newRevenue)
        {
            ValidateNonNegative(newRevenue, nameof(newRevenue));
            SalesRevenue = newRevenue;
        }

        private static void ValidateCommissionRate(decimal rate)
        {
            if (rate < MinCommissionRate || rate > MaxCommissionRate)
                throw new ArgumentOutOfRangeException(nameof(rate),
                    $"Ty le hoa hong phai nam trong khoang [{MinCommissionRate}, {MaxCommissionRate}].");
        }
    }
}
