using System;
using Xunit;
using PayrollLab;

namespace PayrollLab.Tests
{
    internal class TestableEmployee : Employee
    {
        // Constructor uy quyen: goi constructor 3 tham so cua lop co so
        public TestableEmployee(string id, string name, string department)
            : base(id, name, department)
        {
        }

        // Constructor uy quyen: goi constructor 2 tham so cua lop co so
        public TestableEmployee(string id, string name)
            : base(id, name)
        {
        }

        // Ghi de (override): cai dat cu the phuong thuc truu tuong luc chay
        public override decimal CalculateGrossPay()
        {
            return MonthlyBonus;
        }

        // Ghi de (override): cai dat cu the phuong thuc truu tuong luc chay
        public override string GetEmployeeType()
        {
            return "Testable";
        }
    }

    public class EmployeeTests
    {
        [Fact]
        public void Constructor3Params_ValidData_ShouldSetProperties()
        {
            var emp = new TestableEmployee("E001", "Nguyen Van A", "Ky thuat");

            Assert.Equal("E001", emp.EmployeeId);
            Assert.Equal("Nguyen Van A", emp.FullName);
            Assert.Equal("Ky thuat", emp.Department);
            Assert.Equal(0m, emp.MonthlyBonus);
        }

        [Fact]
        public void Constructor2Params_ValidData_ShouldDefaultDepartmentToUnassigned()
        {
            var emp = new TestableEmployee("E002", "Tran Thi B");

            Assert.Equal("E002", emp.EmployeeId);
            Assert.Equal("Tran Thi B", emp.FullName);
            Assert.Equal("Unassigned", emp.Department);
            Assert.Equal(0m, emp.MonthlyBonus);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_EmptyId_ShouldThrowArgumentException(string? invalidId)
        {
            Assert.Throws<ArgumentException>(() =>
                new TestableEmployee(invalidId!, "Ten", "Phong"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_EmptyName_ShouldThrowArgumentException(string? invalidName)
        {
            Assert.Throws<ArgumentException>(() =>
                new TestableEmployee("E001", invalidName!, "Phong"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_EmptyDepartment_ShouldThrowArgumentException(string? invalidDept)
        {
            Assert.Throws<ArgumentException>(() =>
                new TestableEmployee("E001", "Ten", invalidDept!));
        }

        [Fact]
        public void AddBonus_Amount_ValidPositive_ShouldAccumulate()
        {
            var emp = new TestableEmployee("E001", "Ten", "Phong");

            emp.AddBonus(500_000m);
            Assert.Equal(500_000m, emp.MonthlyBonus);

            emp.AddBonus(300_000m);
            Assert.Equal(800_000m, emp.MonthlyBonus);
        }

        [Fact]
        public void AddBonus_Amount_ZeroOrNegative_ShouldThrow()
        {
            var emp = new TestableEmployee("E001", "Ten", "Phong");

            Assert.Throws<ArgumentOutOfRangeException>(() => emp.AddBonus(0m));
            Assert.Throws<ArgumentOutOfRangeException>(() => emp.AddBonus(-100m));
        }

        [Fact]
        public void AddBonus_Amount_WhenFails_BonusShouldNotChange()
        {
            var emp = new TestableEmployee("E001", "Ten", "Phong");
            emp.AddBonus(500_000m);

            Assert.Throws<ArgumentOutOfRangeException>(() => emp.AddBonus(-100m));
            Assert.Equal(500_000m, emp.MonthlyBonus);
        }

        [Fact]
        public void AddBonus_AmountReason_Valid_ShouldAccumulate()
        {
            var emp = new TestableEmployee("E001", "Ten", "Phong");

            emp.AddBonus(1_000_000m, "Hoan thanh KPI");
            Assert.Equal(1_000_000m, emp.MonthlyBonus);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void AddBonus_AmountReason_EmptyReason_ShouldThrow(string? invalidReason)
        {
            var emp = new TestableEmployee("E001", "Ten", "Phong");

            Assert.Throws<ArgumentException>(() =>
                emp.AddBonus(1_000_000m, invalidReason!));
        }

        [Fact]
        public void AddBonus_AmountReason_InvalidReason_BonusShouldNotChange()
        {
            var emp = new TestableEmployee("E001", "Ten", "Phong");
            emp.AddBonus(500_000m);

            Assert.Throws<ArgumentException>(() => emp.AddBonus(1_000_000m, ""));
            Assert.Equal(500_000m, emp.MonthlyBonus);
        }

        [Fact]
        public void AddBonus_RateRef_Valid_ShouldCalculateAndAccumulate()
        {
            var emp = new TestableEmployee("E001", "Ten", "Phong");

            emp.AddBonus(0.02m, 50_000_000m, "Thuong doanh so");
            Assert.Equal(1_000_000m, emp.MonthlyBonus);
        }

        [Fact]
        public void AddBonus_RateRef_Rate0Point5_ShouldBeValid()
        {
            var emp = new TestableEmployee("E001", "Ten", "Phong");

            emp.AddBonus(0.5m, 1_000_000m, "Thuong dac biet");
            Assert.Equal(500_000m, emp.MonthlyBonus);
        }

        [Fact]
        public void AddBonus_RateRef_RateOutOfRange_ShouldThrow()
        {
            var emp = new TestableEmployee("E001", "Ten", "Phong");

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                emp.AddBonus(0m, 1_000_000m, "Ly do"));

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                emp.AddBonus(0.51m, 1_000_000m, "Ly do"));

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                emp.AddBonus(-0.01m, 1_000_000m, "Ly do"));
        }

        [Fact]
        public void AddBonus_RateRef_RefAmountZeroOrNegative_ShouldThrow()
        {
            var emp = new TestableEmployee("E001", "Ten", "Phong");

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                emp.AddBonus(0.1m, 0m, "Ly do"));

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                emp.AddBonus(0.1m, -1_000_000m, "Ly do"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void AddBonus_RateRef_EmptyReason_ShouldThrow(string? invalidReason)
        {
            var emp = new TestableEmployee("E001", "Ten", "Phong");

            Assert.Throws<ArgumentException>(() =>
                emp.AddBonus(0.1m, 1_000_000m, invalidReason!));
        }

        [Fact]
        public void AddBonus_RateRef_WhenFails_BonusShouldNotChange()
        {
            var emp = new TestableEmployee("E001", "Ten", "Phong");
            emp.AddBonus(500_000m);

            Assert.Throws<ArgumentException>(() =>
                emp.AddBonus(0.1m, 1_000_000m, ""));

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                emp.AddBonus(0.51m, 1_000_000m, "Ly do"));

            Assert.Equal(500_000m, emp.MonthlyBonus);
        }

        [Fact]
        public void ResetMonthlyBonus_ShouldSetBonusToZero()
        {
            var emp = new TestableEmployee("E001", "Ten", "Phong");
            emp.AddBonus(1_000_000m);
            Assert.Equal(1_000_000m, emp.MonthlyBonus);

            emp.ResetMonthlyBonus();
            Assert.Equal(0m, emp.MonthlyBonus);
        }

        [Fact]
        public void GetEmployeeType_ShouldReturnDerivedType()
        {
            var emp = new TestableEmployee("E001", "Ten", "Phong");
            Assert.Equal("Testable", emp.GetEmployeeType());
        }

        [Fact]
        public void CalculateGrossPay_ShouldCallDerivedImplementation()
        {
            var emp = new TestableEmployee("E001", "Ten", "Phong");
            emp.AddBonus(500_000m);

            Employee baseRef = emp;
            Assert.Equal(500_000m, baseRef.CalculateGrossPay());
        }

        [Fact]
        public void DisplayPayrollInfo_ShouldNotThrow()
        {
            var emp = new TestableEmployee("E001", "Nguyen Van A", "Ky thuat");
            emp.AddBonus(500_000m);

            var exception = Record.Exception(() => emp.DisplayPayrollInfo());
            Assert.Null(exception);
        }

        [Fact]
        public void AddBonus_MultipleOverloads_ShouldAccumulateCorrectly()
        {
            var emp = new TestableEmployee("E001", "Ten", "Phong");

            emp.AddBonus(500_000m);
            emp.AddBonus(1_000_000m, "KPI");
            emp.AddBonus(0.02m, 10_000_000m, "Doanh so");

            Assert.Equal(1_700_000m, emp.MonthlyBonus);
        }
    }
}
