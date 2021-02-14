using System;
using RtspClientSharp;

namespace MeetingClientWPF.Engine.Models
{
    public interface IMainWindowModel
    {
        event EventHandler<string> StatusChanged;

        IVideoSource VideoSource { get; }

        void Start(ConnectionParameters connectionParameters);
        void Stop();
    }
}