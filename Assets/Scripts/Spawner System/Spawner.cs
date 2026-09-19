using UnityEngine;

public class Spawner : MonoBehaviour
{
   [SerializeField] protected string spawnerID;
   [SerializeField] protected GridZone spawnZone;
   
   public virtual void StartSpawner()
   {
      
   }
   
   public virtual void OnSpawning()
   {
      
   }
}
