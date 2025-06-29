using EnterpriseCrudApp.Application.DTOs;

namespace EnterpriseCrudApp.Application.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();
        Task<EmployeeDto?> GetEmployeeByIdAsync(int id);
        Task<EmployeeDto?> GetEmployeeByEmailAsync(string email);
        Task<IEnumerable<EmployeeDto>> GetEmployeesByDepartmentAsync(string department);
        Task<IEnumerable<EmployeeDto>> GetActiveEmployeesAsync();
        Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto createEmployeeDto);
        Task<EmployeeDto?> UpdateEmployeeAsync(int id, UpdateEmployeeDto updateEmployeeDto);
        Task<bool> DeleteEmployeeAsync(int id);
        Task<bool> DeactivateEmployeeAsync(int id);
        Task<bool> ActivateEmployeeAsync(int id);
        Task<bool> EmployeeExistsAsync(int id);
        Task<bool> EmailExistsAsync(string email, int? excludeId = null);
    }
} 