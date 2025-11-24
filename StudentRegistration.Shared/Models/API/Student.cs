using System.ComponentModel.DataAnnotations;

namespace StudentRegistration.Shared.Models.API;

public sealed class Student
{
   
    public int STUDENT_ID { get; set; }
    
    [Required(ErrorMessage = "First name is required")]
    public string FIRST_NAME { get; set; } = String.Empty;
    
    [Required(ErrorMessage = "Last name is required")]
    public string LAST_NAME { get; set; } = String.Empty;
    
    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress (ErrorMessage = "Not a valid email address")]
    public string EMAIL { get; set; } = String.Empty;
    
    public string PHONE_NUMBER { get; set; } = String.Empty;
    
    public DateTime DATE_OF_BIRTH { get; set; }
    
    public string MAJOR { get; set; } = String.Empty;
}


public sealed class CreateStudentRequest
{
    public int STUDENT_ID { get; set; }
    
    [Required(ErrorMessage = "First name is required")]
    public string FIRST_NAME { get; set; } = String.Empty;
    
    [Required(ErrorMessage = "Last name is required")]
    public string LAST_NAME { get; set; } = String.Empty;
    
    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress (ErrorMessage = "Not a valid email address")]
    public string EMAIL { get; set; } = String.Empty;
    
    public string PHONE_NUMBER { get; set; } = String.Empty;
    
    public DateTime DATE_OF_BIRTH { get; set; }
    
    public string MAJOR { get; set; } = String.Empty;

}
