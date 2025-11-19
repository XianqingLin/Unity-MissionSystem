using UnityEngine;

namespace Gameplay.MissionSystem
{
    [System.Serializable]
    public class ItemMissionReward : MissionReward
    {
        [SerializeField] private string itemName; 
        [SerializeField] private int amount;
        [SerializeField] private string param;

        public ItemMissionReward()
        {
            itemName = string.Empty;
            amount = 0;
            param = string.Empty;
        }

        public ItemMissionReward(string itemName, int amount, string param)
        {
            this.itemName = itemName;
            this.amount = amount;
            this.param = param;
        }

        public override void ApplyReward()
        {
            Debug.Log($"给予 {amount} 个 {itemName}");
        }

#if UNITY_EDITOR
        public string ItemName { get => itemName; set => itemName = value; }
        public int Amount { get => amount; set => amount = value; }
        public string Param { get => param; set => param = value; }
#endif
    }
}
