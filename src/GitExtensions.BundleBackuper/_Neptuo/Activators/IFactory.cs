using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Activators
{
    /// <summary>
    /// An activator for <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">Type of service to create.</typeparam>
    public interface IFactory<out T>
    {
        /// <summary>
        /// Creates service of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>An instance of the type <typeparamref name="T"/>.</returns>
        T Create();
    }

    /// <summary>
    /// An activator for <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">A type of the service to create.</typeparam>
    /// <typeparam name="TContext">A type of the activation parameter.</typeparam>
    public interface IFactory<out T, in TContext>
    {
        /// <summary>
        /// Creates a service of the type <typeparamref name="T"/> with <paramref name="context"/> as activation parameter.
        /// </summary>
        /// <param name="context">An activation context.</param>
        /// <returns>An instance of the type <typeparamref name="T"/>.</returns>
        T Create(TContext context);
    }
}
