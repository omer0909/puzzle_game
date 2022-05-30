using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{
	[SerializeField]
	LevelManager lm;
	[SerializeField]
	Text point;
	void Start()
	{
		int record = PlayerPrefs.GetInt("record");
		if (lm.point == record)
			point.text = "score\t: " + lm.point + "\nIt's a record!";
		else
			point.text = "score\t: " + lm.point + "\nrecord\t: " + record;
	}

	public void restartGame()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
