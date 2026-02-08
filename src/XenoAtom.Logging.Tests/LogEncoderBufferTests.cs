// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Text;
using XenoAtom.Logging.Helpers;

namespace XenoAtom.Logging.Tests;

[TestClass]
public class LogEncoderBufferTests
{
    [TestMethod]
    public void Dispose_CanBeCalledTwice()
    {
        var encoderBuffer = new LogEncoderBuffer();
        _ = encoderBuffer.Encode("hello".AsSpan(), Encoding.UTF8);
        encoderBuffer.Dispose();
        encoderBuffer.Dispose();
    }

    [TestMethod]
    public void Encode_CanBeUsedAfterDispose()
    {
        var encoderBuffer = new LogEncoderBuffer();
        _ = encoderBuffer.Encode("hello".AsSpan(), Encoding.UTF8);
        encoderBuffer.Dispose();

        var bytes = encoderBuffer.Encode("world".AsSpan(), Encoding.UTF8);
        Assert.AreEqual(5, bytes.Length);
        encoderBuffer.Dispose();
    }
}
