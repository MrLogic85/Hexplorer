using System;
using UnityEngine;


public static class Extension {
	
	public static bool IsNotZero(this Vector3 vector)
	{
		return vector.x != 0 || vector.y != 0 || vector.z != 0;
	}

	public static bool IsZero(this Vector3 vector)
	{
		return !vector.IsZero();
	}

	public static double DistanceTo2D(this Vector3 from, Vector3 to)
	{
		var dir = to - from;
		dir.y = 0;
		return dir.magnitude;
	}
}