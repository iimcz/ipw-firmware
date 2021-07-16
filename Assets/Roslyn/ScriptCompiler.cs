using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ScriptCompiler
{
    private static readonly ScriptOptions _options = ScriptOptions
        .Default
        .WithImports("System", "System.Collections.Generic", "System.Linq", "UnityEngine") // TODO: Maybe have these configurable?
        .AddReferences(typeof(MonoBehaviour).Assembly);

    public static List<Type> ScriptTypes = new List<Type>();

    public static Type CompileMonoBehaviour(string code)
    {
        var script = CSharpScript.EvaluateAsync<Type>(code, _options).Result;
        ScriptTypes.Add(script); // Storing a copy to make sure GC doesn't try anything funny

        return script;
    }

    public static Script<T> CompileScript<T>(string code)
    {
        return CSharpScript.Create<T>(code, _options);
    }
}