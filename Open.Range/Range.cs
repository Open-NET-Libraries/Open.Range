using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using static Open.Utility;

namespace Open;

/// <summary>
/// Represents a low and a high value where the low must be less than or equal to the high.
/// </summary>
public readonly struct Range<T> : IRange<T>, IEquatable<Range<T>>
	where T : IComparable<T>
{
	public Range(
		T low,
		T high)
	{
		Range.AssertIsValid(low, high);
		Low = low;
		High = high;
	}

	[ExcludeFromCodeCoverage]
	public Range(
		IRange<T> range)
		: this((range ?? throw new ArgumentNullException(nameof(range))).Low, range.High) { }

	#region IRange<T> 
	/// <inheritdoc />
	public T Low { get; }

	/// <inheritdoc />
	public T High { get; }
	#endregion

	/// <inheritdoc />
	public void Deconstruct(out T low, out T high)
	{
		low = Low;
		high = High;
	}

	/// <inheritdoc />
	public override string ToString()
		=> $"Range<{typeof(T)}>[{Low} - {High}]";

	/// <inheritdoc />
	public bool Equals(Range<T> other)
		=> EqualityComparer<T>.Default.Equals(Low, other.Low)
		&& EqualityComparer<T>.Default.Equals(High, other.High);

	/// <inheritdoc />
	public override bool Equals(object range)
		=> range is Range<T> r && Equals(r);

#if NETSTANDARD2_1_OR_GREATER
	public override int GetHashCode()
		=> HashCode.Combine(Low, High);
#else
	public override int GetHashCode()
	{
		int hashCode = 593764356;
		hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(Low);
		hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(High);
		return hashCode;
	}
#endif

	public static bool operator ==(Range<T> left, Range<T> right) => left.Equals(right);
	public static bool operator !=(Range<T> left, Range<T> right) => !left.Equals(right);
}


public static class Range
{
	/// <summary>
	/// Defines a range of values <typeparamref name="T"/>.
	/// </summary>
	public static Range<T> Create<T>(T low, T high)
		where T : IComparable<T>
		=> new(low, high);

	/// <summary>
	/// Defines the lowest inclusive value of a range.
	/// </summary>
	/// <param name="low"></param>
	/// <param name="inclusive"></param>
	public static Boundary<T> From<T>(T low)
		where T : IComparable<T>
		=> new(low, true);

	/// <summary>
	/// Defines the lower (non-inclusive) boundary of a range.
	/// </summary>
	/// <param name="low">The excluded value to start from.</param>
	public static Boundary<T> Above<T>(T low)
		where T : IComparable<T>
		=> new(low, false);

	/// <summary>
	/// Defines a non-inclusive range.
	/// </summary>
	/// <param name="low">The excluded minimum.</param>
	/// <param name="high">The excluded maximum</param>
	public static Range<Boundary<T>> Between<T>(T low, T high)
		where T : IComparable<T>
		=> new(new(low, false), new(high, false));

	/// <summary>
	/// Defines an inclusive range.
	/// </summary>
	/// <param name="low">The inclusive minimum.</param>
	/// <param name="high">The inclusive maximum</param>
	public static Range<Boundary<T>> Include<T>(T low, T high)
		where T : IComparable<T>
		=> new(new(low, true), new(high, true));

	/// <summary>
	/// Defines a <typeparamref name="T"/> range with included <typeparamref name="TValue"/>.
	/// </summary>
	public static RangeWithValue<T, TValue> WithValue<T, TValue>(T low, T high, TValue value)
		where T : IComparable<T>
		=> new(low, high, value);

	/// <summary>
	/// Returns true if the low is less than or equal to the high.
	/// </summary>
	public static bool IsValid<T>(T low, T high)
		where T : IComparable<T>
	{
		if (CanBeNaN<T>() && (IsNaN(low) || IsNaN(high)))
			return false;

		if (low is ICanRange l && !l.CanRangeWith(high))
			return false;

		try
		{
			return low.CompareTo(high) <= 0;
		}
		// For comparables that don't implement ICanRange.
		catch(ArgumentException)
		{
			return false;
		}
	}

	/// <summary>
	/// Throws an ArgumentException if the low is not less than nor equal to the high.
	/// </summary>
	/// <returns>True. (Allowing for boolean chains.)</returns>
	/// <exception cref="ArgumentException">The low is not less than nor equal to the high.</exception>
	public static bool AssertIsValid<T>(T low, T high)
		where T : IComparable<T>
		=> IsValid(low, high) ? true
		: throw new ArgumentException($"Range is not valid. Low: {low}, High: {high}");
}
