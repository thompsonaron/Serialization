using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.IO;
using System.Text;

public class Generator
{
    [MenuItem("Generator/Generate)")]
    public static void Generate()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using System.IO;");
        sb.AppendLine(Environment.NewLine);
        sb.AppendLine("public class Serializator");
        sb.AppendLine("{");

        var types = System.AppDomain.CurrentDomain.GetAssemblies();
        // getting all assemblies
        foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            // narrowing to desired assemblies
            if (assembly.FullName.Contains("CSharp") && !assembly.FullName.Contains("Editor"))
            {
                // alll classes in assembly
                var assemblyClasses = assembly.GetTypes();
                foreach (var classType in assemblyClasses)
                {
                    sb.AppendLine("byte[] serialize(" + classType.Name + " " + classType.Name.ToLower() + ")");
                    sb.AppendLine("{");
                    sb.AppendLine("var s = new MemoryStream();");
                    sb.AppendLine("var bW = new BinaryWriter(s);");

                    // getting all attributes in class
                    Attribute[] attrs = System.Attribute.GetCustomAttributes(classType);
                    for (int i = 0; i < attrs.Length; i++)
                    {
                        //if attribute is [Serializable] => do stuff
                        if (attrs[i] is SerializableAttribute)
                        {
                            // will get all types of fields with binding flags - without it, it will ONLY get PUBLIC fields
                            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
                            var fields = classType.GetFields(bindingFlags);
                            foreach (var field in classType.GetFields())
                            {
                                if (field.FieldType.IsList())
                                {
                                    bool existsInAssembly = ExistsInAssembly(assemblyClasses, field, CollectionType.List);

                                    sb.AppendLine("bW.Write(" + classType.Name.ToLower() + "." + field.Name + ".Count);");
                                    sb.AppendLine("foreach (var item in " + classType.Name.ToLower() + "." + field.Name + ")");
                                    sb.AppendLine("{");
                                    if (existsInAssembly) { sb.AppendLine("bW.Write(serialize(item));"); }
                                    else { sb.AppendLine("bW.Write(item);"); }
                                    sb.AppendLine("}");
                                }
                                else if (field.FieldType.IsArray)
                                {
                                    bool existsInAssembly = ExistsInAssembly(assemblyClasses, field, CollectionType.Array);

                                    // Array Length
                                    sb.AppendLine("bW.Write(" + classType.Name.ToLower() + "." + field.Name + ".Length);");
                                    sb.AppendLine("foreach (var item in " + classType.Name.ToLower() + "." + field.Name + ")");
                                    sb.AppendLine("{");
                                    if (existsInAssembly) { sb.AppendLine("bW.Write(serialize(item));"); }
                                    else { sb.AppendLine("bW.Write(item);"); }
                                    sb.AppendLine("}");
                                }
                                else
                                {
                                    bool existsInAssembly = ExistsInAssembly(assemblyClasses, field, CollectionType.None);

                                    if (existsInAssembly) { sb.AppendLine("bW.Write(serialize(" + classType.Name.ToLower() + "." + field.Name + "));"); }
                                    else { sb.AppendLine("bW.Write(" + classType.Name.ToLower() + "." + field.Name + ");"); }
                                }
                            }
                        }
                    }
                    sb.AppendLine("return s.ToArray();");
                    sb.AppendLine("}");
                }
            }
        }
        sb.AppendLine("}");
        File.WriteAllText(Application.dataPath + "/Serializator.cs", sb.ToString());
    }

    private static bool ExistsInAssembly(Type[] assemblyClasses, FieldInfo field, CollectionType collectionType)
    {
        bool existsInAssembly = false;
        string fieldClass = field.FieldType.ToString();
        switch (collectionType)
        {
            case CollectionType.Array:
                fieldClass = fieldClass.Substring(0, fieldClass.Length - 2);
                break;
            case CollectionType.List:
                int start = fieldClass.LastIndexOf("[") + 1;
                int length = fieldClass.Length - start - 1;
                fieldClass = fieldClass.Substring(start, length);
                break;
            case CollectionType.None:

                break;
            default:
                break;
        }
        foreach (var item in assemblyClasses)
        {
            if (item.Name == fieldClass)
            {
                existsInAssembly = true;
            }
        }

        return existsInAssembly;
    }
}

public static class Extensions
{
    public static bool IsList(this Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        //return type == typeof(List<>);
    }
}

enum CollectionType
{
    Array, List, None
}
