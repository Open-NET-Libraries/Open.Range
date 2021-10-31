using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Open;

[ExcludeFromCodeCoverage]
public readonly record struct RangeTimeIndexed<T>
	: IRangeTimeIndexed<T>
	where T : IComparable<T>
{
	public RangeTimeIndexed(
		DateTime datetime,
		T low,
		T high)
	{
		Range.AssertIsValid(low, high);

		DateTime = datetime;
		Low = low;
		High = high;
	}

	public RangeTimeIndexed(DateTime datetime, T equal)
		: this(datetime, equal, equal) { }

	/// <inheritdoc />
	public void Deconstruct(out DateTime datetime, out T low, out T high)
	{
		datetime = DateTime;
		low = Low;
		high = High;
	}

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
}
