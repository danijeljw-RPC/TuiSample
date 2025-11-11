using System.Data;
using System.Linq;

namespace TuiSample.App.Util;

public static class DataTableExtensions
{
    public static DataTable CopyToDataTableOrEmpty(this EnumerableRowCollection<DataRow> rows, DataTable schemaLike)
    {
        using var e = rows.GetEnumerator();
        if (!e.MoveNext())
        {
            var empty = schemaLike.Clone();
            return empty;
        }
        return rows.CopyToDataTable();
    }
}
