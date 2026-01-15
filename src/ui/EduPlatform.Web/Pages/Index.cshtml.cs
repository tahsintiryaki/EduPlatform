#region

using EduPlatform.Web.PageModels;
using EduPlatform.Web.Services;
using EduPlatform.Web.ViewModel;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace EduPlatform.Web.Pages;

public class IndexModel(CatalogService catalogService, ILogger<IndexModel> logger) : BasePageModel
{
    public List<CourseViewModel>? Courses { get; set; } = [];


    public async Task<IActionResult> OnGet()
    {
        var coursesAsResult = await catalogService.GetAllCoursesAsync();

        if (coursesAsResult.IsFail) return ErrorPage(coursesAsResult);

        Courses = coursesAsResult.Data!;

        return Page();
    }
}