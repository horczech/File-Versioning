using FluentValidation;

namespace FileVersioning.Server.Application.Validators;

public class FolderAnalyzerValidator : AbstractValidator<FolderAnalyzer>
{
    public FolderAnalyzerValidator() {
        RuleFor(x => x.FolderPath)
            .NotEmpty()
            .WithMessage("The path to the input folder must be filled.");
    }
}