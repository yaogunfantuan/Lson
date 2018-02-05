using UnityEngine;
using System.Collections;
using System;

[AttributeUsageAttribute(AttributeTargets.Field|AttributeTargets.Property,AllowMultiple =false)]
public class LsonAttribute : Attribute {
    public String JsonKey { get; set; }
}
