using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowGame
{
    public class AbilityTrayUI : MonoBehaviour
    {
        [SerializeField] private GameObject trayItemPref;

        private Dictionary<PlayerAbilityType, AbilityTrayItem> trayItems = new Dictionary<PlayerAbilityType, AbilityTrayItem>();

        public void AddNewAbility(PlayerAbilityType abilityType)
        {
            AbilityTrayItem trayItem = Instantiate(trayItemPref, transform).GetComponent<AbilityTrayItem>();
            trayItem.transform.localScale = Vector3.one;
            //TODO : Assign the relevant sprite to the item

            trayItem.Initialize(this);
            trayItems.Add(abilityType, trayItem);
        }

        public void AbilityActivated(PlayerAbilityType abilityType, float duration)
        {
            trayItems[abilityType].StartTimer(duration);
        }

        public void RemoveAbility(PlayerAbilityType abilityType)
        {
            Destroy(trayItems[abilityType]);
            trayItems.Remove(abilityType);
        }
    }
}