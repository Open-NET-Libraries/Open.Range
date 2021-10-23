using Xunit;

namespace Open.RangeTests;

public static class Expand
{

	[Theory]
	[InlineData(-1, 5, 1)]
	[InlineData(10, 20, 15)]
	public static void NoChange(int low, int high, int expand)
	{
		var a = Range.Between(low, high).Expand(expand, true);
		Assert.Equal(low, a.Low);
		Assert.Equal(high, a.High);
	}

	[Theory]
	[InlineData(-1, 5, 6)]
	[InlineData(10, 20, 32)]
	public static void ExpandHigh(int low, int high, int expand)
	{
		var a = Range.Between(low, high).Expand(expand, true);
		Assert.Equal(low, a.Low);
		Assert.Equal(expand, a.High);
		Assert.True(a.High.Inclusive);
	}

	[Theory]
	[InlineData(-1, 5, -2)]
	[InlineData(10, 20, 5)]
	public static void ExpandLow(int low, int high, int expand)
	{
		var a = Range.Between(low, high).Expand(expand, true);
		Assert.Equal(expand, a.Low);
		Assert.Equal(high, a.High);
		Assert.True(a.Low.Inclusive);
	}

	[Fact]
	public static void ExpandNaN()
	{
		var a = Range.Between(double.NaN, double.NaN).Expand(10d, true);
		Assert.Equal(10d, a.Low);
		Assert.Equal(10d, a.High);

		a = Range.Between(5d, double.NaN).Expand(10d, true);
		Assert.Equal(5d, a.Low);
		Assert.Equal(10d, a.High);

		a = Range.Between(20d, double.NaN).Expand(10d, true);
		Assert.Equal(10d, a.Low);
		Assert.Equal(10d, a.High);

		a = Range.Between(double.NaN, 5d).Expand(10d, true);
		Assert.Equal(10d, a.Low);
		Assert.Equal(10d, a.High);

		a = Range.Between(double.NaN, 20d).Expand(10d, true);
		Assert.Equal(10d, a.Low);
		Assert.Equal(20d, a.High);


	}
}
