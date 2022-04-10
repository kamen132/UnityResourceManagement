using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

/// <summary>
/// This class is meant to be used by long-running nested build processes that
/// need to display a progress bar, without constantly closing/opening the bar
/// or accidentally forgetting to close it.
/// 
/// This class is meant to be used with a C# "using" statement.
/// </summary>
/// <author>Daniel Koozer</author>
public class EditorProgressBar : IDisposable
{
    static int refCount;

    public EditorProgressBar()
    {
        refCount++;
    }

    public void Dispose()
    {
        refCount--;

        if (refCount <= 0)
        {
            refCount = 0;
            EditorUtility.ClearProgressBar();
        }
    }

    public void Display(string title, string info, float progress)
    {
        EditorUtility.DisplayProgressBar(title, info, progress);
    }

    public bool DisplayCancellable(string title, string info, float progress)
    {
        return EditorUtility.DisplayCancelableProgressBar(title, info, progress);
    }
}
