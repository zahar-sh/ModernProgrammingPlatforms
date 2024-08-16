namespace Core.Faker
{
    public interface IFaker
    {
        T Create<T>();
    }
}
