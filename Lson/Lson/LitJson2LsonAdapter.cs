using UnityEngine;
using System.Collections;
using com.Lohanry.Lson;
using System;

public class LitJson2LsonAdapter : LsonAdapter {
    private LitJson.JsonData jsonData;

    public LitJson2LsonAdapter(LitJson.JsonData jsonData) {
        this.jsonData = jsonData;
    }

    public override bool Contains(string key) {
        return jsonData.Contains(key);

    }

    public override int GetCount() {
        return jsonData.Count;
    }

    public override object GetValue(string key, Type fieldType) {
        switch (jsonData[key].GetJsonType()) {
            case LitJson.JsonType.Object:
                return new LitJson2LsonAdapter(jsonData[key]);
            case LitJson.JsonType.String:
                if (fieldType == typeof(int)) {
                    return jsonData[key].Int;
                } else if (fieldType == typeof(long)) {
                    return jsonData[key].Long;
                } else if (fieldType == typeof(double)) {
                    return jsonData[key].Double;
                } else {
                    return jsonData[key].String;
                }
            case LitJson.JsonType.Boolean:
                return jsonData[key].Bool;
            case LitJson.JsonType.Double:
                return jsonData[key].Double;
            case LitJson.JsonType.Int:
                return jsonData[key].Int;
            case LitJson.JsonType.Long:
                return jsonData[key].Long;
            default: return null;
        }
    }

    public override JsonType GetValueJsonType(string key) {
        return (JsonType)Enum.ToObject(typeof(JsonType), (int)jsonData[key].GetJsonType());
    }
    
}
