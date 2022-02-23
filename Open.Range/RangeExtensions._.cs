using Open.Arithmetic.Dynamic;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using static Open.Utility;

namespace Open;

public static partial class RangeExtensions
{
	private const string UnableToMeasureRangeOfEmpty = "Unable to acquire a range from an empty set.";

	/// <param name="items">The items to select from.</param>
	/// <param name="selector">A function for selecting values from each item.</param>
	/// <exception cref="ArgumentNullException"><paramref name="items"/> or <paramref name="selector"/> is null.</exception>
	/// <inheritdoc cref="Range{T}(IEnumerable{T}, T)"/>
	public static Range<TSelect> Range<T, TSelect>(
		this IEnumerable<T> items, TSelect defaultIfEmpty, Func<T, TSelect> selector)
		where TSelect : IComparable<TSelect>
	{
		if (items is null)
			throw new ArgumentNullException(nameof(selector));
		if (selector is null)
			throw new ArgumentNullException(nameof(selector));
		Contract.EndContractBlock();

		return items.Select(selector).Range(defaultIfEmpty);
	}

	/// <exception cref="InvalidOperationException">If the set of values is empty.</exception>
	/// <inheritdoc cref="Range{T, TSelect}(IEnumerable{T}, TSelect, Func{T, TSelect})"/>
	public static Range<TSelect> Range<T, TSelect>(
		this IEnumerable<T> items, Func<T, TSelect> selector)
		where TSelect : IComparable<TSelect>
	{
		if (items is null)
			throw new ArgumentNullException(nameof(selector));
		if (selector is null)
			throw new ArgumentNullException(nameof(selector));
		Contract.EndContractBlock();

		return items.Select(selector).Range();
	}

	/// <summary>
	/// Determines the minimum and maximum values of a set.
	/// </summary>
	/// <param name="values">The set of values to measure.</param>
	/// <param name="defaultIfEmpty">A default value to use if the enumerable is empty.</param>
	/// <returns>A range (.Low and .High) representing the minimum and maximum values.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="values"/> is null.</exception>
	public static Range<T> Range<T>(
		this IEnumerable<T> values,
		T defaultIfEmpty)
		where T : IComparable<T>
	{
		if (values is null)
			throw new ArgumentNullException(nameof(values));
		Contract.EndContractBlock();

		using var e = values.GetEnumerator();
		if (!e.MoveNext()) return new(defaultIfEmpty, defaultIfEmpty);

		return CanBeNaN<T>()
			? FromEnumeratorCanBeNaN(e, defaultIfEmpty)
			: FromEnumerator(e);
	}

	/// <exception cref="InvalidOperationException">If the set of values is empty.</exception>
	/// <inheritdoc cref="Range{T}(IEnumerable{T}, T)"/>
	public static Range<T> Range<T>(this IEnumerable<T> values)
		where T : IComparable<T>
	{
		if (values is null)
			throw new ArgumentNullException(nameof(values));
		Contract.EndContractBlock();

		using var e = values.GetEnumerator();
		if (!e.MoveNext()) throw new InvalidOperationException(UnableToMeasureRangeOfEmpty);

		return CanBeNaN<T>()
			? FromEnumeratorCanBeNaN(e)
			: FromEnumerator(e);
	}

	static Range<T> FromEnumerator<T>(IEnumerator<T> e)
		where T : IComparable<T>
	{
		var c = e.Current;
		var min = c;
		var max = c;

		while (e.MoveNext())
		{
			c = e.Current;
			if (c.CompareTo(min) < 0) min = c;
			if (c.CompareTo(max) > 0) max = c;
		}

		return new Range<T>(min, max);
	}

	static Range<T> FromEnumeratorCanBeNaN<T>(IEnumerator<T> e)
		where T : IComparable<T>
	{
		var c = e.Current;
		while (IsNaN(c) && e.MoveNext())
			c = e.Current;

		var min = c;
		var max = c;

		while (e.MoveNext())
		{
			c = e.Current;
			if (IsNaN(c)) continue;
			if (c.CompareTo(min) < 0) min = c;
			if (c.CompareTo(max) > 0) max = c;
		}

		return new Range<T>(min, max);
	}

