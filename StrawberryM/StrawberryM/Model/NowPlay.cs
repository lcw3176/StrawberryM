using System;
using Plugin.SimpleAudioPlayer;

namespace StrawberryM.Model
{
    /// <summary>
    /// 현재 재생되는 노래 데이터 클래스
    /// </summary>
    public static class NowPlay
    {
        public static ISimpleAudioPlayer Audio { get; set; } = CrossSimpleAudioPlayer.Current;
        public static string Title { get; set; } = string.Empty;
        public static TimeSpan NowTime { get; set; }
        public static TimeSpan TotalTime { get; set; }
        public static PlayMode playMode { get; set; } = PlayMode.Whole;

    }

    /// <summary>
    /// 재생방식 ( 한 곡 반복, 전체 반복, 임의 재생 )
    /// </summary>
    public enum PlayMode
    {
        One, Whole, Random
    }
}
