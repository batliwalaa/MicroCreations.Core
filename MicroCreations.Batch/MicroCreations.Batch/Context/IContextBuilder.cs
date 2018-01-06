using System.Threading.Tasks;

namespace MicroCreations.Batch.Context
{
    public interface IContextBuilder
    {
        Task<IContext> GetContext();
    }
}
