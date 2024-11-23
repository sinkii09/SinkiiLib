using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomToggleGroupAttribute : Attribute
{
    public string Name { get; private set; }
    public bool expandByDefault { get; private set; }   
    public CustomToggleGroupAttribute(string name)
    {
        Name = name;
        expandByDefault = true;
    }
}
