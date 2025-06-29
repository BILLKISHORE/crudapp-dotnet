using Microsoft.AspNetCore.Mvc;
using EnterpriseCrudApp.Application.DTOs;
using EnterpriseCrudApp.Application.Interfaces;
using System.Net;

namespace EnterpriseCrudApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(IEmployeeService employeeService, ILogger<EmployeesController> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        /// <summary>
        /// Get all employees
        /// </summary>
        /// <returns>List of all employees</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EmployeeDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAllEmployees()
        {
            try
            {
                _logger.LogInformation("Getting all employees");
                var employees = await _employeeService.GetAllEmployeesAsync();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all employees");
                return StatusCode(500, new { Message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Get employee by ID
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <returns>Employee details</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(EmployeeDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<EmployeeDto>> GetEmployee(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { Message = "Invalid employee ID" });
                }

                _logger.LogInformation("Getting employee with ID: {EmployeeId}", id);
                var employee = await _employeeService.GetEmployeeByIdAsync(id);

                if (employee == null)
                {
                    _logger.LogWarning("Employee not found with ID: {EmployeeId}", id);
                    return NotFound(new { Message = $"Employee with ID {id} not found" });
                }

                return Ok(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting employee with ID: {EmployeeId}", id);
                return StatusCode(500, new { Message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Get employee by email
        /// </summary>
        /// <param name="email">Employee email</param>
        /// <returns>Employee details</returns>
        [HttpGet("email/{email}")]
        [ProducesResponseType(typeof(EmployeeDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<EmployeeDto>> GetEmployeeByEmail(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return BadRequest(new { Message = "Email is required" });
                }

                _logger.LogInformation("Getting employee with email: {Email}", email);
                var employee = await _employeeService.GetEmployeeByEmailAsync(email);

                if (employee == null)
                {
                    _logger.LogWarning("Employee not found with email: {Email}", email);
                    return NotFound(new { Message = $"Employee with email {email} not found" });
                }

                return Ok(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting employee with email: {Email}", email);
                return StatusCode(500, new { Message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Get employees by department
        /// </summary>
        /// <param name="department">Department name</param>
        /// <returns>List of employees in the department</returns>
        [HttpGet("department/{department}")]
        [ProducesResponseType(typeof(IEnumerable<EmployeeDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployeesByDepartment(string department)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(department))
                {
                    return BadRequest(new { Message = "Department is required" });
                }

                _logger.LogInformation("Getting employees from department: {Department}", department);
                var employees = await _employeeService.GetEmployeesByDepartmentAsync(department);
                return Ok(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting employees from department: {Department}", department);
                return StatusCode(500, new { Message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Get all active employees
        /// </summary>
        /// <returns>List of active employees</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<EmployeeDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetActiveEmployees()
        {
            try
            {
                _logger.LogInformation("Getting active employees");
                var employees = await _employeeService.GetActiveEmployeesAsync();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting active employees");
                return StatusCode(500, new { Message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Create a new employee
        /// </summary>
        /// <param name="createEmployeeDto">Employee creation data</param>
        /// <returns>Created employee</returns>
        [HttpPost]
        [ProducesResponseType(typeof(EmployeeDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<EmployeeDto>> CreateEmployee([FromBody] CreateEmployeeDto createEmployeeDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Creating new employee: {Email}", createEmployeeDto.Email);
                var employee = await _employeeService.CreateEmployeeAsync(createEmployeeDto);

                return CreatedAtAction(
                    nameof(GetEmployee),
                    new { id = employee.Id },
                    employee);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
            {
                _logger.LogWarning(ex, "Conflict while creating employee: {Email}", createEmployeeDto.Email);
                return Conflict(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating employee: {Email}", createEmployeeDto.Email);
                return StatusCode(500, new { Message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Update an existing employee
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <param name="updateEmployeeDto">Employee update data</param>
        /// <returns>Updated employee</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(EmployeeDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<EmployeeDto>> UpdateEmployee(int id, [FromBody] UpdateEmployeeDto updateEmployeeDto)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { Message = "Invalid employee ID" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Updating employee with ID: {EmployeeId}", id);
                var employee = await _employeeService.UpdateEmployeeAsync(id, updateEmployeeDto);

                if (employee == null)
                {
                    _logger.LogWarning("Employee not found for update with ID: {EmployeeId}", id);
                    return NotFound(new { Message = $"Employee with ID {id} not found" });
                }

                return Ok(employee);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
            {
                _logger.LogWarning(ex, "Conflict while updating employee with ID: {EmployeeId}", id);
                return Conflict(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating employee with ID: {EmployeeId}", id);
                return StatusCode(500, new { Message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Delete an employee
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { Message = "Invalid employee ID" });
                }

                _logger.LogInformation("Deleting employee with ID: {EmployeeId}", id);
                var success = await _employeeService.DeleteEmployeeAsync(id);

                if (!success)
                {
                    _logger.LogWarning("Employee not found for deletion with ID: {EmployeeId}", id);
                    return NotFound(new { Message = $"Employee with ID {id} not found" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting employee with ID: {EmployeeId}", id);
                return StatusCode(500, new { Message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Deactivate an employee
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <returns>Success status</returns>
        [HttpPatch("{id:int}/deactivate")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeactivateEmployee(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { Message = "Invalid employee ID" });
                }

                _logger.LogInformation("Deactivating employee with ID: {EmployeeId}", id);
                var success = await _employeeService.DeactivateEmployeeAsync(id);

                if (!success)
                {
                    _logger.LogWarning("Employee not found for deactivation with ID: {EmployeeId}", id);
                    return NotFound(new { Message = $"Employee with ID {id} not found" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deactivating employee with ID: {EmployeeId}", id);
                return StatusCode(500, new { Message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Activate an employee
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <returns>Success status</returns>
        [HttpPatch("{id:int}/activate")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ActivateEmployee(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { Message = "Invalid employee ID" });
                }

                _logger.LogInformation("Activating employee with ID: {EmployeeId}", id);
                var success = await _employeeService.ActivateEmployeeAsync(id);

                if (!success)
                {
                    _logger.LogWarning("Employee not found for activation with ID: {EmployeeId}", id);
                    return NotFound(new { Message = $"Employee with ID {id} not found" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while activating employee with ID: {EmployeeId}", id);
                return StatusCode(500, new { Message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Check if employee exists
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <returns>Boolean indicating existence</returns>
        [HttpHead("{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> EmployeeExists(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest();
                }

                var exists = await _employeeService.EmployeeExistsAsync(id);
                return exists ? Ok() : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking if employee exists with ID: {EmployeeId}", id);
                return StatusCode(500);
            }
        }
    }
} 