using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentRegistration.API.DbContexts;
using StudentRegistration.API.Mappers;
using StudentRegistration.Shared.Models.API;
using StudentRegistration.API.DbEntities;

namespace StudentRegistration.API.Controllers;

[ApiController]
[Route("v1/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ILogger<CoursesController> _logger;
    private StudentRegistrationContext StudentRegistrationContext { get; init; }

    public CoursesController(ILogger<CoursesController> logger, StudentRegistrationContext studentRegistrationContext)
    {
        _logger = logger;
        StudentRegistrationContext = studentRegistrationContext;
    }

    // Get Requests Start Here
    [HttpGet("GetAllCourses")] // Retrieve all courses.
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllCourses()
    {
        List<Shared.Models.API.Course> courses =
            await StudentRegistrationContext.Courses.Select(c => c.ToApiModel()).ToListAsync();

        _logger.LogInformation($"Retrieved {courses.Count} courses", courses.Count);
        return Ok(courses);
    }

    [HttpGet("GetCourseById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourseById([Required] [FromQuery] int courseId) // Retrieve a specific course by ID.
    {
        DbEntities.Course? courses = await  StudentRegistrationContext.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId);

        if (courses == null)
        {
            _logger.LogWarning("Course with id {COURSE_ID} not found", courseId);
            return NotFound();
        }
        
        _logger.LogInformation("Retrieved course with {COURSE_ID} " , courseId);
        return Ok(courses.ToApiModel());
    }

    [HttpGet("GetStudentsForCourse")] // Retrieve all courses.
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudentsForCourse([Required][FromQuery] int courseId) // Retrieve all students enrolled in a specific course
    {
        
        DbEntities.Course? courses = await  StudentRegistrationContext.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId); // find the course id

        if (courses == null)
        {
            _logger.LogWarning("Course with course id {COURSE_ID} not found", courseId);
            return NotFound();
        }
        
        List<Shared.Models.API.Student> studentsReturn = await StudentRegistrationContext.Enrollments
            .Where(e => e.CourseId == courseId)
            .Select(e => e.Student.ToApiModel())
            .ToListAsync();
        
        return Ok(studentsReturn);
    }

    [HttpGet("GetCourseSummaries")] // Retrieve all courses.
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourseSummaries() // Retrieve a list of all courses with total student counts
    {
        var courseWithCount = await StudentRegistrationContext.Courses.Select(c => new
        {
            CourseId = c.CourseId,
            Title = c.Title,
            Credits = c.Credits,
            Count = c.Enrollments.Count() // just want the count, dont need nothing else 
        })
        .ToListAsync();

        return Ok(courseWithCount);
    }

    [HttpGet("GetLowEnrollmentCourses")] // Retrieve all courses with enrollment below a given threshold, in this case i did if its less than two
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLowEnrollmentCourses([Required] [FromQuery] int threshold)
    {
            var courses = await StudentRegistrationContext.Courses.Where(c => c.Enrollments.Count() <= threshold).Select(c => new // have to do where before the select because cant do it after on an anonymous object.
                {
                    CourseId = c.CourseId,
                    Title = c.Title,
                    Credits = c.Credits,
                    Count = c.Enrollments.Count() // just want the count, dont need nothing else 
                })
                .ToListAsync();
            
            _logger.LogInformation("Got {Count} number of courses with enrollment below the {threshold}", courses.Count, threshold);
            return Ok(courses);
    }
    
    // Get requests end here
    
    // Post requests start here
    
    [HttpPost("CreateCourse")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateCourse([Required] [FromBody] CreateCourseRequest course)
    {
        int newCoursesID = await StudentRegistrationContext.Courses.MaxAsync(s => (int?)s.CourseId) + 1 ?? 0; // add to the end of courses id to make a new one, so it still increments

        DbEntities.Course newCourse = new DbEntities.Course
        {
            CourseId = newCoursesID,
            Title = course.TITLE,
            Credits = course.CREDITS,
        };
        
        await StudentRegistrationContext.Courses.AddAsync(newCourse);
        await StudentRegistrationContext.SaveChangesAsync();

        _logger.LogInformation("Created new Course with the ID" +
                               " {COURSE_ID}" +
                               " Title: {TITLE} " +
                               "Credits: {CREDITS} " , newCoursesID, course.TITLE, course.CREDITS);
        
        return CreatedAtAction(nameof(GetCourseById), new { course = newCoursesID }, course);
    }
    
    // Post requests end here
    
    // Put requests start here

    [HttpPut("UpdateCourse")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCourse([Required] [FromBody] Shared.Models.API.Course courseId)
    {
        DbEntities.Course? existingCourse =
            await StudentRegistrationContext.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId.COURSE_ID);
        
        if (existingCourse == null)
        {
            _logger.LogWarning("Attempted to update course with id {COURSE_ID} but not found", courseId.COURSE_ID);
            return NotFound();
        }
        
        existingCourse.Title = courseId.TITLE;
        existingCourse.Credits = courseId.CREDITS;
        
        await StudentRegistrationContext.SaveChangesAsync();
        
        _logger.LogInformation("Updated course with course id {COURSE_ID} ", courseId.COURSE_ID);

        return Ok(existingCourse.ToApiModel());
    }
    
    // Put requests end here
    
    // Delete requests start here

    [HttpDelete("DeleteCourse")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCourse([Required] [FromQuery] int courseId)
    {
        DbEntities.Course? course = await StudentRegistrationContext.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId);

        if (course == null)
        {
            _logger.LogWarning("Attempted to delete course with id {COURSE_ID} but not found", courseId);
            return NotFound();
        }
        
        StudentRegistrationContext.Courses.Remove(course);
        await StudentRegistrationContext.SaveChangesAsync();
        _logger.LogInformation("Deleted course with course ID {COURSE_ID}", courseId);
        
        return Ok();
    }
    // delete requests end here

    
}

   