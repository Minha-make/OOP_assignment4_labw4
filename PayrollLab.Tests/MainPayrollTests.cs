using System;
using Xunit;
using PayrollLab;

namespace PayrollLab.Tests
{
    public class MainPayrollTests
    {
        private static Payroll CreateFullPayroll()
        {
            var payroll = new Payroll("2026-09");

            var e001 = new SalariedEmployee("E001", "Nguyen Minh An", "Dao tao",
                                             15_000_000m, 2_000_000m);
            // Chon overload 2 AddBonus(amount, reason) luc bien dich do doi so khop (decimal, string)
            e001.AddBonus(1_000_000m, "Hoan thanh KPI");
            payroll.AddEmployee(e001);

            var e002 = new HourlyEmployee("E002", "Tran Thu Binh", "Ho tro",
                                           100_000m, 150m);
            // Chon overload 1 AddBonus(amount) luc bien dich do doi so chi co mot gia tri kieu decimal
            e002.AddBonus(500_000m);
            payroll.AddEmployee(e002);

            var e003 = new HourlyEmployee("E003", "Le Hoang Chi", "Ho tro",
                                           100_000m, 170m);
            payroll.AddEmployee(e003);

            var e004 = new SalesEmployee("E004", "Pham Quoc Dung", "Kinh doanh",
                                          8_000_000m, 200_000_000m, 0.05m);
            // Chon overload 3 AddBonus(rate, ref, reason) luc bien dich do doi so khop (decimal, decimal, string)
            e004.AddBonus(0.02m, 50_000_000m, "Thuong doanh so");
            payroll.AddEmployee(e004);

            return payroll;
        }

        [Fact]
        public void E001_SalariedEmployee_GrossPay_ShouldBe18Million()
        {
            var payroll = CreateFullPayroll();
            var e001 = payroll.FindEmployee("E001");

            Assert.NotNull(e001);
            Assert.Equal(18_000_000m, e001.CalculateGrossPay());
        }

        [Fact]
        public void E002_HourlyEmployee_150Hours_GrossPay_ShouldBe15Point5Million()
        {
            var payroll = CreateFullPayroll();
            var e002 = payroll.FindEmployee("E002");

            Assert.NotNull(e002);
            Assert.Equal(15_500_000m, e002.CalculateGrossPay());
        }

        [Fact]
        public void E003_HourlyEmployee_170Hours_WithOT_GrossPay_ShouldBe17Point5Million()
        {
            var payroll = CreateFullPayroll();
            var e003 = payroll.FindEmployee("E003");

            Assert.NotNull(e003);
            Assert.Equal(17_500_000m, e003.CalculateGrossPay());
        }

        [Fact]
        public void E004_SalesEmployee_GrossPay_ShouldBe19Million()
        {
            var payroll = CreateFullPayroll();
            var e004 = payroll.FindEmployee("E004");

            Assert.NotNull(e004);
            Assert.Equal(19_000_000m, e004.CalculateGrossPay());
        }

        [Fact]
        public void TotalPayroll_ShouldBe70Million()
        {
            var payroll = CreateFullPayroll();

            Assert.Equal(70_000_000m, payroll.CalculateTotalPayroll());
        }

        [Fact]
        public void PayrollByDepartment_HoTro_ShouldBe33Million()
        {
            var payroll = CreateFullPayroll();

            Assert.Equal(33_000_000m, payroll.CalculatePayrollByDepartment("Ho tro"));
        }

        [Fact]
        public void PayrollByDepartment_DaoTao_ShouldBe18Million()
        {
            var payroll = CreateFullPayroll();

            Assert.Equal(18_000_000m, payroll.CalculatePayrollByDepartment("Dao tao"));
        }

        [Fact]
        public void PayrollByDepartment_KinhDoanh_ShouldBe19Million()
        {
            var payroll = CreateFullPayroll();

            Assert.Equal(19_000_000m, payroll.CalculatePayrollByDepartment("Kinh doanh"));
        }

        [Fact]
        public void HighestPaidEmployee_ShouldBeE004()
        {
            var payroll = CreateFullPayroll();
            var highest = payroll.FindHighestPaidEmployee();

            Assert.NotNull(highest);
            Assert.Equal("E004", highest.EmployeeId);
            Assert.Equal("Pham Quoc Dung", highest.FullName);
            Assert.Equal(19_000_000m, highest.CalculateGrossPay());
        }

        [Fact]
        public void EmployeeTypes_ShouldReturnCorrectTypes()
        {
            var payroll = CreateFullPayroll();

            Assert.Equal("Salaried", payroll.FindEmployee("E001")!.GetEmployeeType());
            Assert.Equal("Hourly", payroll.FindEmployee("E002")!.GetEmployeeType());
            Assert.Equal("Hourly", payroll.FindEmployee("E003")!.GetEmployeeType());
            Assert.Equal("Sales", payroll.FindEmployee("E004")!.GetEmployeeType());
        }

        [Fact]
        public void DisplayPayroll_ShouldNotThrow()
        {
            var payroll = CreateFullPayroll();

            var exception = Record.Exception(() => payroll.DisplayPayroll());
            Assert.Null(exception);
        }
    }
}
