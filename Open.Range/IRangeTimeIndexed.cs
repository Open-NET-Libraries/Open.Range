using System;

namespace Open;

public interface IRangeTimeIndexed<out T>
	: IDateTimeIndexed, IRange<T>
	where T : IComparable<T>
	{ }
