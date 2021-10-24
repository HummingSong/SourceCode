using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicParticle : PSObject
{
	private ParticleSystem[] pss;

	public void Awake()
	{
		pss = GetComponentsInChildren<ParticleSystem>();	
	}

	void OnEnable()
	{
		for(int i = 0; i < pss.Length; ++i)
		{
			pss[i].Play();
		}

		StartCoroutine("CheckIfAlive");
	}

	IEnumerator CheckIfAlive()
	{
		ParticleSystem ps = this.GetComponent<ParticleSystem>();

		while (true && ps != null)
		{
			yield return new WaitForSeconds(0.5f);
			if (!ps.IsAlive(true))
			{
				if (key != null)
					PSObjectPoolManager.instance.SaveObject(key, this.gameObject);
				else
					Destroy(gameObject);
				break;
			}
		}
	}
}
