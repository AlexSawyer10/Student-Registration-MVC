using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StudentRegistration.Shared.Models.API;
using StudentRegistration;

namespace StudentRegistration.Controllers;

public class CoursesController : Controller
{
    
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly String API_URL = $"{Constants.API_URL}/v1/Courses";
    
    public CoursesController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    [HttpGet]
    public async Task<ActionResult> Index()
    {
        try
        {
            HttpClient client = _httpClientFactory.CreateClient();
            IEnumerable<Course>? courses =
                await client.GetFromJsonAsync<IEnumerable<Course>>($"{API_URL}/GetAllCourses");
            
            
            if (courses == null)
            {
                throw new HttpRequestException("No courses found");
            }

            return View(courses.ToList());
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
    public async Task<ActionResult> Save(Course course)
    {
        if (!ModelState.IsValid)
        {
            throw new HttpRequestException($"Model state is not valid! Enter valid inputs.");
                
        }
        
        try
        {
            
            
            HttpClient client = _httpClientFactory.CreateClient();
            HttpResponseMessage response = await client.PutAsJsonAsync($"{API_URL}/UpdateCourse", course);

            if (response.IsSuccessStatusCode == false)
            {
                throw new HttpRequestException($"Error saving course with the id {course.COURSE_ID}");
            }
            
            

            TempData["Message"] = $"Course with the id {course.COURSE_ID} updated";
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
    public async Task<ActionResult> Delete(int courseId)
    {
        try
        {
            HttpClient client = _httpClientFactory.CreateClient();
            HttpResponseMessage response =
                await client.DeleteAsync($"{API_URL}/DeleteCourse?courseId={courseId}");
            
            
            if (response.IsSuccessStatusCode == false)
            {
                throw new HttpRequestException($"Error deleting course with the id: {courseId}");
            }

            TempData["Message"] = $"Course with the id: {courseId} deleted";
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
    public async Task<ActionResult> CreateCourse(CreateCourseRequest course)
    {
        try
        {
            
            HttpClient client = _httpClientFactory.CreateClient();
            HttpResponseMessage response = await client.PostAsJsonAsync($"{API_URL}/CreateCourse", course);

            if (!ModelState.IsValid)
            {
                throw new HttpRequestException($"Model state is not valid! Enter valid inputs.");
                
            }
            
            if (response.IsSuccessStatusCode == false)
            {
                throw new HttpRequestException($"Error creating course with the id: {course.COURSE_ID}");
            }

            TempData["Message"] = $"Course with the id: {course.COURSE_ID} created";
            return RedirectToAction("Index");
        }
        catch (HttpRequestException e)
        {
#if DEBUG
            Debugger.Break();
            throw e;
#endif
            Console.WriteLine("Error fetching enrollments" + e.Message);
            return View("Error");
        }
    }
    
    [HttpGet]
    [Route("Courses/Detail/{courseId}")]
    public async Task<IActionResult> Detail(int courseId)
    {
        
        HttpClient client = _httpClientFactory.CreateClient();
        try
        {
            Course? response = await client.GetFromJsonAsync<Course>($"{Constants.API_URL}/v1/Courses/GetCourseById?courseId={courseId}");
            
            
            if (response == null)
            {
                throw new HttpRequestException("Course not found");
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