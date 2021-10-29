using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Open;

[ExcludeFromCodeCoverage]
public class RangeTimeIndexedWithValue<T, TValue>
	: IRangeWithValue<T, TValue>, IRangeTimeIndexed<T>, IEquatable<RangeTimeIndexedWithValue<T, TValue>>
	where T : IComparable<T>
{
	public RangeTimeIndexedWithValue(
		DateTime datetime,
		T low,
		T high,
		TValue value)
	{
		DateTime = datetime;
		Low = low;
		High = high;
		Value = value;
	}

	#region IRangeWithValue<T, TValue> 
	/// <inheritdoc />
	public T Low { get; }

	/// <inheritdoc />
	public T High { get; }

	/// <inheritdoc />
	public TValue Value { get; }
	#endregion

	/// <inheritdoc />
	public void Deconstruct(out DateTime datetime, out T low, out T high, out TValue value)
	{
		datetime = DateTime;
		low = Low;
		high = High;
		value = Value;
	}

	/// <inheritdoc />
	public override string ToString()
		=> $"{DateTime.ToString(CultureInfo.InvariantCulture)}:[{Low} - {High}]({Value})";

	#region IDateTimeIndexed Members
	public DateTime DateTime { get; }
	#endregion

	/// <inheritdoc />
	public bool Equals(RangeTimeIndexedWithValue<T, TValue> other)
		=> other != null
		&& DateTime.Equals(other.DateTime)
		&& EqualityComparer<T>.Default.Equals(Low, other.Low)
		&& EqualityComparer<T>.Default.Equals(High, other.High)
		&& EqualityComparer<TValue>.Default.Equals(Value, other.Value);

	/// <inheritdoc />
	public override bool Equals(object range)
		=> range is RangeTimeIndexedWithValue<T, TValue> r && Equals(r);

	/// <inheritdoc />
#if NETSTANDARD2_1_OR_GREATER
	public override int GetHashCode()
		=> HashCode.Combine(DateTime, Low, High, Value);
#else
	public override int GetHashCode()
	{
		int hashCode = 593764356;
		hashCode = hashCode * -1521134295 + DateTime.GetHashCode();
		hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(Low);
		hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(High);
		hashCode = hashCode * -1521134295 + EqualityComparer<TValue>.Default.GetHashCode(Value);
		return hashCode;
	}
#endif

}

[ExcludeFromCodeCoverage]
public class RangeTimeIndexedWithValue<T>
	: RangeTimeIndexedWithValue<T, T>
	where T : IComparable<T>
{
	public RangeTimeIndexedWithValue(
		DateTime datetime,
		T low,
		T high,
		T value)
		: base(datetime, low, high, value)
	{
	}
}