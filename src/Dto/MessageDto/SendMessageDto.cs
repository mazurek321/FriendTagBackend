public class SendMessageDto
{
    public string Content { get; set; }
    public IFormFile? Image { get; set; }  
    public double? Latitude { get; set; } 
    public double? Longitude { get; set; } 
}