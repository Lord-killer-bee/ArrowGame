using UnityEngine;
using System.Collections;

namespace ArrowGame
{
    [RequireComponent(typeof(Player))]
    public class PlayerInput : MonoBehaviour
    {

        Player player;

        void Start()
        {
            player = GetComponent<Player>();
        }

        void Update()
        {
            Vector2 directionalInput = new Vector2(Input.GetAxisRaw(GameConsts.HORIZONTAL_CODE), Input.GetAxisRaw(GameConsts.VERTICAL_CODE));
            player.SetDirectionalInput(directionalInput);

            if (Input.GetButtonDown(GameConsts.JUMP_CODE))
            {
                player.OnJumpInputDown();
            }
            if (Input.GetButtonUp(GameConsts.JUMP_CODE))
            {
                player.OnJumpInputUp();
            }
            if (Input.GetButtonDown(GameConsts.FIRE_CODE))
            {
                player.Shoot();
            }
        }
    }
}