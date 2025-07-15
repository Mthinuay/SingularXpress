using System.ComponentModel.DataAnnotations;
namespace SingularExpress.Models.Models
{
    public class Employee
    {
        [Key]
        public string EmployeeNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MaidenName { get; set; }
        public string Title { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Initials { get; set; }
        public string IdType { get; set; } 

        public string IdNumber { get; set; }
        public string PreferredName { get; set; }
        public string Gender { get; set; }
        public string MiddleName { get; set; }
        public string ContactNumber { get; set; }
        public string Nationality { get; set; }
        public string Citizenship { get; set; }
        public bool Disability { get; set; }
        public string DisabilityType { get; set; }
        public string Email { get; set; }

        public string MaritalStatus { get; set; }
        public string HomeAddress { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }

        public DateTime StartDate { get; set; }
        public string Department { get; set; }
        public string JobTitle { get; set; }
        public string EmployeeStatus { get; set; }
        public string ReportsTo { get; set; }
        public string DocumentPath { get; set; }

        // Helper to generate EmployeeNumber
        public static string GenerateEmployeeNumber(string lastName, int existingCount)
        {
            string prefix = lastName.Length >= 3 ? lastName.Substring(0, 3).ToUpper() : lastName.ToUpper().PadRight(3, 'X');
            return $"{prefix}{(existingCount + 1).ToString("D3")}";
        }

        // Extracts DOB, Gender, Nationality from SA ID Number
public void PopulateFromIdNumber()
{
    if (IdType != "id" || string.IsNullOrEmpty(IdNumber) || IdNumber.Length != 13)
        return;

    try
    {
        string dobStr = IdNumber.Substring(0, 6);
        if (DateTime.TryParseExact(dobStr, "yyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime dob))
        {
            int year = int.Parse(dobStr.Substring(0, 2));
            int fullYear = year < 30 ? 2000 + year : 1900 + year;
            DateOfBirth = new DateTime(fullYear, dob.Month, dob.Day);
        }

        int genderDigit = int.Parse(IdNumber.Substring(6, 4));
        Gender = genderDigit >= 5000 ? "Male" : "Female";

        string nationalityDigit = IdNumber.Substring(10, 1);
        Nationality = nationalityDigit == "0" ? "South African" : "Non-South African";
    }
    catch
    {
        
    }
}


public void PopulateFromPassportNumber()
{
    if (IdType != "passport" || string.IsNullOrEmpty(IdNumber))
        return;

    // Basic validation for passport number (6-9 alphanumeric characters)
    if (!System.Text.RegularExpressions.Regex.IsMatch(IdNumber, @"^[a-zA-Z0-9]{6,9}$"))
        return;

    // Since passports do not embed DOB or gender info, you can:
    // - Clear or set defaults for those fields
    DateOfBirth = null;
    Gender = null;
    Nationality = "Non-South African"; // assuming passport means non-SA here

    
}

    }
}