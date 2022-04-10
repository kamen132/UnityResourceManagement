
namespace GMResChecker
{
    public static class ResCheckExtendedFloat
    {
        public static bool IsZero(this float delta)
        {
            return delta < eps && delta > -eps;
        }

        public static bool IsEqual(this float lhs, float rhs)
        {
            var delta = lhs - rhs;
            return delta < eps && delta > -eps;
        }

        public const float eps = 0.000001f;
    }
}


