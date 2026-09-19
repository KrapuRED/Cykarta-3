using System.Collections.Generic;
using UnityEngine;

public class TowerCardHandler : MonoBehaviour
{
    [SerializeField] private List<TowerDataSO> towers = new();
    [SerializeField] private TowerCardUI prefTowerCardUI;
    [SerializeField] private Transform containerTowerCard;
    
    private void Start()
    {
        foreach (TowerDataSO towerData in towers)
        {
            var cardUI = Instantiate(prefTowerCardUI, containerTowerCard);
            cardUI.name = $"Tower Card UI - {towerData.towerName}";
            cardUI.SetTowerCardUI(towerData);
        }
    }
}
