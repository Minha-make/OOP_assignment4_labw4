using System;
using PayrollLab;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("================================================================================");
Console.WriteLine("                HE THONG TINH LUONG VA THUONG NHAN SU");
Console.WriteLine("================================================================================\n");

var payroll = new Payroll("2026-09");

var e001 = new SalariedEmployee("E001", "Nguyen Minh An", "Dao tao", 15_000_000m, 2_000_000m);
// Chon overload 2 AddBonus(amount, reason) luc bien dich do doi so khop (decimal, string)
e001.AddBonus(1_000_000m, "Thuong hoan thanh xuat sac nhiem vu dao tao");
payroll.AddEmployee(e001);

var e002 = new HourlyEmployee("E002", "Tran Thu Binh", "Ho tro", 100_000m, 150m);
// Chon overload 1 AddBonus(amount) luc bien dich do doi so chi co mot gia tri kieu decimal
e002.AddBonus(500_000m);
payroll.AddEmployee(e002);

var e003 = new HourlyEmployee("E003", "Le Hoang Chi", "Ho tro", 100_000m, 170m);
payroll.AddEmployee(e003);

var e004 = new SalesEmployee("E004", "Pham Quoc Dung", "Kinh doanh", 8_000_000m, 200_000_000m, 0.05m);
// Chon overload 3 AddBonus(rate, ref, reason) luc bien dich do doi so khop (decimal, decimal, string)
e004.AddBonus(0.02m, 50_000_000m, "Thuong dat chi tieu doanh so quy 3");
payroll.AddEmployee(e004);

payroll.DisplayPayroll();

Console.WriteLine("--------------------------------------------------------------------------------");
Console.WriteLine("TONG HOP THEO PHONG BAN:");
Console.WriteLine($"  - Phong Dao tao   : {payroll.CalculatePayrollByDepartment("Dao tao"):N0} d");
Console.WriteLine($"  - Phong Ho tro    : {payroll.CalculatePayrollByDepartment("Ho tro"):N0} d");
Console.WriteLine($"  - Phong Kinh doanh: {payroll.CalculatePayrollByDepartment("Kinh doanh"):N0} d");

var highest = payroll.FindHighestPaidEmployee();
Console.WriteLine($"\nNHAN VIEN CO THU NHAP CAO NHAT:");
Console.WriteLine($"  - {highest?.FullName} ({highest?.EmployeeId}) - {highest?.CalculateGrossPay():N0} d");
Console.WriteLine("================================================================================\n");
