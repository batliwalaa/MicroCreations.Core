using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace MicroCreations.Batch.Context
{
    [ExcludeFromCodeCoverage]
    public class DefaultContextBuilder : IContextBuilder
    {
        public async Task<IContext> GetContext()
        {
            await Task.Factory.StartNew(() => { });

            return null;
        }
    }
}
