using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : AIcon
{
	int remaining = 7;
	[SerializeField]
	TextMesh clock;

	public override void Action()
	{
		--remaining;
		clock.text = remaining.ToString();
		if (remaining == 0)
			lm.End();
	}

	public override void Take()
	{
		lm.addPoint += 5;
	}

	private void LateUpdate()
	{
		transform.eulerAngles = Vector3.zero;
	}
}
