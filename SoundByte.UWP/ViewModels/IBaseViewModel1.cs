using System.ComponentModel;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.ViewModels
{
    public interface IBaseViewModel1
    {
        PlaybackService Service { get; }

        event PropertyChangedEventHandler PropertyChanged;

        void Dispose();
        bool Equals(object obj);
        int GetHashCode();
        string ToString();
    }
}