	static Range<T> FromEnumeratorCanBeNaN<T>(IEnumerator<T> e, T defaultIfNaN)
		where T : IComparable<T>
	{
		var c = e.Current;
		while (IsNaN(c) && e.MoveNext())
			c = e.Current;

		var min = c;
		var max = c;

		while (e.MoveNext())
		{
			if (IsNaN(c)) continue;
			c = e.Current;
			if (c.CompareTo(min) < 0) min = c;
			if (c.CompareTo(max) > 0) max = c;
		}

		if (IsNaN(min)) min = defaultIfNaN;
		if (IsNaN(max)) max = defaultIfNaN;

		return new Range<T>(min, max);
	}

	/// <remarks>Use this method of the time to return from the selector could be lengthy.</remarks>
	/// <inheritdoc cref="Range{T, TSelect}(IEnumerable{T}, Func{T, TSelect})"/>
	public static Range<double> Range<T>(this ParallelQuery<T> items, Func<T, double> selector)
	{
		if (items is null)
			throw new ArgumentNullException(nameof(items));
		if (selector is null)
			throw new ArgumentNullException(nameof(selector));
		Contract.EndContractBlock();

		var max = double.NegativeInfinity;
		var min = double.PositiveInfinity;

		object templockMin = new(), templockMax = new();
		items.ForAll(item =>
		{
			var value = selector(item);
			if (double.IsNaN(value)) return;
			lock (templockMin)
			{
				if (value < min)
					min = value;
			}

			lock (templockMax)
			{
				if (value > max)
					max = value;
			}
		});

		return new Range<double>(min, max);
	}

	#region IRange<float> Arithmetic
	/// <inheritdoc cref="SumWith{T}(Open.Range{T}, Open.Range{T})"/>
	[ExcludeFromCodeCoverage]
	public static Range<float> SumWith(this Range<float> r1, Range<float> r2)
		=> new(r1.Low + r2.Low, r1.High + r2.High);

	/// <inheritdoc cref="Subtract{T}(Open.Range{T}, Open.Range{T})"/>
	[ExcludeFromCodeCoverage]
	public static Range<float> Subtract(this Range<float> r1, Range<float> r2)
		=> new(r1.Low - r2.Low, r1.High - r2.High);

	/// <inheritdoc cref="MultiplyBy{T}(Open.Range{T}, Open.Range{T})"/>
	[ExcludeFromCodeCoverage]
	public static Range<float> MultiplyBy(this Range<float> r1, Range<float> r2)
		=> new(r1.Low * r2.Low, r1.High * r2.High);

	/// <inheritdoc cref="DivideBy{T}(Open.Range{T}, Open.Range{T})"/>
	[ExcludeFromCodeCoverage]
	public static Range<float> DivideBy(this Range<float> r1, Range<float> r2)
		=> new(r1.Low / r2.Low, r1.High / r2.High);
	#endregion

	#region IRange<double> Arithmetic
	/// <inheritdoc cref="SumWith{T}(Open.Range{T}, Open.Range{T})"/>
	[ExcludeFromCodeCoverage]
	public static Range<double> SumWith(this Range<double> r1, Range<double> r2)
		=> new(r1.Low + r2.Low, r1.High + r2.High);

	/// <inheritdoc cref="Subtract{T}(Open.Range{T}, Open.Range{T})"/>
	[ExcludeFromCodeCoverage]
	public static Range<double> Subtract(this Range<double> r1, Range<double> r2)
		=> new(r1.Low - r2.Low, r1.High - r2.High);

	/// <inheritdoc cref="MultiplyBy{T}(Open.Range{T}, Open.Range{T})"/>
	[ExcludeFromCodeCoverage]
	public static Range<double> MultiplyBy(this Range<double> r1, Range<double> r2)
		=> new(r1.Low * r2.Low, r1.High * r2.High);

	/// <inheritdoc cref="DivideBy{T}(Open.Range{T}, Open.Range{T})"/>
	[ExcludeFromCodeCoverage]
	public static Range<double> DivideBy(this Range<double> r1, Range<float> r2)
		=> new(r1.Low / r2.Low, r1.High / r2.High);
	#endregion

