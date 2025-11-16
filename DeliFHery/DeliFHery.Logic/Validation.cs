namespace DeliFHery.Logic;

public interface IEntityValidator<in T>
{
    void ValidateAndThrow(T entity);
}

public static class ValidatorExtensions
{
    public static void EnsureValid<T>(this IEntityValidator<T> validator, T entity)
    {
        validator.ValidateAndThrow(entity);
    }
}
