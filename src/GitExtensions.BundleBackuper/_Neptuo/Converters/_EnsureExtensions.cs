using Neptuo.Exceptions.Helpers;
using System;

namespace Neptuo.Converters
{
    public static class _EnsureExtensions
    {
        public static ArgumentOutOfRangeException NotSupportedConversion(this EnsureExceptionHelper ensure, Type targetType, object sourceValue)
        {
            return ensure.ArgumentOutOfRange(
                "TTarget",
                "Target type '{0}' can't be constructed from a value '{1}'.",
                targetType.FullName,
                sourceValue
            );
        }
    }
}
