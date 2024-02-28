using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	public float MoveSpeed = 8f;
	public int Damage = 1;

	[HideInInspector]
	public GameObject Target;

	void Start()
	{
		Destroy(gameObject, 5f);
	}

	void Update()
	{
		if (Target == null)
		{
			Destroy(gameObject);
			return;
		}

		Vector3 dir = Target.transform.position - transform.position;
		dir.Normalize();

		transform.position += dir * MoveSpeed * Time.deltaTime;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Enemy"))
		{
			Destroy(gameObject);
		}
	}
}
