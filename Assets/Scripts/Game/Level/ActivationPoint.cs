using ArrowGame.InGameEvents;
using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowGame
{
    public class ActivationPoint : MonoBehaviour
    {
        [SerializeField] private float checkRadius = 6f;
        [SerializeField] private GameObject enableSprite;
        [SerializeField] private GameObject disableSprite;
        
        Collider2D overlapCollider;

        private void Update()
        {
            CheckForPlayerInRange();
        }

        private void CheckForPlayerInRange()
        {
            overlapCollider = Physics2D.OverlapCircle(transform.position, checkRadius);

            if (overlapCollider == null)
            {
                ToggleActivationPoint(false);
                return;
            }

            if (overlapCollider.tag == GameConsts.PLAYER_TAG)
            {
                ToggleActivationPoint(true);
            }
            else
            {
                ToggleActivationPoint(false);
            }

        }

        private void ToggleActivationPoint(bool enable)
        {
            GameEventManager.Instance.TriggerSyncEvent(new ActivationPointInRangeEvent(this, enable));

            enableSprite.SetActive(enable);
            disableSprite.SetActive(!enable);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, checkRadius);
        }

        public void ActivatePoint()
        {
            //TODO :: Activate the stored monster
        }

    }
}