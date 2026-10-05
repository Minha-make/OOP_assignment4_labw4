# BÁO CÁO GIẢI THÍCH KỸ THUẬT VÀ KẾT QUẢ KIỂM THỬ
## DỰ ÁN: HỆ THỐNG TÍNH LƯƠNG VÀ THƯỞNG NHÂN SỰ (OOP LAB W04 — C# / .NET 8)

---

## 1. PHÂN GIẢI PHƯƠNG THỨC NẠP CHỒNG `AddBonus` TRONG BỘ TEST

Trong bộ kiểm thử chính `MainPayrollTests.cs`, có 3 lời gọi phương thức `AddBonus` với các tham số khác nhau:

1. **Lời gọi đối với nhân viên E001 (Nguyễn Minh An):**
   ```csharp
   e001.AddBonus(1_000_000m, "Hoan thanh KPI");
   ```
   - **Phiên bản nạp chồng được chọn:** `AddBonus(decimal amount, string reason)`.
   - **Giải thích cơ chế:** Lời gọi truyền vào đúng 2 đối số với kiểu dữ liệu tĩnh lần lượt là `decimal` và `string`. Trình biên dịch C# tại thời điểm biên dịch (compile-time) tiến hành so khớp chữ ký hàm (signature matching) và xác định phiên bản 2 tham số là phù hợp nhất (best function member match) theo quy tắc overload resolution của chuẩn C#. Không có bất kỳ sự nhập nhằng hay ép kiểu ngầm định nào phát sinh.

2. **Lời gọi đối với nhân viên E002 (Trần Thu Bình):**
   ```csharp
   e002.AddBonus(500_000m);
   ```
   - **Phiên bản nạp chồng được chọn:** `AddBonus(decimal amount)`.
   - **Giải thích cơ chế:** Lời gọi chỉ cung cấp duy nhất 1 đối số kiểu `decimal`. Trong số 3 phiên bản `AddBonus` được khai báo tại lớp `Employee`, duy nhất overload 1 có danh sách tham số gồm 1 tham số kiểu `decimal`. Trình biên dịch lập tức phân giải và liên kết tĩnh (early binding) lời gọi này tới phương thức `AddBonus(decimal)`.

3. **Lời gọi đối với nhân viên E004 (Phạm Quốc Dũng):**
   ```csharp
   e004.AddBonus(0.02m, 50_000_000m, "Thuong doanh so");
   ```
   - **Phiên bản nạp chồng được chọn:** `AddBonus(decimal rate, decimal referenceAmount, string reason)`.
   - **Giải thích cơ chế:** Danh sách đối số thực tế gồm 3 giá trị có kiểu tương ứng `(decimal, decimal, string)`. Trình biên dịch C# đối chiếu chữ ký và nhận dạng chính xác đây là overload thứ 3 với công thức tính thưởng tỷ lệ `rate * referenceAmount` ($0.02 \times 50.000.000 = 1.000.000$ đ). Quyết định chọn phương thức này diễn ra hoàn toàn tĩnh tại thời điểm biên dịch.

*(Lưu ý: Nhân viên E003 Lê Hoàng Chi không gọi `AddBonus`, giá trị `MonthlyBonus` giữ nguyên mặc định bằng 0 đ theo đúng thiết kế của constructor).*

---

## 2. THỜI ĐIỂM CHỌN NẠP CHỒNG VS. GHI ĐÈ TRONG .NET CLR

- **Phương thức nạp chồng (Method Overloading):**
  - Quá trình lựa chọn diễn ra hoàn toàn tại **thời điểm biên dịch (compile-time)** (gọi là đa hình tĩnh - Static Polymorphism hoặc Early Binding).
  - Trình biên dịch C# căn cứ vào tên hàm, số lượng tham số, kiểu dữ liệu tĩnh của các đối số truyền vào tại vị trí gọi để phát sinh mã IL gọi trực tiếp phương thức đó (thông qua lệnh gọi `call`). Tại thời điểm chương trình thực thi (runtime), không còn bất kỳ bước tra cứu hay lựa chọn nào nữa.
  
