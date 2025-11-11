using System;
using NanoidDotNet;
using Terminal.Gui;

namespace TuiSample.App.Pages;

public sealed class NewOrgPage : CategoryPageBase
{
    public NewOrgPage(Action returnToCategories, Action<string> onComplete) : base(returnToCategories)
    {
        var lbl1 = new Label("ID:") { X = 1, Y = 1 };
        var txt1 = new TextField(Nanoid.Generate()) { X = Pos.Right(lbl1) + 1, Y = Pos.Top(lbl1), Width = 20, ReadOnly = true };

        var lbl2 = new Label("Company Name:") { X = 1, Y = Pos.Bottom(lbl1) + 1 };
        var txt2 = new TextField("") { X = Pos.Right(lbl2) + 1, Y = Pos.Top(lbl2), Width = 40 };

        var lbl3 = new Label("Contact Email:") { X = 1, Y = Pos.Bottom(lbl2) + 1 };
        var txt3 = new TextField("") { X = Pos.Right(lbl3) + 1, Y = Pos.Top(lbl3), Width = 40 };

        var createBtn = new Button("Create Org") { X = 1, Y = Pos.Bottom(lbl3) + 2, IsDefault = true };
        createBtn.Clicked += () =>
        {
            var msg = $"Created org: {txt1.Text} (ABN {txt2.Text})";
            Complete(msg, onComplete);
        };

        Add(lbl1, txt1, lbl2, txt2, lbl3, txt3, createBtn);
    }
}
