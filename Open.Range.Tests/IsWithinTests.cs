using FluentAssertions;
using Xunit;

namespace Open.RangeTests;

public static class IsWithinTests
{
	[Theory]
	[InlineData(1, 2, 3)]
	[InlineData(1, 1, 3)]
	[InlineData(1, 3, 3)]
	public static void IsInRange(int low, int value, int high)
	{
		value.IsInRange(low, high)
			.Should().BeTrue();

		Range.From(low).To(high).Contains(value)
			.Should().BeTrue();
		Range.Include(low, high).Contains(value)
			.Should().BeTrue();

		float f = value;
		f.IsInRange(low, high)
			.Should().BeTrue();
		var fr = Range.From((float)low).To(high);
		fr.Contains(value)
			.Should().BeTrue();

		double d = value;
		d.IsInRange(low, high)
			.Should().BeTrue();
		var dr = Range.From((double)low).To(high);
		dr.Contains(value)
			.Should().BeTrue();

		float.NaN.IsInRange(low, high)
			.Should().BeFalse();
		double.NaN.IsInRange(low, high)
			.Should().BeFalse();
	}

	[Theory]
	[InlineData(1, 3, 2)]
	[InlineData(1, 0, 3)]
	public static void IsNotInRange(int low, int value, int high)
	{
		value.IsInRange(low, high).Should().BeFalse();

		var range = Range.From(low).To(high);
		range.Contains(value).Should().BeFalse();

		float f = value;
		f.IsInRange(low, high).Should().BeFalse();

		double d = value;
		d.IsInRange(low, high).Should().BeFalse();
	}

	[Theory]
	[InlineData(1, 2, 3)]
	[InlineData(1, 3, 4)]
	public static void IsInBounds(int low, int value, int high)
	{
		value.IsInBounds(low, high)
			.Should().BeTrue();

		Range.Above(low).Below(high).Contains(value)
			.Should().BeTrue();
		Range.Between(low, high).Contains(value)
			.Should().BeTrue();

		((float)value).IsInBounds(low, high)
			.Should().BeTrue();
		Range.Above((float)low).Below(high).Contains(value)
			.Should().BeTrue();

		((double)value).IsInBounds(low, high)
			.Should().BeTrue();
		Range.Above((double)low).Below(high).Contains(value)
			.Should().BeTrue();

		float.NaN.IsInBounds(low, high)
			.Should().BeFalse();
		double.NaN.IsInBounds(low, high)
			.Should().BeFalse();
	}

	[Theory]
	[InlineData(1, 3, 2)]
	[InlineData(1, 0, 3)]
	[InlineData(1, 3, 3)]
	[InlineData(1, 1, 3)]
	public static void IsNotInBounds(int low, int value, int high)
	{
		value.IsInBounds(low, high)
			.Should().BeFalse();

		Range.Above(low).Below(high).Contains(value)
			.Should().BeFalse();

		((float)value).IsInBounds(low, high)
			.Should().BeFalse();

		((double)value).IsInBounds(low, high)
			.Should().BeFalse();
	}
}
