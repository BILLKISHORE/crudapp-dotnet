using System.ComponentModel.DataAnnotations;

namespace EnterpriseCrudApp.Domain.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [Phone]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Department { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Position { get; set; } = string.Empty;
        
        [Range(0, double.MaxValue)]
        public decimal Salary { get; set; }
        
        public DateTime HireDate { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        // Computed property
        public string FullName => $"{FirstName} {LastName}";
        
        // Business methods
        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }
        
        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
        
        public void UpdateSalary(decimal newSalary)
        {
            if (newSalary < 0)
                throw new ArgumentException("Salary cannot be negative", nameof(newSalary));
                
            Salary = newSalary;
            UpdatedAt = DateTime.UtcNow;
        }
        
        public void UpdateContactInformation(string? phoneNumber, string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));
                
            PhoneNumber = phoneNumber;
            Email = email;
            UpdatedAt = DateTime.UtcNow;
        }
        
        public void Promote(string newPosition, decimal newSalary)
        {
            if (string.IsNullOrWhiteSpace(newPosition))
                throw new ArgumentException("Position cannot be empty", nameof(newPosition));
                
            if (newSalary < Salary)
                throw new ArgumentException("New salary cannot be less than current salary", nameof(newSalary));
                
            Position = newPosition;
            Salary = newSalary;
            UpdatedAt = DateTime.UtcNow;
        }
        
        public void TransferToDepartment(string newDepartment)
        {
            if (string.IsNullOrWhiteSpace(newDepartment))
                throw new ArgumentException("Department cannot be empty", nameof(newDepartment));
                
            Department = newDepartment;
            UpdatedAt = DateTime.UtcNow;
        }
    }
} 