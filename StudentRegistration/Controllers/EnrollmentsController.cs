using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StudentRegistration.Shared.Models.API;
using StudentRegistration;

namespace StudentRegistration.Controllers;

public class EnrollmentController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly String API_URL = $"{Constants.API_URL}/v1/Enrollments";

    public EnrollmentController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public async Task<ActionResult> Index()
    {
        try
        {
            HttpClient client = _httpClientFactory.CreateClient();
            IEnumerable<Enrollment>? enrollments =
                await client.GetFromJsonAsync<IEnumerable<Enrollment>>($"{API_URL}/GetAllEnrollments");
            if (enrollments == null)
            {
                throw new HttpRequestException("No enrollments found");
            }

            return View(enrollments.ToList());
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
    public async Task<ActionResult> Save(Enrollment enrollment)
    {
        if (!ModelState.IsValid)
        {
            throw new HttpRequestException($"Model state is not valid! Enter valid inputs.");
        }
        try
        {
            HttpClient client = _httpClientFactory.CreateClient();
            HttpResponseMessage response = await client.PutAsJsonAsync($"{API_URL}/UpdateEnrollmentGrade", enrollment);

            if (response.IsSuccessStatusCode == false)
            {
                throw new HttpRequestException($"Error saving Enrollment Grade with the id {enrollment.ENROLLMENT_ID}");
            }

            TempData["Message"] = $"Enrollment with the id {enrollment.ENROLLMENT_ID} updated";
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
    public async Task<ActionResult> CreateEnrollment(CreateEnrollmentRequest enrollment)
    {
        if (!ModelState.IsValid)
        {
            throw new HttpRequestException($"Model state is not valid! Enter valid inputs.");
        }
        
        try
        {
            HttpClient client = _httpClientFactory.CreateClient();
            HttpResponseMessage response = await client.PostAsJsonAsync($"{API_URL}/CreateEnrollment", enrollment);

            if (response.IsSuccessStatusCode == false)
            {
                throw new HttpRequestException($"Error creating enrollment with the id: {enrollment.ENROLLMENT_ID}");
            }

            TempData["Message"] = $"Enrollment with the id: {enrollment.ENROLLMENT_ID} created";
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

    [HttpPost]
    public async Task<ActionResult> Delete(int enrollmentId)
    {
        try
        {
            HttpClient client = _httpClientFactory.CreateClient();
            HttpResponseMessage response =
                await client.DeleteAsync($"{API_URL}/DeleteEnrollment?enrollmentId={enrollmentId}");

            if (response.IsSuccessStatusCode == false)
            {
                throw new HttpRequestException($"Error deleting enrollment {enrollmentId}");
            }

            TempData["Message"] = $"Enrollment with the id: {enrollmentId} deleted";
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
    
    [HttpGet]
    [Route("Enrollment/Detail/{enrollmentId}")]
    public async Task<IActionResult> Detail(int enrollmentId)
    {
        
        HttpClient client = _httpClientFactory.CreateClient();
        try
        {
            Enrollment? response = await client.GetFromJsonAsync<Enrollment>($"{Constants.API_URL}/v1/Enrollments/GetEnrollmentById?enrollmentId={enrollmentId}");

            if (response == null)
            {
                throw new HttpRequestException("Enrollment not found");
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