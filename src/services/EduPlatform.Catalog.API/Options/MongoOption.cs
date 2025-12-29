using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace EduPlatform.Catalog.API.Options;

public class MongoOption
{
    [Required] public string DatabaseName { get; set; } = null!;
    [Required] public string ConnectionString { get; set; } = null!;
}