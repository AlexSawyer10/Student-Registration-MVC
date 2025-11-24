using System.ComponentModel.DataAnnotations;

namespace StudentRegistration.Shared.Models.API;

public sealed class Course
{
    /*dont require cuz its auto generated*/
    public int COURSE_ID { get; set; }
    
    [Required(ErrorMessage = "Title is required")]
    public string TITLE { get; set; }
    
    [Required(ErrorMessage = "Credits is required")]
    public int CREDITS { get; set; } 
}

public sealed class CreateCourseRequest
{
    /*dont require cuz its auto generated*/
    public int COURSE_ID { get; set; }
    
    [Required(ErrorMessage = "Title is required")]
    public string TITLE { get; set; }
    
    [Required(ErrorMessage = "Credits is required")]
    [Range(1,6,ErrorMessage = "Credit must be between 0 and 6")]
    public int CREDITS { get; set; } 

}