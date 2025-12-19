using System;

namespace Neptuo.Activators
{
    /// <summary>
    /// A singleton activator with support for creating singleton from function (on first call).
    /// </summary>
    /// <typeparam name="T">Type of service to create.</typeparam>
    public class InstanceFactory<T> : IFactory<T>
    {
        private T instance;
        private readonly Func<T> instanceGetter;

        /// <summary>
        /// Creates a new instance from already existing instance.
        /// </summary>
        /// <param name="instance">A singleton object.</param>
        public InstanceFactory(T instance)
        {
            Ensure.NotNull(instance, "instance");
            this.instance = instance;
        }

        /// <summary>
        /// Creates a new instance from <paramref name="instanceGetter"/>.
        /// </summary>
        /// <param name="instanceGetter">A function to access singleton object.</param>
        public InstanceFactory(Func<T> instanceGetter)
        {
            Ensure.NotNull(instanceGetter, "instanceGetter");
            this.instanceGetter = instanceGetter;
        }

        public T Create()
        {
            if (instance == null)
            {
                lock (instanceGetter)
                {
                    if (instance == null)
                        instance = instanceGetter();
                }
            }

            return instance;
        }
    }
}
