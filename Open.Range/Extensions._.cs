using Open.Arithmetic.Dynamic;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Open.Range;

public static partial class Extensions
{
	/// <summary>
	/// Returns a range from this boundary (<paramref name="low"/>) to the provided (included) <paramref name="high"/> value.
	/// </summary>
	public static Range<Boundary<T>> To<T>(this Boundary<T> low, T high) => new(low, new Boundary<T>(high, true));

	/// <summary>
	/// Returns a range from this boundary (<paramref name="low"/>) to the provided (excluded) <paramref name="high"/> value.
	/// </summary>
	public static Range<Boundary<T>> Below<T>(this Boundary<T> low, T high) => new(low, new Boundary<T>(high, false));

	public static Range<DateTime> Range<T>(this IEnumerable<T> items, Func<T, DateTime> selector)
	{
		if (items is null)
			throw new NullReferenceException();
		if (selector is null)
			throw new ArgumentNullException(nameof(selector));
		Contract.EndContractBlock();

		var max = DateTime.MinValue;
		var min = DateTime.MaxValue;

		var hasItems = false;

		foreach (var item in items)
		{
			hasItems = true;
			var value = selector(item);
			if (value < min)
				min = value;
			if (value > max)
				max = value;
		}

		return hasItems
			? new Range<DateTime>(min, max)
			: throw new InvalidOperationException("You cannot acquire a date range from an empty set.");
	}

	public static Range<double> Range(this IEnumerable<double> values)
	{
		if (values is null)
			throw new NullReferenceException();
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

	public static Range<double> Range<T>(this IEnumerable<T> items, Func<T, double> selector)
	{
		if (items is null)
			throw new NullReferenceException();
		if (selector is null)
			throw new ArgumentNullException(nameof(selector));
		Contract.EndContractBlock();

		var max = double.NegativeInfinity;
		var min = double.PositiveInfinity;

		foreach (var item in items)
		{
			var value = selector(item);
			if (value < min)
				min = value;
			if (value > max)
				max = value;
		}

		return max < min ?
			new Range<double>(double.NaN, double.NaN) :
			new Range<double>(min, max);
	}

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
		where T : struct, IComparable
	{
		if (r1 is null)
			throw new ArgumentNullException(nameof(r1));
		if (r2 is null)
			throw new ArgumentNullException(nameof(r2));
		Contract.EndContractBlock();

		return new Range<T>(r1.Low.AddValue(r2.Low), r1.High.AddValue(r2.High));
	}

	public static IRange<T> Subtract<T>(this IRange<T> r1, IRange<T> r2)
		where T : struct, IComparable
	{
		if (r1 is null)
			throw new ArgumentNullException(nameof(r1));
		if (r2 is null)
			throw new ArgumentNullException(nameof(r2));
		Contract.EndContractBlock();

		return new Range<T>(r1.Low.SubtractValue(r2.Low), r1.High.SubtractValue(r2.High));
	}

	public static IRange<T> MultiplyBy<T>(this IRange<T> r1, IRange<T> r2)
		where T : struct, IComparable
	{
		if (r1 is null)
			throw new ArgumentNullException(nameof(r1));
		if (r2 is null)
			throw new ArgumentNullException(nameof(r2));
		Contract.EndContractBlock();

		return new Range<T>(r1.Low.MultiplyBy(r2.Low), r1.High.MultiplyBy(r2.High));
	}

	public static IRange<T> DivideBy<T>(this IRange<T> r1, IRange<T> r2)
		where T : struct, IComparable
	{
		if (r1 is null)
			throw new ArgumentNullException(nameof(r1));
		if (r2 is null)
			throw new ArgumentNullException(nameof(r2));
		Contract.EndContractBlock();

		return new Range<T>(r1.Low.DivideBy(r2.Low), r1.High.DivideBy(r2.High));
	}
	#endregion


	public static T Delta<T>(this IRange<T> target)
		where T : struct, IComparable
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

	public static bool IsInRange(this IRange<TimeSpan> target, TimeSpan value, bool includeLimits = false)
	{
		if (target is null)
			throw new ArgumentNullException(nameof(target));
		Contract.EndContractBlock();

		return includeLimits
			? (value >= target.Low && value <= target.High)
			: (value > target.Low && value < target.High);
	}

	public static bool IsInRange(this IRange<DateTime> target, DateTime value, bool includeLimits = false)
	{
		if (target is null)
			throw new ArgumentNullException(nameof(target));
		Contract.EndContractBlock();

		return includeLimits
			? (value >= target.Low && value <= target.High)
			: (value > target.Low && value < target.High);
	}

	public static bool IsInRange(this IRange<int> target, int value, bool includeLimits = false)
	{
		if (target is null)
			throw new ArgumentNullException(nameof(target));
		Contract.EndContractBlock();

		return includeLimits
			? (value >= target.Low && value <= target.High)
			: (value > target.Low && value < target.High);
	}

	public static IEnumerable<DateTime> Dates(this IRange<DateTime> target)
	{
		if (target is null)
			throw new ArgumentNullException(nameof(target));
		Contract.EndContractBlock();

		var startDate = target.Low;
		var endDate = target.High;

		for (var d = startDate; d < endDate; d = d.AddDays(1).Date)
			yield return d.Date;
	}

	public static void UpdateMinMax(this double value, ref double min, ref double max)
	{
		if (double.IsNaN(value)) return;
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