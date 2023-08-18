using UnityEngine;

namespace Extensions
{
    /// <summary>
    /// float extension methods
    /// </summary>
    public static class FloatExtensions
    {
        public static bool NearEqual(this float a, float b, float epsilon = float.Epsilon)
        {
            return Mathf.Abs(a - b) <= epsilon + float.Epsilon ;
        }

        public static bool NearGreater(this float a, float b, float epsilon = float.Epsilon)
        {
            return a > b && !NearEqual(a, b, epsilon);
        }

        public static bool NearLess(this float a, float b, float epsilon = float.Epsilon)
        {
            return a < b && !NearEqual(a, b, epsilon);
        }

        public static bool NearGreaterOrEqual(this float a, float b, float epsilon = float.Epsilon)
        {
            return a >= b || NearEqual(a, b, epsilon);
        }

        public static bool NearLessOrEqual(this float a, float b, float epsilon = float.Epsilon)
        {
            return a <= b || NearEqual(a, b, epsilon);
        }
    }
}