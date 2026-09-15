using TMPro;
using UnityEngine;

public class TowerCardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text towerName;
    [SerializeField] private TMP_Text towerCost;

    public void SetTowerCardUI(string towerName, int  cost)
    {
        this.towerName.text = towerName;
        this.towerCost.text = $"$ {cost}";
    }
}
