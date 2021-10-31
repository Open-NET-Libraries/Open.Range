using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Open.RangeTests;

public static class RangeExtensionsTests
{
	[Fact]
	public static void NullParam()
	{
		Assert.Throws<ArgumentNullException>(
			() => RangeExtensions.Range<int>(null));
		Assert.Throws<ArgumentNullException>(
			() => RangeExtensions.Range<int>(null, null));
		Assert.Throws<ArgumentNullException>(
			() => RangeExtensions.Range((IEnumerable<int>)null, 0, null));
		Assert.Throws<ArgumentNullException>(
			() => RangeExtensions.Range(null, 0));
		Assert.Throws<ArgumentNullException>(
			() => RangeExtensions.Range((IEnumerable<int>)null, (Func<int, double>)null));
		Assert.Throws<ArgumentNullException>(
			() => RangeExtensions.Range(Enumerable.Empty<int>().AsParallel(), null));
		Assert.Throws<ArgumentNullException>(
			() => RangeExtensions.Range(Enumerable.Empty<int>(), 0, null));
	}

	[Fact]
	public static void MinMax()
	{
		Enumerable.Range(5, 20)
			.Select(e => (double)e)
			.Prepend(double.NaN).Prepend(double.NaN)
			.Append(double.NaN).Range()
			.Equals(Range.Create(5d, 24d))
			.Should().BeTrue();

		Enumerable.Range(5, 20000)
			.Select(e => (double)e)
			.Prepend(double.NaN).Prepend(double.NaN)
			.Append(double.NaN).AsParallel().Range(e => e)
			.Equals(Range.Create(5d, 20004d))
			.Should().BeTrue();

		Enumerable.Range(5, 20)
			.Range(e => (double)e)
			.Equals(Range.Create(5d, 24d))
			.Should().BeTrue();

		Enumerable.Range(5, 20)
			.Range(0, e => (double)e)
			.Equals(Range.Create(5d, 24d))
			.Should().BeTrue();

		Assert.Throws<InvalidOperationException>(
			() => Enumerable.Empty<int>().Range());

		Enumerable.Range(5, 20).Range(0)
			.Equals(Range.Create(5, 24))
			.Should().BeTrue();

		Enumerable.Empty<int>().Range(0)
			.Equals(Range.Create(0, 0))
			.Should().BeTrue();

	}
}
