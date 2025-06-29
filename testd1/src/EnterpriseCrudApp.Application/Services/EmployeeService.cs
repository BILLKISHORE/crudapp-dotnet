using AutoMapper;
using EnterpriseCrudApp.Application.DTOs;
using EnterpriseCrudApp.Application.Interfaces;
using EnterpriseCrudApp.Domain.Entities;
using EnterpriseCrudApp.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace EnterpriseCrudApp.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<EmployeeService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all employees");
                var employees = await _unitOfWork.Employees.GetAllAsync();
                return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all employees");
                throw;
            }
        }

        public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving employee with ID: {EmployeeId}", id);
                var employee = await _unitOfWork.Employees.GetByIdAsync(id);
                return employee != null ? _mapper.Map<EmployeeDto>(employee) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving employee with ID: {EmployeeId}", id);
                throw;
            }
        }

        public async Task<EmployeeDto?> GetEmployeeByEmailAsync(string email)
        {
            try
            {
                _logger.LogInformation("Retrieving employee with email: {Email}", email);
                var employee = await _unitOfWork.Employees.GetByEmailAsync(email);
                return employee != null ? _mapper.Map<EmployeeDto>(employee) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving employee with email: {Email}", email);
                throw;
            }
        }

        public async Task<IEnumerable<EmployeeDto>> GetEmployeesByDepartmentAsync(string department)
        {
            try
            {
                _logger.LogInformation("Retrieving employees from department: {Department}", department);
                var employees = await _unitOfWork.Employees.GetByDepartmentAsync(department);
                return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving employees from department: {Department}", department);
                throw;
            }
        }

        public async Task<IEnumerable<EmployeeDto>> GetActiveEmployeesAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving active employees");
                var employees = await _unitOfWork.Employees.GetActiveEmployeesAsync();
                return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving active employees");
                throw;
            }
        }

        public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto createEmployeeDto)
        {
            try
            {
                _logger.LogInformation("Creating new employee: {Email}", createEmployeeDto.Email);

                // Check if email already exists
                if (await _unitOfWork.Employees.EmailExistsAsync(createEmployeeDto.Email))
                {
                    throw new InvalidOperationException($"Employee with email {createEmployeeDto.Email} already exists");
                }

                var employee = _mapper.Map<Employee>(createEmployeeDto);
                var createdEmployee = await _unitOfWork.Employees.AddAsync(employee);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Employee created successfully with ID: {EmployeeId}", createdEmployee.Id);
                return _mapper.Map<EmployeeDto>(createdEmployee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating employee: {Email}", createEmployeeDto.Email);
                throw;
            }
        }

        public async Task<EmployeeDto?> UpdateEmployeeAsync(int id, UpdateEmployeeDto updateEmployeeDto)
        {
            try
            {
                _logger.LogInformation("Updating employee with ID: {EmployeeId}", id);

                var existingEmployee = await _unitOfWork.Employees.GetByIdAsync(id);
                if (existingEmployee == null)
                {
                    _logger.LogWarning("Employee not found with ID: {EmployeeId}", id);
                    return null;
                }

                // Check if email already exists for another employee
                if (await _unitOfWork.Employees.EmailExistsAsync(updateEmployeeDto.Email, id))
                {
                    throw new InvalidOperationException($"Employee with email {updateEmployeeDto.Email} already exists");
                }

                _mapper.Map(updateEmployeeDto, existingEmployee);
                existingEmployee.UpdatedAt = DateTime.UtcNow;

                var updatedEmployee = await _unitOfWork.Employees.UpdateAsync(existingEmployee);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Employee updated successfully with ID: {EmployeeId}", id);
                return _mapper.Map<EmployeeDto>(updatedEmployee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating employee with ID: {EmployeeId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting employee with ID: {EmployeeId}", id);

                var result = await _unitOfWork.Employees.DeleteAsync(id);
                if (result)
                {
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation("Employee deleted successfully with ID: {EmployeeId}", id);
                }
                else
                {
                    _logger.LogWarning("Employee not found for deletion with ID: {EmployeeId}", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting employee with ID: {EmployeeId}", id);
                throw;
            }
        }

        public async Task<bool> DeactivateEmployeeAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deactivating employee with ID: {EmployeeId}", id);

                var employee = await _unitOfWork.Employees.GetByIdAsync(id);
                if (employee == null)
                {
                    _logger.LogWarning("Employee not found with ID: {EmployeeId}", id);
                    return false;
                }

                employee.Deactivate();
                await _unitOfWork.Employees.UpdateAsync(employee);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Employee deactivated successfully with ID: {EmployeeId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deactivating employee with ID: {EmployeeId}", id);
                throw;
            }
        }

        public async Task<bool> ActivateEmployeeAsync(int id)
        {
            try
            {
                _logger.LogInformation("Activating employee with ID: {EmployeeId}", id);

                var employee = await _unitOfWork.Employees.GetByIdAsync(id);
                if (employee == null)
                {
                    _logger.LogWarning("Employee not found with ID: {EmployeeId}", id);
                    return false;
                }

                employee.Activate();
                await _unitOfWork.Employees.UpdateAsync(employee);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Employee activated successfully with ID: {EmployeeId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while activating employee with ID: {EmployeeId}", id);
                throw;
            }
        }

        public async Task<bool> EmployeeExistsAsync(int id)
        {
            try
            {
                return await _unitOfWork.Employees.ExistsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking if employee exists with ID: {EmployeeId}", id);
                throw;
            }
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
        {
            try
            {
                return await _unitOfWork.Employees.EmailExistsAsync(email, excludeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking if email exists: {Email}", email);
                throw;
            }
        }
    }
} 