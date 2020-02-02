using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeManager : MonoBehaviour
{

    public enum Type {
        Fire,
        FireEx,
        Wrench,
        Detachable,
        Tape,
        Damageable
    }

    public Type type;
}