- **Phương thức ghi đè (Method Overriding với `abstract`/`virtual` và `override`):**
  - Quá trình lựa chọn phương thức cụ thể để thực thi diễn ra tại **thời điểm chạy (runtime)** (gọi là đa hình động - Dynamic Polymorphism hoặc Late Binding).
  - Khi một lời gọi như `e.CalculateGrossPay()` được thực hiện qua biến tham chiếu kiểu cơ sở `Employee e`, trình biên dịch chỉ phát sinh lệnh gọi ảo `callvirt`.
  - Khi chạy, môi trường thực thi .NET CLR sẽ đọc con trỏ bảng phương thức (Method Table - tương tự vtable trong C++) nằm trong phần đầu (object header) của đối tượng thật trên bộ nhớ heap. CLR tra cứu địa chỉ của hàm `CalculateGrossPay()` tương ứng với kiểu thực tế lúc chạy (`SalariedEmployee`, `HourlyEmployee`, hay `SalesEmployee`) và điều hướng thực thi chính xác đến phiên bản ghi đè của lớp dẫn xuất đó.

---

## 3. THAY ĐỔI THIẾT KẾ NẾU DÙNG NHIỀU DANH SÁCH RIÊNG TỪNG LOẠI

Nếu trong lớp `Payroll`, ta thay thế `List<Employee> _employees` bằng 3 danh sách riêng biệt:
```csharp
private readonly List<SalariedEmployee> _salariedList;
private readonly List<HourlyEmployee> _hourlyList;
private readonly List<SalesEmployee> _salesList;
```

Thiết kế sẽ gặp phải những hệ quả và đánh đổi tiêu cực nghiêm trọng sau:

1. **Phá vỡ tính bao đóng và làm mã nguồn cồng kềnh (Vi phạm DRY):**
   Mọi phương thức tính toán tổng hợp trong `Payroll` đều bị nhân ba logic. Ví dụ: `CalculateTotalPayroll()` thay vì chỉ cần một dòng ngắn gọn `_employees.Sum(e => e.CalculateGrossPay())` sẽ buộc phải duyệt qua cả 3 danh sách và cộng dồn:
   ```csharp
   return _salariedList.Sum(e => e.CalculateGrossPay())
        + _hourlyList.Sum(e => e.CalculateGrossPay())
        + _salesList.Sum(e => e.CalculateGrossPay());
   ```
   Tương tự, các hàm `FindHighestPaidEmployee()`, `CalculatePayrollByDepartment()`, `DisplayPayroll()` đều phải viết mã tìm kiếm, so sánh và lọc riêng rẽ trên từng danh sách.

2. **Mất hoàn toàn tính đa hình (Polymorphism):**
   Lớp `Payroll` mất đi khả năng đối xử với các đối tượng nhân sự dưới một góc nhìn trừu tượng thống nhất thông qua kiểu cơ sở `Employee`. Thay vào đó, `Payroll` bị phụ thuộc cứng (tight coupling) vào từng kiểu cụ thể của lớp con.

3. **Vi phạm nghiêm trọng nguyên lý Đóng/Mở (Open/Closed Principle - OCP):**
   Bất cứ khi nào doanh nghiệp bổ sung thêm một loại hình nhân sự mới, `Payroll` bắt buộc phải bị sửa đổi để khai báo thêm danh sách mới và sửa đổi toàn bộ các phương thức xử lý bảng lương, làm tăng nguy cơ phát sinh lỗi hồi quy (regression bugs).

---

## 4. NGUYÊN TẮC OPEN/CLOSED KHI BỔ SUNG `ContractEmployee`

Khi hệ thống cần bổ sung một loại nhân sự mới là **`ContractEmployee`** (nhân viên làm việc theo hợp đồng khoán):

