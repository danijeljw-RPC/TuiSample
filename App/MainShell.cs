using System;
using System.Collections.Generic;
using Terminal.Gui;
using TuiSample.App.Auth;
using TuiSample.App.Model;
using TuiSample.App.Pages;

namespace TuiSample.App;

public sealed class MainShell
{
    private const string TokenEnv = "APP_TOKEN";

    private Toplevel _top = Application.Top;

    // Main UI parts
    private Window? _mainWin;
    private FrameView? _leftFrame;
    private ListView? _list;
    private FrameView? _rightFrame;
    private View? _descPanel;
    // private Label? _descTitle;
    private TextView? _descText;

    private readonly List<Category> _categories;

    public MainShell()
    {
        _categories = new()
        {
            new Category(
                "New User",
                "Create a new user with name and email. Result will be shown then you return here.",
                onComplete => new NewUserPage(BackToCategories, onComplete)),

            new Category(
                "New Org",
                "Create an organisation with ABN and contact. After creation you return here.",
                onComplete => new NewOrgPage(BackToCategories, onComplete)),

            new Category(
                "Discounts",
                "Search and manage discount codes. Enter to edit. New adds a discount.",
                onComplete => new DiscountsPage(BackToCategories, onComplete))
        };
    }

    // ---------------- Login ----------------
    public void ShowLogin()
    {
        _top.RemoveAll();

        var win = new Window("Login") { X = 0, Y = 1, Width = Dim.Fill(), Height = Dim.Fill() };

        var userLbl = new Label("Username:") { X = 2, Y = 2 };
        var userTxt = new TextField("") { X = Pos.Right(userLbl) + 1, Y = Pos.Top(userLbl), Width = 30 };

        var passLbl = new Label("Password:") { X = 2, Y = Pos.Bottom(userLbl) + 1 };
        var passTxt = new TextField("") { X = Pos.Right(passLbl) + 1, Y = Pos.Top(passLbl), Width = 30, Secret = true };

        var status = new Label("") { X = 2, Y = Pos.Bottom(passLbl) + 2, Width = Dim.Fill() };
        var loginBtn = new Button("Login") { X = 2, Y = Pos.Bottom(status) + 1, IsDefault = true };

        loginBtn.Clicked += async () =>
        {
            loginBtn.Enabled = false;
            status.Text = "Authenticating...";
            try
            {
                var u = userTxt.Text?.ToString() ?? string.Empty;
                var p = passTxt.Text?.ToString() ?? string.Empty;
                var token = await MockApi.AuthAsync(u, p);
                //var token = await MockApi.AuthAsync(userTxt.Text.ToString(), passTxt.Text.ToString());
                Environment.SetEnvironmentVariable(TokenEnv, token, EnvironmentVariableTarget.Process);
                ShowMain();
            }
            catch (Exception ex) { status.Text = $"Login failed: {ex.Message}"; }
            finally { loginBtn.Enabled = true; }
        };
        passTxt.KeyPress += e => { if (e.KeyEvent.Key == Key.Enter) { loginBtn.OnClicked(); e.Handled = true; } };

        var menu = BuildMenuBar(includeLogoff: false);
        _top.Add(menu, win);
        win.Add(userLbl, userTxt, passLbl, passTxt, status, loginBtn);
        Application.Refresh();
    }

