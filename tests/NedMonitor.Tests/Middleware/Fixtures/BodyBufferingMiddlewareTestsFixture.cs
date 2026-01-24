using Microsoft.AspNetCore.Http;
using System.Text;

namespace NedMonitor.Tests.Middleware.Fixtures;

public sealed class BodyBufferingMiddlewareTestsFixture
{
    public DefaultHttpContext CreateContextWithNonSeekableBody(string body)
    {
        var bytes = Encoding.UTF8.GetBytes(body);
        var context = new DefaultHttpContext
        {
            Request =
            {
                Body = new NonSeekableReadStream(bytes),
                ContentLength = bytes.Length
            }
        };

        return context;
    }

    private sealed class NonSeekableReadStream(byte[] buffer) : Stream
    {
        private readonly MemoryStream _inner = new(buffer);

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => _inner.Length;

        public override long Position
        {
            get => _inner.Position;
            set => throw new NotSupportedException();
        }

        public override void Flush() { }

        public override int Read(byte[] buffer, int offset, int count)
            => _inner.Read(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin)
            => throw new NotSupportedException();

        public override void SetLength(long value)
            => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count)
            => throw new NotSupportedException();
    }
}
