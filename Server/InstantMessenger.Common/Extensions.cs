using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace InstantMessenger.Common
{
    public static class Extensions
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static Collection<T> InsertRange<T>(this Collection<T> collection, IEnumerable<T> other)
        {
            foreach (var item in other)
            {
                collection.Insert(0, item);
            }

            return collection;
        }

        public static Collection<T> AddRange<T>(this Collection<T> collection, IEnumerable<T> other)
        {
            foreach (var item in other)
            {
                collection.Add(item);
            }

            return collection;
        }
    }
}
