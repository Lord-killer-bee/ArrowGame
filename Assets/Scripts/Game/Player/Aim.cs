using UnityEngine;
using System.Collections;

namespace ArrowGame
{
    public class Aim : MonoBehaviour
    {
        Player player;
        [SerializeField]
        private GameObject Gun;

        private bool _PointRight = true;
        public float radius = 0.7f;

        private bool InputAble = true;

        private void Start()
        {
            player = GetComponent<Player>();
        }

        private void Update()
        {
            if (InputAble)
            {
                float xInput = player.directionalInput.x;
                float yInput = player.directionalInput.y;

                if (xInput < 0)
                {
                    _PointRight = false;
                }
                else
                {
                    _PointRight = true;
                }

                if(xInput == 0 && yInput == 0)
                {
                    xInput = player.controller.collisions.faceDir;
                }

                Gun.transform.localPosition = new Vector3(xInput * radius, yInput * radius, 0);
                float atan2 = Mathf.Atan2(Gun.transform.localPosition.y, Gun.transform.localPosition.x);

                if (_PointRight)
                    Gun.transform.rotation = Quaternion.Euler(0f, 0f, atan2 * Mathf.Rad2Deg - 90f);
                else
                    Gun.transform.rotation = Quaternion.Euler(0f, 0, atan2 * Mathf.Rad2Deg - 90f);
            }
        }
    }
}