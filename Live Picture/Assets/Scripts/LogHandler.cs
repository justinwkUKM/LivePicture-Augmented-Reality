
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LogHandler : MonoBehaviour
{

    string myLog;
    Queue myLogQueue = new Queue();
    StringBuilder sb_log = new StringBuilder();


	void Awake()
    {
		Application.logMessageReceived += HandleLog;
	}


    void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }


    void HandleLog(string logString, string stackTrace, LogType type)
    {
        sb_log.Append('\n');
        sb_log.Append(logString);

        if (type == LogType.Exception)
        {
            sb_log.Append('\n');
            sb_log.Append(stackTrace);
        }
        if (sb_log.Length > 5000)
        {
            int count = sb_log.Length - 5000;
            sb_log.Remove(0, count);
        }

        myLog = sb_log.ToString();

    }


    
    Rect rect_button = scaleRectW(0f, 0f, 0.1f, 0.025f);
	Rect rect_log = scaleRectW(0f, 0.025f, 1f, 1f);
	bool show_logs = true;
    GUIStyle style_log, style_button;
    Vector2 scrollPosition = Vector2.zero;

	GUILayoutOption[]  gui_layout_options;


    void OnGUI()
    {
        if (style_log == null)
        {
            style_log = defaultStyle(TextAnchor.LowerLeft, 0.015f, Color.white);
            style_button = defaultStyle(TextAnchor.MiddleCenter, 0.01f, Color.white);

			gui_layout_options = new GUILayoutOption[] { GUILayout.Width(UnityEngine.Screen.width), GUILayout.Height(UnityEngine.Screen.height) };
		}

        if (GUI.Button(rect_button, show_logs ? "LOGS ON" : "LOGS OFF", style_button))
        {
            show_logs = !show_logs;

            if (!show_logs)
            {
				

				if (myLogQueue != null)
                {
                    myLogQueue.Clear();
                    myLog = null;
                }
            }
        }

        if (show_logs)
        {
            GUILayout.BeginArea(rect_log);
			this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, gui_layout_options);
			GUI.skin.box.wordWrap = false;     // set the wordwrap on for box only.
            GUILayout.Box(myLog, style_log);        // just your message as parameter.
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
	}


	
	public static Rect scaleRectH(float x, float y, float w, float h)
	{

		float width = (float)Screen.width * w;
		float height = (float)Screen.width * h;
		float x_offset = (float)Screen.width * x;
		float y_offset = (float)Screen.width * y;

		return new Rect(x_offset, y_offset, width, height);
	}

	public static Rect scaleRectW(float x, float y, float w, float h)
	{

		float width = (float)Screen.height * w;
		float height = (float)Screen.height * h;
		float x_offset = (float)Screen.height * x;
		float y_offset = (float)Screen.height * y;

		return new Rect(x_offset, y_offset, width, height);
	}



	public static GUIStyle defaultStyle(TextAnchor anchor)
	{

		GUIStyle style = new GUIStyle(GUI.skin.button);
		style.normal.textColor = Color.white;
		style.fontSize = (int)size(0.03f);
		style.alignment = anchor;
		return style;

	}

	public static GUIStyle defaultStyle(TextAnchor anchor, float font_size, Color color)
	{

		GUIStyle style = new GUIStyle(GUI.skin.button);
		style.normal.textColor = color;
		style.fontSize = (int)size(font_size);
		style.alignment = anchor;
		return style;

	}


	public static float size(float value)
	{
		return (float)Screen.height * value;
	}
}
