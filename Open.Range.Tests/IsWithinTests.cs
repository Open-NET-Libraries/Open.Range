using Xunit;

namespace Open.Range.Tests;
public static class IsWithinTests
{
	[Theory]
	[InlineData(1, 2, 3)]
	[InlineData(1, 1, 3)]
	[InlineData(1, 3, 3)]
	public static void IsInRange(int low, int value, int high)
	{
		Assert.True(value.IsInRange(low, high));

		var range = Range.From(low).To(high);
		Assert.True(range.Contains(value));

		float f = value;
		Assert.True(f.IsInRange(low, high));
		var fr = Range.From((float)low).To(high);
		Assert.True(fr.Contains(value));

		double d = value;
		Assert.True(d.IsInRange(low, high));
		var dr = Range.From((double)low).To(high);
		Assert.True(dr.Contains(value));

		Assert.False(float.NaN.IsInRange(low, high));
		Assert.False(double.NaN.IsInRange(low, high));
	}

	[Theory]
	[InlineData(1, 3, 2)]
	[InlineData(1, 0, 3)]
	public static void IsNotInRange(int low, int value, int high)
	{
		Assert.False(value.IsInRange(low, high));

		var range = Range.From(low).To(high);
		Assert.False(range.Contains(value));

		float f = value;
		Assert.False(f.IsInRange(low, high));

		double d = value;
		Assert.False(d.IsInRange(low, high));
	}

	[Theory]
	[InlineData(1, 2, 3)]
	[InlineData(1, 3, 4)]
	public static void IsInBounds(int low, int value, int high)
	{
		Assert.True(value.IsInBounds(low, high));

		var range = Range.Above(low).Below(high);
		Assert.True(range.Contains(value));

		float f = value;
		Assert.True(f.IsInBounds(low, high));
		var fr = Range.Above((float)low).Below(high);
		Assert.True(fr.Contains(value));

		double d = value;
		Assert.True(d.IsInBounds(low, high));
		var dr = Range.Above((double)low).Below(high);
		Assert.True(dr.Contains(value));

		Assert.False(float.NaN.IsInBounds(low, high));
		Assert.False(double.NaN.IsInBounds(low, high));
	}

	[Theory]
	[InlineData(1, 3, 2)]
	[InlineData(1, 0, 3)]
	[InlineData(1, 3, 3)]
	[InlineData(1, 1, 3)]
	public static void IsNotInBounds(int low, int value, int high)
	{
		Assert.False(value.IsInBounds(low, high));

		var range = Range.Above(low).Below(high);
		Assert.False(range.Contains(value));

		float f = value;
		Assert.False(f.IsInBounds(low, high));

		double d = value;
		Assert.False(d.IsInBounds(low, high));
	}
}
