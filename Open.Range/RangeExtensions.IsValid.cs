using System;

namespace Open;

public static partial class RangeExtensions
{
	/// <exception cref="ArgumentNullException">If <paramref name="range"/> is null.</exception>
	/// <inheritdoc cref="Open.Range.IsValid{T}(T, T)"/>
	public static bool IsValidRange<T>(this IRange<T> range)
		where T : IComparable<T>
		=> range is null
		? throw new ArgumentNullException(nameof(range))
		: Open.Range.IsValid(range.Low, range.High);

	/// <exception cref="ArgumentNullException">If <paramref name="range"/> is null.</exception>
	/// <inheritdoc cref="Open.Range.AssertIsValid{T}(T, T)"/>
	public static bool AssertIsValidRange<T>(this IRange<T> range)
		where T : IComparable<T>
		=> range is null
		? throw new ArgumentNullException(nameof(range))
		: Open.Range.AssertIsValid(range.Low, range.High);
}
