using System;

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
		if (IsNaN(value)) return range;

		var rLow = range.Low;
		var rHigh = range.High;
		T low = rLow;
		T high = rHigh;
		var diff = low.CompareTo(high);
		var cLow = value.CompareTo(low);
		var cHigh = value.CompareTo(high);
		var lowIsNaN = IsNaN(low);
		if (diff == 0)
		{
			if (lowIsNaN)
				return new Range<T>(value, value);

			if (cLow == 0)
				return range;
		}

		var lowChange = lowIsNaN || cLow < 0;
		var highChange = cHigh > 0 || IsNaN(high);
		return lowChange || highChange
			? (new(lowChange ? value : rLow, highChange ? value : rHigh))
			: range;
	}

	/// <inheritdoc cref="Expand{T}(Open.Range{T}, T)" />
	public static RangeWithValue<T, TValue> Expand<T, TValue>(
		this RangeWithValue<T, TValue> range, T value)
		where T : IComparable<T>
	{
		if (IsNaN(value)) return range;

		var rLow = range.Low;
		var rHigh = range.High;
		T low = rLow;
		T high = rHigh;
		var diff = low.CompareTo(high);
		var cLow = value.CompareTo(low);
		var cHigh = value.CompareTo(high);
		var lowIsNaN = IsNaN(low);
		if (diff == 0)
		{
			if (lowIsNaN)
				return new(value, value, range.Value);

			if (cLow == 0)
				return range;
		}

		var lowChange = lowIsNaN || cLow < 0;
		var highChange = cHigh > 0 || IsNaN(high);
		return lowChange || highChange
			? new(lowChange ? value : rLow, highChange ? value : rHigh, range.Value)
			: range;
	}

	/// <inheritdoc cref="Expand{T}(Open.Range{T}, T)" />
	public static Range<Boundary<T>> Expand<T>(
		this Range<Boundary<T>> range, Boundary<T> value)
		where T : IComparable<T>
	{
		var v = value.Value;
		if (IsNaN(v)) return range;

		var rLow = range.Low;
		var rHigh = range.High;
		T low = rLow;
		T high = rHigh;
		var diff = low.CompareTo(high);
		var cLow = v.CompareTo(low);
		var cHigh = v.CompareTo(high);
		var lowIsNaN = IsNaN(low);
		if (diff == 0)
		{
			if (lowIsNaN)
			{
				return new Range<Boundary<T>>(value, value);
			}

			if (cLow == 0)
			{
				return !value.Inclusive
					|| range.Low.Inclusive && range.High.Inclusive
						? range
						: new Range<Boundary<T>>(value, value);
			}
		}

		var lowChange = lowIsNaN || cLow < 0 || cLow == 0 && !rLow.Inclusive;
		var highChange = cHigh > 0 || cHigh == 0 && !rHigh.Inclusive || IsNaN(high);
		return lowChange || highChange
			? (new(lowChange ? value : rLow, highChange ? value : rHigh))
			: range;
	}

	/// <param name="inclusive">Specifies if the value is inclusive.</param>
	/// <inheritdoc cref="Expand{T}(Open.Range{T}, T)" />
	public static Range<Boundary<T>> Expand<T>(
		this Range<Boundary<T>> range, T value, bool inclusive)
		where T : IComparable<T>
		=> range.Expand(new Boundary<T>(value, inclusive));

	static bool IsNaN<T>(T value)
		=> value is double d && double.IsNaN(d)
		|| value is float f && float.IsNaN(f);

}

