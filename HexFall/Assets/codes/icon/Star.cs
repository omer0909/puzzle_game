using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : AIcon
{
	public override void Action()
	{

	}
	public override void Take()
	{
		lm.addPoint += 5;
		lm.multiplyPoint *= 2;
	}
}
