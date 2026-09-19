using System;
using UnityEngine;

public class Entity : MonoBehaviour
{
   [SerializeField] private Transform entitySystemContainer;
   public StateMachine StateMachine { get; private set; }
   public EntityRunTimeData EntityRunTimeData { get; private set; }
   
   public bool IsCanMove { get;  set; }

   private bool isError;
   
   private void Awake()
   {
      StateMachine = entitySystemContainer != null
         ? entitySystemContainer.GetComponentInChildren<StateMachine>()
         : GetComponentInChildren<StateMachine>();
   }

   public void InitializeEntity(EntityRunTimeData runTimeData)
   {
      EntityRunTimeData = runTimeData;
      EntityManager.Instance.RegisterEntity(this);
   }

   public void OnEntityUpdate(float deltaTime)
   {
      if (StateMachine == null)
      {
         if (!isError)
         {
            isError = true;
            Debug.LogError($"Entity {name} has no StateMachine!", this);
         }
         return;
      }
      
      StateMachine.UpdateStateMachine(deltaTime);
   }

   public virtual void OnMoveEntity(float deltaTime)
   {
      
   }

   public virtual void OnCheckIrresponsibleThinking(float deltaTime)
   {
      EntityRunTimeData.stateTimer += deltaTime;
   }
   
   public virtual void OnIncreaseIrresponsibleThinking(float deltaTime)
   {
      
   }

   public void DestroyEntity()
   {
      EntityManager.Instance.UnregisterEntity(this);
      Destroy(gameObject);
   }
}
