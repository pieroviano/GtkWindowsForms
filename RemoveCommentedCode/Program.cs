using Antlr4.Runtime;

namespace RemoveCommentedCode;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Usage: CommentCleaner <path-to-folder>");
            return 1;
        }

        var rootPath = Path.GetFullPath(args[0]);

        if (!Directory.Exists(rootPath))
        {
            Console.WriteLine("Directory not found.");
            return 1;
        }

        var files = Directory
            .GetFiles(rootPath, "*.cs", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            Console.WriteLine($"Processing: {file}");
            var text = await File.ReadAllTextAsync(file);

            var cleaned = RemoveCommentedCode(text);

            if (cleaned != text)
                await File.WriteAllTextAsync(file, cleaned);
        }

        Console.WriteLine("Done.");
        return 0;
    }

    private static string RemoveCommentedCode(string source)
    {
        var inputStream = new AntlrInputStream(source);
        var lexer = new CSharpLexer(inputStream);
        var tokenStream = new CommonTokenStream(lexer);
        tokenStream.Fill();

        var rewriter = new TokenStreamRewriter(tokenStream);

        // Keep track of token line numbers we delete
        var deletedLines = new HashSet<int>();

        foreach (var token in tokenStream.GetTokens())
        {
            if (!IsRemovableComment(token))
                continue;

            var commentText = ExtractCommentText(token.Text);

            if (!PassesHeuristic(commentText))
                continue;

            if (LooksLikeCode(commentText))
            {
                rewriter.Delete(token);

                // Mark all lines that the token spans
                for (int line = token.Line; line <= token.Line + CountLineBreaks(token.Text); line++)
                    deletedLines.Add(line);
            }
        }

        var result = rewriter.GetText();

        // Only remove lines that became empty due to deleted comments
        var lines = result.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
        for (int i = 0; i < lines.Length; i++)
        {
            if (deletedLines.Contains(i + 1) && string.IsNullOrWhiteSpace(lines[i]))
                lines[i] = null; // mark line for removal
        }

        return string.Join(Environment.NewLine, lines.Where(l => l != null));
    }

    // Helper to count how many line breaks are in a token
    private static int CountLineBreaks(string text)
    {
        return text.Count(c => c == '\n');
    }

    /// <summary>
    /// Only allow true comments. Never touch directives or doc comments.
    /// </summary>
    private static bool IsRemovableComment(IToken token)
    {
        if (token.Type != CSharpLexer.SINGLE_LINE_COMMENT &&
            token.Type != CSharpLexer.DELIMITED_COMMENT)
            return false;

        var text = token.Text;

        // Preserve XML doc comments
        if (text.StartsWith("///"))
            return false;

        // Preserve commented-out directives
        if (text.StartsWith("//#") || text.StartsWith("/*#"))
            return false;

        return true;
    }

    private static string ExtractCommentText(string comment)
    {
        if (comment.StartsWith("//"))
            return comment.Substring(2);

        if (comment.StartsWith("/*") && comment.EndsWith("*/"))
            return comment.Substring(2, comment.Length - 4);

        return comment;
    }

    /// <summary>
    /// Fast pre-filter before expensive parsing.
    /// Avoids parsing normal explanatory comments.
    /// </summary>
    private static bool PassesHeuristic(string text)
    {
        text = text.Trim();

        if (string.IsNullOrWhiteSpace(text))
            return false;

        // Must contain something code-like
        return text.Contains(";") ||
               text.Contains("{") ||
               text.Contains("}") ||
               text.Contains("=") ||
               text.Contains("class ") ||
               text.Contains("public ") ||
               text.Contains("private ") ||
               text.Contains("protected ") ||
               text.Contains("internal ");
    }

    private static bool LooksLikeCode(string text)
    {
        text = text.Trim();

        if (string.IsNullOrWhiteSpace(text))
            return false;

        try
        {
            var input = new AntlrInputStream(text);
            var lexer = new CSharpLexer(input);
            var tokens = new CommonTokenStream(lexer);
            var parser = new CSharpParser(tokens);

            parser.RemoveErrorListeners();
            parser.ErrorHandler = new BailErrorStrategy();

            // Try parsing as compilation unit
            try
            {
                parser.compilation_unit();
                return true;
            }
            catch
            {
                tokens.Reset();
                parser = new CSharpParser(tokens);
                parser.RemoveErrorListeners();
                parser.ErrorHandler = new BailErrorStrategy();

                parser.statement();
                return true;
            }
        }
        catch
        {
            return false;
        }
    }
}
