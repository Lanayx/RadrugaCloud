namespace Core.Tools.CopyHelper
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Class ObjectExtensions
    /// </summary>
    public static class ObjectExtensions
    {
        #region Static Fields

        /// <summary>
        /// The clone method.
        /// </summary>
        private static readonly MethodInfo CloneMethod = typeof(object).GetMethod(
            "MemberwiseClone",
            BindingFlags.NonPublic | BindingFlags.Instance);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The copy.
        /// </summary>
        /// <param name="original">
        /// The original.
        /// </param>
        /// <typeparam name="T">
        /// Type of the copied object
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public static T Copy<T>(this T original)
        {
            return (T)Copy((object)original);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The copy.
        /// </summary>
        /// <param name="originalObject">
        /// The original object.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private static object Copy(this object originalObject)
        {
            return InternalCopy(originalObject, new Dictionary<object, object>(new ReferenceEqualityComparer()));
        }

        /// <summary>
        /// The copy fields.
        /// </summary>
        /// <param name="originalObject">
        /// The original object.
        /// </param>
        /// <param name="visited">
        /// The visited.
        /// </param>
        /// <param name="cloneObject">
        /// The clone object.
        /// </param>
        /// <param name="typeToReflect">
        /// The type to reflect.
        /// </param>
        /// <param name="bindingFlags">
        /// The binding flags.
        /// </param>
        /// <param name="filter">
        /// The filter.
        /// </param>
        private static void CopyFields(
            object originalObject,
            IDictionary<object, object> visited,
            object cloneObject,
            Type typeToReflect,
            BindingFlags bindingFlags =
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy,
            Func<FieldInfo, bool> filter = null)
        {
            foreach (FieldInfo fieldInfo in typeToReflect.GetFields(bindingFlags))
            {
                if (filter != null && filter(fieldInfo) == false)
                {
                    continue;
                }

                if (IsPrimitive(fieldInfo.FieldType))
                {
                    continue;
                }

                var originalFieldValue = fieldInfo.GetValue(originalObject);
                var clonedFieldValue = InternalCopy(originalFieldValue, visited);
                fieldInfo.SetValue(cloneObject, clonedFieldValue);
            }
        }

        /// <summary>
        /// The internal copy.
        /// </summary>
        /// <param name="originalObject">
        /// The original object.
        /// </param>
        /// <param name="visited">
        /// The visited.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private static object InternalCopy(object originalObject, IDictionary<object, object> visited)
        {
            if (originalObject == null)
            {
                return null;
            }

            var typeToReflect = originalObject.GetType();
            if (IsPrimitive(typeToReflect))
            {
                return originalObject;
            }

            if (visited.ContainsKey(originalObject))
            {
                return visited[originalObject];
            }

            if (typeof(Delegate).IsAssignableFrom(typeToReflect))
            {
                return null;
            }

            var cloneObject = CloneMethod.Invoke(originalObject, null);
            if (typeToReflect.IsArray)
            {
                var arrayType = typeToReflect.GetElementType();
                if (IsPrimitive(arrayType) == false)
                {
                    var clonedArray = (Array)cloneObject;
                    clonedArray.ForEach(
                        (array, indices) =>
                        array.SetValue(InternalCopy(clonedArray.GetValue(indices), visited), indices));
                }
            }

            visited.Add(originalObject, cloneObject);
            CopyFields(originalObject, visited, cloneObject, typeToReflect);
            RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);
            return cloneObject;
        }

        /// <summary>
        /// The is primitive.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool IsPrimitive(this Type type)
        {
            if (type == typeof(string))
            {
                return true;
            }

            return type.IsValueType & type.IsPrimitive;
        }

        /// <summary>
        /// The recursive copy base type private fields.
        /// </summary>
        /// <param name="originalObject">
        /// The original object.
        /// </param>
        /// <param name="visited">
        /// The visited.
        /// </param>
        /// <param name="cloneObject">
        /// The clone object.
        /// </param>
        /// <param name="typeToReflect">
        /// The type to reflect.
        /// </param>
        private static void RecursiveCopyBaseTypePrivateFields(
            object originalObject,
            IDictionary<object, object> visited,
            object cloneObject,
            Type typeToReflect)
        {
            if (typeToReflect.BaseType != null)
            {
                RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.BaseType);
                CopyFields(
                    originalObject,
                    visited,
                    cloneObject,
                    typeToReflect.BaseType,
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    info => info.IsPrivate);
            }
        }

        #endregion
    }
}