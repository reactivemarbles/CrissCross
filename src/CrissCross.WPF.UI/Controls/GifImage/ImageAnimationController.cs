// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Media.Animation;

namespace CrissCross.WPF.UI.Controls
{
    /// <summary>
    /// Provides a way to pause, resume or seek a GIF animation.
    /// </summary>
    internal class ImageAnimationController : IDisposable
    {
        private static readonly DependencyPropertyDescriptor _sourceDescriptor;

        private readonly Image _image;
        private readonly ObjectAnimationUsingKeyFrames _animation;
        private readonly AnimationClock _clock;
        private readonly ClockController _clockController;

        private bool _isSuspended;

        static ImageAnimationController() => _sourceDescriptor = DependencyPropertyDescriptor.FromProperty(Image.SourceProperty, typeof(Image));

        internal ImageAnimationController(Image image, ObjectAnimationUsingKeyFrames animation, bool autoStart)
        {
            _image = image;
            _animation = animation;
            _animation.Completed += AnimationCompleted;
            _clock = _animation.CreateClock();
            _clockController = _clock.Controller;
            _sourceDescriptor.AddValueChanged(image, ImageSourceChanged);

            // ReSharper disable once PossibleNullReferenceException
            _clockController.Pause();

            _image.ApplyAnimationClock(Image.SourceProperty, _clock);

            IsPaused = !autoStart;
            if (autoStart)
            {
                _clockController.Resume();
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ImageAnimationController"/> class.
        /// </summary>
        ~ImageAnimationController()
        {
            Dispose(false);
        }

        /// <summary>
        /// Raised when the current frame changes.
        /// </summary>
        public event EventHandler? CurrentFrameChanged;

        /// <summary>
        /// Gets the number of frames in the image.
        /// </summary>
        public int FrameCount => _animation.KeyFrames.Count;

        /// <summary>
        /// Gets the duration of the animation.
        /// </summary>
        public TimeSpan Duration => _animation.Duration.HasTimeSpan
                  ? _animation.Duration.TimeSpan
                  : TimeSpan.Zero;

        /// <summary>
        /// Gets a value indicating whether returns a value that indicates whether the animation is paused.
        /// </summary>
        public bool IsPaused { get; private set; }

        /// <summary>
        /// Gets a value indicating whether returns a value that indicates whether the animation is complete.
        /// </summary>
        public bool IsComplete => _clock.CurrentState == ClockState.Filling;

        /// <summary>
        /// Gets the current frame index.
        /// </summary>
        public int CurrentFrame
        {
            get
            {
                var time = _clock.CurrentTime;
                var frameAndIndex =
                    _animation.KeyFrames
                              .Cast<ObjectKeyFrame>()
                              .Select((f, i) => new { Time = f.KeyTime.TimeSpan, Index = i })
                              .FirstOrDefault(fi => fi.Time >= time);
                if (frameAndIndex != null)
                {
                    return frameAndIndex.Index;
                }

                return -1;
            }
        }

        /// <summary>
        /// Seeks the animation to the specified frame index.
        /// </summary>
        /// <param name="index">The index of the frame to seek to.</param>
        public void GotoFrame(int index)
        {
            var frame = _animation.KeyFrames[index];
            _clockController.Seek(frame.KeyTime.TimeSpan, TimeSeekOrigin.BeginTime);
        }

        /// <summary>
        /// Pauses the animation.
        /// </summary>
        public void Pause()
        {
            IsPaused = true;
            _clockController.Pause();
        }

        /// <summary>
        /// Starts or resumes the animation. If the animation is complete, it restarts from the beginning.
        /// </summary>
        public void Play()
        {
            IsPaused = false;
            if (!_isSuspended)
            {
                _clockController.Resume();
            }
        }

        /// <summary>
        /// Disposes the current object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal void SetSuspended(bool isSuspended)
        {
            if (isSuspended == _isSuspended)
            {
                return;
            }

            var wasSuspended = _isSuspended;
            _isSuspended = isSuspended;
            if (wasSuspended)
            {
                if (!IsPaused)
                {
                    _clockController.Resume();
                }
            }
            else
            {
                _clockController.Pause();
            }
        }

        /// <summary>
        /// Disposes the current object.
        /// </summary>
        /// <param name="disposing">true to dispose both managed an unmanaged resources, false to dispose only managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _image.BeginAnimation(Image.SourceProperty, null);
                _animation.Completed -= AnimationCompleted;
                _sourceDescriptor.RemoveValueChanged(_image, ImageSourceChanged);
                _image.Source = null!;
            }
        }

        private void AnimationCompleted(object? sender, EventArgs e) => _image.RaiseEvent(new System.Windows.RoutedEventArgs(ImageBehavior.AnimationCompletedEvent, _image));

        private void ImageSourceChanged(object? sender, EventArgs e) => OnCurrentFrameChanged();

        private void OnCurrentFrameChanged() => CurrentFrameChanged?.Invoke(this, EventArgs.Empty);
    }
}
