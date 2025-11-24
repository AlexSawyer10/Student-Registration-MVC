namespace StudentRegistration;

public class Constants
{
    public static String API_URL { get; private set; } = String.Empty;
    
    public static void SetAPI_URL(String url)
    {
        if (String.IsNullOrWhiteSpace(API_URL) == false)
        {
            throw new InvalidOperationException("API_URL has already been set");
        }
        
        API_URL = url;
    }
}