using System.Text;

/// <summary>Replaces all occurrences of oldText with newText in the given file. Supports any text-based files; does not support binary formats like .docx, .xlsx, .pdf.</summary>
public static class TextReplacer
{
    public static void InFile(string filePath, string oldText, string newText)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"ERROR: File not found: {filePath}");
            return;
        }

        if (string.IsNullOrWhiteSpace(oldText))
        {
            Console.WriteLine("ERROR: oldText cannot be empty.");
            return;
        }

        try
        {
            string content = File.ReadAllText(filePath, Encoding.UTF8);
            int count = 0;
            string newContent = content.Replace(oldText, newText, out count);

            if (count > 0)
            {
                File.WriteAllText(filePath, newContent, Encoding.UTF8);
                Console.WriteLine($"Replaced {count} \"{oldText}\" → \"{newText}\" in: {filePath}");
            }
            else
            {
                Console.WriteLine($"No matches for \"{oldText}\" in: {filePath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.GetType().Name}: {ex.Message}");
        }
    }

    public static void InFolder(string folderPath, string extension, string oldText, string newText)
    {
        if (!Directory.Exists(folderPath))
        {
            Console.WriteLine($"ERROR: Folder not found: {folderPath}");
            return;
        }

        var files = Directory.GetFiles(folderPath, $"*{extension}", SearchOption.AllDirectories)
            .Where(f => !f.Contains("\\bin\\") && !f.Contains("\\obj\\"))
            .OrderBy(f => f);

        int totalReplaced = 0;
        int filesChanged = 0;

        foreach (var file in files)
        {
            try
            {
                string content = File.ReadAllText(file, Encoding.UTF8);
                string newContent = content.Replace(oldText, newText, out int count);

                if (count > 0)
                {
                    File.WriteAllText(file, newContent, Encoding.UTF8);
                    Console.WriteLine($"Replaced {count} time(s) in: {file}");
                    totalReplaced += count;
                    filesChanged++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in {Path.GetFileName(file)}: {ex.Message}");
            }
        }

        Console.WriteLine($"DONE! Changed {filesChanged} files. Total replacements: {totalReplaced}");
    }

    // Helper: counts replacements
    private static string Replace(this string source, string oldValue, string newValue, out int count)
    {
        count = 0;
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(oldValue)) return source;

        var sb = new StringBuilder();
        int start = 0, pos;
        while ((pos = source.IndexOf(oldValue, start)) != -1)
        {
            sb.Append(source, start, pos - start).Append(newValue);
            start = pos + oldValue.Length;
            count++;
        }
        sb.Append(source, start, source.Length - start);
        return sb.ToString();
    }
}