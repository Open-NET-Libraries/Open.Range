using System;
using System.Collections.Generic;
using System.Text;

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
		var low = value.CompareTo(range.Low);
		if (low < 0) return new(value, range.High);
		var high = value.CompareTo(range.High);
		return high > 0 ? new(range.Low, value) : range;
	}

	/// <inheritdoc cref="Expand{T}(Open.Range{T}, T)" />
	public static Range<float> Expand(
		this Range<float> range, float value)
	{
		var low = range.Low;
		return value < low
			? new(value, range.High)
			: range.High < value
			? new(low, value)
			: range;
	}

	/// <inheritdoc cref="Expand{T}(Open.Range{T}, T)" />
	public static Range<double> Expand(
		this Range<double> range, double value)
	{
		var low = range.Low;
		return value < low
			? new(value, range.High)
			: range.High < value
			? new(low, value)
			: range;
	}

	/// <inheritdoc cref="Expand{T}(Open.Range{T}, T)" />
	public static RangeWithValue<T, TValue> Expand<T, TValue>(
		this RangeWithValue<T, TValue> range, T value)
		where T : IComparable<T>
	{
		var low = value.CompareTo(range.Low);
		if (low < 0) return new(value, range.High, range.Value);
		var high = value.CompareTo(range.High);
		return high > 0 ? new(range.Low, value, range.Value) : range;
	}

	/// <inheritdoc cref="Expand{T}(Open.Range{T}, T)" />
	public static RangeWithValue<float, TValue> Expand<TValue>(this RangeWithValue<float, TValue> range, float value)
	{
		var low = range.Low;
		return value < low
			? new(value, range.High, range.Value)
			: range.High < value
			? new(low, value, range.Value)
			: range;
	}

	/// <inheritdoc cref="Expand{T}(Open.Range{T}, T)" />
	public static RangeWithValue<double, TValue> Expand<TValue>(this RangeWithValue<double, TValue> range, double value)
	{
		var low = range.Low;
		return value < low
			? new(value, range.High, range.Value)
			: range.High < value
			? new(low, value, range.Value)
			: range;
	}

	/// <inheritdoc cref="Expand{T}(Open.Range{T}, T)" />
	public static Range<Boundary<T>> Expand<T>(
		this Range<Boundary<T>> range, Boundary<T> value)
		where T : IComparable<T>
	{
		var v = value.Value;
		var rLow = range.Low;
		var rHigh = range.High;
		T low = rLow;
		T high = rHigh;
		var diff = low.CompareTo(high);
		var cLow = v.CompareTo(low);
		var cHigh = v.CompareTo(high);
		if (diff == 0)
		{
			if (cLow == 0)
			{
				return !value.Inclusive
					|| range.Low.Inclusive && range.High.Inclusive
						? range
						: new Range<Boundary<T>>(value, value);
			}
		}
		else if (cLow > 0 && cHigh < 0) return range;

		return cLow < 0	? new(value, rHigh)
			: cLow == 0 ? rLow.Inclusive || !value.Inclusive ? range : new(value, rHigh)
            : cHigh > 0 ? new(rLow, value) : rLow.Inclusive || !value.Inclusive ? range
			: new(rLow, value);
	}

}
