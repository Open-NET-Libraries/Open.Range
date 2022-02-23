namespace Open;

/// <summary>
/// Exposes the "CanRangeWith" method.
/// </summary>
public interface ICanRange
{
	/// <summary>
	/// Checks to see if <paramref name="other"/> can be ranged with this instance.
	/// </summary>
	/// <returns>
	/// true if this can be ranged with <paramref name="other"/>; otherwise false.
	/// </returns>
	bool CanRangeWith(object other);
}

/// <inheritdoc />
public interface ICanRange<in T> : ICanRange
{
	/// <inheritdoc />
	bool CanRangeWith(T other);
}
