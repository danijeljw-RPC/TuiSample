using System;
using Terminal.Gui;

namespace TuiSample.App.Model;

public sealed class Category
{
    public string Name { get; }
    public string Description { get; set; }
    public Func<Action<string>, View> BuildView { get; }

    public Category(string name, string description, Func<Action<string>, View> build)
    {
        Name = name;
        Description = description;
        BuildView = build;
    }
}
