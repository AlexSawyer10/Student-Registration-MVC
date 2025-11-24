using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StudentRegistration.Shared.Models.API;
using StudentRegistration;

namespace StudentRegistration.Controllers;


public class StudentController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly String API_URL = $"{Constants.API_URL}/v1/Student";

    public StudentController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }


    [HttpGet]
    public async Task<ActionResult> Index()
    {
        try
        {
            HttpClient client = _httpClientFactory.CreateClient();
            IEnumerable<Student>? students =
                await client.GetFromJsonAsync<IEnumerable<Student>>($"{API_URL}/GetAllStudents");
            if (students == null)
            {
                throw new HttpRequestException("No students found");
            }

            return View(students.ToList());
        }
        catch (HttpRequestException e)
        {
#if DEBUG
            Debugger.Break();
            throw e;
#endif
            Console.WriteLine("Error fetching employees" + e.Message);
            return View("Error");
        }
    }

   

    
    
    [HttpPost]
    public async Task<ActionResult> Save(Student student)
    {
        if (!ModelState.IsValid)
        {
            throw new HttpRequestException($"Model state is not valid! Enter valid inputs.");
        }
        
        try
        {
            
            HttpClient client = _httpClientFactory.CreateClient();
            HttpResponseMessage response = await client.PutAsJsonAsync($"{API_URL}/UpdateStudent", student);

            if (response.IsSuccessStatusCode == false)
            {
                throw new  HttpRequestException($"Error saving student {student.STUDENT_ID}");
            }
            TempData["Message"] = $"Student {student.STUDENT_ID} updated";
            return RedirectToAction("Index");
          
        }
        catch (HttpRequestException e)
        {
#if DEBUG
            Debugger.Break();
            throw e;
#endif
            Console.WriteLine("Error fetching employees" + e.Message);
            return View("Error");
        }
    }

    [HttpPost]
    public async Task<ActionResult> CreateStudent(CreateStudentRequest student)
    {
        if (!ModelState.IsValid)
        {
            throw new HttpRequestException($"Model state is not valid! Enter valid inputs.");
        }
        
        try
        {
            HttpClient client = _httpClientFactory.CreateClient();
            HttpResponseMessage response = await client.PostAsJsonAsync($"{API_URL}/CreateStudent", student);

            if (response.IsSuccessStatusCode == false)
            {
                throw new HttpRequestException($"Error creating student {student.FIRST_NAME}  {student.LAST_NAME}");
            }
            TempData["Message"] = $"Student {student.FIRST_NAME} {student.LAST_NAME} created";
            return  RedirectToAction("Index");
        }
        catch (HttpRequestException e)
        {
#if DEBUG
            Debugger.Break();
#endif
            
            TempData["Error"] = e.Message;
            Console.WriteLine("Error creating student, it was probably because of duplicate emails" + e.Message);
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public async Task<ActionResult> Delete(int studentId)
    {
        try
        {
            HttpClient client = _httpClientFactory.CreateClient();
            HttpResponseMessage response = await client.DeleteAsync($"{API_URL}/DeleteStudent?studentId={studentId}");

            if (response.IsSuccessStatusCode == false)
            {
                throw new HttpRequestException($"Error deleting student {studentId}");
            }

            TempData["Message"] = $"Student {studentId} deleted";
            return RedirectToAction("Index");
        }
        catch (HttpRequestException e)
        {
#if DEBUG
            Debugger.Break();
            throw e;
#endif
            Console.WriteLine("Error fetching students" + e.Message);
            return View("Error");
        }
    }
    
    [HttpGet]
    [Route("Student/Detail/{studentId}")]
    public async Task<IActionResult> Detail(int studentId)
    {
        
        HttpClient client = _httpClientFactory.CreateClient();
        try
        {
            Student? response = await client.GetFromJsonAsync<Student>($"{Constants.API_URL}/v1/Student/GetStudentById?studentId={studentId}");

            if (response == null)
            {
                throw new HttpRequestException("Student not found");
            }
            return View(response);
        }
        catch (HttpRequestException e)
        {
#if DEBUG
            Debugger.Break();
#endif
            Console.WriteLine("Message: " + e.Message);
            return View("Error");
        }
    }
    
    
}