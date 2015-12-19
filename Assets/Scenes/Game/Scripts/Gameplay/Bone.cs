using UnityEngine;
using System.Collections;

public class Bone : MonoBehaviour
{
	private Rigidbody2D _rb;

	void Awake()
	{
		_rb = GetComponent<Rigidbody2D>();
	}

	void FixedUpdate()
	{
		WorldUtils.StayInBounds(ref _rb);

		Debug.DrawLine(_rb.position, _rb.position + _rb.velocity.normalized * 2.0f, Color.magenta);
	}
}
