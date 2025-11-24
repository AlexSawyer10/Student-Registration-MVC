using System.ComponentModel.DataAnnotations;

namespace StudentRegistration.Shared.Models.API;

public class Enrollment
{
    public int ENROLLMENT_ID { get; set; }
    
    [Required(ErrorMessage = "Student Id is required")]
    public int STUDENT_ID { get; set; }
    
    [Required(ErrorMessage = "Course Id is required")]
    public int COURSE_ID { get; set; }
    
    [Range(0, 110, ErrorMessage = "Grade must be between 0 and 110")]
    public decimal? GRADE { get; set; }
}

public sealed class CreateEnrollmentRequest
{
    public int ENROLLMENT_ID { get; set; }
    
    [Required(ErrorMessage = "Student Id is required")]
    public int STUDENT_ID { get; set; }
    
    [Required(ErrorMessage = "Course Id is required")]
    public int COURSE_ID { get; set; }
    
    [Range(0.00, 100.00, ErrorMessage = "Grade must be between 0 and 100")]
    public decimal? GRADE { get; set; }
}