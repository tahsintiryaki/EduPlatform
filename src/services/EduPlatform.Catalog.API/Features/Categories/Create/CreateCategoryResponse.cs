using System.Net;
using EduPlatform.Catalog.API.Repositories;
using EduPlatform.Shared;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduPlatform.Catalog.API.Features.Categories.Create;

public record CreateCategoryResponse(Guid Id);