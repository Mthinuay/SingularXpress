using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SingularExpress.Api.DTOs
{
    public class EmployeeDto
    {
        
        public string EmployeeNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MaidenName { get; set; }
        public string Title { get; set; }
        
        // Dates as strings yyyy-MM-dd or null
        public string DateOfBirth { get; set; }
        
        public string Initials { get; set; }
        public string IdType { get; set; } 
        //String as there are often leading zeros and not used in calculations
        public string IdNumber { get; set; }
        public string PreferredName { get; set; }
        public string Gender { get; set; }
        public string MiddleName { get; set; }
         //String as there are leading zeros and not used in calculations
        public string ContactNumber { get; set; }
        public string Nationality { get; set; }
        public string Citizenship { get; set; }
        public bool Disability { get; set; }
        public string DisabilityType { get; set; }
        public string Email { get; set; }
        public string MaritalStatus { get; set; }
        public string HomeAddress { get; set; }
        public string City { get; set; }
         //String as there are sometimes leading zeros and not used in calculations
        public string PostalCode { get; set; }
        
        // Dates as strings yyyy-MM-dd or null
        public string StartDate { get; set; }
        
        public string Department { get; set; }
        public string JobTitle { get; set; }
        public string EmployeeStatus { get; set; }
        public string ReportsTo { get; set; }
        public string DocumentPath { get; set; }
    }
}