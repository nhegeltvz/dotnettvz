using Data.Data.Common;

namespace Data.Services.Validation.Interfaces
{
    public interface IValidator<T>
    {
        Result Validate(T entity);
    }
}
