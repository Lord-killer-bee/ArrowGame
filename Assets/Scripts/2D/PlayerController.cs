using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerController : Unit
{
	[Space(10)]
	[Header("Dash")]

	[SerializeField]
	private float _DashMultiplier = 5f;

	[SerializeField]
	private float _DashKnockBackMultiplier = 5f;

	[SerializeField]
	private float _DashDuration = 0.1f;
	public float DashingCooldown = 1f;
	
	[Space(10)]
	[Header("Shooting")]

	[SerializeField]
	private GameObject _BulletPrefab;

	[SerializeField]
	private Transform _BulletSpawn;
	public float ShootingCooldown = 1f;
	public float PushBackForce = 1500f; 

	private GameObject _SpawnnedBullet;

	private int _CurJumps = 0;
    private DateTime _DashTimer;
	private DateTime _ShootTimer;

	private bool _DashAvailable = true;
	private bool _ShootAvailable = true;

	private float _XInput = 0f;
	private float _RTrigger = 0f;
	public bool _CanMove = true;

	public float Raylength = 1f;
	
	private void Start()
	{
	}

	private int _PushForceMult = 1;
	bool _ShootTrigger = false;

	private void Update()
	{
		GroundedTest();

		if(InputAble)
		{
			_XInput = Input.GetAxis("Horizontal");
		
			//if(Input.GetButtonDown("Jump") && _CurJumps < MaxJumps)
			if(Input.GetButtonDown("Jump"))
			{
				Jump(1f);
				_CurJumps++;
			}
		
			if(Input.GetButtonDown("Dash") && _DashAvailable)
			{
				Dash();
			}

			if(Input.GetButtonUp("Fire"))
			{
				Shoot(_BulletPrefab);

			}


			if (!_DashAvailable)
			{
				if((DateTime.Now - _DashTimer).Seconds >= DashingCooldown)
				{
					_DashAvailable = true;
				}
			}

			if (!_ShootAvailable)
			{
				if ((DateTime.Now - _ShootTimer).Milliseconds >= ShootingCooldown)
				{
					_ShootAvailable = true;
				}
			}

		}

	}
	
    private void FixedUpdate()
	{
		if(_CanMove)
		{
			Move(_XInput);
		}
	}

	public bool _Dashing = false;
	private void Dash()
	{
		if(_XInput == 0)
		{
			if(IsRotated)
			_XInput = -1;
			else
			_XInput = 1;
		}

		Move(_XInput, _DashMultiplier);
		_CanMove = false;
		_Dashing = true;
		Invoke("ResetCanMove", _DashDuration);

        _DashAvailable = false;
        _DashTimer = DateTime.Now;
	}

	private void ResetCanMove()
	{
		_CanMove = true;
		_Dashing = false;
	}

	private void Shoot(GameObject Bullet)
	{
		if(_ShootAvailable)
		{
			_SpawnnedBullet = Instantiate(Bullet, _BulletSpawn.position, _BulletSpawn.rotation);

			_ShootTimer = DateTime.Now;
			_ShootAvailable = false;
		}
	}
	
	private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
			RayCastJumpTest();
        }
    }

	private void ResetTimeScale()
	{
		Time.timeScale = 1;
	}

	private void RayCastJumpTest()
    {
        RaycastHit hit;
		Vector2 physicsCentre = this.transform.position + this.GetComponent<PolygonCollider2D>().transform.position;
		Vector2 physicsLeft = physicsCentre - new Vector2(0.3f, 0);
		Vector2 physicsRight = physicsCentre + new Vector2(0.3f, 0);

		if((Physics.Raycast(physicsLeft, Vector2.down, out hit, Raylength)) || (Physics.Raycast(physicsRight, Vector2.down, out hit, Raylength)))
		{
			if(hit.transform.gameObject.CompareTag("Platform"))
			{
				_CurJumps = 0;	
				//_Anim.SetBool("Grounded", true);
			}
			else
			{
				//_Anim.SetBool("Grounded", false);
			}
		}
		else
		{
			//_Anim.SetBool("Grounded", false);
		}
    }

	private void GroundedTest()
    {
        RaycastHit hit;
		Vector2 physicsCentre = this.transform.position + this.GetComponent<PolygonCollider2D>().transform.position;
		Vector2 physicsLeft = physicsCentre - new Vector2(0.3f, 0);
		Vector2 physicsRight = physicsCentre + new Vector2(0.3f, 0);

		if((Physics.Raycast(physicsLeft, Vector2.down, out hit, Raylength)) || (Physics.Raycast(physicsRight, Vector2.down, out hit, Raylength)))
		{
			if(hit.transform.gameObject.CompareTag("Platform"))
			{
				
				//_Anim.SetBool("Grounded", true);
			}
			else
			{
				//_Anim.SetBool("Grounded", false);
			}
		}
		else
		{
			//_Anim.SetBool("Grounded", false);
		}
    }


}