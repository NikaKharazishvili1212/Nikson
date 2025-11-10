using System.Runtime.InteropServices;
using System.Text;

/// <summary>Collects all source files from a folder (ignoring bin/obj) and copies them to the clipboard.</summary>
public static class TextCollector
{
    private static readonly string[] IgnoredFolders = { "bin", "obj" };

    public static void CollectInFolder(string folderPath, string extensions)
    {
        // 1. Check folder
        if (!Directory.Exists(folderPath))
        {
            Console.WriteLine($"ERROR: Directory not found: {folderPath}");
            return;
        }

        // 2. Parse extensions
        var extList = extensions.Split(',')
                               .Select(e => e.Trim())
                               .Where(e => e.Length > 0)
                               .ToArray();

        if (extList.Length == 0)
        {
            Console.WriteLine("ERROR: No valid extensions provided.");
            return;
        }

        // 3. Collect files
        var sb = new StringBuilder();

        try
        {
            foreach (var ext in extList)
            {
                var files = Directory.GetFiles(folderPath, $"*{ext}", SearchOption.AllDirectories)
                                     .Where(f => !IgnoredFolders.Any(i => f.ToLowerInvariant().Contains($"\\{i}\\")))
                                     .OrderBy(f => f);

                foreach (var f in files)
                {
                    sb.AppendLine($"// ===== FILE: {Path.GetRelativePath(folderPath, f)} =====")
                      .AppendLine(File.ReadAllText(f))
                      .AppendLine().AppendLine().AppendLine();
                }
            }

            SetClipboardText(sb.ToString());
            Console.WriteLine($"All ({extensions}) files collected and copied to clipboard. You can use Ctrl+V to paste.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.GetType().Name}: {ex.Message}");
        }
    }

    // --- Native clipboard (unchanged) ---
    [DllImport("user32.dll", SetLastError = true)] private static extern bool OpenClipboard(IntPtr hWndNewOwner);
    [DllImport("user32.dll", SetLastError = true)] private static extern bool EmptyClipboard();
    [DllImport("user32.dll")] private static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);
    [DllImport("kernel32.dll")] private static extern IntPtr GlobalAlloc(uint uFlags, long dwBytes);
    [DllImport("kernel32.dll")] private static extern IntPtr GlobalLock(IntPtr hMem);
    [DllImport("kernel32.dll")] private static extern bool GlobalUnlock(IntPtr hMem);
    [DllImport("user32.dll")] private static extern bool CloseClipboard();

    private const uint CF_UNICODETEXT = 13;
    // Copies a string to the system clipboard using native Win32 calls (no Clipboard.SetText)
    private static void SetClipboardText(string text)
    {
        if (string.IsNullOrEmpty(text)) return;

        OpenClipboard(IntPtr.Zero);
        EmptyClipboard();

        int byteCount = (text.Length + 1) * 2;
        IntPtr hGlobal = GlobalAlloc(0x0042, byteCount);
        IntPtr pGlobal = GlobalLock(hGlobal);
        Marshal.Copy(text.ToCharArray(), 0, pGlobal, text.Length);
        Marshal.WriteInt16(pGlobal, text.Length * 2, 0);
        GlobalUnlock(hGlobal);
        SetClipboardData(CF_UNICODETEXT, hGlobal);
        CloseClipboard();
    }
}