using System;
using System.Runtime.InteropServices;
[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct MathfInternal
{
	public static volatile float FloatMinNormal = 1.17549435E-38f;

	public static volatile float FloatMinDenormal = 1.401298E-45f;

	// disable once CompareOfFloatsByEqualityOperator
	public static bool IsFlushToZeroEnabled = MathfInternal.FloatMinDenormal == 0f;
}