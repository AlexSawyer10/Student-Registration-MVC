using System.ComponentModel.DataAnnotations;
using System.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentRegistration.API.DbContexts;
using StudentRegistration.API.Mappers;
using StudentRegistration.Shared.Models.API;
using StudentRegistration.API.DbEntities;
using Enrollment = StudentRegistration.API.DbEntities.Enrollment;

namespace StudentRegistration.API.Controllers;

[ApiController]
[Route("v1/[controller]")]
public class EnrollmentsController : ControllerBase
{
    private readonly ILogger<EnrollmentsController> _logger;
    private StudentRegistrationContext StudentRegistrationContext { get; init; }

    public EnrollmentsController(ILogger<EnrollmentsController> logger,
        StudentRegistrationContext studentRegistrationContext)
    {
        _logger = logger;
        StudentRegistrationContext = studentRegistrationContext;

    }

    // Get Requests Start Here
    [HttpGet("GetAllEnrollments")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllEnrollments() //Retrieves all enrollments.
    {
        // i enumerable executes in memory
        List<Shared.Models.API.Enrollment> enrollments =
            await StudentRegistrationContext.Enrollments.Select(e => e.ToApiModel()).ToListAsync();

        _logger.LogInformation($"Retrieved {enrollments.Count} enrollments", enrollments.Count);
        return Ok(enrollments);
    }

    [HttpGet("GetEnrollmentById")] // Retrieve a single enrollment record by id
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEnrollmentById([Required] [FromQuery] int enrollmentId)
    {
        DbEntities.Enrollment? enrollment =
            await StudentRegistrationContext.Enrollments.FirstOrDefaultAsync(e => e.EnrollmentId == enrollmentId);
        // first or default async gets a single 
        if (enrollment == null)
        {
            _logger.LogWarning("Enrollment with id {ENROLLMENT_ID} not found", enrollmentId);
            return NotFound();
        }

        _logger.LogInformation("Retrieved enrollment with {ENROLLMENT_ID} ", enrollmentId);
        return Ok(enrollment.ToApiModel());
    }

    [HttpGet("GetEnrollmentsByStudent")] // Retrieve all enrollments for a given student
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEnrollmentsByStudent([Required] [FromQuery] int studentId)
    {

        List<Shared.Models.API.Enrollment> enrollments = await
            StudentRegistrationContext.Enrollments.Where(e => e.StudentId == studentId)
                .Select(e => e.ToApiModel())
                .ToListAsync();
        if (enrollments.Count == 0)
        {
            _logger.LogWarning($"student with id {studentId} was not found enrolled in anything ", studentId);
            return NotFound();
        }

        return Ok(enrollments);
    }

    [HttpGet("GetEnrollmentsByCourse")] // Retrieve all enrollments for a given course
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEnrollmentsByCourse([Required] [FromQuery] int courseId)
    {
        List<Shared.Models.API.Enrollment> enrollments = await
            StudentRegistrationContext.Enrollments.Where(e => e.CourseId == courseId)
                .Select(e => e.ToApiModel())
                .ToListAsync();
        if (enrollments.Count == 0)
        {
            _logger.LogWarning($"course with id {courseId} was not found enrolled in anything ", courseId);
            return NotFound();
        }

        return Ok(enrollments);
    }

    [HttpGet("GetEnrollmentStatistics")] // Retrieve average grade per student and number of enrollments for that student
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEnrollmentStatistics()
    {
        IQueryable<int>
            studentIDs =
                StudentRegistrationContext.Enrollments.Select(s => s.StudentId)
                    .Distinct(); // get all students that are enrolled in something

        List<object?>
            studentsAvgAndEnrollmentCount =
                new List<object?>(); // make it object because you want multiple data types returned

        foreach (int studentID in studentIDs.ToList())
        {
            List<decimal?> courseGrade = // type decimal cuz grades is a double and can be null
                await StudentRegistrationContext.Enrollments
                    .Where(e => e.StudentId == studentID)
                    .Select(e => e.Grade)
                    .ToListAsync();

            if (courseGrade.Count > 0)
            {
                var student =
                    await StudentRegistrationContext.Students.FirstOrDefaultAsync(e => e.StudentId == studentID);

                decimal? studentsGradeAverage = courseGrade.Average();

                studentsAvgAndEnrollmentCount.Add(new
                {
                    StudentId = student.StudentId,
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    AverageGrade = studentsGradeAverage,
                    TotalEnrollments = courseGrade.Count
                });
            }
            else
            {
                _logger.LogWarning("Course grades not found"); // warning and not error because you can still retrieve others if one is not found
            }
        }
        
        return Ok(studentsAvgAndEnrollmentCount);
    }
    
    
    // Get requests end here
    
    // Delete requests start here
    [HttpDelete("DeleteEnrollment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEnrollment([Required] [FromQuery] int enrollmentId)
    {
        DbEntities.Enrollment? enrollment = await StudentRegistrationContext.Enrollments.FirstOrDefaultAsync(e => e.EnrollmentId == enrollmentId);

        if (enrollment == null)
        {
            _logger.LogWarning("Attempted to delete enrollment with id {ENROLLMENT_ID} but not found", enrollmentId);
            return NotFound();
        }
        
        StudentRegistrationContext.Enrollments.Remove(enrollment);
        await StudentRegistrationContext.SaveChangesAsync();
        _logger.LogInformation("Deleted enrollment with Enrollment ID {ENROLLMENT_ID}", enrollmentId);
        
        return Ok();
    }
    // delete requests end here 
    
    // Post requests start here
    
    [HttpPost("CreateEnrollment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateEnrollment([Required] [FromBody] CreateEnrollmentRequest enrollment)
    {
        int newEnrollmentID = await StudentRegistrationContext.Enrollments.MaxAsync(e => (int?)e.EnrollmentId) + 1 ?? 0; // add to the end of enrollment id to make a new one, so it still increments

        DbEntities.Enrollment newEnrollment = new DbEntities.Enrollment
        {
            EnrollmentId = newEnrollmentID,
            StudentId = enrollment.STUDENT_ID,
            CourseId = enrollment.COURSE_ID,
            Grade = enrollment.GRADE,
        };
        
        await StudentRegistrationContext.Enrollments.AddAsync(newEnrollment);
        await StudentRegistrationContext.SaveChangesAsync();
        
        _logger.LogInformation("Created new Enrollment with the ID" +
                               " {ENROLLMENT_ID}" +
                               " Student Id: {STUDENT_ID} "+
                               "Course Id: {COURSE_ID} " +
                               "GRADE: {GRADE}", newEnrollmentID, enrollment.STUDENT_ID, enrollment.COURSE_ID, enrollment.GRADE);
        
        return CreatedAtAction(nameof(GetEnrollmentById), new { enrollmentId = newEnrollmentID }, enrollment);
    }
    
    // post requests end here
    
    // put requests start here

    [HttpPut("UpdateEnrollmentGrade")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEnrollmentGrade([Required] [FromBody] Shared.Models.API.Enrollment enrollment)
    {
        DbEntities.Enrollment? existingEnrollment =
            await StudentRegistrationContext.Enrollments.FirstOrDefaultAsync(e => e.EnrollmentId == enrollment.ENROLLMENT_ID);
        
        if (existingEnrollment== null)
        {
            _logger.LogWarning("Attempted to update enrollment with id {ENROLLMENT_ID} but not found", enrollment.ENROLLMENT_ID);
            return NotFound();
        }

        existingEnrollment.CourseId = enrollment.COURSE_ID;
        existingEnrollment.Grade = enrollment.GRADE;
        
        await StudentRegistrationContext.SaveChangesAsync();
        
        _logger.LogInformation("Updated enrollment with enrollment id {ENROLLMENT_ID} ", enrollment);
        return Ok(existingEnrollment.ToApiModel());

    }
    
    
}
