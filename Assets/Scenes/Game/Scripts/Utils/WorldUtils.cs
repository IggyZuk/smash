using UnityEngine;
using System.Collections;

public class WorldUtils : MonoBehaviour
{
	// Check collision between the object and the edges of the screen
	public static void StayInBounds(ref Rigidbody2D rb)
	{
		Vector3 viewportPosition = Camera.main.WorldToViewportPoint(rb.position);

		WorldSettings settings = GameSettings.Instance.WorldSettings;

		if(viewportPosition.x < settings.EdgeMargin || viewportPosition.x > 1f - settings.EdgeMargin)
		{
			rb.position = Camera.main.ViewportToWorldPoint(new Vector2(Mathf.Clamp(viewportPosition.x, settings.EdgeMargin, 1f - settings.EdgeMargin), viewportPosition.y));
			rb.velocity = new Vector2(-rb.velocity.x * settings.EdgeVelocityFriction, rb.velocity.y);
		}
	}

	public static float GetLeftEdge()
	{
		return Camera.main.ViewportToWorldPoint(new Vector2(0f, 0f)).x;
	}

	public static float GetRightEdge()
	{
		return Camera.main.ViewportToWorldPoint(new Vector2(1f, 0f)).x;
	}

	public static float GetTopEdge()
	{
		return Camera.main.ViewportToWorldPoint(new Vector2(0f, 1f)).y;
	}

	public static float GetBottomEdge()
	{
		return Camera.main.ViewportToWorldPoint(new Vector2(0f, 0f)).y;
	}
}
