using System.Diagnostics.CodeAnalysis;

namespace MicroCreations.Batch.Operations
{
    [ExcludeFromCodeCoverage]
    public class OperationArgument
    {
        public string Name { get; set; }

        public object Value { get; set; }
    }
}
