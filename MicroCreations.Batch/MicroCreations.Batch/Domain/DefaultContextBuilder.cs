using System.Threading.Tasks;
using MicroCreations.Batch.Domain.Interfaces;

namespace MicroCreations.Batch.Domain
{
    public class DefaultContextBuilder : IContextBuilder
    {
        public async Task<IContext> GetContext()
        {
            await Task.Factory.StartNew(() => { });

            return null;
        }
    }
}
