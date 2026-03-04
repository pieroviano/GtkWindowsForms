using Prepare.Common.Text;

namespace Prepare.Tests
{
    public class CommentCleanerTests
    {
        [Fact]
        public void RemoveCommentBlock_NoMatch_ReturnsOriginalContent()
        {
            var content =
@"line1
line2
line3";

            var result = CommentCleaner.RemoveCommentBlock(content, "NOT_FOUND");

            Assert.Equal(content.Replace("\r\n", "\n"), result);
        }

        [Fact]
        public void OtherRemoveCommentBlock_DontRemovesInlineCommentOnly()
        {
            var content =
@"var a = 5; REMOVE_THIS // Other
var b = 6;";

            var expected =
@"var a = 5; REMOVE_THIS // Other
var b = 6;";

            var result = CommentCleaner.RemoveCommentBlock(content, "REMOVE_THIS");

            Assert.Equal(expected.Replace("\r\n", "\n"), result);
        }

        [Fact]
        public void RemoveCommentBlock_RemovesInlineCommentOnly()
        {
            var content =
@"var a = 5; // REMOVE_THIS
var b = 6;";

            var expected =
@"var a = 5;
var b = 6;";

            var result = CommentCleaner.RemoveCommentBlock(content, "REMOVE_THIS");

            Assert.Equal(expected.Replace("\r\n", "\n"), result);
        }

        [Fact]
        public void OtherRemoveCommentBlock_DontRemovesEntireCommentBlock_InMiddle()
        {
            var content =
@"line1
REMOVE_THIS;// 
// still comment
// another line
line2";

            var expected =
@"line1
REMOVE_THIS;// 
// still comment
// another line
line2";

            var result = CommentCleaner.RemoveCommentBlock(content, "REMOVE_THIS");

            Assert.Equal(expected.Replace("\r\n", "\n"), result);
        }

        [Fact]
        public void RemoveCommentBlock_RemovesEntireCommentBlock_InMiddle()
        {
            var content =
@"line1
// REMOVE_THIS
// still comment
// another line
line2";

            var expected =
@"line1
line2";

            var result = CommentCleaner.RemoveCommentBlock(content, "REMOVE_THIS");

            Assert.Equal(expected.Replace("\r\n", "\n"), result);
        }

        [Fact]
        public void RemoveCommentBlock_RemovesCommentBlock_AtBeginning()
        {
            var content =
@"// REMOVE_THIS
// comment line
line1
line2";

            var expected =
@"line1
line2";

            var result = CommentCleaner.RemoveCommentBlock(content, "REMOVE_THIS");

            Assert.Equal(expected.Replace("\r\n", "\n"), result);
        }

        [Fact]
        public void RemoveCommentBlock_RemovesCommentBlock_AtEnd()
        {
            var content =
@"line1
line2
// REMOVE_THIS
// comment line";

            var expected =
@"line1
line2";

            var result = CommentCleaner.RemoveCommentBlock(content, "REMOVE_THIS");

            Assert.Equal(expected.Replace("\r\n", "\n"), result);
        }

        [Fact]
        public void RemoveCommentBlock_RemovesOnlyBlockContainingTarget()
        {
            var content =
@"// keep this
// still keep

// REMOVE_THIS
// remove this too

line1";

            var expected =
@"// keep this
// still keep

line1";

            var result = CommentCleaner.RemoveCommentBlock(content, "REMOVE_THIS");

            Assert.Equal(expected.Replace("\r\n", "\n"), result);
        }

        [Fact]
        public void RemoveCommentBlock_MultipleBlocks_RemovesAllMatchingBlocks()
        {
            var content =
@"// REMOVE_THIS
// block1

line1

// REMOVE_THIS
// block2

line2";

            var expected =
@"line1

line2";

            var result = CommentCleaner.RemoveCommentBlock(content, "REMOVE_THIS");

            Assert.Equal(expected.Replace("\r\n", "\n"), result);
        }

        [Fact]
        public void RemoveCommentBlock_InlineThenBlock_HandlesBoth()
        {
            var content =
@"var a = 1; // REMOVE_THIS

// REMOVE_THIS
// comment block

var b = 2;";

            var expected =
@"var a = 1;

var b = 2;";

            var result = CommentCleaner.RemoveCommentBlock(content, "REMOVE_THIS");

            Assert.Equal(expected.Replace("\r\n", "\n"), result);
        }

        [Fact]
        public void RemoveCommentBlock_InlineCommentWithoutLeadingSlashes_IsNotTreatedAsBlock()
        {
            var content =
@"var a = 1;    // REMOVE_THIS
// some other comment
var b = 2;";

            var expected =
@"var a = 1;
// some other comment
var b = 2;";

            var result = CommentCleaner.RemoveCommentBlock(content, "REMOVE_THIS");

            Assert.Equal(expected.Replace("\r\n", "\n"), result);
        }

        [Fact]
        public void RemoveCommentBlock_Regex_InlineMatch_RemovesInlineComment()
        {
            var content =
        @"var a = 5; // REMOVE_123
var b = 6;";

            var expected =
        @"var a = 5;
var b = 6;";

            var result = CommentCleaner.RemoveCommentBlock(content, "/REMOVE_\\d+/");

            Assert.Equal(expected.Replace("\r\n", "\n"), result);
        }

        [Fact]
        public void RemoveCommentBlock_Regex_BlockMatch_RemovesEntireBlock()
        {
            var content =
        @"line1
// REMOVE_ABC
// still comment
// another line
line2";

            var expected =
        @"line1
line2";

            var result = CommentCleaner.RemoveCommentBlock(content, "/REMOVE_.+/");

            Assert.Equal(expected.Replace("\r\n", "\n"), result);
        }

        [Fact]
        public void RemoveCommentBlock_Regex_MultipleBlocks_RemovesAllMatchingBlocks()
        {
            var content =
        @"// REMOVE_1
// block1

line1

// REMOVE_2
// block2

line2";

            var expected =
        @"line1

line2";

            var result = CommentCleaner.RemoveCommentBlock(content, "/REMOVE_\\d/");

            Assert.Equal(expected.Replace("\r\n", "\n"), result);
        }

        [Fact]
        public void RemoveCommentBlock_Regex_NoMatch_DoesNotModifyContent()
        {
            var content =
        @"// KEEP_THIS
line1";

            var result = CommentCleaner.RemoveCommentBlock(content, "/REMOVE_.+/");

            Assert.Equal(content.Replace("\r\n", "\n"), result);
        }

        [Fact]
        public void RemoveCommentBlock_Regex_MatchesInsideCommentLine_RemovesBlock()
        {
            var content =
        @"// prefix REMOVE_999 suffix
// another comment
line1";

            var expected =
        @"line1";

            var result = CommentCleaner.RemoveCommentBlock(content, "/REMOVE_\\d+/");

            Assert.Equal(expected.Replace("\r\n", "\n"), result);
        }

        [Fact]
        public void RemoveCommentBlock_Regex_DifferentFromPlainText()
        {
            var content =
        @"// REMOVE_123
line1";

            // Plain string should NOT match
            var plainResult = CommentCleaner.RemoveCommentBlock(content, "REMOVE_\\d+");
            Assert.Equal(content.Replace("\r\n", "\n"), plainResult);

            // Regex should match
            var regexResult = CommentCleaner.RemoveCommentBlock(content, "/REMOVE_\\d+/");
            Assert.Equal("line1", regexResult);
        }
    }
}
