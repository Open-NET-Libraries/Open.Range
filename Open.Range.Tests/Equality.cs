using Xunit;

namespace Open.RangeTests;

public static class Equality
{
	[Theory]
	[InlineData(1d, 5d)]
	[InlineData(1d, 1d)]
	[InlineData(1d, double.NaN)]
	[InlineData(double.NaN, double.NaN)]
	public static void IsEqual(double low, double high)
	{
		{
			var a = Range.From(low).To(high);
			var b = Range.From(low).To(high);
			Assert.True(a.Equals(b));

			a = Range.Above(low).Below(high);
			b = Range.Above(low).Below(high);
			Assert.True(a.Equals(b));

			a = Range.From(low).Below(high);
			b = Range.From(low).Below(high);
			Assert.True(a.Equals(b));

			a = Range.Above(low).To(high);
			b = Range.Above(low).To(high);
			Assert.True(a.Equals(b));
		}

		{
			var a = Range.WithValue(low, high, 1);
			var b = Range.WithValue(low, high, 1);
			Assert.True(a.Equals(b));
		}
	}
}
