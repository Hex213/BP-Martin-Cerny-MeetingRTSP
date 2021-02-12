using System;
using MeetingClientWPF.Engine;
using RtspClientSharp;

namespace SimpleRtspPlayer.GUI.Models
{
    public interface IMainWindowModel
    {
        event EventHandler<string> StatusChanged;

        IVideoSource VideoSource { get; }

        void Start(ConnectionParameters connectionParameters);
        void Stop();
    }
}