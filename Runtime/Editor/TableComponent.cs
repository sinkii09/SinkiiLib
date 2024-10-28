using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableComponent : MonoBehaviour
{
    //[TableList]
    //public List<PersonData> people = new List<PersonData>();

    [TableList]
    public PersonData[] people2;
}
[System.Serializable]
public class PersonData
{
    public string name;
    public int age;
    public float height;
    public bool isActive;
}