using System;

namespace Open;

public interface IRange<out T>
	where T : IComparable<T>
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
