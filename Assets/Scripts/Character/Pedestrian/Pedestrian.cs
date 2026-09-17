using UnityEngine;

public class Pedestrian : Character
{
    [SerializeField] private float irresponsibleThinkingMeterMaxValue;
    [SerializeField] private float currentIrresponsibleThinkingMeter;
    [SerializeField] private float irresponsibleThinkingIncreaseRate;
}
