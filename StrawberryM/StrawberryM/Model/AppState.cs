namespace StrawberryM.Model
{
    /// <summary>
    /// 현재 앱 상태 데이터 클래스, notification 표시 여부를 위해 사용
    /// </summary>
    public static class NowAppState
    {
        public static AppState state { get; set; } = AppState.onStart;
    }

    /// <summary>
    /// 앱 상태 ( onSleep, onClose : 백그라운드 // onStart, onResume : 시작 )
    /// </summary>

    public enum AppState
    {
        onSleep, onStart, onClose, onResume
    }
}
