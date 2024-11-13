﻿using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Antelcat.AutoGen.Sample.Models;

namespace Antelcat.AutoGen.Native;

public class Class
{
    public int     Int    { get; set; }
    public string? String { get; set; }

    public Func<string>?              Delegate;
    public event Func<string>?        Event;
    public Class?                     ObjectRef     { get; set; }
    public IEnumerable<Class>?        CollectionRef { get; set; }
    public Dictionary<string, Class>? DictionaryRef { get; set; }

    [JsonIgnore]
    public int Hash => GetHashCode();

    public bool DelegateNotNull => Delegate != null;
    public bool EventsNotNull   => Event    != null;

    public override string ToString() => JsonSerializer.Serialize(this, new ClassSerializerContext(new()
    {
        WriteIndented = true
    }).Class);

    private static Class Origin => new ()
    {
        Int    = 1,
        String = "123",
        ObjectRef = new Class
        {
            Int    = 2,
            String = "???"
        },
        Delegate = () => "?",
        CollectionRef = new List<Class>
        {
            new()
            {
                Int    = 3,
                String = "!!!"
            }
        },
        DictionaryRef = new()
        {
            {
                "1", new()
                {
                    Int = 4
                }
            }
        }
    };
    
    public static void RunTest()
    {
        var origin = Origin;
        origin.Event += () => "!";
        
        ObjectCloneExtensions.Register(typeof(Class));
        ObjectCloneExtensions.Register(typeof(List<>));
        ObjectCloneExtensions.Register(typeof(Dictionary<,>));
        ObjectCloneExtensions.Register(typeof(KeyValuePair<,>));

        var watch  = new Stopwatch();
        watch.Start();
        var cloned = origin.DeepClone();
        var time   = watch.ElapsedTicks;
        Console.WriteLine(time);
        
        Console.WriteLine("origin :");
        watch.Restart();
        var originStr = origin.ToString();
        time   = watch.ElapsedTicks;
        Console.WriteLine(time);
        Console.WriteLine(originStr);
        Console.WriteLine($"{nameof(Equals)} : {origin == cloned}");
        Console.WriteLine("cloned :");

        var clonedStr = cloned.ToString();
        Console.WriteLine(clonedStr);

        Console.WriteLine($"Json Equals : {originStr == clonedStr}");
    }

    public static void RunTime()
    {
        var origin = Origin;
        var watch  = new Stopwatch();
        watch.Start();
        origin.DeepClone();
        var time   = watch.ElapsedTicks;
        Console.WriteLine($"Clone : {time}");

        var typeInfo = new ClassSerializerContext(new()
        {
            WriteIndented = true
        }).Class;
        watch.Restart();
        JsonSerializer.Serialize(origin, typeInfo);
        time   = watch.ElapsedTicks;
        Console.WriteLine($"Serialize : {time}");
    }

}


[JsonSerializable(typeof(Class))]
[JsonSerializable(typeof(IEnumerable<Class>))]
[JsonSerializable(typeof(List<Class>))]
[JsonSerializable(typeof(Dictionary<string, Class>))]
[JsonSerializable(typeof(KeyValuePair<string, Class>))]
public partial class ClassSerializerContext : JsonSerializerContext
{
}