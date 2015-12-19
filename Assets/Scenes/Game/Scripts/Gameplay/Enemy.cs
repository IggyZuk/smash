using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
	private Rigidbody2D _rb;
	private tk2dSprite _sprite;

	private bool _isAlive = true;
	private int _health = 2;

	private float _time;
	private Vector2 _radius;
	private Vector2 _speed;

	void Awake()
	{
		// Find and assign references to components
		_rb = GetComponent<Rigidbody2D>();
		_sprite = GetComponent<tk2dSprite>();

		_time = Random.value * 360f;
		_radius = new Vector2(Random.Range(1f, 5f), Random.Range(1f, 5f));
		_speed = new Vector2(Random.Range(1f, 3f), Random.Range(1f, 3f));
	}

	void FixedUpdate()
	{
		// Some basic movement for the enemy
		if(_isAlive)
		{
			_time += Time.deltaTime;

			_rb.velocity = Vector2.zero;
			_rb.AddForce(new Vector3(Mathf.Cos(_time * _speed.x) * _radius.x, Mathf.Sin(_time * _speed.y) * _radius.y), ForceMode2D.Impulse);

			Debug.DrawLine(transform.position, transform.position + new Vector3(Mathf.Cos(Time.time), Mathf.Sin(Time.time)));
		}

		WorldUtils.StayInBounds(ref _rb);

		Debug.DrawLine(_rb.position, _rb.position + _rb.velocity.normalized * 2.0f, Color.magenta);
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		// If we've collided with the player then fire an event to let the player know! Also let's take away some health for the enemy
		if(collider.tag == "Player")
		{
			_rb.gravityScale = 0.9f;
			Vector2 vel = GameController.Instance.Player.GetVelocity();
			vel.y = 7.0f;
			_rb.AddForce(vel, ForceMode2D.Impulse);

			_sprite.SetSprite(Random.value < 0.5f ? "GoombaDead1" : "GoombaDead2");

			_isAlive = false;

			if(--_health <= 0)
			{
				GameController.Instance.OnPlayerBoost(Vector3.up, 1.1f);
				Explode();
			}
			else
			{
				GameController.Instance.OnPlayerBoost(Vector3.up, 0.75f);
				GameController.Instance.Camera.Screenshake(0.25f, 0.5f);
			}

			_sprite.color = Color.Lerp(Color.white, Color.red, 1f / (float)_health);
		}
	}

	private void Explode()
	{
		VisualUtils.AddExplosion(this.transform.position);

		for(int i = 0; i < 6; ++i)
		{
			GameObject go = GameObject.Instantiate(GameSettings.Instance.Prefabs.Bone, this.transform.position, Quaternion.AngleAxis(Random.value * 360f, Vector3.forward)) as GameObject;
			Rigidbody2D boneRigidbody = go.GetComponent<Rigidbody2D>();

			Vector2 vel = new Vector2(Random.Range(-20f, 20f), Random.value * 20f);
			boneRigidbody.AddForce(vel, ForceMode2D.Impulse);

			boneRigidbody.angularVelocity = 0.0f;
			boneRigidbody.AddTorque(-boneRigidbody.velocity.x * 0.8f, ForceMode2D.Impulse);
		}

		GameObject.Destroy(this.gameObject);
	}
}
