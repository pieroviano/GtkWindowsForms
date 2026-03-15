using System;
using System.Collections.Generic;
using System.Linq;

namespace XmlDocSync.Cleaning
{
    public static class XmlDocDuplicateCleaner
    {
        public static string RemoveRepeatedXmlDocBlocks(string text)
        {
            var lines = text.Replace("\r\n", "\n").Split('\n');
            var result = new List<string>();

            int i = 0;

            while (i < lines.Length)
            {
                if (IsXmlDocLine(lines[i]))
                {
                    var block = ReadXmlBlock(lines, ref i);
                    var cleanedBlock = RemoveInternalDuplicates(block);
                    result.AddRange(cleanedBlock);
                }
                else
                {
                    result.Add(lines[i]);
                    i++;
                }
            }

            return string.Join(Environment.NewLine, result);
        }

        private static bool IsXmlDocLine(string line)
        {
            return line.TrimStart().StartsWith("///");
        }

        private static List<string> ReadXmlBlock(string[] lines, ref int index)
        {
            var block = new List<string>();
            while (index < lines.Length && IsXmlDocLine(lines[index]))
            {
                block.Add(lines[index]);
                index++;
            }
            return block;
        }

        private static List<string> RemoveInternalDuplicates(List<string> block)
        {
            var normalized = block.Select(l => l.TrimStart()).ToList();

            // Only remove duplicates if at least 50% of the block is repeated
            // This prevents removing legitimate repeated closing tags like </param>, </returns>, etc.
            int minDuplicateLength = Math.Max(3, normalized.Count / 2);

            // Try to find repeated sequences, preferring longer matches first
            for (int len = normalized.Count / 2; len >= minDuplicateLength; len--)
            {
                for (int start = 0; start + len < normalized.Count; start++)
                {
                    // Check if sequence [start, start+len) appears again anywhere after it
                    for (int secondStart = start + 1; secondStart + len <= normalized.Count; secondStart++)
                    {
                        bool match = true;
                        for (int k = 0; k < len; k++)
                        {
                            if (normalized[start + k] != normalized[secondStart + k])
                            {
                                match = false;
                                break;
                            }
                        }
                        if (match)
                        {
                            // Found repetition: keep only lines before second occurrence
                            return block.Take(secondStart).ToList();
                        }
                    }
                }
            }

            return block;
        }
    }
}
