using System;
using System.Collections.Generic;
using System.Globalization;

namespace Open;

public readonly struct RangeTimeIndexed<T> : IRangeTimeIndexed<T>, IEquatable<RangeTimeIndexed<T>>
		where T : IComparable<T>
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
