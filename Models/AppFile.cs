using System.ComponentModel.DataAnnotations;

namespace cem.Models;

public class AppFile
{
    public int Id { get; set; }

    [Required]
    public required string FileName { get; set; }

    [Required]
    public required string ContentType { get; set; }

    [Required]
    public required string FilePath { get; set; }

    public DateTime UploadDate { get; set; } = DateTime.UtcNow;

    public long FileSize { get; set; }

    public string UploadedById { get; set; } = string.Empty;
    public ApplicationUser? UploadedBy { get; set; }

    public string EntityType { get; set; } = string.Empty; 
    public int EntityId { get; set; }   

}   