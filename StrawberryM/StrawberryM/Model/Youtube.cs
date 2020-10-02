using Xamarin.Forms;

namespace StrawberryM.Model
{
    /// <summary>
    /// youtube 검색 결과 데이터 클래스
    /// </summary>
    class Youtube
    {
        public string id { get; set; }
        public string title { get; set; }
        public Command downloadCommand { get; set; }
    }
}
