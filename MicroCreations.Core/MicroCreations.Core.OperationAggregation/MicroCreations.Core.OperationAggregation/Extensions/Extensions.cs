using MicroCreations.Core.OperationAggregation.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MicroCreations.Core.OperationAggregation.Extensions
{
    public static class Extensions
    {
        public static int ToInt(this OperationArgument instance)
        {
            var result = 0;

            return int.TryParse(instance?.Value.ToString() ?? string.Empty, out result) ? result : 0;
        }

        public static uint ToUInt(this OperationArgument instance)
        {
            var result = 0U;

            return uint.TryParse(instance?.Value.ToString() ?? string.Empty, out result) ? result : 0U;
        }

        public static short ToShort(this OperationArgument instance)
        {
            var result = (short)0;

            return short.TryParse(instance?.Value.ToString() ?? string.Empty, out result) ? result : (short)0;
        }

        public static ushort ToUShort(this OperationArgument instance)
        {
            var result = (ushort)0;

            return ushort.TryParse(instance?.Value.ToString() ?? string.Empty, out result) ? result : (ushort)0;
        }

        public static long ToLong(this OperationArgument instance)
        {
            var result = 0L;

            return long.TryParse(instance?.Value.ToString() ?? string.Empty, out result) ? result : 0L;
        }

        public static ulong ToULong(this OperationArgument instance)
        {
            var result = 0UL;

            return ulong.TryParse(instance?.Value.ToString() ?? string.Empty, out result) ? result : 0UL;
        }

        public static double ToDouble(this OperationArgument instance)
        {
            var result = 0D;

            return double.TryParse(instance?.Value.ToString() ?? string.Empty, out result) ? result : 0D;
        }

        public static float ToFloat(this OperationArgument instance)
        {
            var result = 0F;

            return float.TryParse(instance?.Value.ToString() ?? string.Empty, out result) ? result : 0F;
        }

        public static decimal ToDecimal(this OperationArgument instance)
        {
            var result = 0M;

            return decimal.TryParse(instance?.Value.ToString() ?? string.Empty, out result) ? result : 0M;
        }

        public static byte ToByte(this OperationArgument instance)
        {
            var result = (byte)0;

            return byte.TryParse(instance?.Value.ToString() ?? string.Empty, out result) ? result : (byte)0;
        }

        public static sbyte ToSByte(this OperationArgument instance)
        {
            var result = (sbyte)0;

            return sbyte.TryParse(instance?.Value.ToString() ?? string.Empty, out result) ? result : (sbyte)0;
        }

        public static bool ToBool(this OperationArgument instance)
        {
            var result = false;

            return bool.TryParse(instance?.Value.ToString() ?? string.Empty, out result) ? result : false;
        }

        public static T ToEnum<T>(this OperationArgument instance)
        {
            return Enum.IsDefined(typeof(T), instance?.Value) ? (T)Enum.ToObject(typeof(T), instance?.Value) : default(T);
        }

        public static OperationArgument Get(this IEnumerable<OperationArgument> instance, string key)
        {
            return instance.FirstOrDefault(x => string.Equals(x.Name, key, StringComparison.OrdinalIgnoreCase));
        }

        public static T Convert<T>(this OperationResult instance)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(instance.Value));
        }
    }
}
