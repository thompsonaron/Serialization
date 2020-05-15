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
        var types = System.AppDomain.CurrentDomain.GetAssemblies();

        // serializator
        StringBuilder sbSer = new StringBuilder();
        sbSer.AppendLine("using System.Collections.Generic;");
        sbSer.AppendLine("using System.IO;");
        sbSer.AppendLine("using System;");
        sbSer.AppendLine(Environment.NewLine);
        sbSer.AppendLine("public class Serializator");
        sbSer.AppendLine("{");


        // deserializator
        StringBuilder sbDeser = new StringBuilder();

        // deserializator ref
        StringBuilder sbDeserRef = new StringBuilder();

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

                    // getting all attributes in class
                    Attribute[] attrs = System.Attribute.GetCustomAttributes(classType);
                    for (int i = 0; i < attrs.Length; i++)
                    {
                        //if attribute is [Serializable] => do stuff
                        if (attrs[i] is SerializableAttribute)
                        {
                            // serializator
                            sbSer.AppendLine("public byte[] serialize(" + classType.Name + " " + classType.Name.ToLower() + ")");
                            sbSer.AppendLine("{");
                            sbSer.AppendLine("var s = new MemoryStream();");
                            sbSer.AppendLine("var bW = new BinaryWriter(s);");


                            //deserializator
                            sbDeser.AppendLine();
                            sbDeser.AppendLine("public " + classType.Name + " Deserialize" + classType.Name + " (byte[] b)");
                            sbDeser.AppendLine("{");
                            sbDeser.AppendLine("var s = new MemoryStream(b);");
                            sbDeser.AppendLine("var bR = new BinaryReader(s);");
                            sbDeser.AppendLine("var obj = new " + classType.Name + "();");

                            // deserializator ref
                            sbDeserRef.AppendLine();
                            sbDeserRef.AppendLine("public " + classType.Name + " Deserialize" + classType.Name + " (ref byte[] b, ref MemoryStream s, ref BinaryReader bR)");
                            sbDeserRef.AppendLine("{");
                            sbDeserRef.AppendLine("var obj = new " + classType.Name + "();");
                            Debug.Log(classType.Name);
                            // will get all types of fields with binding flags - without it, it will ONLY get PUBLIC fields
                            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
                            var fields = classType.GetFields(bindingFlags);
                            foreach (var field in classType.GetFields())
                            {
                                if (field.FieldType.IsList())
                                {
                                    // meaning if it is class from this project (editor clases are excluded above)
                                    bool existsInAssembly = ExistsInAssembly(assemblyClasses, field, CollectionType.List);

                                    // serializator
                                    sbSer.AppendLine("bW.Write(" + classType.Name.ToLower() + "." + field.Name + ".Count);");
                                    sbSer.AppendLine("foreach (var item in " + classType.Name.ToLower() + "." + field.Name + ")");
                                    sbSer.AppendLine("{");
                                    if (existsInAssembly) { sbSer.AppendLine("bW.Write(serialize(item));"); }
                                    else { sbSer.AppendLine("bW.Write(item);"); }
                                    sbSer.AppendLine("}");

                                    // deserializator
                                    string[] classNameSplit = field.FieldType.ToString().Split('[', ']');
                                    string[] className = classNameSplit[1].Split('.');

                                    if (className.Length > 1)
                                    {
                                        sbDeser.AppendLine("obj." + field.Name + " = new List<" + className[1] + ">();");
                                        sbDeserRef.AppendLine("obj." + field.Name + " = new List<" + className[1] + ">();");
                                    }
                                    else
                                    {
                                        sbDeser.AppendLine("obj." + field.Name + " = new List<" + className[0] + ">();");
                                        sbDeserRef.AppendLine("obj." + field.Name + " = new List<" + className[0] + ">();");
                                    }
                                    sbDeser.AppendLine("int " + field.Name + "ListSize = bR.ReadInt32();");
                                    sbDeserRef.AppendLine("int " + field.Name + "ListSize = bR.ReadInt32();");

                                    sbDeser.AppendLine("for (int i = 0; i < " + field.Name + "ListSize; i++)");
                                    sbDeserRef.AppendLine("for (int i = 0; i < " + field.Name + "ListSize; i++)");
                                    sbDeser.AppendLine("{");
                                    sbDeserRef.AppendLine("{");
                                    // meaning that field is "primitive"
                                    if (className.Length > 1)
                                    {
                                        sbDeser.AppendLine("obj." + field.Name + ".Add(bR.Read" + className[1] + "());");
                                        sbDeserRef.AppendLine("obj." + field.Name + ".Add(bR.Read" + className[1] + "());");
                                    }
                                    // meaning it is a class
                                    else
                                    {
                                        sbDeser.AppendLine("obj." + field.Name + ".Add(Deserialize" + className[0] + "(ref b, ref s,ref bR));");
                                        sbDeserRef.AppendLine("obj." + field.Name + ".Add(Deserialize" + className[0] + "(ref b, ref s,ref bR));");
                                    }

                                    sbDeser.AppendLine("}");
                                    sbDeserRef.AppendLine("}");

                                    // deserializator reference

                                }
                                else if (field.FieldType.IsArray)
                                {
                                    bool existsInAssembly = ExistsInAssembly(assemblyClasses, field, CollectionType.Array);

                                    // serializator
                                    // Array Length
                                    sbSer.AppendLine("bW.Write(" + classType.Name.ToLower() + "." + field.Name + ".Length);");
                                    sbSer.AppendLine("foreach (var item in " + classType.Name.ToLower() + "." + field.Name + ")");
                                    sbSer.AppendLine("{");
                                    if (existsInAssembly) { sbSer.AppendLine("bW.Write(serialize(item));"); }
                                    else { sbSer.AppendLine("bW.Write(item);"); }
                                    sbSer.AppendLine("}");

                                    // deserializator
                                    string[] classNameSplit = field.FieldType.ToString().Split('[');
                                    string[] className = classNameSplit[0].Split('.');
                                    sbDeser.AppendLine("int " + field.Name + "ArraySize = bR.ReadInt32();");
                                    sbDeserRef.AppendLine("int " + field.Name + "ArraySize = bR.ReadInt32();");
                                    if (className.Length > 1)
                                    {
                                        sbDeser.AppendLine("obj." + field.Name + " = new " + className[1] + "[" + field.Name + "ArraySize];");
                                        sbDeserRef.AppendLine("obj." + field.Name + " = new " + className[1] + "[" + field.Name + "ArraySize];");
                                    }
                                    else
                                    {
                                        sbDeser.AppendLine("obj." + field.Name + " = new " + className[0] + "[" + field.Name + "ArraySize];");
                                        sbDeserRef.AppendLine("obj." + field.Name + " = new " + className[0] + "[" + field.Name + "ArraySize];");
                                    }

                                    sbDeser.AppendLine("for (int i = 0; i <" + field.Name + "ArraySize; i++)");
                                    sbDeserRef.AppendLine("for (int i = 0; i <" + field.Name + "ArraySize; i++)");
                                    sbDeser.AppendLine("{");
                                    sbDeserRef.AppendLine("{");
                                    // "primitive" class
                                    if (className.Length > 1)
                                    {
                                        sbDeser.AppendLine("obj." + field.Name + "[i] = bR.Read" + className[1] + "();");
                                        sbDeserRef.AppendLine("obj." + field.Name + "[i] = bR.Read" + className[1] + "();");
                                    }
                                    // a class
                                    else
                                    {
                                        sbDeser.AppendLine("obj." + field.Name + "[i] = Deserialize" + className[0] + "(ref b, ref s, ref bR);");
                                        sbDeserRef.AppendLine("obj." + field.Name + "[i] = Deserialize" + className[0] + "(ref b, ref s, ref bR);");
                                    }
                                    sbDeser.AppendLine("}");
                                    sbDeserRef.AppendLine("}");

                                }
                                else
                                {
                                    bool existsInAssembly = ExistsInAssembly(assemblyClasses, field, CollectionType.None);

                                    // serializator
                                    if (existsInAssembly) { sbSer.AppendLine("bW.Write(serialize(" + classType.Name.ToLower() + "." + field.Name + "));"); }
                                    else { sbSer.AppendLine("bW.Write(" + classType.Name.ToLower() + "." + field.Name + ");"); }

                                    // deserializator
                                    // TODO might need reference ==> "(ref b)" instead of (b)
                                    if (existsInAssembly)
                                    {
                                        sbDeser.AppendLine("obj." + field.Name + " = Deserialize" + field.FieldType + "(ref b, ref s, ref bR);");
                                        sbDeserRef.AppendLine("obj." + field.Name + " = Deserialize" + field.FieldType + "(ref b, ref s, ref bR);");
                                    }
                                    else
                                    {
                                        string[] fieldTypeSplit = field.FieldType.ToString().Split('.');
                                        sbDeser.AppendLine("obj." + field.Name + " = bR.Read" + fieldTypeSplit[1] + "();");
                                        sbDeserRef.AppendLine("obj." + field.Name + " = bR.Read" + fieldTypeSplit[1] + "();");
                                    }
                                }
                            }
                            // serializator
                            sbSer.AppendLine("return s.ToArray();");
                            sbSer.AppendLine("}");

                            // deserializator
                            sbDeser.AppendLine("return obj;");
                            sbDeserRef.AppendLine("return obj;");
                            sbDeser.AppendLine("}");
                            sbDeserRef.AppendLine("}");
                        }
                    }

                }
            }
        }
        // deserializator
        sbSer.AppendLine(sbDeser.ToString());

        // deserializator reference
        sbSer.AppendLine(sbDeserRef.ToString());

        // serializator
        sbSer.AppendLine("}");
        File.WriteAllText(Application.dataPath + "/Serializator.cs", sbSer.ToString());
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
