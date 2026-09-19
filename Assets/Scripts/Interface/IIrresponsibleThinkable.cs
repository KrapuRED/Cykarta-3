using UnityEngine;

public interface IIrresponsibleThinkable
{
    public IrresponsibleThinkingData IrresponsibleThinkingData { get; set; }
    public float ThinkingCheckTimer { get; set; }

    public void CheckIrresponsibleThinking(float deltaTime)
    {
        
    }

    public void IncreaseIrresponsibleThinking(float deltaTime)
    {
        
    }
}
