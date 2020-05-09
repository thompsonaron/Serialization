using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

public class Generator
{
    [MenuItem("Generator/Generate)")]
    public static void Generate()
    {
        var types = System.AppDomain.CurrentDomain.GetAssemblies();
        // getting all assemblies
        foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            // narrowing to desired assemblies
            if (assembly.FullName.Contains("CSharp") && !assembly.FullName.Contains("Editor"))
            {
                // alll classes in assembly
                foreach (var classType in assembly.GetTypes())
                {
                    // getting all attributes in class
                    Attribute[] attrs = System.Attribute.GetCustomAttributes(classType);
                    for (int i = 0; i < attrs.Length; i++)
                    {
                        // if attribute is [Serializable] => do stuff
                        if (attrs[i] is SerializableAttribute)
                        {
                            //UnityEngine.Debug.Log("We have a match");

                            // go through every field
                            // write c# code
                            classType.GetFields();

                            foreach (var field in classType.GetFields())
                            {
                                //if (field.)
                                {

                                }
                                Debug.Log(field.Name);
                            }

                        }
                        //Debug.Log(attrs[i].ToString());
                    }
                }
            }
        }
    }
}
