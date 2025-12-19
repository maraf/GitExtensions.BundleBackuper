namespace Neptuo.Activators
{
    /// <summary>
    /// An implementation of the <see cref="IFactory{T}"/> for types with parameterless constructor.
    /// </summary>
    public class DefaultFactory<T> : IFactory<T>
        where T : new()
    {
        public T Create()
            => new T();
    }
}