    // ---------------- Main ----------------
    public void ShowMain()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(TokenEnv)))
        {
            ShowLogin();
            return;
        }

        _top.RemoveAll();

        var menu = BuildMenuBar(includeLogoff: true);

        _mainWin = new Window("Main") { X = 0, Y = 1, Width = Dim.Fill(), Height = Dim.Fill() };

        // Left
        _leftFrame = new FrameView("Categories") { X = 0, Y = 0, Width = 28, Height = Dim.Fill() };
        _list = new ListView(_categories.ConvertAll(c => c.Name))
        {
            X = 0, Y = 0, Width = Dim.Fill(), Height = Dim.Fill() - 2, AllowsMarking = false
        };

        // var editDescBtn = new Button("Edit Desc") { X = 0, Y = Pos.Bottom(_list) + 0, Width = Dim.Fill() };
        // editDescBtn.Clicked += EditCurrentDescription;

        _list.OpenSelectedItem += args => OpenSelectedCategory(args.Item);
        _list.SelectedItemChanged += _ => UpdateDescriptionPanel();
        _leftFrame.Add(_list); //, editDescBtn);

        // Right host
        _rightFrame = new FrameView("Details") { X = Pos.Right(_leftFrame), Y = 0, Width = Dim.Fill(), Height = Dim.Fill() };

        // Default: description panel
        _descPanel = BuildDescriptionPanel();
        _rightFrame.Add(_descPanel);

        // ESC globally returns to categories
        _mainWin.KeyPress += e =>
        {
            if (e.KeyEvent.Key == Key.Esc)
            {
                BackToCategories();
                e.Handled = true;
            }
        };

        _mainWin.Add(_leftFrame, _rightFrame);
        _top.Add(menu, _mainWin);
        Application.Refresh();
        _list.SetFocus();      // ready for arrows
        UpdateDescriptionPanel();
    }

    private MenuBar BuildMenuBar(bool includeLogoff)
    {
        var fileItems = new List<MenuItem>();
        if (includeLogoff)
        {
            fileItems.Add(new MenuItem("_Logoff", "Clear token and return to login", () =>
            {
                Environment.SetEnvironmentVariable(TokenEnv, null, EnvironmentVariableTarget.Process);
                ShowLogin();
            }));
        }
        fileItems.Add(new MenuItem("_Quit", "Exit", () => Application.RequestStop()));

        return new MenuBar(new[]
        {
            new MenuBarItem("_File", fileItems.ToArray()),
            new MenuBarItem("_Help", new[]
            {
                new MenuItem("_About", "About", () =>
                    MessageBox.Query("About", "Terminal.Gui sample.\nESC always returns to Categories.", "OK"))
            })
        });
    }

    // ---------- Description panel ----------
    private View BuildDescriptionPanel()
    {
        var v = new View { X = 0, Y = 0, Width = Dim.Fill(), Height = Dim.Fill() };
        // _descTitle = new Label("") { X = 1, Y = 1 };
        _descText = new TextView
        {
            X = 1,
            Y = 1,
            Width = Dim.Fill() - 2,
            Height = Dim.Fill() - 1,
            ReadOnly = true,
            WordWrap = true
        };
        // v.Add(_descTitle, _descText);
        v.Add(_descText);
        return v;
    }

    private void UpdateDescriptionPanel()
    {
        // if (_list == null || _descTitle == null || _descText == null) return;
        if (_list == null || _descText == null) return;
        var idx = _list.SelectedItem;
        if (idx < 0 || idx >= _categories.Count) return;

        var c = _categories[idx];
        //_descTitle.Text = c.Name;
        _descText.Text = c.Description;
        _rightFrame!.Title = "Details";
        _rightFrame.RemoveAll();
        _rightFrame.Add(_descPanel!);
        _list.SetFocus();
    }

    private void EditCurrentDescription()
    {
        if (_list == null) return;
        var idx = _list.SelectedItem;
        if (idx < 0 || idx >= _categories.Count) return;

        var c = _categories[idx];
        var d = new Dialog($"Edit Description: {c.Name}", 70, 20);
        var tv = new TextView { X = 1, Y = 1, Width = Dim.Fill() - 2, Height = Dim.Fill() - 4, Text = c.Description };
        var ok = new Button("OK") { IsDefault = true };
        var cancel = new Button("Cancel");
        ok.Clicked += () => { c.Description = tv.Text.ToString() ?? ""; Application.RequestStop(); UpdateDescriptionPanel(); };
        cancel.Clicked += () => Application.RequestStop();
        d.Add(tv);
        d.AddButton(ok); d.AddButton(cancel);
        Application.Run(d);
    }

    // ---------- Navigation ----------
    private void OpenSelectedCategory(int index)
    {
        if (index < 0 || index >= _categories.Count) return;

        var cat = _categories[index];
        _rightFrame!.RemoveAll();
        _rightFrame.Title = cat.Name;

        // Build page and pass completion callback
        var view = cat.BuildView(OnActionCompleted);
        _rightFrame.Add(view);
        view.SetFocus();
    }

    private void OnActionCompleted(string outcome)
    {
        MessageBox.Query("Outcome", outcome, "OK");
        BackToCategories();
    }

    private void BackToCategories()
    {
        // When returning, show the description of the currently highlighted category
        UpdateDescriptionPanel();
    }
}
