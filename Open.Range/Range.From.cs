using System;
using System.Collections.Generic;
using System.Text;

namespace Open.Range;

public static class Range
{
	public static Boundary<T> From<T>(T low, bool inclusive = true) => new(low, inclusive);

	public static Boundary<T> Above<T>(T low) => new(low, false);
}
