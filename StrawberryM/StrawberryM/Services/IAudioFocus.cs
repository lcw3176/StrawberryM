﻿namespace StrawberryM.Services
{
    /// <summary>
    /// audio focus 인터페이스
    /// </summary>
    public interface IAudioFocus
    {
        void RequestFocus();
        void ReleaseAudioResources();
    }
}
