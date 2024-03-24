using System;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Math;

namespace AutoPilotControl.Nmea0183
{
	public static class MissingMath
	{
		private const int maxRoundingDigits = 15;
		private const double doubleRoundLimit = 1E16;
		// This table is required for the Round function which can specify the number of digits to round to
		private static readonly double[] roundPower10Double = new double[] {
			1E0, 1E1, 1E2, 1E3, 1E4, 1E5, 1E6, 1E7, 1E8,
			1E9, 1E10, 1E11, 1E12, 1E13, 1E14, 1E15
		};

		public static double Round(double value, int digits)
		{
			if ((digits < 0) || (digits > maxRoundingDigits))
			{
				throw new ArgumentOutOfRangeException(nameof(digits));
			}

			if (Abs(value) < doubleRoundLimit)
			{
				double power10 = roundPower10Double[digits];
				value *= power10;
				value = Math.Round(value);
				value /= power10;
			}

			return value;
		}
	}
}
