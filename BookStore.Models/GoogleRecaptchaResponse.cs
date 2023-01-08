namespace BookStore.Models;

public class GoogleRecaptchaResponse
{
    public bool success { get; set; }
    public DateTime? challenge_ts { get; set; }  
    public string? hostname { get; set; }        
    public  string[]? error_codes { get; set; }
}