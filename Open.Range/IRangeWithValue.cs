using System;

namespace Open;

public interface IRangeWithValue<out T, out TValue>
	: IRange<T>
	where T : IComparable<T>
{
	TValue Value { get; }
}
