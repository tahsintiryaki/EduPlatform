using EduPlatform.Catalog.API.Features.Courses.Create;
using EduPlatform.Catalog.API.Features.Courses.Dtos;

namespace EduPlatform.Catalog.API.Features.Courses;

public class CourseMapping:Profile
{
    public CourseMapping()
    {
        CreateMap<Course, CreateCourseCommand>().ReverseMap();
        CreateMap<Course, CourseDto>().ReverseMap();
        CreateMap<Feature, FeatureDto>().ReverseMap();
    }
}