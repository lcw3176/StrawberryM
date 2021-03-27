using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;

namespace StrawberryM.Services
{
    class RankService
    {
        private const string totalUrl = "https://www.melon.com/chart/day/index.htm";
        private const string foreignUrl = "https://www.melon.com/chart/day/index.htm?classCd=AB0000";
        private const string trotUrl = "https://www.melon.com/chart/day/index.htm?classCd=GN0700";

        public async Task<Dictionary<int, string>> GetRankFromUrl(string gerne)
        {
            string url = string.Empty;
            

            if (gerne == "total")
            {
                url = totalUrl;
            }

            else if(gerne == "foreign")
            {
                url = foreignUrl;
            }

            else if(gerne == "trot")
            {
                url = trotUrl;
            }

            
            var result = await Task.Run(() => Process(url));

            return result;
        }


        private Dictionary<int, string> Process(string processedUrl)
        {
            try
            {
                Dictionary<int, string> dic = new Dictionary<int, string>();

                using (HttpClient http = new HttpClient())
                {
                    using (HttpResponseMessage respone = http.GetAsync(processedUrl).Result)
                    {
                        using (HttpContent content = respone.Content)
                        {
                            string html = content.ReadAsStringAsync().Result;

                            HtmlDocument doc = new HtmlDocument();
                            doc.LoadHtml(html);

                            HtmlNodeCollection songCollections = doc.DocumentNode.SelectNodes("//div[contains(@class, 'ellipsis rank01')]/span/a");
                            HtmlNodeCollection singerCollections = doc.DocumentNode.SelectNodes("//span[contains(@class, 'checkEllipsis')]");

                            List<string> songList = new List<string>();
                            List<string> singerList = new List<string>();

                            foreach (var i in songCollections)
                            {
                                songList.Add(HttpUtility.HtmlDecode(i.InnerText).Trim());
                            }

                            foreach (var i in singerCollections)
                            {
                                singerList.Add(HttpUtility.HtmlDecode(i.InnerText).Trim());
                            }



                            for (int i = 0; i < songList.Count; i++)
                            {
                                dic[i] = singerList[i] + "<divided>" + songList[i];
                            }

                        }
                    }
                }
                return dic;

            }

            catch
            {
                return null;
            }
        }
    }
}
