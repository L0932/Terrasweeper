using UnityEngine;
using UnityEditor;
using System.Collections;

public class TestScript : MonoBehaviour {
	[MenuItem("Screenshot/Take screenshot")]
	static void Screenshot()
	{
		ScreenCapture.CaptureScreenshot("test.png");
	}	
}
