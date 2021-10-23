using System;

namespace Open;

public static partial class BoundaryExtensions
{
	/// <summary>
	/// Returns a range from this boundary (<paramref name="low"/>) to the provided (included) <paramref name="high"/> value.
	/// </summary>
	public static Range<Boundary<T>> To<T>(this Boundary<T> low, T high)
		where T : IComparable<T>
		=> new(low, new Boundary<T>(high, true));

	/// <summary>
	/// Returns a range from this boundary (<paramref name="low"/>) to the provided (excluded) <paramref name="high"/> value.
	/// </summary>
	public static Range<Boundary<T>> Below<T>(this Boundary<T> low, T high)
		where T : IComparable<T>
		=> new(low, new Boundary<T>(high, false));
}
