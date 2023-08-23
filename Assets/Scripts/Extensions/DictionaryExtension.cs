#nullable enable
using System;
using System.Collections.Generic;

namespace Extensions
{
    public static class DictionaryExtension
    {
        /// <summary>
        ///     获得或创建一个新的值
        /// </summary>
        public static TValue GetOrNew<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, Func<TValue> create)
        {
            if (dic.TryGetValue(key, out TValue value))
            {
                return value;
            }
            else
            {
                TValue newValue = create();
                dic.Add(key, newValue);
                return newValue;
            }
        }

        /// <summary>
        ///     获得或创建一个新的值
        /// </summary>
        public static TValue GetOrNew<TKey, TValue, TInput>(this Dictionary<TKey, TValue> dic, TKey key, Func<TInput, TValue> create, TInput input)
        {
            if (dic.TryGetValue(key, out TValue value))
            {
                return value;
            }
            else
            {
                TValue newValue = create(input);
                dic.Add(key, newValue);
                return newValue;
            }
        }
    }
}