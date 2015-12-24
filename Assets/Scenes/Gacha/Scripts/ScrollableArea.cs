using UnityEngine;
using System.Collections;

namespace Gacha
{
	[System.Serializable]
	public class Item
	{
		public string Name;
		public Color Color;
		internal Transform Transform;
	}

	public class ScrollableArea : MonoBehaviour
	{
		public tk2dTextMesh NameText;
		public tk2dTextMesh NumberText;

		public Transform Content;
		public BoxCollider Collider;
		public GameObject ItemPrefab;
		public float ItemWidth;
		public Item[] Items;

		private bool _isDragging = false;
		private float _startPosX = 0f;
		private float _contentLastPosX = 0f;
		private float _lastFramePosX = 0f;
		private float _velocity = 0f;

		// Use this for initialization
		void Start()
		{
			// Register to input events
			InputController.OnTouchBegan += TouchBegan;
			InputController.OnTouchMoved += TouchMoved;
			InputController.OnTouchEnded += TouchEnded;

			// Spawn items
			for(int i = 0; i < Items.Length; ++i)
			{
				GameObject go = Object.Instantiate(ItemPrefab, (Vector3.right * i * ItemWidth) + Vector3.back, Quaternion.identity) as GameObject;
				go.transform.SetParent(Content);
				Items[i].Transform = go.transform;
			}
		}

		void OnDestroy()
		{
			// Unregister from input events
			InputController.OnTouchBegan -= TouchBegan;
			InputController.OnTouchMoved -= TouchMoved;
			InputController.OnTouchEnded -= TouchEnded;
		}

		private void TouchBegan(Vector2 pos)
		{
			RaycastHit hit;
			Physics.Raycast(Camera.main.ScreenPointToRay(pos), out hit, 100f);

			if(hit.collider == Collider)
			{
				_isDragging = true;
				_startPosX = pos.x;
				_contentLastPosX = Content.position.x;
			}
		}

		private void TouchMoved(Vector2 pos)
		{
			if(_isDragging)
			{
				float newPosX = (pos.x - _startPosX) / 50f;
				Content.position = Vector3.right * (newPosX + _contentLastPosX);
				_lastFramePosX = pos.x;
			}
		}

		private void TouchEnded(Vector2 pos)
		{
			if(_isDragging)
			{
				_isDragging = false;
				_velocity = (pos.x - _lastFramePosX) / 50f;
			}
		}

		void Update()
		{
			// Rescale all items
			foreach(var obj in Items)
			{
				obj.Transform.localScale = Vector3.one * 0.9f;
				obj.Transform.GetComponent<tk2dSprite>().color = obj.Color * Color.gray;
			}

			// Get the closest item
			int idx = (int)(Mathf.Floor(-Content.position.x / ItemWidth + 0.5f));
			idx = Mathf.Clamp(idx, 0, Items.Length - 1);

			Item closestItem = Items[idx];

			// Set global name text to the item text
			NameText.text = closestItem.Name;
			NumberText.text = string.Format("{0}/{1}", idx + 1, Items.Length);

			// Make item bigger
			closestItem.Transform.localScale = Vector3.one * 1.1f;
			closestItem.Transform.GetComponent<tk2dSprite>().color = closestItem.Color * Color.white;

			if(_isDragging == false)
			{
				// Smooth movement
				_velocity -= _velocity * 5f * Time.deltaTime;
				Content.position += Vector3.right * _velocity;

				float m = (1f - Mathf.Abs(_velocity)) * 0.1f;

				// Snap onto the closest item
				Content.position = Vector3.Lerp(Content.position, (Vector3.left * closestItem.Transform.localPosition.x), m);

			}
		}
	}
}
