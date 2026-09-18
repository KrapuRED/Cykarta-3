using UnityEngine;

public class Entity : MonoBehaviour
{
   public EntityRunTimeData runTimeData { get; private set; }

   public void InitializeEntity(EntityRunTimeData runTimeData)
   {
      
   }

   public void DestroyEntity()
   {
      EntityManager.Instance.UnregisterEntity(this);
      Destroy(gameObject);
   }
}
