using Asp.Versioning.Builder;
using EduPlatform.Catalog.API.Features.Courses.Create;
using EduPlatform.Catalog.API.Features.Courses.Delete;
using EduPlatform.Catalog.API.Features.Courses.GetAll;
using EduPlatform.Catalog.API.Features.Courses.GetAllByUserId;
using EduPlatform.Catalog.API.Features.Courses.GetById;
using EduPlatform.Catalog.API.Features.Courses.Update;

namespace EduPlatform.Catalog.API.Features.Courses;

public static class CourseEndpointExt
{
    public static void AddCCourseGroupEndpointExt(this WebApplication app,ApiVersionSet apiVersionSet)
    {
        app.MapGroup("api/v1/courses").WithTags("Course")
            .WithApiVersionSet(apiVersionSet)
            .CreateCourseGroupItemEndpoint()
            .GetAllCourseGroupItemEndpoint()
            .GetByIdCourseGroupItemEndpoint()
            .UpdateCourseGroupItemEndpoint()
            .DeleteCourseGroupItemEndpoint()
            .GetByUserIdCourseGroupItemEndpoint();
    }
}