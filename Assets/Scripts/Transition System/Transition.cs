using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

public abstract class Transition : MonoBehaviour
{
    public abstract IEnumerator TransitionIn();
    public abstract IEnumerator TransitionOut();
}