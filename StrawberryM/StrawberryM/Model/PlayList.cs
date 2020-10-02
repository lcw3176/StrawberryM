using Xamarin.Forms;

namespace StrawberryM.Model
{
    /// <summary>
    /// 유저 플레이 리스트 데이터 클래스
    /// </summary>
    public class PlayList
    {
        public int index { get; set; }
        public string name { get; set; }
        public Command playCommand { get; set; }
        public Command deleteCommand { get; set; }
    }
}
