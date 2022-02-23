using FluentAssertions;
using Xunit;

namespace Open.RangeTests;

public static class Expand
{
	[Theory]
	[InlineData(-1, 5, 1)]
	[InlineData(10, 20, 15)]
	public static void NoChange(double low, double high, double expand)
	{
		var a = Range.Between(low, high).Expand(expand, true);
		Assert.Equal(low, a.Low);
		Assert.Equal(high, a.High);
		var b = a.Expand(double.NaN, true);
		a.Equals(b).Should().BeTrue();
		b = a.Expand(double.NaN, false);
		a.Equals(b).Should().BeTrue();
	}

	[Theory]
	[InlineData(-1, 5, 6)]
	[InlineData(10, 20, 32)]
	public static void ExpandHigh(int low, int high, int expand)
	{
		var a = Range.Between(low, high).Expand(expand, true);
		Assert.Equal(low, a.Low);
		Assert.Equal(expand, a.High);
		a.High.Inclusive.Should().BeTrue();
	}

	[Theory]
	[InlineData(-1, 5, -2)]
	[InlineData(10, 20, 5)]
	public static void ExpandLow(int low, int high, int expand)
	{
		var a = Range.Between(low, high).Expand(expand, true);
		Assert.Equal(expand, a.Low);
		Assert.Equal(high, a.High);
		a.Low.Inclusive.Should().BeTrue();
	}
}
