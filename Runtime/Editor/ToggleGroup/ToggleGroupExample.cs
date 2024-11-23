using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleGroupExample : MonoBehaviour
{
    [SerializeField]
    [CustomToggleGroup("GroupA")]
    public bool GroupA;

    [SerializeField]
    [CustomToggleGroup("GroupA")]
    public int a;

    [SerializeField]
    [CustomToggleGroup("GroupA")]
    public int b;

    [SerializeField]
    [CustomToggleGroup("GroupB")]
    public MyStruct myStruct;
}
[System.Serializable]
public class MyStruct
{
    public int someInt;
    public string someString;
    public AnotherStruct anotherStruct;
}

[System.Serializable]
public class AnotherStruct
{
    public float someFloat;
    public bool someBool;
}