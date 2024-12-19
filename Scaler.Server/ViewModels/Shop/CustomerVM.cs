using FluentValidation;

namespace Scaler.Server.ViewModels.Shop
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomerVm
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Gender { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<OrderVm>? Orders { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CustomerViewModelValidator : AbstractValidator<CustomerVm>
    {
        /// <inheritdoc />
        public CustomerViewModelValidator()
        {
            RuleFor(register => register.Name).NotEmpty().WithMessage("Customer name cannot be empty");
            RuleFor(register => register.Gender).NotEmpty().WithMessage("Gender cannot be empty");
        }
    }
}
