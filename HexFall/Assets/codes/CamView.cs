using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CamView : MonoBehaviour
{
	public float rateView = 25;

	private void Awake()
	{
		Camera cam = GetComponent<Camera>();
		cam.orthographicSize = rateView * (1 / cam.aspect);
	}

#if UNITY_EDITOR
	public void camView()
	{
		Awake();
	}
#endif
}

#if UNITY_EDITOR

[CustomEditor(typeof(CamView))]
public class CamViewEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		if (!Application.isPlaying)
		{
			CamView myCamView = (CamView)target;
			if (GUILayout.Button("CamView"))
			{
				myCamView.camView();
			}
		}
	}
}
#endif
