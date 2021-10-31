using System;
using System.Diagnostics.CodeAnalysis;

namespace Open;

[ExcludeFromCodeCoverage]
public readonly record struct RangeWithValue<T, TValue>
	: IRange<T>, IEquatable<RangeWithValue<T, TValue>>
	where T : IComparable<T>
{
	public RangeWithValue(
		T low,
		T high,
		TValue value)
	{
		Range.AssertIsValid(low, high);

		Low = low;
		High = high;
		Value = value;
	}

	public RangeWithValue(IRangeWithValue<T, TValue> source)
	{
		if (source is null) throw new ArgumentNullException(nameof(source));
		Low = source.Low;
		High = source.High;
		Value = source.Value;
	}

	public RangeWithValue(IRange<T> range, TValue value)
	{
		if (range is null) throw new ArgumentNullException(nameof(range));
		Low = range.Low;
		High = range.High;
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
	public void Deconstruct(out T low, out T high, out TValue value)
	{
		low = Low;
		high = High;
		value = Value;
	}

	public override string ToString()
		=> $"[{Low} - {High}]({Value})";
}
