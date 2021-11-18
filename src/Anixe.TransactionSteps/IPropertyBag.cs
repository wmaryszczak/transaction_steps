namespace Anixe.TransactionSteps
{
  /// <summary>
  /// A collection that contains items resolvable by type or by name and type.
  /// </summary>
  public interface IPropertyBag
  {
    /// <summary>
    /// Determines whether contains an item of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of item to check.</typeparam>
    /// <returns>true if contains with the specified type; otherwise false.</returns>
    bool Contains<T>();

    /// <summary>
    /// Determines whether contains an item of type <typeparamref name="T"/> and provided name.
    /// </summary>
    /// <param name="name">The name of the item to locate.</param>
    /// <typeparam name="T">The type of item to locate.</typeparam>
    /// <returns>true if contains with the specified type and name; otherwise false.</returns>
    /// <exception cref="System.ArgumentNullException">name is null.</exception>
    bool Contains<T>(string name);

    /// <summary>
    /// Returns an item of type <typeparamref name="T"/> or returns the default value of the type.
    /// </summary>
    /// <typeparam name="T">The type of item to get.</typeparam>
    /// <returns>Item to get or the default value when does not contain.</returns>
    T? Get<T>();

    /// <summary>
    /// Returns an item of type <typeparamref name="T"/> and provided name or returns the default value of the type.
    /// </summary>
    /// <param name="name">The name of the item to get.</param>
    /// <typeparam name="T">The type of item to get.</typeparam>
    /// <returns>Item to get or the default value when does not contain.</returns>
    /// <exception cref="System.ArgumentNullException">name is null.</exception>
    T? Get<T>(string name);

    /// <summary>
    /// Returns an item of type <typeparamref name="T"/> or throws <see cref="System.InvalidOperationException"/> when it does not contain.
    /// </summary>
    /// <typeparam name="T">The type of item to get.</typeparam>
    /// <returns>Item to get.</returns>
    /// <exception cref="System.InvalidOperationException">The required item is missing.</exception>
    T GetRequired<T>();

    /// <summary>
    /// Returns an item of type <typeparamref name="T"/> and provided name or throws <see cref="System.InvalidOperationException"/> when it does not contain.
    /// </summary>
    /// <param name="name">The name of the item to get.</param>
    /// <typeparam name="T">The type of item to get.</typeparam>
    /// <returns>Item to get.</returns>
    /// <exception cref="System.ArgumentNullException">name is null.</exception>
    /// <exception cref="System.InvalidOperationException">The required item is missing.</exception>
    T GetRequired<T>(string name);

    /// <summary>
    /// Returns an item of type <typeparamref name="T"/> or returns the default value of the type.
    /// </summary>
    /// <typeparam name="T">The type of item to get.</typeparam>
    /// <param name="value">When this method returns, contains item to get, if the type was found;
    /// otherwise, the default value for the type of the value parameter.</param>
    /// <returns><see langword="true"/> if contains an element with specified type; otherwise, <see langword="false"/>.</returns>
    bool TryGet<T>(out T value);

    /// <summary>
    /// Returns an item of type <typeparamref name="T"/> or returns the default value of the type and name.
    /// </summary>
    /// <typeparam name="T">The type of item to get.</typeparam>
    /// <param name="name">The name of the item to get.</param>
    /// <param name="value">When this method returns, contains item to get, if the type and name was found;
    /// otherwise, the default value for the type and name of the value parameter.</param>
    /// <returns><see langword="true"/> if contains an element with specified type and name; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="System.ArgumentNullException">name is null.</exception>
    bool TryGet<T>(string name, out T value);

    /// <summary>
    /// Sets an item of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of item to set.</typeparam>
    /// <param name="property">The item to set.</param>
    /// <exception cref="System.ArgumentNullException">name is null.</exception>
    void Set<T>(T property);

    /// <summary>
    /// Sets an item of type <typeparamref name="T"/> and provided name.
    /// </summary>
    /// <typeparam name="T">The type of item to set.</typeparam>
    /// <param name="name">The name of the item to set.</param>
    /// <param name="property">The item to set.</param>
    /// <exception cref="System.ArgumentNullException">name is null.</exception>
    void Set<T>(string name, T property);

    /// <summary>
    /// Removes an item of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of item to unset.</typeparam>
    void Unset<T>();

    /// <summary>
    /// Removes an item of type <typeparamref name="T"/> and provided name.
    /// </summary>
    /// <typeparam name="T">The type of item to unset.</typeparam>
    /// <param name="name">The name of the item to unset.</param>
    /// <exception cref="System.ArgumentNullException">name is null.</exception>
    void Unset<T>(string name);

    /// <summary>
    /// Returns a copy of this instance with all items.
    /// </summary>
    /// <returns>A new instance with all copied items.</returns>
    IPropertyBag Clone();

    /// <summary>
    /// Returns a copy of this intsance with all items except provided typed items.
    /// </summary>
    /// <param name="exclude">An array of types to exclude.</param>
    /// <returns>A new instance with all copied items except provided typed items.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="exclude"/> is null.</exception>
    IPropertyBag Clone(System.Type[] exclude);
  }
}
