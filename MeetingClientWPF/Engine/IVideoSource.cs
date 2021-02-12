using System;
using MeetingClientWPF.Engine.RawFramesDecoding.DecodedFrames;

namespace MeetingClientWPF.Engine
{
    public interface IVideoSource
    {
        event EventHandler<IDecodedVideoFrame> FrameReceived;
    }
}