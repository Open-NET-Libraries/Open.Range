using System;
using static Open.Utility;
namespace Open;

public static partial class RangeExtensions
{
	/// <summary>
	/// If the value is less than the range a new range is created with the value as the low.<br/>
	/// If the value is greater than the range a new range is created with the value as the high.<br/>
	/// Otherwise if the value is within the range (inclusive) then the provided range is returned.
	/// </summary>
	/// <param name="range">The range to compare the value against.</param>
	/// <param name="value">The value to verify if it exceeds the range.</param>
	public static Range<T> Expand<T>(
		this Range<T> range, T value)
		where T : IComparable<T>
	{
		var (rLow, rHigh) = range;
		T low = rLow, high = rHigh;

		var diff = low.CompareTo(high);
		var bV = value as IBoundary;
		var cLow = bV is null
			? value.CompareTo(low)
			: bV.CompareLowTo(low);

		if (diff == 0 && cLow == 0)
			return range;

		var cHigh = bV is null
			? value.CompareTo(high)
			: bV.CompareHighTo(high);

		var lowChange = cLow < 0;
		var highChange = cHigh > 0;
		return lowChange || highChange
			? (new(lowChange ? value : rLow, highChange ? value : rHigh))
			: range;
	}

	/// <inheritdoc cref="Expand{T}(Open.Range{T}, T)" />
	public static RangeWithValue<T, TValue> Expand<T, TValue>(
		this RangeWithValue<T, TValue> range, T value)
		where T : IComparable<T>
	{
		var r = new Range<T>(range);
		var e = Expand(r, value);
		if (e == r) return range;
		return new RangeWithValue<T, TValue>(e, range.Value);
	}

	/// <param name="inclusive">Specifies if the value is inclusive.</param>
	/// <inheritdoc cref="Expand{T}(Open.Range{T}, T)" />
	public static Range<Boundary<T>> Expand<T>(
		this Range<Boundary<T>> range, T value, bool inclusive)
		where T : IComparable<T>
		=> IsNaN(value) ? range : range.Expand(new Boundary<T>(value, inclusive));

	/// <summary>
	/// Creates a range that encompasses both ranges.
	/// </summary>
	public static Range<T> Combine<T>(
		this Range<T> range, Range<T> expansion)
		where T : IComparable<T>
		=> range.Expand(expansion.Low).Expand(expansion.High);

	/// <inheritdoc cref="Combine{T}(Open.Range{T}, Open.Range{T})"/>
	public static Range<Boundary<T>> Combine<T>(
		this Range<Boundary<T>> range, Range<Boundary<T>> expansion)
		where T : IComparable<T>
		=> range.Expand(expansion.Low).Expand(expansion.High);

	/// <summary>
	/// Attempts to derive the intersection of the
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="range"></param>
	/// <param name="other"></param>
	/// <param name="result"></param>
	/// <returns>true if the ranges intersect; otherwise false</returns>
	public static bool TryIntersect<T>(
		this Range<T> range, Range<T> other, out Range<T> result)
		where T : IComparable<T>
	{
		result = default;
		var bHigh = range.High as IBoundary;
		var cHiLo = bHigh is null
			? range.High.CompareTo(other.Low)
			: bHigh.CompareHighTo(other.Low);

		// The other low is higher than this high?
		if (cHiLo < 0) return false;
		// The edges actually touch.
		if (cHiLo == 0)
		{
			result = new(range.Low, other.High);
			return true;
		}
		var bLow = range.Low as IBoundary;
		var cLoHi = bLow is null
			? range.Low.CompareTo(other.High)
			: bLow.CompareLowTo(other.High);

		// The other high is lower than this low?
		if (cLoHi > 0) return false;
		// The edges actually touch.
		if (cLoHi == 0)
		{
			result = new(other.Low, range.High);
			return true;
		}

		var cLow = bLow is null
			? range.Low.CompareTo(other.Low)
			: bLow.CompareLowTo(other.Low);
		var cHigh = bHigh is null
			? range.High.CompareTo(other.High)
			: bHigh.CompareHighTo(other.High);

		// is this within or exactly equal to the other range?
		if (cLow >= 0 && cHigh <= 0)
		{
			result = range;
			return true;
		}

		// Is the other range within the bounds?
		if (cLow < 0 && cHigh > 0)
		{
			result = other;
			return true;
		}

		// All other possibilities.
		result = new(cLow > 0 ? range.Low : other.Low, cHigh < 0 ? range.High : other.High);
		return true;
	}
}
