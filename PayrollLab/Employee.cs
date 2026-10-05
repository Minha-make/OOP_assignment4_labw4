using System;

namespace PayrollLab
{
    /// <summary>
    /// Lop truu tuong co so dai dien cho nhan vien trong he thong.
    /// </summary>
    public abstract class Employee
    {
        /// <summary>Ma dinh danh duy nhat cua nhan vien.</summary>
        public string EmployeeId { get; private set; }

        /// <summary>Ho va ten day du cua nhan vien.</summary>
        public string FullName { get; private set; }

        /// <summary>Phong ban lam viec cua nhan vien.</summary>
        public string Department { get; private set; }

        /// <summary>Tien thuong tich luy trong thang hien hanh.</summary>
        public decimal MonthlyBonus { get; private set; }

        /// <summary>
        /// Khoi tao nhan vien voi day du thong tin dinh danh va phong ban.
        /// </summary>
        /// <param name="id">Ma nhan vien, khong duoc rong hoac chi chua khoang trang.</param>
        /// <param name="name">Ho ten nhan vien, khong duoc rong hoac chi chua khoang trang.</param>
        /// <param name="department">Phong ban, khong duoc rong hoac chi chua khoang trang.</param>
        /// <exception cref="ArgumentException">Nem ra khi id, name hoac department rong hoac chi chua khoang trang.</exception>
        public Employee(string id, string name, string department)
        {
            ValidateStringNotEmpty(id, nameof(id));
            ValidateStringNotEmpty(name, nameof(name));
            ValidateStringNotEmpty(department, nameof(department));

            EmployeeId = id;
            FullName = name;
            Department = department;
            MonthlyBonus = 0m;
        }

        /// <summary>
        /// Khoi tao nhan vien voi phong ban mac dinh la "Unassigned".
        /// </summary>
        /// <param name="id">Ma nhan vien, khong duoc rong hoac chi chua khoang trang.</param>
        /// <param name="name">Ho ten nhan vien, khong duoc rong hoac chi chua khoang trang.</param>
        /// <exception cref="ArgumentException">Nem ra khi id hoac name rong hoac chi chua khoang trang.</exception>
        // Constructor uy quyen: goi constructor 3 tham so va gan phong ban mac dinh
        public Employee(string id, string name)
            : this(id, name, "Unassigned")
        {
        }

        private void ApplyBonus(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount),
                    "So tien thuong phai lon hon 0.");

            MonthlyBonus += amount;
        }

        /// <summary>
        /// Them tien thuong co dinh vao khoan thuong thang cua nhan vien.
        /// </summary>
        /// <param name="amount">So tien thuong can them, phai lon hon 0.</param>
        /// <exception cref="ArgumentOutOfRangeException">Nem ra khi amount nho hon hoac bang 0.</exception>
        public void AddBonus(decimal amount)
        {
            ApplyBonus(amount);
        }

        /// <summary>
        /// Them tien thuong co dinh kem ly do khen thuong.
        /// </summary>
        /// <param name="amount">So tien thuong, phai lon hon 0.</param>
        /// <param name="reason">Ly do thuong, khong duoc rong hoac chi chua khoang trang.</param>
        /// <exception cref="ArgumentOutOfRangeException">Nem ra khi amount nho hon hoac bang 0.</exception>
        /// <exception cref="ArgumentException">Nem ra khi reason rong hoac chi chua khoang trang.</exception>
        public void AddBonus(decimal amount, string reason)
        {
            ValidateStringNotEmpty(reason, nameof(reason));
            ApplyBonus(amount);
        }

        /// <summary>
        /// Them tien thuong tinh theo ty le phan tram tren gia tri tham chieu kem ly do.
        /// </summary>
        /// <param name="rate">Ty le thuong, phai nam trong khoang (0, 0.5].</param>
        /// <param name="referenceAmount">Gia tri tham chieu tinh thuong, phai lon hon 0.</param>
        /// <param name="reason">Ly do thuong, khong duoc rong hoac chi chua khoang trang.</param>
        /// <exception cref="ArgumentOutOfRangeException">Nem ra khi rate hoac referenceAmount ngoai khoang hop le.</exception>
        /// <exception cref="ArgumentException">Nem ra khi reason rong hoac chi chua khoang trang.</exception>
        public void AddBonus(decimal rate, decimal referenceAmount, string reason)
        {
            ValidateStringNotEmpty(reason, nameof(reason));

            if (rate <= 0m || rate > 0.5m)
                throw new ArgumentOutOfRangeException(nameof(rate),
                    "Ty le thuong phai nam trong khoang (0, 0.5].");

            if (referenceAmount <= 0m)
                throw new ArgumentOutOfRangeException(nameof(referenceAmount),
                    "Gia tri tham chieu phai lon hon 0.");

            decimal bonusAmount = rate * referenceAmount;
            ApplyBonus(bonusAmount);
        }

        /// <summary>
        /// Dat lai tien thuong thang ve 0 khi bat dau chu ky tinh luong moi.
        /// </summary>
        public void ResetMonthlyBonus()
        {
            MonthlyBonus = 0m;
        }

        /// <summary>
        /// Tinh tong thu nhap gop trong thang cua nhan vien.
        /// </summary>
        /// <returns>Tong so tien thu nhap gop cua nhan vien.</returns>
        // Phuong thuc truu tuong: dinh nghia giao dien chung, phien ban cu the duoc chon luc chay
        public abstract decimal CalculateGrossPay();

        /// <summary>
        /// Lay ten phan loai nhan vien.
        /// </summary>
        /// <returns>Ten chuoi dai dien cho loai nhan vien.</returns>
        // Phuong thuc truu tuong: buoc lop con cai dat loai nhan su tuong ung luc chay
        public abstract string GetEmployeeType();

        /// <summary>
        /// Hien thi thong tin bang luong co ban cua nhan vien ra man hinh console.
        /// </summary>
        public virtual void DisplayPayrollInfo()
        {
            Console.WriteLine($"=== THONG TIN BANG LUONG ===");
            Console.WriteLine($"  Ma NV       : {EmployeeId}");
            Console.WriteLine($"  Ho ten      : {FullName}");
            Console.WriteLine($"  Phong ban   : {Department}");
            Console.WriteLine($"  Loai NV     : {GetEmployeeType()}");
            Console.WriteLine($"  Thuong thang: {MonthlyBonus:N0} d");
        }

        private static void ValidateStringNotEmpty(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(
                    $"{fieldName} khong duoc rong hoac chi chua khoang trang.", fieldName);
        }

        /// <summary>
        /// Kiem tra gia tri so khong duoc am.
        /// </summary>
        /// <param name="value">Gia tri so can kiem tra, phai lon hon hoac bang 0.</param>
        /// <param name="fieldName">Ten truong phuc vu thong bao ngoai le.</param>
        /// <exception cref="ArgumentOutOfRangeException">Nem ra khi value nho hon 0.</exception>
        protected static void ValidateNonNegative(decimal value, string fieldName)
        {
            if (value < 0m)
                throw new ArgumentOutOfRangeException(fieldName,
                    $"{fieldName} khong duoc mang gia tri am.");
        }
    }
}
