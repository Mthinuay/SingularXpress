using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SingularExpress.Models.Models;
using SingularExpress.Models;

namespace SingularExpress.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly ModelDbContext _context;
            private readonly ILogger<EmployeeController> _logger;


        public EmployeeController(ModelDbContext context, ILogger<EmployeeController> logger)
        {
            _context = context;
                    _logger = logger;

        }

[HttpPost("add")]
public async Task<IActionResult> AddEmployee([FromBody] Employee newEmployee)
{
    var errors = ValidateEmployee(newEmployee);
    if (errors.Any())
        return BadRequest(new { errors });

    bool idExists = await _context.Employees
        .AnyAsync(e => e.IdNumber == newEmployee.IdNumber);

    if (idExists)
        return Conflict(new { errors = new { idNumber = new[] { "ID number already exists." } } });

    int sameSurnameCount = await _context.Employees
        .CountAsync(e => e.LastName.ToLower().StartsWith(newEmployee.LastName.ToLower()));

    newEmployee.EmployeeNumber = Employee.GenerateEmployeeNumber(newEmployee.LastName, sameSurnameCount);

    // Call the same PopulateByIdOrPassport method that converts date strings to DateTime
    PopulateByIdOrPassport(newEmployee);

    _context.Employees.Add(newEmployee);
    await _context.SaveChangesAsync();

    return Ok(newEmployee);
}


        
        [HttpGet("by-idnumber/{idNumber}")]
public async Task<IActionResult> GetEmployeeByIdNumber(string idNumber)
{
    if (string.IsNullOrWhiteSpace(idNumber))
        return BadRequest(new { error = "ID Number is required." });

    var employee = await _context.Employees
        .FirstOrDefaultAsync(e => e.IdNumber == idNumber);

    if (employee == null)
        return NotFound(new { error = "Employee not found." });

    return Ok(employee);
}


        [HttpPut("edit/{employeeNumber}")]
        public async Task<IActionResult> EditEmployee(
            string employeeNumber,
            [FromBody] Employee updatedEmployee)
        {
            var existing = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeNumber == employeeNumber);

            if (existing == null)
                return NotFound(new { error = "Employee not found." });

            bool checkIdUnique = existing.IdNumber != updatedEmployee.IdNumber;

            var errors = ValidateEmployee(updatedEmployee, checkIdUnique);
            if (errors.Any())
                return BadRequest(new { errors });

          if (checkIdUnique)
{
    bool idConflict = await _context.Employees
        .AnyAsync(e => e.IdNumber == updatedEmployee.IdNumber);

    if (idConflict)
        return Conflict(new { errors = new { idNumber = new[] { "ID number already exists." } } });

                // Only re-populate DOB and other ID-based info if ID changed
  
        _logger.LogInformation("EditEmployee called with DOB: {dob}", updatedEmployee.DateOfBirth);
                _logger.LogInformation("Existing called with DOB: {dob}", existing.DateOfBirth);


    PopulateByIdOrPassport(updatedEmployee);
}


            updatedEmployee.EmployeeNumber = existing.EmployeeNumber;
          //  PopulateByIdOrPassport(updatedEmployee);

            _context.Entry(existing).CurrentValues.SetValues(updatedEmployee);
            await _context.SaveChangesAsync();

            return Ok(updatedEmployee);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _context.Employees.ToListAsync();
            return Ok(employees);
        }

        // ✅ Helper Methods Below

        private void PopulateByIdOrPassport(Employee emp)
        {
            if (emp.IdType == "id")
                emp.PopulateFromIdNumber();
            else if (emp.IdType == "passport")
                emp.PopulateFromPassportNumber();
        }

        private Dictionary<string, string[]> ValidateEmployee(Employee emp, bool checkIdUnique = false)
        {
            var errors = new Dictionary<string, string[]>();

            void AddError(string field, string message)
            {
                if (!errors.ContainsKey(field))
                    errors[field] = Array.Empty<string>();

                errors[field] = errors[field].Append(message).ToArray();
            }

            if (string.IsNullOrWhiteSpace(emp.FirstName))
                AddError("firstName", "First name is required.");

            if (string.IsNullOrWhiteSpace(emp.LastName))
                AddError("lastName", "Last name is required.");

            if (string.IsNullOrWhiteSpace(emp.Email))
            {
                AddError("email", "Email is required.");
            }
            else if (!emp.Email.EndsWith("@singular.co.za", StringComparison.OrdinalIgnoreCase))
            {
                AddError("email", "Email must end with @singular.co.za.");
            }

            if (emp.IdType == "id")
            {
                if (string.IsNullOrWhiteSpace(emp.IdNumber) ||
                    emp.IdNumber.Length != 13 ||
                    !emp.IdNumber.All(char.IsDigit))
                {
                    AddError("idNumber", "ID Number must be exactly 13 digits.");
                }
            }
            else if (emp.IdType == "passport")
            {
                if (string.IsNullOrWhiteSpace(emp.IdNumber) ||
                    emp.IdNumber.Length < 6 || emp.IdNumber.Length > 9 ||
                    !emp.IdNumber.All(char.IsLetterOrDigit))
                {
                    AddError("idNumber", "Passport must be 6–9 alphanumeric characters.");
                }
            }
            else
            {
                AddError("idType", "ID type must be either 'id' or 'passport'.");
            }

            return errors;
        }
    }
}