using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Open;

[ExcludeFromCodeCoverage]
public record class RangeTimeIndexedWithValue<T, TValue>
	: IRangeWithValue<T, TValue>, IRangeTimeIndexed<T>
	where T : IComparable<T>
{
	public RangeTimeIndexedWithValue(
		DateTime datetime,
		T low,
		T high,
		TValue value)
	{
		Range.AssertIsValid(low, high);

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

	public override string ToString()
		=> $"{DateTime.ToString(CultureInfo.InvariantCulture)}:[{Low} - {High}]({Value})";

	#region IDateTimeIndexed Members
	public DateTime DateTime { get; }
	#endregion

}

[ExcludeFromCodeCoverage]
public record class RangeTimeIndexedWithValue<T>
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