using System.Text;
using System.Text.RegularExpressions;
using Prepare.Common.Content;
using Prepare.Common.IO;

namespace Prepare.Common.Text;

public class EncodeDetector
{

    public static void EnsureUtf8Encoding(string file)
    {
#pragma warning disable CS8604 // Possible null reference argument.
        CopyFileEnsuringEncoding(Path.GetDirectoryName(file), file);
#pragma warning restore CS8604 // Possible null reference argument.
    }

    public static void CopyFileEnsuringEncoding(string targetDir, string file)
    {
        var destFile = Path.Combine(targetDir, Path.GetFileName(file));

        try
        {
            // Read bytes first to detect binary files (skip conversion for binaries)
            var bytes = File.ReadAllBytes(file);

            // If file contains a NUL byte it's very likely binary -> copy as-is
            if (Array.IndexOf(bytes, (byte)0) >= 0)
            {
                if (file != destFile)
                {
                    LogUtility.FileCopyAndLog(file, destFile, true);
                }
            }
            else
            {
                // Detect encoding and convert to UTF8 when necessary
                var encoding = EncodeDetector.DetectEncoding(file);

                if (encoding != null && encoding.CodePage != Encoding.UTF8.CodePage)
                {
                    // Decode using detected encoding and write as UTF8
                    var text = encoding.GetString(bytes);
                    Directory.CreateDirectory(targetDir);
                    CsFile.WriteAllText(destFile, text, Encoding.UTF8);
                }
                else
                {
                    if (file != destFile)
                    {
                        LogUtility.FileCopyAndLog(file, destFile, true);
                    }
                }
            }
        }
        catch
        {
            // Fallback: if anything goes wrong, copy the file to avoid blocking the workflow
            if (file != destFile)
            {
                LogUtility.FileCopyAndLog(file, destFile, true);
            }
        }
    }

    public static Encoding DetectEncoding(string path)
    {
        var bytes = File.ReadAllBytes(path);

        // 1. BOM
        if (bytes.Length >= 3 &&
            bytes[0] == 0xEF &&
            bytes[1] == 0xBB &&
            bytes[2] == 0xBF)
        {
            return Encoding.UTF8;
        }

        // 2. Strict UTF-8 validation
        bool utf8Valid;
        string? utf8Text = null;

        try
        {
            var utf8 = new UTF8Encoding(false, true);
            utf8Text = utf8.GetString(bytes);
            utf8Valid = true;
        }
        catch
        {
            utf8Valid = false;
        }

        if (!utf8Valid)
            return Encoding.GetEncoding("GB18030");

        // 3. Heuristic scoring
        var gbText = Encoding.GetEncoding("GB18030").GetString(bytes);

        var utf8Score = ScoreChineseText(utf8Text??"");
        var gbScore = ScoreChineseText(gbText);

        return gbScore > utf8Score
            ? Encoding.GetEncoding("GB18030")
            : Encoding.UTF8;
    }

    static int ScoreChineseText(string text)
    {
        var score = 0;

        // Penalize rare CJK characters
        foreach (var c in text)
        {
            if (c >= 0x4E00 && c <= 0x9FFF)
            {
                // basic CJK is fine
                score += 2;
            }
            else if (c >= 0x3400 && c <= 0x4DBF)
            {
                // Extension A: less common
                score -= 1;
            }
#pragma warning disable CS0652 // Comparison to integral constant is useless; the constant is outside the range of the type
            else if (c >= 0x20000)
#pragma warning restore CS0652 // Comparison to integral constant is useless; the constant is outside the range of the type
            {
                // Rare / archaic
                score -= 5;
            }
        }

        // Bonus for common Chinese words
        if (Regex.IsMatch(text, "图片|目录|索引|异常|参数|设置"))
            score += 10;

        return score;
    }
}