### 4.1. Phần mã KHÔNG ĐƯỢC PHÉP và KHÔNG CẦN PHẢI SỬA (Đóng với sửa đổi - Closed for modification):
- **File `Payroll.cs` (Lớp `Payroll`):** Hoàn toàn **không cần chỉnh sửa dù chỉ một dòng mã**. Phương thức `AddEmployee(Employee employee)` nhận vào kiểu trừu tượng `Employee`, và các phương thức `CalculateTotalPayroll()`, `CalculatePayrollByDepartment()`, `FindHighestPaidEmployee()`, `DisplayPayroll()` đều gọi `CalculateGrossPay()` thông qua cơ chế đa hình. Nhờ vậy, đối tượng `ContractEmployee` mới sẽ được tự động tích hợp trơn tru vào mọi quy trình tính toán của bảng lương.
- **File `Employee.cs` (Lớp `Employee`):** Giữ nguyên vẹn, vì giao diện cơ sở, thuộc tính chung và các quy tắc nghiệp vụ cốt lõi không thay đổi.
- **Các file `SalariedEmployee.cs`, `HourlyEmployee.cs`, `SalesEmployee.cs`:** Hoàn toàn độc lập, không bị ảnh hưởng.

### 4.2. Phần mã PHẢI VIẾT MỚI hoặc SỬA ĐỔI (Mở để mở rộng - Open for extension):
- **Tạo mới file `ContractEmployee.cs`:**
  - Khai báo lớp `public class ContractEmployee : Employee`.
  - Cung cấp các thuộc tính đặc thù (ví dụ: `ContractAmount` - giá trị hợp đồng trọn gói).
  - Cài đặt tối thiểu 2 constructor ủy quyền (constructor rút gọn gọi constructor đầy đủ, constructor đầy đủ gọi `base(...)`).
  - Ghi đè bắt buộc các phương thức trừu tượng bằng từ khóa `override`:
    - `public override decimal CalculateGrossPay() => ContractAmount + MonthlyBonus;`
    - `public override string GetEmployeeType() => "Contract";`
    - `public override void DisplayPayrollInfo() { ... }`
- **File chạy ứng dụng `Program.cs` hoặc file kiểm thử mới:**
  - Khởi tạo đối tượng `new ContractEmployee(...)` và truyền vào phương thức `payroll.AddEmployee(...)` để đưa vào kỳ lương thực tế.

---

## 5. KẾT QUẢ KIỂM THỬ THỰC TẾ (TEST RESULTS)

Nguyên văn kết quả chạy lệnh `dotnet test` tự động trên toàn bộ hệ thống kiểm thử của dự án:

