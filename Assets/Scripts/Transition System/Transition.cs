using UnityEngine;
using System.Collections;

public abstract class Transition : MonoBehaviour
{
    public abstract IEnumerator TransitionIn();
    public abstract IEnumerator TransitionOut();
}