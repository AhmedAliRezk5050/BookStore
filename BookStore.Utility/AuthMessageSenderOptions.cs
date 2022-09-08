namespace BookStore.Utility;

public class AuthMessageSenderOptions
{
    public string? GmailAddress { get; set; }
    public string? GmailPassword { get; set; }
    
    public string? SendGridKey { get; set; }
}