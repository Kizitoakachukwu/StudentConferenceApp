namespace Presentation.Pages.Models
{
    public class DocumentDto
    {
        public string DocumentType { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
        public string FilePath { get; set; } = string.Empty;
    }
}
