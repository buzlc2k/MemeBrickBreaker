using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ZLinq;


namespace BullBrukBruker
{
    public static class ZLinqUtils
    {
        public static IEnumerable<T> AsEnumerable<TValueEnumerator, T>(ValueEnumerable<TValueEnumerator, T> valueEnumerable)
            where TValueEnumerator : struct, IValueEnumerator<T>
        {
            using var element = valueEnumerable.Enumerator;
            while (element.TryGetNext(out var current))
                yield return current;
        }
    }
}

