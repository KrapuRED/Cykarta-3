using UnityEngine;

[CreateAssetMenu(fileName = "IsDetectingArea", menuName = "State Machine/Condition/IsDetectingAreaCondition")]
public class IsDetectingArea : ConditionSO
{
   public override bool CheckCondition(Entity entity)
   {
      var tower = entity.GetComponent<Tower>();
      
      return tower.TowerRunTimeData.isTowerUnLockToTarget;
   }
}
