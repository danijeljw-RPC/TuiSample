using System;
using Terminal.Gui;

namespace TuiSample.App.Pages;

public sealed class NewUserPage : CategoryPageBase
{
    public NewUserPage(Action returnToCategories, Action<string> onComplete) : base(returnToCategories)
    {
        var lbl1 = new Label("First Name:") { X = 1, Y = 1 };
        var txt1 = new TextField("") { X = Pos.Right(lbl1) + 1, Y = Pos.Top(lbl1), Width = 30 };

        var lbl2 = new Label("Last Name:") { X = 1, Y = Pos.Bottom(lbl1) + 1 };
        var txt2 = new TextField("") { X = Pos.Right(lbl2) + 1, Y = Pos.Top(lbl2), Width = 30 };

        var lbl3 = new Label("Email:") { X = 1, Y = Pos.Bottom(lbl2) + 1 };
        var txt3 = new TextField("") { X = Pos.Right(lbl3) + 1, Y = Pos.Top(lbl3), Width = 30 };

        var createBtn = new Button("Create User") { X = 1, Y = Pos.Bottom(lbl3) + 2, IsDefault = true };
        createBtn.Clicked += () =>
        {
            var msg = $"Created user: {txt1.Text} {txt2.Text} <{txt3.Text}>";
            Complete(msg, onComplete);
        };

        Add(lbl1, txt1, lbl2, txt2, lbl3, txt3, createBtn);
    }
}
