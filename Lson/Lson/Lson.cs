using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using com.Lohanry;
namespace com.Lohanry.Lson {
    public enum INITTYPE {
        ALL, FIELD, PROPERTY
    }
    public class Lson {
        public static Lson instanceLson = new Lson();

        /// <summary>
        /// Parse json to object
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="adapter">Json Data Adapter</param>
        /// <param name="initType">Init FIELD or PROPERTY or All</param>
        /// <returns>object</returns>
        public static T FromJson<T>(LsonAdapter adapter, INITTYPE initType = INITTYPE.ALL) {
            T instance = System.Activator.CreateInstance<T>();
            FromJson(instance, adapter, initType);
            return instance;
        }
        /// <summary>
        /// Parse json to object
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="adapter">Json Data Adapter</param>
        /// <param name="initType">Init FIELD or PROPERTY or All</param>
        /// <returns>object</returns>
        public static T FormJsonByLsonAttribute<T>(LsonAdapter adapter, INITTYPE initType = INITTYPE.ALL) {
            T instance = System.Activator.CreateInstance<T>();
            FormJsonByLsonAttribute(instance, adapter, initType);
            return instance;
        }
        /// <summary>
        /// Parse json to object
        /// </summary>
        /// <param name="instance">object instance</param>
        /// <param name="adapter">Json Data Adapter</param>
        /// <param name="initType">Init FIELD or PROPERTY or All</param>
        /// <returns>object</returns>
        private static object FromJson(object instance, LsonAdapter adapter, INITTYPE initType = INITTYPE.ALL) {
            if (instance == null) {
                Debug.Log("Lson:Instance cant be null");
                return null;
            }
            Type type = instance.GetType();
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            switch (initType) {
                case INITTYPE.ALL:
                    SetValues(adapter, instance, fieldInfos);
                    SetValues(adapter, instance, propertyInfos);
                    break;
                case INITTYPE.FIELD:
                    SetValues(adapter, instance, fieldInfos);
                    break;
                case INITTYPE.PROPERTY:
                    SetValues(adapter, instance, propertyInfos);
                    break;
            }
            if (typeof(ILsonBean).IsAssignableFrom(type)) {
                type.GetMethod("AfterInit").Invoke(instance, null);
            }
            return instance;
        }
        /// <summary>
        /// Parse json to object
        /// </summary>
        /// <param name="instance">object instance</param>
        /// <param name="adapter">Json Data Adapter</param>
        /// <param name="initType">Init FIELD or PROPERTY or All</param>
        /// <returns>object</returns>
        public static object FormJsonByLsonAttribute(object instance, LsonAdapter adapter, INITTYPE initType = INITTYPE.ALL) {
            if (instance == null) {
                Debug.Log("Lson:Instance cant be null");
                return null;
            }
            Type type = instance.GetType();
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            switch (initType) {
                case INITTYPE.ALL:
                    SetValues(adapter, instance, fieldInfos, true);
                    SetValues(adapter, instance, propertyInfos, true);
                    break;
                case INITTYPE.FIELD:
                    SetValues(adapter, instance, fieldInfos, true);
                    break;
                case INITTYPE.PROPERTY:
                    SetValues(adapter, instance, propertyInfos, true);
                    break;
            }
            if (typeof(ILsonBean).IsAssignableFrom(type)) {
                type.GetMethod("AfterInit").Invoke(instance, null);
            }
            return instance;
        }
        /// <summary>
        /// Set object value
        /// </summary>
        /// <param name="instance">object instance</param>
        /// <param name="adapter">Json Data Adapter</param>
        /// <param name="propertyInfos">propertyInfos of object</param>
        /// <param name="isOnlyLsonAttribute">Is only check LsonAttribute</param>
        private static void SetValues(LsonAdapter adapter, object instance, PropertyInfo[] propertyInfos, bool isOnlyLsonAttribute = false) {
            for (int i = 0, j = propertyInfos.Length; i < j; i++) {
                PropertyInfo propertyInfo = propertyInfos[i];
                //false不查找继承链
                object[] attributes = propertyInfo.GetCustomAttributes(false);
                LsonAttribute lsonAttribute = GetLsonAttribute(attributes);
                String jsonKey = propertyInfo.Name;
                if (lsonAttribute != null) {
                    jsonKey = lsonAttribute.JsonKey;
                } else if (isOnlyLsonAttribute) {
                    continue;
                }
                if (!adapter.Contains(jsonKey)) continue;
                object value = adapter.GetValue(jsonKey,propertyInfo.PropertyType);
                if (adapter.GetValueJsonType(jsonKey) == JsonType.Object) {
                    object _instance = propertyInfo.GetValue(instance, null);
                    if (isOnlyLsonAttribute) {
                        value = FormJsonByLsonAttribute(_instance, (LsonAdapter)value);
                    } else {
                        value = FromJson(_instance, (LsonAdapter)value);
                    }
                    instanceLson.SetValue(instance, propertyInfo, value);
                }
                instanceLson.SetValue(instance, propertyInfo, value);
            }
        }
        /// <summary>
        /// Set object value
        /// </summary>
        /// <param name="instance">object instance</param>
        /// <param name="adapter">Json Data Adapter</param>
        /// <param name="fieldInfos">fieldInfos of object</param>
        /// <param name="isOnlyLsonAttribute">Is only check LsonAttribute</param>
        public static void SetValues(LsonAdapter adapter, object instance, FieldInfo[] fieldInfos, bool isOnlyLsonAttribute = false) {
            for (int i = 0, j = fieldInfos.Length; i < j; i++) {
                FieldInfo fieldInfo = fieldInfos[i];
                //false不查找继承链
                object[] attributes = fieldInfo.GetCustomAttributes(false);
                LsonAttribute lsonAttribute = GetLsonAttribute(attributes);
                String jsonKey = fieldInfo.Name;
                if (lsonAttribute != null) {
                    jsonKey = lsonAttribute.JsonKey;
                } else if (isOnlyLsonAttribute) {
                    continue;
                }
                if (!adapter.Contains(jsonKey)) continue;
                object value = adapter.GetValue(jsonKey,fieldInfo.FieldType);
                if (adapter.GetValueJsonType(jsonKey) == JsonType.Object) {
                    object _instance = fieldInfo.GetValue(instance);
                    if (isOnlyLsonAttribute) {
                        value = FormJsonByLsonAttribute(_instance, (LsonAdapter)value);
                    } else {
                        value = FromJson(_instance, (LsonAdapter)value);
                    }
                }
                instanceLson.SetValue(instance, fieldInfo, value);
            }
        }

        public virtual void SetValue(object instance, FieldInfo fieldInfo, object value) {
            fieldInfo.SetValue(instance, value);
        }

        public virtual void SetValue(object instance, PropertyInfo propertyInfo, object value) {
            propertyInfo.SetValue(instance, value, null);
        }
        /// <summary>
        /// GetLsonAttribute from Object
        /// </summary>
        /// <param name="attributes">CustomAttributes of object</param>
        /// <returns></returns>
        private static LsonAttribute GetLsonAttribute(object[] attributes) {
            LsonAttribute lsonAttribute = null;
            for (int i = 0, j = attributes.Length; i < j; i++) {
                if (attributes[i].GetType() == typeof(LsonAttribute)) {
                    lsonAttribute = (LsonAttribute)attributes[i];
                    break;
                }
            }
            return lsonAttribute;
        }
    }
}
