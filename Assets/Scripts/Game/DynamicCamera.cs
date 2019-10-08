using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowGame
{
    public class DynamicCamera : MonoBehaviour
    {
        public Vector3 Offset;
        public float SmoothTime = 0.5f;

        private GameObject Player;

        private Vector3 _Velocity;

        void Start()
        {
            Player = GameObject.FindGameObjectWithTag(GameConsts.PLAYER_TAG);
        }

        void LateUpdate()
        {
            Move();
        }

        void Move()
        {
            if (Player == null)
                return;

            Vector3 CenterPoint = GetCenterPoint();
            Vector3 newPosition = CenterPoint + Offset;
            transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref _Velocity, SmoothTime);
        }

        private Vector3 GetCenterPoint()
        {
            return Player.transform.position;
        }
    }
}