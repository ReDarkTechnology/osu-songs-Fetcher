using System;
using System.Windows.Forms;
public static class Utility {
	public static Vector2 GetScreenSize(bool squarize = true){
		var screen = new Vector2(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
		if(squarize){
			screen = SquarizeVector(screen);
		}
		return screen;
	}
	public static Vector2 LocalCanvas = new Vector2(1f, 1f);
	public static Vector2 ConvertWorldToLocal(Vector2 world){
		var screen = GetScreenSize();
		var originalScreen = GetScreenSize(false);
		var _add = (originalScreen/screen) / 4;
		var ratio = GetRatio(LocalCanvas, screen);
		return world * (float) ratio;
	}
	public static Vector2 ConvertLocalToWorld(Vector2 local){
		var screen = GetScreenSize();
		var originalScreen = GetScreenSize(false);
		var _add = (originalScreen/screen) / 4;
		var ratio = GetRatio(LocalCanvas, screen);
		var scale = ResizeScreen(LocalCanvas, GetScreenSize());
		return local / (float) ratio;
	}
	public static Vector2 SquarizeVector(Vector2 vector){
		// disable once CompareOfFloatsByEqualityOperator
		if(vector.x < vector.y){
			return new Vector2(vector.x, vector.x);
		}
		if(vector.x > vector.y){
			return new Vector2(vector.y, vector.y);
		}
		return vector;
	}
	public static Vector2 ResizeScreen(Vector2 canvas, Vector2 original)
    {
		double ratio = GetRatio(canvas, original);

        // now we can get the new height and width
        int newHeight = Convert.ToInt32(original.y * ratio);
        int newWidth = Convert.ToInt32(original.x * ratio);
        
        return new Vector2(newWidth, newHeight);
    }
	public static double GetRatio(Vector2 canvas, Vector2 original)
    {
        // Figure out the ratio
        double ratioX = (double)canvas.x / (double)original.x;
        double ratioY = (double)canvas.y / (double)original.y;
        // use whichever multiplier is smaller
        return ratioX < ratioY ? ratioX : ratioY;
    }
	public static string FloatToString(float fr){
		var result = fr.ToString();
		try {
			result = result.Replace((",").ToCharArray()[0], (".").ToCharArray()[0]);
		}catch{
			
		}
		return result;
	}
	public static float StringToFloat(string fr){
		fr = fr.Replace(".", ",");
		return float.Parse(fr);
	}
}
public enum Space
{
	World,
	Self
}
public enum SendMessageOptions
{
	RequireReceiver,
	DontRequireReceiver
}
public enum PrimitiveType
{
	Sphere,
	Capsule,
	Cylinder,
	Cube,
	Plane,
	Quad
}
internal enum RotationOrder
{
	OrderXYZ,
	OrderXZY,
	OrderYZX,
	OrderYXZ,
	OrderZXY,
	OrderZYX
}
