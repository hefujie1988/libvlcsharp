﻿#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("LibVLCSharp.Forms")]
[assembly: InternalsVisibleTo("LibVLCSharp.Uno")]

namespace LibVLCSharp.Shared.MediaPlayerElement
{
    /// <summary>
    /// Media player element manager
    /// </summary>
    /// <remarks>The <see cref="MediaPlayerElementManagerBase.MediaPlayer"/> property needs to be set in order to work.
    /// The <see cref="MediaPlayerElementManagerBase.VideoView"/> property needs to be set if the <see cref="AspectRatioManager"/> is used.
    /// The <see cref="MediaPlayerElementManagerBase.LibVLC"/> property needs to be set if the <see cref="CastRenderersDiscoverer"/> is used.
    /// </remarks>
    internal class MediaPlayerElementManager : MediaPlayerElementManagerBase
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MediaPlayerElementManager"/> class
        /// </summary>
        /// <param name="dispatcher">dispatcher</param>
        /// <param name="displayInformation">display information</param>
        /// <param name="displayRequest">display request object</param>
        public MediaPlayerElementManager(IDispatcher dispatcher, IDisplayInformation displayInformation, IDisplayRequest displayRequest)
            : base(dispatcher)
        {
            SubManagers = new MediaPlayerElementManagerBase[] {
               new AspectRatioManager(dispatcher, displayInformation),
               new AudioTracksManager(dispatcher),
               new AutoHideNotifier(dispatcher),
               new CastRenderersDiscoverer(dispatcher),
               new DeviceAwakeningManager(dispatcher, displayRequest),
               new SubtitlesTracksManager(dispatcher)
             };
        }

        private IEnumerable<MediaPlayerElementManagerBase> SubManagers { get; }

        /// <summary>
        /// Called when <see cref="MediaPlayerElementManagerBase.VideoView"/> property value changes
        /// </summary>
        /// <param name="oldValue">old value</param>
        /// <param name="newValue">new value</param>
        protected override void OnVideoViewChanged(IVideoControl? oldValue, IVideoControl? newValue)
        {
            base.OnVideoViewChanged(oldValue, newValue);
            foreach (var subManager in SubManagers)
            {
                subManager.VideoView = VideoView;
            }
        }

        /// <summary>
        /// Called when <see cref="LibVLC"/> property value changes
        /// </summary>
        /// <param name="oldValue">old value</param>
        /// <param name="newValue">new value</param>
        protected override void OnLibVLCChanged(LibVLC? oldValue, LibVLC? newValue)
        {
            base.OnLibVLCChanged(oldValue, newValue);
            foreach (var subManager in SubManagers)
            {
                subManager.LibVLC = LibVLC;
            }
        }

        /// <summary>
        /// Called when <see cref="MediaPlayerElementManagerBase.MediaPlayer"/> property value changes
        /// </summary>
        protected override void OnMediaPlayerChanged()
        {
            base.OnMediaPlayerChanged();
            foreach (var subManager in SubManagers)
            {
                subManager.MediaPlayer = MediaPlayer;
            }
        }

        /// <summary>
        /// Gets a manager of a given type
        /// </summary>
        /// <typeparam name="T">manager type</typeparam>
        /// <returns>the manager of the given type</returns>
        public T Get<T>() where T : MediaPlayerElementManagerBase
        {
            return SubManagers.OfType<T>().First();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources
        /// </summary>
        public override void Dispose()
        {
            foreach (var subManager in SubManagers)
            {
                subManager.Dispose();
            }
            base.Dispose();
        }
    }
}
#nullable restore