using System.Threading.Tasks;

namespace MicroCreations.Batch.Domain.Interfaces
{
    public interface IContextBuilder
    {
        Task<IContext> GetContext();
    }
}
