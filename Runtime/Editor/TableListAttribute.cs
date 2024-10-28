using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableListAttribute : PropertyAttribute
{
    public string[] Headers { get; private set; }
    public bool UseFieldNamesAsHeaders { get; private set; }

    public TableListAttribute(params string[] headers)
    {
        Headers = headers;
        UseFieldNamesAsHeaders = headers == null || headers.Length == 0;
    }

    public TableListAttribute()
    {
        UseFieldNamesAsHeaders = true;
    }
}