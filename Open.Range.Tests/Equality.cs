using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Open.RangeTests;

public static class Equality
{
	[Fact]
	public static void IsEqual()
	{
		var a = Range.From(5).To(10);
	}
}
