using FluentValidation.TestHelper;
using WatchStore.BL.Validators;
using WatchStore.Core.Requests;

namespace WatchStore.Tests;

public class ValidatorTests
{
    private readonly CreateWatchRequestValidator _validator;

    public ValidatorTests()
    {
        _validator = new CreateWatchRequestValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Price_Is_Negative()
    {
        var model = new CreateWatchRequest("Rolex", "Model", -100m, 2020, 40);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Should_Have_Error_When_Brand_Is_Empty()
    {
        var model = new CreateWatchRequest("", "Model", 100m, 2020, 40); 

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Brand);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Data_Is_Correct()
    {
        var model = new CreateWatchRequest("Rolex", "Submariner", 15000m, 2023, 41);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}