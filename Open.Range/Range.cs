using System;
using System.Collections.Generic;
using System.Globalization;

namespace Open;

public interface IDateTimeIndexed
{
	DateTime DateTime { get; }
}

public interface IRange<out T>
{
	/// <summary>
	/// The low (minimum) value.
	/// </summary>
	T Low { get; }

	/// <summary>
	/// The high (maximum) value.
	/// </summary>
	T High { get; }
}

public interface IRangeWithValue<out T, out TValue> : IRange<T>
{
	TValue Value { get; }
}

public interface IRangeTimeIndexed<out T> : IDateTimeIndexed, IRange<T> { }

public readonly struct Range<T> : IRange<T>, IEquatable<Range<T>>
{
	public Range(
		T low,
		T high)
	{
		Low = low;
		High = high;
	}

	public Range(T equal)
		: this(equal, equal) { }

	#region IRange<T> 
	/// <inheritdoc />
	public T Low { get; }

	/// <inheritdoc />
	public T High { get; }
	#endregion

	/// <inheritdoc />
	public override string ToString()
		=> $"[{Low} - {High}]";

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

public readonly struct RangeWithValue<T, TValue> : IRange<T>, IEquatable<RangeWithValue<T, TValue>>
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

public readonly struct RangeTimeIndexed<T> : IRangeTimeIndexed<T>, IEquatable<RangeTimeIndexed<T>>
{
	public RangeTimeIndexed(
		DateTime datetime,
		T low,
		T high)
	{
		DateTime = datetime;
		Low = low;
		High = high;
	}

	public RangeTimeIndexed(DateTime datetime, T equal)
		: this(datetime, equal, equal) { }


	#region IRange<T> 
	/// <inheritdoc />
	public T Low { get; }

	/// <inheritdoc />
	public T High { get; }
	#endregion

	#region IDateTimeIndexed Members
	/// <inheritdoc />
	public DateTime DateTime { get; }
	#endregion

	/// <inheritdoc />
	public override string ToString()
		=> DateTime.ToString(CultureInfo.InvariantCulture) + ':' + Low + '-' + High;

	/// <inheritdoc />
	public bool Equals(RangeTimeIndexed<T> other)
		=> DateTime.Equals(other.DateTime)
		&& EqualityComparer<T>.Default.Equals(Low, other.Low)
		&& EqualityComparer<T>.Default.Equals(High, other.High);

	/// <inheritdoc />
	public override bool Equals(object range)
		=> range is RangeTimeIndexed<T> r && Equals(r);

	/// <inheritdoc />
#if NETSTANDARD2_1_OR_GREATER
	public override int GetHashCode()
		=> HashCode.Combine(DateTime, Low, High);
#else
	public override int GetHashCode()
	{
		int hashCode = 593764356;
		hashCode = hashCode * -1521134295 + DateTime.GetHashCode();
		hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(Low);
		hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(High);
		return hashCode;
	}
#endif

	/// <inheritdoc />
	public static bool operator ==(RangeTimeIndexed<T> left, RangeTimeIndexed<T> right) => left.Equals(right);
	/// <inheritdoc />
	public static bool operator !=(RangeTimeIndexed<T> left, RangeTimeIndexed<T> right) => !left.Equals(right);
}

public class RangeTimeIndexedWithValue<T, TValue> : IRangeWithValue<T, TValue>, IRangeTimeIndexed<T>, IEquatable<RangeTimeIndexedWithValue<T, TValue>>
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

public class RangeTimeIndexedWithValue<T> : RangeTimeIndexedWithValue<T, T>
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