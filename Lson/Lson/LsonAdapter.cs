using UnityEngine;
using System.Collections;
using System;

namespace com.Lohanry.Lson {
    public enum JsonType {
        None = 0,
        Object = 1,
        Array = 2,
        String = 3,
        Int = 4,
        Long = 5,
        Double = 6,
        Boolean = 7
    }

    public abstract class LsonAdapter {
        public abstract bool Contains(string key);
        public abstract int GetCount();
        public abstract object GetValue(string key, Type fieldType);
        public abstract JsonType GetValueJsonType(string key);
    }
}
