namespace EduPlatform.Catalog.API.Features.Courses.Create;

public class CreateCourseCommandValidator:AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Course name cannot be empty");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Course description cannot be empty");
        RuleFor(x => x.Price).NotEmpty().WithMessage("Course price cannot be empty");
        RuleFor(x => x.CategoryId).NotEmpty().WithMessage("Course categoryId cannot be empty");
        
    }
}