using System;
using UnityEngine;

public class Entity : MonoBehaviour
{
   [SerializeField] private Transform entitySystemContainer;
   public StateMachine StateMachine { get; private set; }

   public EntityRunTimeData RunTimeData { get; private set; }
   public bool IsCanMove { get;  set; }
   
   private void Awake()
   {
      StateMachine = entitySystemContainer.GetComponent<StateMachine>();
   }

   public void InitializeEntity(EntityRunTimeData runTimeData)
   {
      RunTimeData = runTimeData;
      EntityManager.Instance.RegisterEntity(this);
   }

   public virtual void OnEntityUpdate(float deltaTime)
   {
      StateMachine.UpdateStateMachine(deltaTime);
   }

   public virtual void OnMoveEntity(float deltaTime)
   {
      
   }

   public void DestroyEntity()
   {
      EntityManager.Instance.UnregisterEntity(this);
      Destroy(gameObject);
   }
}
