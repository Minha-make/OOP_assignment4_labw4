using System;

namespace PayrollLab
{
    /// <summary>
    /// Dai dien cho nhan vien lam viec huong thu lao theo gio lam viec thuc te.
    /// </summary>
    public class HourlyEmployee : Employee
    {
        private const decimal StandardHours = 160m;
        private const decimal OvertimeMultiplier = 1.5m;
        private const decimal MinWorkedHours = 0m;
        private const decimal MaxWorkedHours = 250m;

        /// <summary>Don gia thu lao cho moi gio lam viec tieu chuan.</summary>
        public decimal HourlyRate { get; private set; }

        /// <summary>Tong so gio lam viec thuc te trong thang.</summary>
        public decimal WorkedHours { get; private set; }

        /// <summary>
        /// Khoi tao nhan vien theo gio voi day du don gia va so gio lam viec.
        /// </summary>
        /// <param name="id">Ma nhan vien, khong duoc rong.</param>
        /// <param name="name">Ho ten nhan vien, khong duoc rong.</param>
        /// <param name="department">Phong ban, khong duoc rong.</param>
        /// <param name="hourlyRate">Don gia moi gio, phai lon hon hoac bang 0.</param>
        /// <param name="workedHours">So gio lam viec trong thang, phai tu 0 den 250 gio.</param>
        /// <exception cref="ArgumentException">Nem ra khi chuoi thong tin rong hoac khoang trang.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Nem ra khi don gia hoac so gio lam ngoai khoang cho phep.</exception>
        // Constructor uy quyen: chuyen tiep thong tin dinh danh len constructor lop cha
        public HourlyEmployee(string id, string name, string department,
                              decimal hourlyRate, decimal workedHours)
            : base(id, name, department)
        {
            ValidateNonNegative(hourlyRate, nameof(hourlyRate));
            ValidateWorkedHours(workedHours);

            HourlyRate = hourlyRate;
            WorkedHours = workedHours;
        }

        /// <summary>
        /// Khoi tao nhan vien theo gio voi phong ban mac dinh va gio lam bang 0.
        /// </summary>
        /// <param name="id">Ma nhan vien, khong duoc rong.</param>
        /// <param name="name">Ho ten nhan vien, khong duoc rong.</param>
        /// <param name="hourlyRate">Don gia moi gio, phai lon hon hoac bang 0.</param>
        /// <exception cref="ArgumentException">Nem ra khi chuoi thong tin rong hoac khoang trang.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Nem ra khi don gia am.</exception>
        // Constructor uy quyen: goi constructor 5 tham so voi gio lam bang 0 va phong ban mac dinh
        public HourlyEmployee(string id, string name, decimal hourlyRate)
            : this(id, name, "Unassigned", hourlyRate, 0m)
        {
        }

        /// <summary>
        /// Tinh thu nhap gop gom luong gio chuan, thu lao ngoai gio (he so 1.5) va thuong.
        /// </summary>
        /// <returns>Tong thu nhap gop trong thang.</returns>
        // Ghi de (override): tinh luong theo gio, duoc trinh thuc thi CLR dispatch luc chay
        public override decimal CalculateGrossPay()
        {
            decimal basePay;

            if (WorkedHours <= StandardHours)
            {
                basePay = WorkedHours * HourlyRate;
            }
            else
            {
                // Cong thuc gio vuot 160: 160h dau tinh luong chuan, phan vuot nhan he so lam them 1.5
                basePay = StandardHours * HourlyRate
                        + (WorkedHours - StandardHours) * HourlyRate * OvertimeMultiplier;
            }

            return basePay + MonthlyBonus;
        }

        /// <summary>
        /// Lay phan loai nhan vien lam viec theo gio.
        /// </summary>
        /// <returns>Chuoi "Hourly".</returns>
        // Ghi de (override): tra ve ten loai nhan su theo co che da hinh luc chay
        public override string GetEmployeeType()
        {
            return "Hourly";
        }

        /// <summary>
        /// Hien thi thong tin bang luong cua nhan vien theo gio va chi tiet lam them (neu co).
        /// </summary>
        // Ghi de (override): tai su dung hien thi tu lop cha va in them chi tiet don gia, gio lam
        public override void DisplayPayrollInfo()
        {
            base.DisplayPayrollInfo();
            Console.WriteLine($"  Don gia gio : {HourlyRate:N0} d");
            Console.WriteLine($"  So gio lam  : {WorkedHours:N0} gio");

            if (WorkedHours > StandardHours)
            {
                decimal normalPay = StandardHours * HourlyRate;
                decimal overtimePay = (WorkedHours - StandardHours) * HourlyRate * OvertimeMultiplier;
                Console.WriteLine($"  Luong chuan : {normalPay:N0} d (160 gio)");
                Console.WriteLine($"  Luong OT    : {overtimePay:N0} d ({WorkedHours - StandardHours:N0} gio x {OvertimeMultiplier})");
            }
            else
            {
                decimal normalPay = WorkedHours * HourlyRate;
                Console.WriteLine($"  Luong chuan : {normalPay:N0} d ({WorkedHours:N0} gio)");
            }

            Console.WriteLine($"  TONG THU NHAP: {CalculateGrossPay():N0} d");
            Console.WriteLine();
        }

        private static void ValidateWorkedHours(decimal hours)
        {
            if (hours < MinWorkedHours || hours > MaxWorkedHours)
                throw new ArgumentOutOfRangeException(nameof(hours),
                    $"So gio lam viec phai nam trong khoang [{MinWorkedHours}, {MaxWorkedHours}].");
        }
    }
}
