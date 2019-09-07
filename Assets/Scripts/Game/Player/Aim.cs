using UnityEngine;
using System.Collections;

public class Aim : MonoBehaviour 
{

	[SerializeField]
	private GameObject Gun;

	private float _LastX = 1f;
	private bool _PointRight = true;
	private bool _PointLeft = false;
	public float XDistance = 0.7f;
	public float YDistance = 0.7f;

    private bool InputAble = true;

	private void Awake()
	{
	}

	private void Update()
	{
		if(InputAble)
		{
			float xInput = Input.GetAxisRaw("Horizontal");
 			float yInput = Input.GetAxisRaw("Vertical");

			if(xInput == 0 && yInput <= 0.5 && yInput >= -0.5)
            {
                xInput = _LastX;
            } 
            
			if(xInput > 0) 
			{
				xInput = XDistance;
				_PointRight = true;
                _PointLeft = false;
			}

			if(xInput < 0) 
			{
				xInput = XDistance;
				_PointRight = false;
                _PointLeft = true;
			}

			if(yInput > 0.5) 
			{
				yInput = YDistance;
			}

			if(yInput < -0.5) 
			{
				yInput = -1*YDistance;
			}

			if(yInput <= 0.5 && yInput >= -0.5)  
			{
				yInput = 0;
			}

            if(xInput != 0)
			{
				_LastX = xInput;
			}

            Gun.transform.localPosition = new Vector3(xInput, yInput, 0);
            float atan2 = Mathf.Atan2(Gun.transform.localPosition.y, Gun.transform.localPosition.x); 
		    //Gun.transform.rotation = Quaternion.LookRotation(Gun.transform.localPosition);

           if(_PointRight)
		    Gun.transform.rotation = Quaternion.Euler(0f,0f,atan2*Mathf.Rad2Deg - 90f);

            if(_PointLeft)
            {
               Gun.transform.rotation = Quaternion.Euler(0f,180f,atan2*Mathf.Rad2Deg - 90f);
            }
	    }
	}
}

// public class Aim : MonoBehaviour 
// {
// 	public GameObject Orb;
// 	public int TeamNumber;
// 	public float Distance = 1f;
// 	public float Speed = 0.1f;

// 	private Vector3 _Direction = Vector3.right;

// 	void Update()
// 	{
// 		float rh = Input.GetAxis("Right_Horizontal_" + TeamNumber);
// 		float rv = Input.GetAxis("Right_Vertical_" + TeamNumber);

// 		float xIn = Input.GetAxis("Horizontal_" + TeamNumber);

// 		if(Mathf.Abs(rh) > 0.15f || Mathf.Abs(rv) > 0.15f)
// 		{
// 			Vector3 TargetDirection = new Vector3(rh, rv, 0.0f);
// 			_Direction = -1*TargetDirection;
// 		}

// 		bool right = true;

// 		if(xIn > 0)
// 		right = true;

// 		if(xIn < 0)
// 		right = false;

// 		float newangle = Vector3.Angle(_Direction, transform.right);
// /*
// 		if(rv < 0 && right)
// 		{
// 			newangle = Vector3.Angle(_Direction, transform.right);
// 		}
// 		if(rv < 0 && !right)
// 		{
// 			newangle = Vector3.Angle(_Direction, transform.right);
// 			newangle = 180 - newangle;
// 		}
// 		if(rv > 0 && right)
// 		{
// 			newangle = -1*Vector3.Angle(_Direction, transform.right);
// 		}
// 		if(rv > 0 && !right)
// 		{
// 			newangle = -1*Vector3.Angle(_Direction, transform.right);
// 			newangle = 180 - newangle;
// 		}

// 		if(xIn == 0 && right)
// 		{
// 			newangle = Vector3.Angle(_Direction, transform.right);
// 		}

// 		if(xIn == 0 && !right)
// 		{
// 			newangle = Vector3.Angle(_Direction, transform.right);
// 			newangle = 180 - newangle;
// 		}
// */

// 		newangle = Mathf.Round(newangle / 45.0f) * 45.0f;
// 		Debug.Log(newangle);
// /*
// 		if(right)
// 		newangle = Mathf.Clamp(newangle, -90, 90);

// 		if(!right)
// 		newangle = Mathf.Clamp(newangle, 90, 270);
// */
// 		Vector3 newDirection = new Vector3(Mathf.Cos(Mathf.Deg2Rad * newangle), Mathf.Sin(Mathf.Deg2Rad * newangle), 0);
// 		Ray newray = new Ray(transform.position, newDirection);

// 		//Ray ray = new Ray(transform.position, _Direction);

// 		float angleY = (transform.position.y - Orb.transform.position.y);
// 		float angleX = -(transform.position.x - Orb.transform.position.x);

// 		float angle = Mathf.Atan2(angleY, angleX) * Mathf.Rad2Deg-90;
// 		float anglecheck = Mathf.Round(angle / 45.0f) * 45.0f;

// 		//Vector3 DirectionCheck = new Vector3(Mathf.Cos(Mathf.Deg2Rad * anglecheck), Mathf.Sin(Mathf.Deg2Rad * anglecheck), 0);

// 		//Ray raycheck = new Ray(transform.position, DirectionCheck);

// 		if(Mathf.Abs(rh) + Mathf.Abs(rv) > 1)
// 		{
// 			Orb.transform.position = newray.GetPoint(Distance);
// 			Orb.transform.rotation = Quaternion.AngleAxis(anglecheck, Vector3.forward*-1);
// 		}

// 	}

// }
