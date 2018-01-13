using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace MicroCreations.Batch.Common
{
    // GetObjectData is marked with SecurityCriticalAttribute which prevents execution in security-transparent code.
    // to work around this condition, ISafeSerilizationData is used, see documentation
    // https://msdn.microsoft.com/en-us/library/system.runtime.serialization.isafeserializationdata(v=vs.110).aspx
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class BatchException : Exception
    {
        [NonSerialized]
        private ExceptionState _state;

        public BatchException(string message)
        {
            _state = new ExceptionState { Message = message };

            SerializeObjectState += (exception, eventArgs) =>
            {
                eventArgs.AddSerializedState(_state);
            };
        }

        public string ErrorMessage => _state.Message;

        [Serializable]
        [ExcludeFromCodeCoverage]
        private struct ExceptionState : ISafeSerializationData
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public string Message { get; set; }

            public void CompleteDeserialization(object deserialized)
            {
                if (deserialized is BatchException exception)
                {
                    exception._state = this;
                }
            }
        }
    }
}
