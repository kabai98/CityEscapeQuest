using CityEscapeQuest.WebApi.Controllers;
using FluentValidation;

namespace CityEscapeQuest.WebApi.Validation;

public sealed class ReportLocationRequestValidator : AbstractValidator<ReportLocationRequest>
{
    public ReportLocationRequestValidator()
    {
        RuleFor(request => request.Latitude).InclusiveBetween(-90, 90);
        RuleFor(request => request.Longitude).InclusiveBetween(-180, 180);
        RuleFor(request => request.AccuracyMeters).InclusiveBetween(0.1, 10_000);
        RuleFor(request => request.MeasuredAtUtc).NotEmpty();
    }
}
