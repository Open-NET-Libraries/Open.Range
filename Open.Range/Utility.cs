namespace Open;

internal static class Utility
{
	public static bool CanBeNaN<T>()
		=> typeof(T) == typeof(double) || typeof(T) == typeof(float);

	public static bool IsNaN<T>(T value)
		=> value switch
		{
			double d => double.IsNaN(d),
			float f => float.IsNaN(f),
			_ => false
		};
}
