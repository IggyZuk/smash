using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour
{
	public static System.Action<Vector2> OnTouchBegan;
	public static System.Action<Vector2> OnTouchMoved;
	public static System.Action<Vector2> OnTouchEnded;

	void Update()
	{

#if UNITY_EDITOR || UNITY_STANDALONE
		if(Input.GetMouseButtonDown(0))
		{
			if(OnTouchBegan != null) OnTouchBegan(Input.mousePosition);
		}
		else if(Input.GetMouseButton(0))
		{
			if(OnTouchMoved != null) OnTouchMoved(Input.mousePosition);
		}
		else if(Input.GetMouseButtonUp(0))
		{
			if(OnTouchEnded != null) OnTouchEnded(Input.mousePosition);
		}
#else
        // Touch device input
        if (Input.touchCount > 0)
		{
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
			{
                if(OnTouchBegan != null) OnTouchBegan(Input.GetTouch(0).position);
            }
			else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
			{
                if(OnTouchMoved != null) OnTouchMoved(Input.GetTouch(0).position);
            }
			else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
			{
               if(OnTouchEnded != null) OnTouchEnded(Input.GetTouch(0).position);
            }
        }
#endif
	}
}
