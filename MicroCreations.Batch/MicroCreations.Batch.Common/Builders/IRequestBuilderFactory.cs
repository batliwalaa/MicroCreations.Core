namespace MicroCreations.Batch.Builders
{
    public interface IRequestBuilderFactory<TSelector>
    {
        IRequestBuilder<TSelector> GetRequestBuilder(TSelector selector);
    }
}
