using Microsoft.EntityFrameworkCore;
using EnterpriseCrudApp.Domain.Entities;
using EnterpriseCrudApp.Domain.Interfaces;
using EnterpriseCrudApp.Infrastructure.Data;

namespace EnterpriseCrudApp.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _context.Employees
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .ToListAsync();
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Employee?> GetByEmailAsync(string email)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.Email.ToLower() == email.ToLower());
        }

        public async Task<IEnumerable<Employee>> GetByDepartmentAsync(string department)
        {
            return await _context.Employees
                .Where(e => e.Department.ToLower() == department.ToLower())
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetActiveEmployeesAsync()
        {
            return await _context.Employees
                .Where(e => e.IsActive)
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .ToListAsync();
        }

        public async Task<Employee> AddAsync(Employee employee)
        {
            var entry = await _context.Employees.AddAsync(employee);
            return entry.Entity;
        }

        public async Task<Employee> UpdateAsync(Employee employee)
        {
            var existingEmployee = await _context.Employees.FindAsync(employee.Id);
            if (existingEmployee == null)
            {
                throw new InvalidOperationException($"Employee with ID {employee.Id} not found");
            }

            _context.Entry(existingEmployee).CurrentValues.SetValues(employee);
            _context.Entry(existingEmployee).Property(e => e.CreatedAt).IsModified = false;
            _context.Entry(existingEmployee).Property(e => e.UpdatedAt).IsModified = true;

            return existingEmployee;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return false;
            }

            _context.Employees.Remove(employee);
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Employees
                .AnyAsync(e => e.Id == id);
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
        {
            var query = _context.Employees
                .Where(e => e.Email.ToLower() == email.ToLower());

            if (excludeId.HasValue)
            {
                query = query.Where(e => e.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
} 