using System;
using Terminal.Gui;

namespace TuiSample.App.Dialogs
{
    public static class DiscountEditorDialog
    {
        /// <summary>Create or edit a discount. Returns true if saved.</summary>
        public static bool Show(ref string code, ref string name, ref double percent, bool isNew)
        {
            // work on locals to avoid touching ref params inside lambdas
            string codeLocal = code ?? string.Empty;
            string nameLocal = name ?? string.Empty;
            double percentLocal = percent;

            var d = new Dialog(isNew ? "New Discount" : "Edit Discount", 60, 15);

            var lblCode = new Label("Code:") { X = 1, Y = 1 };
            var txtCode = new TextField(codeLocal) { X = 15, Y = 1, Width = 40 };
            txtCode.ReadOnly = !isNew; // immutable when editing

            var lblName = new Label("Name:") { X = 1, Y = 3 };
            var txtName = new TextField(nameLocal) { X = 15, Y = 3, Width = 40 };

            var lblPct = new Label("Percent:") { X = 1, Y = 5 };
            var txtPct = new TextField(percentLocal.ToString("0.##")) { X = 15, Y = 5, Width = 10 };

            var ok = new Button("OK") { IsDefault = true };
            var cancel = new Button("Cancel");

            bool saved = false;

            ok.Clicked += () =>
            {
                if (!double.TryParse(txtPct.Text?.ToString() ?? "", out var p) || p < 0 || p > 100)
                {
                    MessageBox.ErrorQuery("Invalid", "Percent must be 0..100", "OK");
                    return;
                }

                codeLocal = txtCode.Text?.ToString() ?? "";
                nameLocal = txtName.Text?.ToString() ?? "";
                percentLocal = p;

                saved = true;
                Application.RequestStop();
            };

            cancel.Clicked += () => Application.RequestStop();

            d.Add(lblCode, txtCode, lblName, txtName, lblPct, txtPct);
            d.AddButton(ok);
            d.AddButton(cancel);

            Application.Run(d);

            if (saved)
            {
                code = codeLocal;
                name = nameLocal;
                percent = percentLocal;
            }

            return saved;
        }
    }
}
