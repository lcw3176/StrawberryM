using StrawberryM.Model;
using Xamarin.Forms;

namespace StrawberryM.Services
{
    class PropertyService
    {
        public void SetLastInfo(object playMode)
        {
            SetDataToProperties(FileState.PlayMode.ToString(), playMode);
            //SetDataToProperties(FileState.SongName.ToString(), songName);
            //SetDataToProperties(FileState.Time.ToString(), time);
        }

        public object GetDataFromProperties(string key)
        {

            if (!Application.Current.Properties.ContainsKey(key))
            {
                return null;
            }

            Application.Current.Properties.TryGetValue(key, out object value);

            return value;
        }

        private void SetDataToProperties(string key, object value)
        {
            if (value == null)
            {
                return;
            }

            if (Application.Current.Properties.ContainsKey(key))
            {
                Application.Current.Properties[key] = value;
            }

            else
            {
                Application.Current.Properties.Add(key, value);
            }
        }
    }
}
