using Data.Enum;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Data.Models;

public class MediaCreateModel
{
    public string Url { get; set; } = string.Empty;
    public MediaType Type { get; set; }
    public Guid PostId { get; set; }
}

public class MediaListModel
{
    public IFormFile? File { get; set; }
    public MediaType Type { get; set; }
}