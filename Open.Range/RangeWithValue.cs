using System;
using System.Collections.Generic;

namespace Open;

public readonly struct RangeWithValue<T, TValue>
	: IRange<T>, IEquatable<RangeWithValue<T, TValue>>
	where T : IComparable<T>
{
	public RangeWithValue(
		T low,
		T high,
		TValue value)
	{
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
	public void Deconstruct(out T low, out T high, TValue value)
	{
		low = Low;
		high = High;
		value = Value;
	}

	/// <inheritdoc />
	public override string ToString()
		=> $"[{Low} - {High}]({Value})";

	/// <inheritdoc />
	public bool Equals(RangeWithValue<T, TValue> other)
		=> EqualityComparer<T>.Default.Equals(Low, other.Low)
		&& EqualityComparer<T>.Default.Equals(High, other.High)
		&& EqualityComparer<TValue>.Default.Equals(Value, other.Value);

	/// <inheritdoc />
	public override bool Equals(object range)
		=> range is RangeWithValue<T, TValue> r && Equals(r);

	/// <inheritdoc />
#if NETSTANDARD2_1_OR_GREATER
	public override int GetHashCode()
		=> HashCode.Combine(Low, High, Value);
#else
	public override int GetHashCode()
	{
		int hashCode = 593764356;
		hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(Low);
		hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(High);
		hashCode = hashCode * -1521134295 + EqualityComparer<TValue>.Default.GetHashCode(Value);
		return hashCode;
	}
#endif

	/// <inheritdoc />
	public static bool operator ==(RangeWithValue<T, TValue> left, RangeWithValue<T, TValue> right) => left.Equals(right);

	/// <inheritdoc />
	public static bool operator !=(RangeWithValue<T, TValue> left, RangeWithValue<T, TValue> right) => !left.Equals(right);

}
