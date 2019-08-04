using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CustomGravity : MonoBehaviour
{

	[SerializeField]
	private float _GravityMultiplier;

	private Rigidbody _RB;

	private void Awake()
	{
		_RB = GetComponent<Rigidbody>();
		_RB.useGravity = false;
	}

	private void FixedUpdate()
	{
		var newVel = _RB.velocity;
		newVel.y += Physics.gravity.y * _GravityMultiplier * Time.fixedDeltaTime;
		_RB.velocity = newVel;
	}

}