	#region IRange<int> Arithmetic
	/// <inheritdoc cref="SumWith{T}(Open.Range{T}, Open.Range{T})"/>
	[ExcludeFromCodeCoverage]
	public static Range<int> SumWith(this Range<int> r1, Range<int> r2)
		=> new(r1.Low + r2.Low, r1.High + r2.High);

	/// <inheritdoc cref="Subtract{T}(Open.Range{T}, Open.Range{T})"/>
	[ExcludeFromCodeCoverage]
	public static Range<int> Subtract(this Range<int> r1, Range<int> r2)
		=> new(r1.Low - r2.Low, r1.High - r2.High);

	/// <inheritdoc cref="MultiplyBy{T}(Open.Range{T}, Open.Range{T})"/>
	[ExcludeFromCodeCoverage]
	public static Range<int> MultiplyBy(this Range<int> r1, Range<int> r2)
		=> new(r1.Low * r2.Low, r1.High * r2.High);

	/// <inheritdoc cref="DivideBy{T}(Open.Range{T}, Open.Range{T})"/>
	[ExcludeFromCodeCoverage]
	public static Range<int> DivideBy(this Range<int> r1, Range<int> r2)
		=> new(r1.Low / r2.Low, r1.High / r2.High);
	#endregion

	#region IRange<TimeSpan> Arithmetic
	/// <inheritdoc cref="SumWith{T}(Open.Range{T}, Open.Range{T})"/>
	[ExcludeFromCodeCoverage]
	public static Range<TimeSpan> SumWith(this Range<TimeSpan> r1, Range<TimeSpan> r2)
		=> new(r1.Low + r2.Low, r1.High + r2.High);

	/// <inheritdoc cref="Subtract{T}(Open.Range{T}, Open.Range{T})"/>
	[ExcludeFromCodeCoverage]
	public static Range<TimeSpan> Subtract(this Range<TimeSpan> r1, Range<TimeSpan> r2)
		=> new(r1.Low - r2.Low, r1.High - r2.High);
	#endregion

	#region IRange<IComparable> Arithmetic
	/// <summary>
	/// Attempts to sum two ranges together.<br/>
	/// </summary>
	[ExcludeFromCodeCoverage]
	public static Range<T> SumWith<T>(this Range<T> r1, Range<T> r2)
		where T : struct, IComparable, IComparable<T>
		=> new(r1.Low.AddValue(r2.Low), r1.High.AddValue(r2.High));

	/// <summary>
	/// Attempts to subtract one range from another.
	/// </summary>
	[ExcludeFromCodeCoverage]
	public static Range<T> Subtract<T>(this Range<T> r1, Range<T> r2)
		where T : struct, IComparable, IComparable<T>
		=> new(r1.Low.SubtractValue(r2.Low), r1.High.SubtractValue(r2.High));

	/// <summary>
	/// Attempts to mutiply two ranges together.
	/// </summary>
	[ExcludeFromCodeCoverage]
	public static Range<T> MultiplyBy<T>(this Range<T> r1, Range<T> r2)
		where T : struct, IComparable, IComparable<T>
		=> new(r1.Low.MultiplyBy(r2.Low), r1.High.MultiplyBy(r2.High));

	/// <summary>
	/// Attempts to divide one range by another.
	/// </summary>
	[ExcludeFromCodeCoverage]
	public static Range<T> DivideBy<T>(this Range<T> r1, Range<T> r2)
		where T : struct, IComparable, IComparable<T>
		=> new(r1.Low.DivideBy(r2.Low), r1.High.DivideBy(r2.High));
	#endregion

	/// <summary>
	/// Determines the difference in values (subtracting) from high to low.
	/// </summary>
	/// <exception cref="ArgumentNullException">If the <paramref name="target"/> is null.</exception>
	[ExcludeFromCodeCoverage]
	public static T Delta<T>(this IRange<T> target)
		where T : struct, IComparable<T>, IComparable
	{
		if (target is null)
			throw new ArgumentNullException(nameof(target));
		Contract.EndContractBlock();

		return target.High.SubtractValue(target.Low);
	}

