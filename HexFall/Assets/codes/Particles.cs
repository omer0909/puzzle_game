using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particles : MonoBehaviour
{
	ParticleSystem[] particles;
	int index;
	void Awake()
	{
		particles = new ParticleSystem[transform.childCount];
		for (int i = 0; i < particles.Length; i++)
			particles[i] = transform.GetChild(i).GetComponent<ParticleSystem>();
	}

	public void Play(Vector3 pos, Color color)
	{
		particles[index].transform.position = pos;
		ParticleSystem.MainModule main = particles[index].main;
		main.startColor = color;
		particles[index].Play();
		index = (index + 1) % particles.Length;
	}
}
