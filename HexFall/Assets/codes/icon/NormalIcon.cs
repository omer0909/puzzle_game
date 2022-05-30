using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalIcon : AIcon
{
	public override void Action()
	{

	}
	public override void Take()
	{
		lm.addPoint += 5;
	}
}