	/// <inheritdoc cref="Delta{T}(IRange{T})"/>
	[ExcludeFromCodeCoverage]
	public static int Delta(this IRange<int> target)
	{
		if (target is null)
			throw new ArgumentNullException(nameof(target));
		Contract.EndContractBlock();

		return target.High - target.Low;
	}

	/// <inheritdoc cref="Delta{T}(IRange{T})"/>
	[ExcludeFromCodeCoverage]
	public static long Delta(this IRange<long> target)
	{
		if (target is null)
			throw new ArgumentNullException(nameof(target));
		Contract.EndContractBlock();

		return target.High - target.Low;
	}

	/// <inheritdoc cref="Delta{T}(IRange{T})"/>
	[ExcludeFromCodeCoverage]
	public static float Delta(this IRange<float> target)
	{
		if (target is null)
			throw new ArgumentNullException(nameof(target));
		Contract.EndContractBlock();

		return target.High - target.Low;
	}

	/// <inheritdoc cref="Delta{T}(IRange{T})"/>
	[ExcludeFromCodeCoverage]
	public static double Delta(this IRange<double> target)
	{
		if (target is null)
			throw new ArgumentNullException(nameof(target));
		Contract.EndContractBlock();

		return target.High - target.Low;
	}

	/// <inheritdoc cref="Delta{T}(IRange{T})"/>
	[ExcludeFromCodeCoverage]
	public static TimeSpan Delta(this IRange<TimeSpan> target)
	{
		if (target is null)
			throw new ArgumentNullException(nameof(target));
		Contract.EndContractBlock();

		return target.High - target.Low;
	}

	/// <inheritdoc cref="Delta{T}(IRange{T})"/>
	[ExcludeFromCodeCoverage]
	public static TimeSpan Delta(this IRange<DateTime> target)
	{
		if (target is null)
			throw new ArgumentNullException(nameof(target));
		Contract.EndContractBlock();

		return TimeSpan.FromTicks(target.High.Ticks - target.Low.Ticks);
	}

	/// <summary>
	/// Returns all the dates from the low of the range to the high.
	/// </summary>
	/// <param name="target">The range of dates to build from.</param>
	/// <param name="inclusive">If the high value should be included (only matters if the time of day is zero.</param>
	/// <exception cref="ArgumentNullException">If the <paramref name="target"/> is null.</exception>
	public static IEnumerable<DateTime> Dates(this IRange<DateTime> target, bool inclusive = false)
	{
		if (target is null)
			throw new ArgumentNullException(nameof(target));
		Contract.EndContractBlock();

		return DatesCore(target, inclusive);

		static IEnumerable<DateTime> DatesCore(IRange<DateTime> target, bool inclusive)
		{
			var startDate = target.Low;
			var endDate = target.High;

			for (var d = startDate; d < endDate; d = d.AddDays(1).Date)
				yield return d.Date;

			if (inclusive && startDate != endDate)
				yield return endDate;
		}
	}

	public static void UpdateMinMax<T>(this T value, ref T min, ref T max)
		where T : IComparable<T>
	{
		if (value.CompareTo(min) < 0) min = value;
		if (value.CompareTo(max) > 0) max = value;
	}

	public static void UpdateMinMax(this double value, ref double min, ref double max)
	{
		if (double.IsNaN(value)) return;
		if (value < min) min = value;
		if (value > max) max = value;
	}

	public static void UpdateMinMax(this float value, ref float min, ref float max)
	{
		if (float.IsNaN(value)) return;
		if (value < min) min = value;
		if (value > max) max = value;
	}

	public static double Transpose(this double value, double min, double max, double newMin, double newMax)
	{
		// ReSharper disable once CompareOfFloatsByEqualityOperator
		if (min == max)
			return double.NaN;

		var oldDelta = max - min;
		var newDelta = newMax - newMin;
		var ratio = newDelta / oldDelta;

		var position = value - min;
		return newMin + position * ratio;
	}

	public static double Transpose(this double value, Range<double> source, Range<double> target)
		=> value.Transpose(source.Low, source.High, target.Low, target.High);
}