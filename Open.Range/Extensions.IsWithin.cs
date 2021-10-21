using System;

namespace Open.Range;

public static partial class Extensions
{
	public static bool Contains<T>(
		this IRange<Boundary<T>> range,
		T value)
		where T : IComparable<T>
	{
		if (range is null) throw new ArgumentNullException(nameof(range));
		var low = value.CompareTo(range.Low);
		if (low < 0) return false;
		if (low == 0) return range.Low.Inclusive;
		var high = value.CompareTo(range.High);
		return high < 0 || high == 0 && range.High.Inclusive;
	}

	public static bool Contains(
		this IRange<Boundary<float>> range,
		float value)
	{
		if (range is null) throw new ArgumentNullException(nameof(range));
		float low = range.Low;
		if(value < low) return false;
		if(value == low) return range.Low.Inclusive;
		float high = range.High;
		return value < high || value == high && range.High.Inclusive;
	}

	public static bool Contains(
		this IRange<Boundary<double>> range,
		double value)
	{
		if (range is null) throw new ArgumentNullException(nameof(range));
		double low = range.Low;
		if (value < low) return false;
		if (value == low) return range.Low.Inclusive;
		double high = range.High;
		return value < high || value == high && range.High.Inclusive;
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
		=> value.CompareTo(minimum) >= 0
		&& value.CompareTo(maximum) <= 0;

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

	/// <inheritdoc cref="IsInBounds{T}(T, T, T)" />
	public static bool IsInBounds(
		this float value,
		float minimum,
		float maximum)
		=> minimum < value && value < maximum;

	/// <inheritdoc cref="IsInBounds{T}(T, T, T)" />
	public static bool IsInBounds(
		this double value,
		double minimum,
		double maximum)
		=> minimum < value && value < maximum;


}
