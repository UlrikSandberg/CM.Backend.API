namespace CM.Backend.Queries.Builders
{
    public interface IBuilder<T>
    {
        T Build();
    }
}