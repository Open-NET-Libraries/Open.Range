using Open.Arithmetic.Dynamic;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

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

		using var e = items.GetEnumerator();
		if (!e.MoveNext()) return new(defaultIfEmpty, defaultIfEmpty);

		var min = selector(e.Current);
		var max = min;

		while (e.MoveNext())
		{
			var c = selector(e.Current);
			if (c.CompareTo(min) < 0) min = c;
			if (c.CompareTo(max) > 0) max = c;
		}

		return new Range<TSelect>(min, max);
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

		using var e = items.GetEnumerator();
		if (!e.MoveNext()) throw new InvalidOperationException(UnableToMeasureRangeOfEmpty);

		var min = selector(e.Current);
		var max = min;

		while (e.MoveNext())
		{
			var c = selector(e.Current);
			if (c.CompareTo(min) < 0) min = c;
			if (c.CompareTo(max) > 0) max = c;
		}

		return new Range<TSelect>(min, max);
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

		var min = e.Current;
		var max = e.Current;

		while (e.MoveNext())
		{
			var c = e.Current;
			if (c.CompareTo(min) < 0) min = c;
			if (c.CompareTo(max) > 0) max = c;
		}

		return new Range<T>(min, max);
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

		var min = e.Current;
		var max = e.Current;

		while (e.MoveNext())
		{
			var c = e.Current;
			if (c.CompareTo(min) < 0) min = c;
			if (c.CompareTo(max) > 0) max = c;
		}

		return new Range<T>(min, max);
	}

	/// <remarks>NaN values are ignored.</remarks>
	/// <inheritdoc cref="Range{T}(IEnumerable{T}, T)"/>
	public static Range<float> Range(this IEnumerable<float> values)
	{
		if (values is null)
			throw new ArgumentNullException(nameof(values));
		Contract.EndContractBlock();

		var max = float.NegativeInfinity;
		var min = float.PositiveInfinity;

		foreach (var value in values)
		{
			if (float.IsNaN(value)) continue;
			if (value < min)
				min = value;
			if (value > max)
				max = value;
		}

		return max < min ?
			new Range<float>(float.NaN, float.NaN) :
			new Range<float>(min, max);
	}

	/// <inheritdoc cref="Range(IEnumerable{float})"/>
	public static Range<double> Range(this IEnumerable<double> values)
	{
		if (values is null)
			throw new ArgumentNullException(nameof(values));
		Contract.EndContractBlock();

		var max = double.NegativeInfinity;
		var min = double.PositiveInfinity;

		foreach (var value in values)
		{
			if (double.IsNaN(value)) continue;
			if (value < min)
				min = value;
			if (value > max)
				max = value;
		}

		return max < min ?
			new Range<double>(double.NaN, double.NaN) :
			new Range<double>(min, max);
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
				if (value < min)
					min = value;
			lock (templockMax)
				if (value > max)
					max = value;
		});

		return max < min ?
			new Range<double>(double.NaN, double.NaN) :
			new Range<double>(min, max);
	}

	#region IRange<float> Arithmetic
	public static IRange<float> SumWith(this IRange<float> r1, IRange<float> r2)
	{
		if (r1 is null)
			throw new ArgumentNullException(nameof(r1));
		if (r2 is null)
			throw new ArgumentNullException(nameof(r2));
		Contract.EndContractBlock();

		return new Range<float>(r1.Low + r2.Low, r1.High + r2.High);
	}

	public static IRange<float> Subtract(this IRange<float> r1, IRange<float> r2)
	{
		if (r1 is null)
			throw new ArgumentNullException(nameof(r1));
		if (r2 is null)
			throw new ArgumentNullException(nameof(r2));
		Contract.EndContractBlock();

		return new Range<float>(r1.Low - r2.Low, r1.High - r2.High);
	}

	public static IRange<float> MultiplyBy(this IRange<float> r1, IRange<float> r2)
	{
		if (r1 is null)
			throw new ArgumentNullException(nameof(r1));
		if (r2 is null)
			throw new ArgumentNullException(nameof(r2));
		Contract.EndContractBlock();

		return new Range<float>(r1.Low * r2.Low, r1.High * r2.High);
	}

	public static IRange<float> DivideBy(this IRange<float> r1, IRange<float> r2)
	{
		if (r1 is null)
			throw new ArgumentNullException(nameof(r1));
		if (r2 is null)
			throw new ArgumentNullException(nameof(r2));
		Contract.EndContractBlock();

		return new Range<float>(r1.Low / r2.Low, r1.High / r2.High);
	}
	#endregion

	#region IRange<double> Arithmetic
	public static IRange<double> SumWith(this IRange<double> r1, IRange<double> r2)
	{
		if (r1 is null)
			throw new ArgumentNullException(nameof(r1));
		if (r2 is null)
			throw new ArgumentNullException(nameof(r2));
		Contract.EndContractBlock();

		return new Range<double>(r1.Low + r2.Low, r1.High + r2.High);
	}

	public static IRange<double> Subtract(this IRange<double> r1, IRange<double> r2)
	{
		if (r1 is null)
			throw new ArgumentNullException(nameof(r1));
		if (r2 is null)
			throw new ArgumentNullException(nameof(r2));
		Contract.EndContractBlock();

		return new Range<double>(r1.Low - r2.Low, r1.High - r2.High);
	}

	public static IRange<double> MultiplyBy(this IRange<double> r1, IRange<double> r2)
	{
		if (r1 is null)
			throw new ArgumentNullException(nameof(r1));
		if (r2 is null)
			throw new ArgumentNullException(nameof(r2));
		Contract.EndContractBlock();

		return new Range<double>(r1.Low * r2.Low, r1.High * r2.High);
	}

	public static IRange<double> DivideBy(this IRange<double> r1, IRange<float> r2)
	{
		if (r1 is null)
			throw new ArgumentNullException(nameof(r1));
		if (r2 is null)
			throw new ArgumentNullException(nameof(r2));
		Contract.EndContractBlock();

		return new Range<double>(r1.Low / r2.Low, r1.High / r2.High);
	}
	#endregion

	#region IRange<int> Arithmetic
	public static IRange<int> SumWith(this IRange<int> r1, IRange<int> r2)
	{
		if (r1 is null)
			throw new ArgumentNullException(nameof(r1));
		if (r2 is null)
			throw new ArgumentNullException(nameof(r2));
		Contract.EndContractBlock();

		return new Range<int>(r1.Low + r2.Low, r1.High + r2.High);
	}

	public static IRange<int> Subtract(this IRange<int> r1, IRange<int> r2)
	{
		if (r1 is null)
			throw new ArgumentNullException(nameof(r1));
		if (r2 is null)
			throw new ArgumentNullException(nameof(r2));
		Contract.EndContractBlock();

		return new Range<int>(r1.Low - r2.Low, r1.High - r2.High);
	}

	public static IRange<int> MultiplyBy(this IRange<int> r1, IRange<int> r2)
	{
		if (r1 is null)
			throw new ArgumentNullException(nameof(r1));
		if (r2 is null)
			throw new ArgumentNullException(nameof(r2));
		Contract.EndContractBlock();

		return new Range<int>(r1.Low * r2.Low, r1.High * r2.High);
	}

	public static IRange<int> DivideBy(this IRange<int> r1, IRange<int> r2)
	{
		if (r1 is null)
			throw new ArgumentNullException(nameof(r1));
		if (r2 is null)
			throw new ArgumentNullException(nameof(r2));
		Contract.EndContractBlock();

		return new Range<int>(r1.Low / r2.Low, r1.High / r2.High);
	}
	#endregion

	#region IRange<TimeSpan> Arithmetic
	public static IRange<TimeSpan> SumWith(this IRange<TimeSpan> r1, IRange<TimeSpan> r2)
	{
		if (r1 is null)
			throw new ArgumentNullException(nameof(r1));
		if (r2 is null)
			throw new ArgumentNullException(nameof(r2));
		Contract.EndContractBlock();

		return new Range<TimeSpan>(r1.Low + r2.Low, r1.High + r2.High);
	}

	public static IRange<TimeSpan> Subtract(this IRange<TimeSpan> r1, IRange<TimeSpan> r2)
	{
		if (r1 is null)
			throw new ArgumentNullException(nameof(r1));
		if (r2 is null)
			throw new ArgumentNullException(nameof(r2));
		Contract.EndContractBlock();

		return new Range<TimeSpan>(r1.Low - r2.Low, r1.High - r2.High);
	}
	#endregion

	#region IRange<IComparable> Arithmetic
	public static IRange<T> SumWith<T>(this IRange<T> r1, IRange<T> r2)
		where T : struct, IComparable, IComparable<T>
	{
		if (r1 is null)
			throw new ArgumentNullException(nameof(r1));
		if (r2 is null)
			throw new ArgumentNullException(nameof(r2));
		Contract.EndContractBlock();

		return new Range<T>(r1.Low.AddValue(r2.Low), r1.High.AddValue(r2.High));
	}

	public static IRange<T> Subtract<T>(this IRange<T> r1, IRange<T> r2)
		where T : struct, IComparable, IComparable<T>
	{
		if (r1 is null)
			throw new ArgumentNullException(nameof(r1));
		if (r2 is null)
			throw new ArgumentNullException(nameof(r2));
		Contract.EndContractBlock();

		return new Range<T>(r1.Low.SubtractValue(r2.Low), r1.High.SubtractValue(r2.High));
	}

	public static IRange<T> MultiplyBy<T>(this IRange<T> r1, IRange<T> r2)
		where T : struct, IComparable, IComparable<T>
	{
		if (r1 is null)
			throw new ArgumentNullException(nameof(r1));
		if (r2 is null)
			throw new ArgumentNullException(nameof(r2));
		Contract.EndContractBlock();

		return new Range<T>(r1.Low.MultiplyBy(r2.Low), r1.High.MultiplyBy(r2.High));
	}

	public static IRange<T> DivideBy<T>(this IRange<T> r1, IRange<T> r2)
		where T : struct, IComparable, IComparable<T>
	{
		if (r1 is null)
			throw new ArgumentNullException(nameof(r1));
		if (r2 is null)
			throw new ArgumentNullException(nameof(r2));
		Contract.EndContractBlock();

		return new Range<T>(r1.Low.DivideBy(r2.Low), r1.High.DivideBy(r2.High));
	}
	#endregion


	/// <summary>
	/// Determines the difference in values (subtracting) from high to low.
	/// </summary>
	/// <param name="target"></param>
	/// <exception cref="ArgumentNullException"></exception>
	public static T Delta<T>(this IRange<T> target)
		where T : struct, IComparable<T>, IComparable
	{
		if (target is null)
			throw new ArgumentNullException(nameof(target));
		Contract.EndContractBlock();

		return target.High.SubtractValue(target.Low);
	}

	public static int Delta(this IRange<int> target)
	{
		if (target is null)
			throw new ArgumentNullException(nameof(target));
		Contract.EndContractBlock();

		return target.High - target.Low;
	}

	public static long Delta(this IRange<long> target)
	{
		if (target is null)
			throw new ArgumentNullException(nameof(target));
		Contract.EndContractBlock();

		return target.High - target.Low;
	}

	public static float Delta(this IRange<float> target)
	{
		if (target is null)
			throw new ArgumentNullException(nameof(target));
		Contract.EndContractBlock();

		return target.High - target.Low;
	}

	public static double Delta(this IRange<double> target)
	{
		if (target is null)
			throw new ArgumentNullException(nameof(target));
		Contract.EndContractBlock();

		return target.High - target.Low;
	}

	public static TimeSpan Delta(this IRange<TimeSpan> target)
	{
		if (target is null)
			throw new ArgumentNullException(nameof(target));
		Contract.EndContractBlock();

		return target.High - target.Low;
	}

	public static TimeSpan Delta(this IRange<DateTime> target)
	{
		if (target is null)
			throw new ArgumentNullException(nameof(target));
		Contract.EndContractBlock();

		return TimeSpan.FromTicks(target.High.Ticks - target.Low.Ticks);
	}

	public static IEnumerable<DateTime> Dates(this IRange<DateTime> target, bool inclusive = false)
	{
		if (target is null)
			throw new ArgumentNullException(nameof(target));
		Contract.EndContractBlock();

		var startDate = target.Low;
		var endDate = target.High;

		for (var d = startDate; d < endDate; d = d.AddDays(1).Date)
			yield return d.Date;

		if (inclusive && startDate != endDate)
			yield return endDate;
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