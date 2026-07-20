using Prepare.Common.Alignment;

namespace Prepare.Tests
{
    public class AlignmentCheckpointTests
    {
        [Fact]
        public void Read_MissingFile_ReturnsFallback()
        {
            var path = Path.Combine(Path.GetTempPath(), "ckpt-" + Guid.NewGuid().ToString("N"), "last-aligned.txt");
            Assert.Equal("bootstrap-sha", AlignmentCheckpoint.Read(path, "bootstrap-sha"));
        }

        [Fact]
        public void WriteThenRead_RoundTrips_IgnoringComments()
        {
            var dir = Path.Combine(Path.GetTempPath(), "ckpt-" + Guid.NewGuid().ToString("N"));
            var path = Path.Combine(dir, "last-aligned.txt");
            try
            {
                AlignmentCheckpoint.Write(path, "abc123");
                Assert.True(File.Exists(path));
                Assert.Contains("#", File.ReadAllText(path)); // has the comment header
                Assert.Equal("abc123", AlignmentCheckpoint.Read(path, "fallback"));
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }

        [Fact]
        public void Read_EmptyOrCommentOnly_ReturnsFallback()
        {
            var dir = Path.Combine(Path.GetTempPath(), "ckpt-" + Guid.NewGuid().ToString("N"));
            var path = Path.Combine(dir, "last-aligned.txt");
            Directory.CreateDirectory(dir);
            try
            {
                File.WriteAllText(path, "# only a comment\n   \n");
                Assert.Equal("fallback", AlignmentCheckpoint.Read(path, "fallback"));
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }
    }
}
