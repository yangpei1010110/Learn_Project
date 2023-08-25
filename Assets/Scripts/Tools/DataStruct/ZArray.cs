#nullable enable
using System;

namespace Tools.DataStruct
{
    /// <summary>
    ///     Z阶曲线数组 z-order curve
    /// </summary>
    public class ZArray<T>
    {
        public enum ZArraySize
        {
            Size1   = 1 * 1,
            Size2   = 2 * 2,
            Size4   = 4 * 4,
            Size16  = 16 * 16,
            Size32  = 32 * 32,
            Size64  = 64 * 64,
            Size128 = 128 * 128,
            Size256 = 256 * 256,
        }

        public readonly T[] arr;

        public ZArray(ZArraySize size) => arr = new T[(int)size];

        public ref T this[in int x, in int y]
        {
            get
            {
                if (x < 0 || 255 < x || y < 0 || 255 < y)
                {
                    throw new IndexOutOfRangeException();
                }
                else
                {
                    return ref arr[GetZOrderIndex(x, y)];
                }
            }
        }


        public static int GetZOrderIndex(in int x, in int y) => GetSequence(x) + (GetSequence(y) << 1);

        public static int GetSequence(in int index)
        {
            if (index < 0 || 255 < index)
            {
                throw new IndexOutOfRangeException();
            }
            else
            {
                if (index == 0)
                {
                    return 0;
                }
                else if (index == 1)
                {
                    return 1;
                }
                else
                {
                    int result = 0;
                    for (int i = 0; i < 32; i++)
                    {
                        if (BitOf(index, i))
                        {
                            result |= 1 << (i << 1);
                        }
                    }

                    return result;
                }
            }
        }

        public static bool BitOf(in int i, in int bitPosition) => (i & (1 << bitPosition)) != 0;
    }
}