using System.ComponentModel.DataAnnotations;
using System.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentRegistration.API.DbContexts;
using StudentRegistration.API.Mappers;
using StudentRegistration.Shared.Models.API;
using StudentRegistration.API.DbEntities;
using Enrollment = StudentRegistration.Shared.Models.API.Enrollment;
using Course = StudentRegistration.Shared.Models.API.Course;
using Student = StudentRegistration.Shared.Models.API.Student;

namespace StudentRegistration.API.Controllers;

// Note: The difference between iQueryable and List<Shared.Models.API.Student> is iQueryable allows you to run more complex queries and use that variable in another query
// TODO
// FIX ROUTE?
// https://learn.microsoft.com/en-us/dotnet/csharp/linq/ refer for linq documentation

// required is when a value must not be null
[ApiController]
[Route("v1/[controller]")]
public class StudentController : ControllerBase
{
    private readonly ILogger<StudentController> _logger;
    private StudentRegistrationContext StudentRegistrationContext { get; init; }
    
    public StudentController(ILogger<StudentController> logger, StudentRegistrationContext studentRegistrationContext)
    {
        _logger = logger;
        StudentRegistrationContext = studentRegistrationContext;
    }
    
    // Get Requests Start Here
    [HttpGet("GetAllStudents")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllStudents() //Retrieves all students.
    {
        // i enumerable executes in memory
        List<Shared.Models.API.Student> students = await StudentRegistrationContext.Students.Select(e => e.ToApiModel()).ToListAsync();
        
        _logger.LogInformation($"Retrieved {students.Count} students" , students.Count);
        return Ok(students);
    }

    [HttpGet("GetStudentById")] //. get student by a specific id
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudentById([Required] [FromQuery] int studentId)
    {
        DbEntities.Student? students = await  StudentRegistrationContext.Students.FirstOrDefaultAsync(e => e.StudentId == studentId);
        // first or default async gets a single 
        if (students == null)
        {
            _logger.LogWarning("Student with id {STUDENT_ID} not found", studentId);
            return NotFound();
        }
        
        _logger.LogInformation("Retrieved student with {STUDENT_ID} " , studentId);
        return Ok(students.ToApiModel());
    }

    [HttpGet("GetCoursesForStudent")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCoursesForStudent([Required][FromQuery] int studentId) // retrieve all courses a student is enrolled in. 
    {
        
        List<DbEntities.Course>? courses = await StudentRegistrationContext.Courses.Where(c => c.Enrollments
          .Any(e => e.StudentId == studentId)) // any finds all the courses where enrollment matches that student
            .Select(e => new DbEntities.Course {CourseId = e.CourseId, Credits = e.Credits, Title = e.Title})
          .ToListAsync(); 
        
      if (courses.Count == 0) // if nothing comes back
      {
          _logger.LogWarning("Student with id {STUDENT_ID} was not found in any courses", studentId);
          return NotFound();
      }
        return Ok(courses);
    }

    [HttpGet("GetStudentSummaries")] // Retrieve all students with their average grade
    public async Task<IActionResult> GetStudentSummaries() // get all students, then find their average grades. to get their average grades you should add up all the grades the divide that by the amount of grades your getting
    {
        IQueryable<int> studentIDs = StudentRegistrationContext.Students.Select(s => s.StudentId); // get all students

        List<object?> studentsAverageGradeList = new List<object?>(); // make it object because you want multiple data types returned

        foreach (int studentID in studentIDs.ToList())
        {
            List<decimal?> courseGrade = // type decimal cuz grades is a double and can be null
                await StudentRegistrationContext.Enrollments.Where(e => e.StudentId == studentID)
                    .Select(e => e.Grade )
                    .ToListAsync();
            
            if (courseGrade.Count > 0)
            {
                var student = await StudentRegistrationContext.Students.FirstOrDefaultAsync(e => e.StudentId == studentID);
                decimal? studentsGradeAverage = courseGrade.Average();
                
                studentsAverageGradeList.Add(new
                {
                StudentId = student.StudentId,
                FirstName = student.FirstName,
                LastName = student.LastName,
                studentsGradeAverage = studentsGradeAverage
                });
            }
            else if (courseGrade.Count == 0)
            {
                _logger.LogWarning("Student with id {STUDENT_ID} was not found", studentID); // warning and not error because you can still retrieve others if one is not found
            }
        }
        return Ok(studentsAverageGradeList);
    }
    
    // Get Requests End Here
    
    // Post Requests Start Here
    [HttpPost("CreateStudent")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateStudent([Required] [FromBody] CreateStudentRequest student)
    {
        try
        {



            int newStudentID =
                await StudentRegistrationContext.Students.MaxAsync(s => (int?)s.StudentId) + 1 ??
                0; // add to the end of student id to make a new one, so it still increments

            DbEntities.Student newStudent = new DbEntities.Student
            {
                StudentId = newStudentID,
                FirstName = student.FIRST_NAME,
                LastName = student.LAST_NAME,
                Email = student.EMAIL,
                PhoneNumber = student.PHONE_NUMBER,
                DateOfBirth = student.DATE_OF_BIRTH,
                Major = student.MAJOR
            };

            await StudentRegistrationContext.Students.AddAsync(newStudent);
            await StudentRegistrationContext.SaveChangesAsync();

            _logger.LogInformation("Created new Student with the ID" +
                                   " {StudentId}" +
                                   " Name: {FirstName} {LastName}" +
                                   "Email: {Email} " +
                                   "Phone Number: {PhoneNumber}" +
                                   "DOB: {DateOfBirth}" +
                                   "Major: {Major}", newStudentID, student.FIRST_NAME, student.LAST_NAME, student.EMAIL,
                student.PHONE_NUMBER,
                student.DATE_OF_BIRTH, student.MAJOR);

            return CreatedAtAction(nameof(GetStudentById), new { student = newStudentID }, student);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest("Cant have duplicate emails");
        }
    }
    
    // Post Requests End Here
    
    // Requests Start Here
    [HttpDelete("DeleteStudent")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteStudent([Required] [FromQuery] int studentId)
    {
        DbEntities.Student? student = await StudentRegistrationContext.Students.FirstOrDefaultAsync(e => e.StudentId == studentId);

        if (student == null)
        {
            _logger.LogWarning("Attempted to delete student with id {STUDENT_ID} but not found", studentId);
            return NotFound();
        }
        
        StudentRegistrationContext.Students.Remove(student);
        await StudentRegistrationContext.SaveChangesAsync();
        _logger.LogInformation("Deleted student with Student ID {STUDENT_ID}", studentId);
        
        return Ok();
    }
    //Delete Requests End Here
    
    
    // Put Requests Start Here
    [HttpPut("UpdateStudent")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStudent([Required] [FromBody]  Shared.Models.API.Student student)
    {
        DbEntities.Student? existingStudent =
            await StudentRegistrationContext.Students.FirstOrDefaultAsync(e => e.StudentId == student.STUDENT_ID);
        
        if (existingStudent== null)
        {
            _logger.LogWarning("Attempted to update student with id {STUDENT_ID} but not found", student.STUDENT_ID);
            return NotFound();
        }

        existingStudent.FirstName = student.FIRST_NAME;
        existingStudent.LastName = student.LAST_NAME;
        existingStudent.Email = student.EMAIL;
        existingStudent.PhoneNumber = student.PHONE_NUMBER; // might need to string but when i add its redundant so idk
        existingStudent.DateOfBirth = student.DATE_OF_BIRTH;
        existingStudent.Major = student.MAJOR;
        
        await StudentRegistrationContext.SaveChangesAsync();
        
        _logger.LogInformation("Updated student with student id {STUDENT_ID} ", student.STUDENT_ID);
        return Ok(existingStudent.ToApiModel());

        
    }
    
    // Put Requests End Here
    
}