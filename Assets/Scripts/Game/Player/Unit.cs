using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ArrowGame
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Unit : MonoBehaviour
    {
        public int TeamNumber;
        protected Rigidbody2D _RB;
        protected Animator _Anim;

        [Header("Movement")]
        [SerializeField]
        protected float _MoveSpeed = 8f;

        [Header("IGNORE")]
        public bool IsRotated = false;
        public bool InputAble = true;

        [Header("Camera Shake")]
        public float ShakeMagnitude = 5f;
        public float ShakeRoughness = 2f;
        public float ShakeFadeIn = 0.2f;
        public float ShakeFadeOut = 1f;

        protected void Awake()
        {
            _RB = GetComponent<Rigidbody2D>();
            _Anim = GetComponent<Animator>();
        }

        public virtual void Move(float dir, float mult = 1f)
        {
            if (dir < 0)
            {
                IsRotated = true;
                transform.localEulerAngles = new Vector3(0, 180, 0);
            }
            else if (dir > 0)
            {
                IsRotated = false;
                transform.localEulerAngles = new Vector3(0, 0, 0);
            }

            var vel = new Vector2(dir, 0) * _MoveSpeed * mult;
            vel.y = _RB.velocity.y;
            _RB.velocity = vel;
        }
    }
}