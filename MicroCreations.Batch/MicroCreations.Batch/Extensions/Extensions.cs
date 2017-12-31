using MicroCreations.Batch.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MicroCreations.Batch.Extensions
{
    public static class Extensions
    {
        public static int ToInt(this OperationArgument instance)
        {
            return int.TryParse(instance?.Value.ToString() ?? string.Empty, out var result) ? result : 0;
        }

        public static uint ToUInt(this OperationArgument instance)
        {
            return uint.TryParse(instance?.Value.ToString() ?? string.Empty, out var result) ? result : 0U;
        }

        public static short ToShort(this OperationArgument instance)
        {
            return short.TryParse(instance?.Value.ToString() ?? string.Empty, out var result) ? result : (short)0;
        }

        public static ushort ToUShort(this OperationArgument instance)
        {
            return ushort.TryParse(instance?.Value.ToString() ?? string.Empty, out var result) ? result : (ushort)0;
        }

        public static long ToLong(this OperationArgument instance)
        {
            return long.TryParse(instance?.Value.ToString() ?? string.Empty, out var result) ? result : 0L;
        }

        public static ulong ToULong(this OperationArgument instance)
        {
            return ulong.TryParse(instance?.Value.ToString() ?? string.Empty, out var result) ? result : 0UL;
        }

        public static double ToDouble(this OperationArgument instance)
        {
            return double.TryParse(instance?.Value.ToString() ?? string.Empty, out var result) ? result : 0D;
        }

        public static float ToFloat(this OperationArgument instance)
        {
            return float.TryParse(instance?.Value.ToString() ?? string.Empty, out var result) ? result : 0F;
        }

        public static decimal ToDecimal(this OperationArgument instance)
        {
            return decimal.TryParse(instance?.Value.ToString() ?? string.Empty, out var result) ? result : 0M;
        }

        public static byte ToByte(this OperationArgument instance)
        {
            return byte.TryParse(instance?.Value.ToString() ?? string.Empty, out var result) ? result : (byte)0;
        }

        public static sbyte ToSByte(this OperationArgument instance)
        {
            return sbyte.TryParse(instance?.Value.ToString() ?? string.Empty, out var result) ? result : (sbyte)0;
        }

        public static bool ToBool(this OperationArgument instance)
        {
            return bool.TryParse(instance?.Value.ToString() ?? string.Empty, out var result) && result;
        }

        public static T ToEnum<T>(this OperationArgument instance)
        {
            return Enum.IsDefined(typeof(T), instance.Value) ? (T)Enum.ToObject(typeof(T), instance.Value) : default(T);
        }

        public static T Convert<T>(this OperationArgument instance)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(instance.Value));
        }

        public static OperationArgument Get(this IEnumerable<OperationArgument> instance, string key)
        {
            return instance.FirstOrDefault(x => string.Equals(x.Name, key, StringComparison.OrdinalIgnoreCase));
        }

        public static T Convert<T>(this OperationResult instance)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(instance.Value));
        }

        public static OperationResult Get(this IEnumerable<OperationResult> instance, string supportedOperationName)
        {
            return instance.FirstOrDefault(x => x.OperationName == supportedOperationName);
        }
    }
}
