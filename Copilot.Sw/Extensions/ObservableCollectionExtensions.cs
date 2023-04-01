using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Copilot.Sw.Extensions;

public static class ObservableCollectionExtensions
{
    public static ObservableCollection<T>? ToObservableCollection<T>(this IEnumerable<T> list)
    {
        if (list == null)
        {
            return null;
        }

        return new ObservableCollection<T>(list);
    }
}
