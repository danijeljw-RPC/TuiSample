using System;
using System.Data;
using System.Linq;
using Terminal.Gui;
using TuiSample.App.Dialogs;
using TuiSample.App.Util;

namespace TuiSample.App.Pages;

public sealed class DiscountsPage : CategoryPageBase
{
    private readonly DataTable _table;
    private readonly TableView _tv;
    private readonly TextField _search;

    public DiscountsPage(Action returnToCategories, Action<string> onComplete) : base(returnToCategories)
    {
        // Search + New/Edit buttons
        var lbl = new Label("Search:") { X = 1, Y = 1 };
        _search = new TextField("") { X = Pos.Right(lbl) + 1, Y = Pos.Top(lbl), Width = 30 };
        var btnNew = new Button("New") { X = Pos.Right(_search) + 2, Y = Pos.Top(_search) };
        var btnEdit = new Button("Edit") { X = Pos.Right(btnNew) + 2, Y = Pos.Top(_search) };

        _table = Seed();
        _tv = new TableView
        {
            X = 1,
            Y = Pos.Bottom(lbl) + 1,
            Width = Dim.Fill() - 2,
            Height = Dim.Fill() - 2
        };
        _tv.Table = _table;

        _search.KeyPress += e =>
        {
            if (e.KeyEvent.Key == Key.Enter) { ApplyFilter(); e.Handled = true; }
        };

        btnNew.Clicked += () =>
        {
            string code = $"DISC{_table.Rows.Count + 1:000}";
            string name = "New Discount";
            double pct = 0;
            if (DiscountEditorDialog.Show(ref code, ref name, ref pct, isNew: true))
            {
                _table.Rows.Add(code, name, pct);
                ApplyFilter();
                Complete($"Added discount {code}", onComplete);
            }
        };

        btnEdit.Clicked += () =>
        {
            var row = _tv.SelectedRow;
            if (_tv.Table == null || row < 0 || row >= _tv.Table.Rows.Count)
                return;

            var current = _tv.Table.Rows[row];
            string code = current.Field<string>("Code") ?? "";
            string name = current.Field<string>("Name") ?? "";
            double pct = current.Field<double?>("Percent") ?? 0.0;

            if (DiscountEditorDialog.Show(ref code, ref name, ref pct, isNew: false))
            {
                // Find and update in the base table by Code
                var baseRow = _table.AsEnumerable().FirstOrDefault(r => r.Field<string>("Code") == code);
                if (baseRow != null)
                {
                    baseRow.SetField("Name", name);
                    baseRow.SetField("Percent", pct);
                }
                ApplyFilter();
                Complete($"Updated discount {code}", onComplete);
            }
        };

        _tv.KeyPress += e =>
        {
            if (e.KeyEvent.Key == Key.Enter)
            {
                btnEdit.OnClicked();
                e.Handled = true;
            }
        };

        Add(lbl, _search, btnNew, btnEdit, _tv);
    }

    private void ApplyFilter()
    {
        var q = _search.Text.ToString() ?? "";
        if (string.IsNullOrWhiteSpace(q))
        {
            _tv.Table = _table;
            _tv.Update();
            return;
        }

        var lower = q.ToLowerInvariant();
        var filtered = _table.AsEnumerable()
            .Where(r =>
                (r.Field<string>("Code") ?? "").ToLowerInvariant().Contains(lower) ||
                (r.Field<string>("Name") ?? "").ToLowerInvariant().Contains(lower))
            .CopyToDataTableOrEmpty(_table);
        _tv.Table = filtered;
        _tv.Update();
    }

    private static DataTable Seed()
    {
        var t = new DataTable();
        t.Columns.Add("Code", typeof(string));
        t.Columns.Add("Name", typeof(string));
        t.Columns.Add("Percent", typeof(double));

        t.Rows.Add("DISC001", "Welcome", 5.0);
        t.Rows.Add("DISC002", "VIP", 12.5);
        t.Rows.Add("DISC003", "EOFY", 15.0);
        t.Rows.Add("DISC004", "Partner", 7.5);
        return t;
    }
}
