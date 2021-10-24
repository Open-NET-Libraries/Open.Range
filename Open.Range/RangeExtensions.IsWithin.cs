using System;
using static Open.Utility;

namespace Open;

public static partial class RangeExtensions
{
	/// <summary>
	/// Returns true if the value exists within the defined range.
	/// </summary>
	/// <param name="value">The value to verify is in the range.</param>
	/// <returns>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
	public static bool Contains<T>(
		this IRange<Boundary<T>> range,
		T value)
		where T : IComparable<T>
	{
		if (range is null) throw new ArgumentNullException(nameof(range));
		range.AssertIsValidRange();  
		if (IsNaN(value))
			return false;
		var low = value.CompareTo(range.Low);
		if (low < 0) return false;
		if (low == 0) return range.Low.Inclusive;
		var high = value.CompareTo(range.High);
		return high < 0 || high == 0 && range.High.Inclusive;
	}

	/// <summary>
	/// <paramref name="minimum"/> &lt;= <paramref name="value"/> and <paramref name="value"/> &lt;= <paramref name="maximum"/>
	/// </summary>
	/// <param name="value">The value to verify is in the range.</param>
	/// <param name="minimum">The minimum value.</param>
	/// <param name="maximum">The maximum value.</param>
	/// <returns>
	/// True if the <paramref name="value"/> is greater than
	/// the <paramref name="minimum"/> and less than
	/// the <paramref name="maximum"/>, or equal to either;
	/// otherwise false.
	/// </returns>
	public static bool IsInRange<T>(
		this T value,
		T minimum,
		T maximum)
		where T : IComparable<T>
	{
		if (CanBeNaN<T>() && (IsNaN(value) || IsNaN(minimum) || IsNaN(maximum)))
			return false;
		return value.CompareTo(minimum) >= 0
			&& value.CompareTo(maximum) <= 0;
	}

	/// <inheritdoc cref="IsInRange{T}(T, T, T)" />
	public static bool IsInRange(
		this float value,
		float minimum,
		float maximum)
		=> minimum <= value && value <= maximum;

	/// <inheritdoc cref="IsInRange{T}(T, T, T)" />
	public static bool IsInRange(
		this double value,
		double minimum,
		double maximum)
		=> minimum <= value && value <= maximum;

	/// <summary>
	/// <paramref name="minimum"/> &gt; <paramref name="value"/> and <paramref name="value"/> &lt; <paramref name="maximum"/>
	/// </summary>
	/// <param name="value">The value to verify is in the bounds.</param>
	/// <param name="minimum">The minimum value.</param>
	/// <param name="maximum">The maximum value.</param>
	/// <returns>
	/// True if the <paramref name="value"/> is greater than
	/// the <paramref name="minimum"/> and less than
	/// the <paramref name="maximum"/>;
	/// otherwise false.
	/// </returns>
	public static bool IsInBounds<T>(
		this T value,
		T minimum,
		T maximum)
		where T : IComparable<T>
		=> value.CompareTo(minimum) > 0
		&& value.CompareTo(maximum) < 0;



	//public static bool IsWithin()
}
