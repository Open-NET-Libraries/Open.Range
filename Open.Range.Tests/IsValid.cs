using Xunit;

namespace Open.RangeTests;

public static class IsValid
{
	[Theory]
	[InlineData(1d, 2d)]
	[InlineData(-100d, -6d)]
	[InlineData(-50d, 40d)]
	public static void CheckValidity(double low, double high)
		=> Assert.True(Range.Create(low, high).IsValidRange());

	[Theory]
	[InlineData(5d, 2d)]
	[InlineData(-100d, -600d)]
	[InlineData(50d, 40d)]
	[InlineData(double.NaN, 40d)]
	[InlineData(50d, double.NaN)]
	public static void CheckInvalid(double low, double high)
		=> Assert.False(Range.Create(low, high).IsValidRange());
}
