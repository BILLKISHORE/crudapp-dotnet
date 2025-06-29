using FluentValidation;
using EnterpriseCrudApp.Application.DTOs;

namespace EnterpriseCrudApp.Application.Validators
{
    public class CreateEmployeeValidator : AbstractValidator<CreateEmployeeDto>
    {
        public CreateEmployeeValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(100).WithMessage("First name cannot exceed 100 characters")
                .Matches("^[a-zA-Z\\s]+$").WithMessage("First name can only contain letters and spaces");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters")
                .Matches("^[a-zA-Z\\s]+$").WithMessage("Last name can only contain letters and spaces");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Please provide a valid email address")
                .MaximumLength(255).WithMessage("Email cannot exceed 255 characters");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters")
                .Matches(@"^[\+]?[0-9\s\-\(\)]+$").WithMessage("Invalid phone number format")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            RuleFor(x => x.Department)
                .NotEmpty().WithMessage("Department is required")
                .MaximumLength(100).WithMessage("Department cannot exceed 100 characters")
                .Must(BeValidDepartment).WithMessage("Please select a valid department");

            RuleFor(x => x.Position)
                .NotEmpty().WithMessage("Position is required")
                .MaximumLength(100).WithMessage("Position cannot exceed 100 characters");

            RuleFor(x => x.Salary)
                .GreaterThan(0).WithMessage("Salary must be greater than 0")
                .LessThan(10000000).WithMessage("Salary seems unrealistic");

            RuleFor(x => x.HireDate)
                .NotEmpty().WithMessage("Hire date is required")
                .LessThanOrEqualTo(DateTime.Today).WithMessage("Hire date cannot be in the future")
                .GreaterThan(new DateTime(1900, 1, 1)).WithMessage("Hire date must be after 1900");
        }

        private bool BeValidDepartment(string department)
        {
            var validDepartments = new[]
            {
                "Engineering", "Human Resources", "Finance", "Marketing", 
                "Sales", "Operations", "IT", "Legal", "Customer Service"
            };
            
            return validDepartments.Contains(department, StringComparer.OrdinalIgnoreCase);
        }
    }
} 