namespace Open;

/// <summary>
/// Exposes the "CanRangeWith" method.
/// </summary>
public interface ICanRange
{
	/// <returns>
	/// Returns true if this can be ranged with <paramref name="other"/>; otherwise false.
	/// </returns>
	bool CanRangeWith(object other);
}

/// <inheritdoc />
public interface ICanRange<in T> : ICanRange
{
	/// <inheritdoc />
	bool CanRangeWith(T other);
}
