using FluentValidation;
using LibraryManagement.Application.DTO;

namespace LibraryManagement.API.Validators
{
    public class BookAddRequestValidator : AbstractValidator<BookAddRequest>
    {
        public BookAddRequestValidator() 
        {
            RuleFor(b => b.Title)
                .NotEmpty().WithMessage("Title cannot be empty")
                .MinimumLength(3).WithMessage("Title should be minimum 3 characters long and must be less or equal to maximum 100 characters long")
                .MaximumLength(100).WithMessage("Title must be less or equal to maximum 100 characters long");
            
            RuleFor(b=>b.Author)
                .NotEmpty().WithMessage("Author cannot be empty")
                .MinimumLength(3).WithMessage("Author must be 3 characters long")
                .MaximumLength(100).WithMessage("Author must be less or equal to maximum 100 characters long");

            RuleFor(b => b.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0")
                .LessThan(100000).WithMessage("Price must be less than 100000");               
        }
    }
}
