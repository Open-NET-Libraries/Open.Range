using System;
using System.Collections.Generic;
using System.Text;

namespace Open;

public static class Range
{
	/// <summary>
	/// Defines a range of values <typeparamref name="T"/>.
	/// </summary>
	public static Range<T> Create<T>(T low, T high)
		=> new(low, high);

	/// <summary>
	/// Defines the lowest inclusive value of a range.
	/// </summary>
	/// <param name="low"></param>
	/// <param name="inclusive"></param>
	public static Boundary<T> From<T>(T low)
		=> new(low, true);

	/// <summary>
	/// Defines the lower (non-inclusive) boundary of a range.
	/// </summary>
	/// <param name="low">The excluded value to start from.</param>
	public static Boundary<T> Above<T>(T low)
		=> new(low, false);

	/// <summary>
	/// Defines a non-inclusive range.
	/// </summary>
	/// <param name="low">The excluded minimum.</param>
	/// <param name="high">The excluded maximum</param>
	public static Range<Boundary<T>> Between<T>(T low, T high)
		=> new(new(low, false), new(high, false));

	/// <summary>
	/// Defines an inclusive range.
	/// </summary>
	/// <param name="low">The inclusive minimum.</param>
	/// <param name="high">The inclusive maximum</param>
	public static Range<Boundary<T>> Include<T>(T low, T high)
		=> new(new(low, true), new(high, true));


}
