using UnityEngine;
using System.Collections;

namespace ArrowGame
{
    public class Aim : MonoBehaviour
    {

        [SerializeField]
        private GameObject Gun;

        private bool _PointRight = true;
        public float radius = 0.7f;

        private bool InputAble = true;

        private void Update()
        {
            if (InputAble)
            {
                float xInput = Input.GetAxisRaw(GameConsts.HORIZONTAL_CODE);
                float yInput = Input.GetAxisRaw(GameConsts.VERTICAL_CODE);

                if (GetComponent<PlayerController>().isRotated)
                {
                    _PointRight = false;
                }
                else
                {
                    _PointRight = true;
                }

                if(xInput == 0 && yInput == 0)
                {
                    xInput = 1;
                }

                Gun.transform.localPosition = new Vector3(Mathf.Abs(xInput) * radius, yInput * radius, 0);
                float atan2 = Mathf.Atan2(Gun.transform.localPosition.y, Gun.transform.localPosition.x);

                if (_PointRight)
                    Gun.transform.rotation = Quaternion.Euler(0f, 0f, atan2 * Mathf.Rad2Deg - 90f);
                else
                    Gun.transform.rotation = Quaternion.Euler(0f, 180f, atan2 * Mathf.Rad2Deg - 90f);
            }
        }
    }
}