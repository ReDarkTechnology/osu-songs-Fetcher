using System;
using System.Windows.Forms;
public static class Time
{
	public static float timeScale = 1;
	public static float deltaTime = 0.1f;
	public static float unscaledDeltaTime = 0.1f;
	public static float fixedDeltaTime = 0.01f;
	public static int frameCount = 1;
	public static int targetFPS = 60;
	public static Timer currentTimer;
	public static Action UpdateVoid;
	public static DateTime time1 = DateTime.Now;
    public static DateTime time2 = DateTime.Now;
	static void Update(object sender, EventArgs e)
	{
		time2 = DateTime.Now;
		deltaTime = ((time2.Ticks - time1.Ticks) / 10000000f) * timeScale;
        fixedDeltaTime = deltaTime;
        unscaledDeltaTime = (time2.Ticks - time1.Ticks) / 10000000f;
        time1 = time2;
	}

	public static void InitializeTime(){
		currentTimer = new Timer();
		currentTimer.Interval = 1;
		currentTimer.Tick += Update;
		currentTimer.Enabled = true;
		currentTimer.Start();
	}
}
