using System;
using Xunit;
using PayrollLab;

namespace PayrollLab.Tests
{
    public class EdgeAndErrorTests
    {
        [Fact]
        public void Case01_HourlyEmployee_WorkedHours160_ExactBoundary_NoOvertime()
        {
            var emp = new HourlyEmployee("H001", "Tran Van Bien", "Ho tro", 100_000m, 160m);
            Assert.Equal(16_000_000m, emp.CalculateGrossPay());
        }

        [Fact]
        public void Case02_HourlyEmployee_WorkedHours0And250_ValidBoundaries()
        {
            var emp0 = new HourlyEmployee("H002", "Nguyen Khong Gio", "Ho tro", 100_000m, 0m);
            Assert.Equal(0m, emp0.CalculateGrossPay());

            var emp250 = new HourlyEmployee("H003", "Le Toi Da Gio", "Ho tro", 100_000m, 250m);
            Assert.Equal(29_500_000m, emp250.CalculateGrossPay());
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-0.01)]
        [InlineData(250.01)]
        [InlineData(251)]
        public void Case03_HourlyEmployee_WorkedHoursOutOfRange_ShouldThrow(decimal invalidHours)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new HourlyEmployee("H004", "Loi Gio", "Ho tro", 100_000m, invalidHours));
        }

        [Fact]
        public void Case04a_SalesEmployee_CommissionRate0And0Point3_ValidBoundaries()
        {
            var sales0 = new SalesEmployee("S001", "Hoa Hong Khong", "Kinh doanh", 10_000_000m, 100_000_000m, 0m);
            Assert.Equal(10_000_000m, sales0.CalculateGrossPay());

            var sales30 = new SalesEmployee("S002", "Hoa Hong Ba Muoi", "Kinh doanh", 10_000_000m, 100_000_000m, 0.3m);
            Assert.Equal(40_000_000m, sales30.CalculateGrossPay());
        }

        [Theory]
        [InlineData(-0.01)]
        [InlineData(-0.1)]
        [InlineData(0.3001)]
        [InlineData(0.31)]
        [InlineData(1.0)]
        public void Case04b_SalesEmployee_CommissionRateOutOfRange_ShouldThrow(decimal invalidRate)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SalesEmployee("S003", "Loi Hoa Hong", "Kinh doanh", 10_000_000m, 100_000_000m, invalidRate));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t\n")]
        public void Case05_EmptyOrWhitespaceStrings_ShouldThrowArgumentException(string? invalidStr)
        {
            Assert.Throws<ArgumentException>(() =>
                new SalariedEmployee(invalidStr!, "Ten Hop Le", "Phong", 10_000_000m, 0m));

            Assert.Throws<ArgumentException>(() =>
                new SalariedEmployee("E999", invalidStr!, "Phong", 10_000_000m, 0m));

            Assert.Throws<ArgumentException>(() =>
                new SalariedEmployee("E999", "Ten Hop Le", invalidStr!, 10_000_000m, 0m));

            Assert.Throws<ArgumentException>(() =>
                new Payroll(invalidStr!));
        }

        [Fact]
        public void Case06_NegativeFinancialValues_ShouldThrowArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SalariedEmployee("E010", "Luong Am", "Phong", -1m, 0m));

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SalariedEmployee("E011", "Phu Cap Am", "Phong", 10_000_000m, -500_000m));

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new HourlyEmployee("E012", "Don Gia Am", "Phong", -10_000m, 40m));

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SalesEmployee("E013", "Luong CB Am", "Phong", -1m, 10_000_000m, 0.05m));

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SalesEmployee("E014", "Doanh So Am", "Phong", 10_000_000m, -100m, 0.05m));

            var sales = new SalesEmployee("E015", "Sales Hop Le", "Phong", 10_000_000m, 0m, 0.05m);
            Assert.Throws<ArgumentOutOfRangeException>(() => sales.UpdateSalesRevenue(-1m));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-500000)]
        public void Case07_AddBonus_AmountZeroOrNegative_ShouldThrow(decimal invalidAmount)
        {
            var emp = new SalariedEmployee("E020", "Nhan Vien", "Phong", 10_000_000m, 0m);

            Assert.Throws<ArgumentOutOfRangeException>(() => emp.AddBonus(invalidAmount));
            Assert.Throws<ArgumentOutOfRangeException>(() => emp.AddBonus(invalidAmount, "Ly do hop le"));
        }

        [Fact]
        public void Case08a_AddBonus_Rate0Point5_ExactUpperBoundary_Valid()
        {
            var emp = new SalariedEmployee("E021", "Nhan Vien", "Phong", 10_000_000m, 0m);
            emp.AddBonus(0.5m, 2_000_000m, "Thuong toi da 50%");
            Assert.Equal(1_000_000m, emp.MonthlyBonus);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-0.01)]
        [InlineData(0.5001)]
        [InlineData(0.51)]
        [InlineData(1.0)]
        public void Case08b_AddBonus_RateOutOfRange_ShouldThrow(decimal invalidRate)
        {
            var emp = new SalariedEmployee("E022", "Nhan Vien", "Phong", 10_000_000m, 0m);
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                emp.AddBonus(invalidRate, 1_000_000m, "Ly do"));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-1000000)]
        public void Case09_AddBonus_ReferenceAmountZeroOrNegative_ShouldThrow(decimal invalidRef)
        {
            var emp = new SalariedEmployee("E023", "Nhan Vien", "Phong", 10_000_000m, 0m);
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                emp.AddBonus(0.1m, invalidRef, "Ly do"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        public void Case10_AddBonus_ReasonEmptyOrWhitespace_ShouldThrow(string? invalidReason)
        {
            var emp = new SalariedEmployee("E024", "Nhan Vien", "Phong", 10_000_000m, 0m);

            Assert.Throws<ArgumentException>(() =>
                emp.AddBonus(500_000m, invalidReason!));

            Assert.Throws<ArgumentException>(() =>
                emp.AddBonus(0.1m, 1_000_000m, invalidReason!));
        }

        [Fact]
        public void Case11_Payroll_DuplicateEmployeeId_OrNull_ShouldBeRejected()
        {
            var payroll = new Payroll("2026-09");
            var emp1 = new SalariedEmployee("E001", "Nguyen A", "Ke toan", 10_000_000m, 0m);
            var empDuplicate = new HourlyEmployee("E001", "Tran Trung Ma", "Ho tro", 100_000m, 40m);

            payroll.AddEmployee(emp1);
            Assert.Single(payroll.Employees);

            var ex = Assert.Throws<ArgumentException>(() => payroll.AddEmployee(empDuplicate));
            Assert.Contains("E001", ex.Message);
            Assert.Single(payroll.Employees);

            Assert.Throws<ArgumentNullException>(() => payroll.AddEmployee(null!));
        }

        [Fact]
        public void Case12_Payroll_EmptyPayroll_ShouldHandleGracefully()
        {
            var emptyPayroll = new Payroll("2026-10");

            Assert.Equal(0m, emptyPayroll.CalculateTotalPayroll());
            Assert.Equal(0m, emptyPayroll.CalculatePayrollByDepartment("Bat Ky"));
            Assert.Equal(0m, emptyPayroll.CalculatePayrollByDepartment(""));
            Assert.Null(emptyPayroll.FindHighestPaidEmployee());
            Assert.Empty(emptyPayroll.Employees);

            var exception = Record.Exception(() => emptyPayroll.DisplayPayroll());
            Assert.Null(exception);
        }

        [Fact]
        public void Case13_ResetMonthlyBonus_ThenRecalculateGrossPay_ShouldReflectReset()
        {
            var emp = new SalariedEmployee("E030", "Le Van Reset", "Ky thuat", 12_000_000m, 1_000_000m);
            emp.AddBonus(2_000_000m, "Thuong du an");

            Assert.Equal(2_000_000m, emp.MonthlyBonus);
            Assert.Equal(15_000_000m, emp.CalculateGrossPay());

            emp.ResetMonthlyBonus();

            Assert.Equal(0m, emp.MonthlyBonus);
            Assert.Equal(13_000_000m, emp.CalculateGrossPay());
        }

        [Fact]
        public void Case14_Payroll_AddNewEmployeeAfterTotalCalculation_TotalShouldUpdate()
        {
            var payroll = new Payroll("2026-09");
            var emp1 = new SalariedEmployee("E041", "Nhan Vien 1", "IT", 10_000_000m, 0m);
            payroll.AddEmployee(emp1);

            decimal initialTotal = payroll.CalculateTotalPayroll();
            Assert.Equal(10_000_000m, initialTotal);

            var emp2 = new HourlyEmployee("E042", "Nhan Vien 2", "IT", 100_000m, 50m);
            payroll.AddEmployee(emp2);

            decimal updatedTotal = payroll.CalculateTotalPayroll();
            Assert.Equal(15_000_000m, updatedTotal);
        }

        [Fact]
        public void Case15_AddBonus_WhenFails_PreviousMonthlyBonusMustRemainIntact()
        {
            var emp = new SalesEmployee("E050", "Nguyen Giu Thuong", "Kinh doanh", 8_000_000m, 50_000_000m, 0.1m);
            emp.AddBonus(1_500_000m, "Thuong hop le dot 1");
            Assert.Equal(1_500_000m, emp.MonthlyBonus);

            Assert.Throws<ArgumentOutOfRangeException>(() => emp.AddBonus(-500_000m));
            Assert.Equal(1_500_000m, emp.MonthlyBonus);

            Assert.Throws<ArgumentException>(() => emp.AddBonus(1_000_000m, "   "));
            Assert.Equal(1_500_000m, emp.MonthlyBonus);

            Assert.Throws<ArgumentOutOfRangeException>(() => emp.AddBonus(0.55m, 10_000_000m, "Vuot tran"));
            Assert.Equal(1_500_000m, emp.MonthlyBonus);

            Assert.Throws<ArgumentOutOfRangeException>(() => emp.AddBonus(0.1m, -10_000_000m, "Am"));
            Assert.Equal(1_500_000m, emp.MonthlyBonus);

            Assert.Throws<ArgumentException>(() => emp.AddBonus(0.1m, 10_000_000m, ""));
            Assert.Equal(1_500_000m, emp.MonthlyBonus);
        }
    }
}
