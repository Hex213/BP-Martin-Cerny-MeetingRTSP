using System;
using MeetingClientWPF.Engine.RawFramesDecoding.DecodedFrames;

namespace MeetingClientWPF.Engine
{
    interface IAudioSource
    {
        event EventHandler<IDecodedAudioFrame> FrameReceived;
    }
}
