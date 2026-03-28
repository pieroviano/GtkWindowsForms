using System.Globalization;

namespace System.Resources
{
    public class ResXFileRefConverterTests
    {
        [Fact]
        public void CanConvertFrom_StringType_ReturnsTrue()
        {
            var converter = new ResXFileRef.Converter();
            Assert.True(converter.CanConvertFrom(null, typeof(string)));
        }

        [Fact]
        public void CanConvertFrom_NonStringType_ReturnsFalse()
        {
            var converter = new ResXFileRef.Converter();
            Assert.False(converter.CanConvertFrom(null, typeof(int)));
        }

        [Fact]
        public void CanConvertTo_StringType_ReturnsTrue()
        {
            var converter = new ResXFileRef.Converter();
            Assert.True(converter.CanConvertTo(null, typeof(string)));
        }

        [Fact]
        public void CanConvertTo_NonStringType_ReturnsFalse()
        {
            var converter = new ResXFileRef.Converter();
            Assert.False(converter.CanConvertTo(null, typeof(int)));
        }

        [Fact]
        public void ParseResxFileRefString_ParsesQuotedFileName()
        {
            var input = "\"C:/file.txt\";System.String;utf-8";
            var parts = ResXFileRef.Converter.ParseResxFileRefString(input);
            Assert.Equal("C:/file.txt", parts[0]);
            Assert.Equal("System.String", parts[1]);
            Assert.Equal("utf-8", parts[2]);
        }

        [Fact]
        public void ParseResxFileRefString_ParsesUnquotedFileName()
        {
            var input = "C:/file.txt;System.String;utf-8";
            var parts = ResXFileRef.Converter.ParseResxFileRefString(input);
            Assert.Equal("C:/file.txt", parts[0]);
            Assert.Equal("System.String", parts[1]);
            Assert.Equal("utf-8", parts[2]);
        }

        [Fact]
        public void ParseResxFileRefString_InvalidFormat_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => ResXFileRef.Converter.ParseResxFileRefString("invalid"));
        }

        [Fact]
        public void ConvertTo_String_ReturnsToString()
        {
            var converter = new ResXFileRef.Converter();
            var fileRef = new ResXFileRef("file.txt", typeof(string).AssemblyQualifiedName);
            var result = converter.ConvertTo(null, CultureInfo.InvariantCulture, fileRef, typeof(string));
            Assert.Equal(fileRef.ToString(), result);
        }
    }
}
