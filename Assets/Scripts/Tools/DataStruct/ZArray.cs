#nullable enable
using System;

namespace Tools.DataStruct
{
    public class ZArray<T>
    {
        public readonly T[] arr;

        public ZArray(int width)
        {
            if (width < 0 || 1024 < width)
            {
                throw new IndexOutOfRangeException("");
            }


            arr = new T[width * width];
        }

        public ref T this[ int x,  int y] => ref arr[ZArrayIndex.X[x] + (ZArrayIndex.X[y] << 1)];
    }
}