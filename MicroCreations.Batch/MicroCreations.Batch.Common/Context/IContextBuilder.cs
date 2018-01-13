using System.Threading.Tasks;

namespace MicroCreations.Batch.Common.Context
{
    public interface IContextBuilder
    {
        Task<IContext> GetContext();
    }
}
