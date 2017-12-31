namespace MicroCreations.Batch.Domain.Interfaces
{
    public interface IRequestBuilderFactory<TSelector>
    {
        IRequestBuilder<TSelector> Get(TSelector selector);
    }
}