```text
Test run for C:\Users\Hi\Desktop\Documents\OOP\assignment4\PayrollLab.Tests\bin\Debug\net8.0\PayrollLab.Tests.dll (.NETCoreApp,Version=v8.0)
VSTest version 17.11.1 (x64)

Starting test execution, please wait...
A total of 1 test files matched the specified pattern.
[xUnit.net 00:00:00.00] xUnit.net VSTest Adapter v2.5.3.1+6b60a9e56a (64-bit .NET 8.0.31)
[xUnit.net 00:00:00.14]   Discovering: PayrollLab.Tests
[xUnit.net 00:00:00.18]   Discovered:  PayrollLab.Tests
[xUnit.net 00:00:00.18]   Starting:    PayrollLab.Tests
  Passed PayrollLab.Tests.EmployeeTests.Constructor_EmptyDepartment_ShouldThrowArgumentException(invalidDept: "") [2 ms]
  Passed PayrollLab.Tests.EmployeeTests.Constructor_EmptyDepartment_ShouldThrowArgumentException(invalidDept: "   ") [< 1 ms]
  Passed PayrollLab.Tests.EmployeeTests.Constructor_EmptyDepartment_ShouldThrowArgumentException(invalidDept: null) [< 1 ms]
  Passed PayrollLab.Tests.EmployeeTests.CalculateGrossPay_ShouldCallDerivedImplementation [< 1 ms]
  Passed PayrollLab.Tests.MainPayrollTests.E003_HourlyEmployee_170Hours_WithOT_GrossPay_ShouldBe17Point5Million [5 ms]
  Passed PayrollLab.Tests.EmployeeTests.AddBonus_AmountReason_EmptyReason_ShouldThrow(invalidReason: "   ") [< 1 ms]
  Passed PayrollLab.Tests.EmployeeTests.AddBonus_AmountReason_EmptyReason_ShouldThrow(invalidReason: "") [< 1 ms]
  Passed PayrollLab.Tests.EmployeeTests.AddBonus_AmountReason_EmptyReason_ShouldThrow(invalidReason: null) [< 1 ms]
  Passed PayrollLab.Tests.EmployeeTests.Constructor_EmptyId_ShouldThrowArgumentException(invalidId: "   ") [< 1 ms]
  Passed PayrollLab.Tests.EdgeAndErrorTests.Case03_HourlyEmployee_WorkedHoursOutOfRange_ShouldThrow(invalidHours: 251) [< 1 ms]
  Passed PayrollLab.Tests.EdgeAndErrorTests.Case03_HourlyEmployee_WorkedHoursOutOfRange_ShouldThrow(invalidHours: -1) [< 1 ms]
  Passed PayrollLab.Tests.EdgeAndErrorTests.Case03_HourlyEmployee_WorkedHoursOutOfRange_ShouldThrow(invalidHours: -0.01) [< 1 ms]
  Passed PayrollLab.Tests.EmployeeTests.Constructor_EmptyId_ShouldThrowArgumentException(invalidId: null) [< 1 ms]
  Passed PayrollLab.Tests.EmployeeTests.Constructor_EmptyId_ShouldThrowArgumentException(invalidId: "") [< 1 ms]
  Passed PayrollLab.Tests.EdgeAndErrorTests.Case09_AddBonus_ReferenceAmountZeroOrNegative_ShouldThrow(invalidRef: -1000000) [< 1 ms]
  Passed PayrollLab.Tests.EdgeAndErrorTests.Case09_AddBonus_ReferenceAmountZeroOrNegative_ShouldThrow(invalidRef: -1) [< 1 ms]
  Passed PayrollLab.Tests.EdgeAndErrorTests.Case09_AddBonus_ReferenceAmountZeroOrNegative_ShouldThrow(invalidRef: 0) [< 1 ms]
  Passed PayrollLab.Tests.EmployeeTests.AddBonus_RateRef_RateOutOfRange_ShouldThrow [< 1 ms]
  Passed PayrollLab.Tests.EmployeeTests.GetEmployeeType_ShouldReturnDerivedType [< 1 ms]
  Passed PayrollLab.Tests.MainPayrollTests.PayrollByDepartment_DaoTao_ShouldBe18Million [1 ms]
  Passed PayrollLab.Tests.EdgeAndErrorTests.Case14_Payroll_AddNewEmployeeAfterTotalCalculation_TotalShouldUpdate [< 1 ms]
  Passed PayrollLab.Tests.EmployeeTests.AddBonus_AmountReason_Valid_ShouldAccumulate [< 1 ms]
  Passed PayrollLab.Tests.MainPayrollTests.TotalPayroll_ShouldBe70Million [< 1 ms]
  Passed PayrollLab.Tests.EmployeeTests.AddBonus_RateRef_RefAmountZeroOrNegative_ShouldThrow [< 1 ms]
  Passed PayrollLab.Tests.EdgeAndErrorTests.Case10_AddBonus_ReasonEmptyOrWhitespace_ShouldThrow(invalidReason: null) [< 1 ms]
  Passed PayrollLab.Tests.MainPayrollTests.PayrollByDepartment_KinhDoanh_ShouldBe19Million [< 1 ms]
  Passed PayrollLab.Tests.EdgeAndErrorTests.Case10_AddBonus_ReasonEmptyOrWhitespace_ShouldThrow(invalidReason: "\t") [< 1 ms]
  Passed PayrollLab.Tests.EdgeAndErrorTests.Case10_AddBonus_ReasonEmptyOrWhitespace_ShouldThrow(invalidReason: "   ") [< 1 ms]
  Passed PayrollLab.Tests.EdgeAndErrorTests.Case10_AddBonus_ReasonEmptyOrWhitespace_ShouldThrow(invalidReason: "") [< 1 ms]
  Passed PayrollLab.Tests.EmployeeTests.AddBonus_RateRef_EmptyReason_ShouldThrow(invalidReason: null) [< 1 ms]
  Passed PayrollLab.Tests.EdgeAndErrorTests.Case08b_AddBonus_RateOutOfRange_ShouldThrow(invalidRate: 0) [< 1 ms]
  Passed PayrollLab.Tests.EdgeAndErrorTests.Case08b_AddBonus_RateOutOfRange_ShouldThrow(invalidRate: 0.5001) [< 1 ms]
  Passed PayrollLab.Tests.EdgeAndErrorTests.Case08b_AddBonus_RateOutOfRange_ShouldThrow(invalidRate: -0.01) [< 1 ms]
  Passed PayrollLab.Tests.EdgeAndErrorTests.Case08b_AddBonus_RateOutOfRange_ShouldThrow(invalidRate: 1) [< 1 ms]
  Passed PayrollLab.Tests.EdgeAndErrorTests.Case08b_AddBonus_RateOutOfRange_ShouldThrow(invalidRate: 0.51) [< 1 ms]
  Passed PayrollLab.Tests.EmployeeTests.AddBonus_RateRef_EmptyReason_ShouldThrow(invalidReason: "   ") [< 1 ms]
  Passed PayrollLab.Tests.EmployeeTests.AddBonus_RateRef_EmptyReason_ShouldThrow(invalidReason: "") [< 1 ms]
  Passed PayrollLab.Tests.EdgeAndErrorTests.Case08a_AddBonus_Rate0Point5_ExactUpperBoundary_Valid [< 1 ms]
  Passed PayrollLab.Tests.EmployeeTests.Constructor_EmptyName_ShouldThrowArgumentException(invalidName: "") [< 1 ms]
  Passed PayrollLab.Tests.MainPayrollTests.HighestPaidEmployee_ShouldBeE004 [< 1 ms]
  Passed PayrollLab.Tests.EdgeAndErrorTests.Case04b_SalesEmployee_CommissionRateOutOfRange_ShouldThrow(invalidRate: 0.31) [< 1 ms]
  Passed PayrollLab.Tests.EmployeeTests.Constructor_EmptyName_ShouldThrowArgumentException(invalidName: "   ") [< 1 ms]
  Passed PayrollLab.Tests.MainPayrollTests.PayrollByDepartment_HoTro_ShouldBe33Million [< 1 ms]
  Passed PayrollLab.Tests.EmployeeTests.Constructor_EmptyName_ShouldThrowArgumentException(invalidName: null) [< 1 ms]
  Passed PayrollLab.Tests.EdgeAndErrorTests.Case04b_SalesEmployee_CommissionRateOutOfRange_ShouldThrow(invalidRate: 1) [< 1 ms]
  Passed PayrollLab.Tests.EmployeeTests.AddBonus_Amount_ValidPositive_ShouldAccumulate [< 1 ms]
  Passed PayrollLab.Tests.EdgeAndErrorTests.Case04b_SalesEmployee_CommissionRateOutOfRange_ShouldThrow(invalidRate: -0.1) [< 1 ms]
  Passed PayrollLab.Tests.EdgeAndErrorTests.Case04b_SalesEmployee_CommissionRateOutOfRange_ShouldThrow(invalidRate: 0.3001) [< 1 ms]
  Passed PayrollLab.Tests.EdgeAndErrorTests.Case04b_SalesEmployee_CommissionRateOutOfRange_ShouldThrow(invalidRate: -0.01) [< 1 ms]
  Passed PayrollLab.Tests.MainPayrollTests.EmployeeTypes_ShouldReturnCorrectTypes [< 1 ms]
  Passed PayrollLab.Tests.EmployeeTests.AddBonus_Amount_WhenFails_BonusShouldNotChange [< 1 ms]
  Passed PayrollLab.Tests.EmployeeTests.Constructor3Params_ValidData_ShouldSetProperties [< 1 ms]
  Passed PayrollLab.Tests.EmployeeTests.AddBonus_MultipleOverloads_ShouldAccumulateCorrectly [< 1 ms]
  Passed PayrollLab.Tests.EmployeeTests.AddBonus_AmountReason_InvalidReason_BonusShouldNotChange [< 1 ms]
  Passed PayrollLab.Tests.EmployeeTests.DisplayPayrollInfo_ShouldNotThrow [1 ms]
  Passed PayrollLab.Tests.EmployeeTests.ResetMonthlyBonus_ShouldSetBonusToZero [< 1 ms]
  Passed PayrollLab.Tests.EmployeeTests.AddBonus_RateRef_Valid_ShouldCalculateAndAccumulate [< 1 ms]
  Passed PayrollLab.Tests.EmployeeTests.Constructor2Params_ValidData_ShouldDefaultDepartmentToUnassigned [< 1 ms]
  Passed PayrollLab.Tests.MainPayrollTests.DisplayPayroll_ShouldNotThrow [3 ms]
  Passed PayrollLab.Tests.EmployeeTests.AddBonus_RateRef_WhenFails_BonusShouldNotChange [< 1 ms]
  Passed PayrollLab.Tests.MainPayrollTests.E002_HourlyEmployee_150Hours_GrossPay_ShouldBe15Point5Million [< 1 ms]
  Passed PayrollLab.Tests.EmployeeTests.AddBonus_RateRef_Rate0Point5_ShouldBeValid [< 1 ms]
  Passed PayrollLab.Tests.MainPayrollTests.E001_SalariedEmployee_GrossPay_ShouldBe18Million [< 1 ms]
  Passed PayrollLab.Tests.EmployeeTests.AddBonus_Amount_ZeroOrNegative_ShouldThrow [< 1 ms]
  Passed PayrollLab.Tests.MainPayrollTests.E004_SalesEmployee_GrossPay_ShouldBe19Million [< 1 ms]
  Passed PayrollLab.Tests.EdgeAndErrorTests.Case11_Payroll_DuplicateEmployeeId_OrNull_ShouldBeRejected [10 ms]
  Passed PayrollLab.Tests.EdgeAndErrorTests.Case07_AddBonus_AmountZeroOrNegative_ShouldThrow(invalidAmount: -500000) [< 1 ms]
  Passed PayrollLab.Tests.EdgeAndErrorTests.Case07_AddBonus_AmountZeroOrNegative_ShouldThrow(invalidAmount: -1) [< 1 ms]
  Passed PayrollLab.Tests.EdgeAndErrorTests.Case07_AddBonus_AmountZeroOrNegative_ShouldThrow(invalidAmount: 0) [< 1 ms]
  Passed PayrollLab.Tests.EdgeAndErrorTests.Case12_Payroll_EmptyPayroll_ShouldHandleGracefully [< 1 ms]
  Passed PayrollLab.Tests.EdgeAndErrorTests.Case02_HourlyEmployee_WorkedHours0And250_ValidBoundaries [< 1 ms]
[xUnit.net 00:00:00.16]   Finished:    PayrollLab.Tests
  Passed PayrollLab.Tests.EdgeAndErrorTests.Case01_HourlyEmployee_WorkedHours160_ExactBoundary_NoOvertime [< 1 ms]
  Passed PayrollLab.Tests.EdgeAndErrorTests.Case06_NegativeFinancialValues_ShouldThrowArgumentOutOfRangeException [< 1 ms]
  Passed PayrollLab.Tests.EdgeAndErrorTests.Case13_ResetMonthlyBonus_ThenRecalculateGrossPay_ShouldReflectReset [< 1 ms]
  Passed PayrollLab.Tests.EdgeAndErrorTests.Case15_AddBonus_WhenFails_PreviousMonthlyBonusMustRemainIntact [< 1 ms]
  Passed PayrollLab.Tests.EdgeAndErrorTests.Case04a_SalesEmployee_CommissionRate0And0Point3_ValidBoundaries [< 1 ms]

Test Run Successful.
Total tests: 81
     Passed: 81
     Failed: 0
 Total time: 0.5019 Seconds

Build succeeded.
    0 Warning(s)
    0 Error(s)
```
