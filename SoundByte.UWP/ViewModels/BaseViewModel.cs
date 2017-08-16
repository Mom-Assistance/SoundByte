//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Popups;
using SoundByte.UWP.Services;
using SoundByte.Core.API.Endpoints;

namespace SoundByte.UWP.ViewModels
{
    /// <summary>
    /// Base class for all view models to extend off of
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged, IDisposable, IBaseViewModel, IBaseViewModel1
    {
        public BaseViewModel()
        {
        }

        /// <summary>
        /// The global playback service
        /// </summary>
        public PlaybackService Service => PlaybackService.Current;

        #region Property Changed Event Handlers
        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        protected void UpdateProperty([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        /// <summary>
        /// Performs a shuffle of the tracks
        /// </summary>
        /// <param name="tracks"></param>
        /// <param name="token"></param>
        public static async Task ShuffleTracksAsync(List<Track> tracks, string token)
        {
            App.IsLoading = true;

            var startPlayback = await PlaybackService.Current.StartMediaPlayback(tracks, token, true);

            if (!startPlayback.success)
                await new MessageDialog(startPlayback.message, "Error playing shuffled tracks.").ShowAsync();
           
            App.IsLoading = false;
        }

        /// <summary>
        /// Dispose the model
        /// </summary>
        public virtual void Dispose()
        {
            // On the base view model, we do nothing
}

        public override bool Equals(object obj)
        {
            var model = obj as BaseViewModel;
            return model != null &&
                   EqualityComparer<PlaybackService>.Default.Equals(Service, model.Service);
        }

        public override int GetHashCode()
        {
            return 1514353572 + EqualityComparer<PlaybackService>.Default.GetHashCode(Service);
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
