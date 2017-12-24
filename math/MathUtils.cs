namespace JarvOS
{
	public static class MathUtils
	{
		public static float Lerp(float a, float b, float percentage)
		{
			return ((1 - percentage) * a) + (percentage * b);
		}

		public static double Lerp(double a, double b, double percentage)
		{
			return ((1 - percentage) * a) + (percentage * b);
		}
	}
}