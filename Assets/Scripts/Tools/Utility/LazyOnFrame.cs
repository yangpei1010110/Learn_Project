using System;
using UnityEngine;

namespace Tools.Utility
{
    public class LazyOnFrame<T>
    {
        private int     _frame;
        private T       _value;
        private Func<T> _getValue;

        public T Value
        {
            get
            {
                if (_frame != Time.frameCount)
                {
                    _frame = Time.frameCount;
                    _value = _getValue();
                }

                return _value;
            }
        }

        public static implicit operator T(LazyOnFrame<T> l)
        {
            return l.Value;
        }
        
        public LazyOnFrame(Func<T> getValue)
        {
            _frame = -1;
            _getValue = getValue;
        }
    }
}