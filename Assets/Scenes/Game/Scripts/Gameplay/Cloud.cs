using UnityEngine;
using System.Collections;

public class Cloud : MonoBehaviour
{
	private tk2dSprite _sprite;
	private float _moveSpeed;

	void Awake()
	{
		_sprite = GetComponent<tk2dSprite>();
		_moveSpeed = 0.5f;
	}

	void Update()
	{
		Vector3 pos = this.transform.position;
		pos.x += _moveSpeed * Time.deltaTime;

		if(pos.x > WorldUtils.GetRightEdge() + _sprite.GetBounds().extents.x)
		{
			pos.x = WorldUtils.GetLeftEdge() - _sprite.GetBounds().extents.x;
		}

		if(pos.y < WorldUtils.GetBottomEdge() - _sprite.GetBounds().extents.y)
		{
			pos.x = Random.Range(WorldUtils.GetLeftEdge(), WorldUtils.GetRightEdge());
			pos.y = WorldUtils.GetTopEdge() + _sprite.GetBounds().extents.y;
		}

		transform.position = pos;
	}
}
