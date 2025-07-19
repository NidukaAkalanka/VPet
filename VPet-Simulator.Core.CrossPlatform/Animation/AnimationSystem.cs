using System;
using System.Collections.Generic;
using VPet_Simulator.Core.CrossPlatform.Models;

namespace VPet_Simulator.Core.CrossPlatform.Animation
{
    /// <summary>
    /// Basic animation frame interface
    /// </summary>
    public interface IAnimationFrame
    {
        /// <summary>
        /// Duration of this frame in milliseconds
        /// </summary>
        int Duration { get; }
        
        /// <summary>
        /// Path to the image file for this frame
        /// </summary>
        string ImagePath { get; }
    }

    /// <summary>
    /// Simple animation frame implementation
    /// </summary>
    public class AnimationFrame : IAnimationFrame
    {
        public int Duration { get; set; }
        public string ImagePath { get; set; }

        public AnimationFrame(string imagePath, int duration = 100)
        {
            ImagePath = imagePath;
            Duration = duration;
        }
    }

    /// <summary>
    /// Animation sequence for a specific pet action
    /// </summary>
    public class AnimationSequence
    {
        public AnimationType Type { get; set; }
        public List<IAnimationFrame> Frames { get; set; } = new List<IAnimationFrame>();
        public bool IsLooping { get; set; } = true;
        public string Name { get; set; } = "";

        public AnimationSequence(AnimationType type, string name = "")
        {
            Type = type;
            Name = name;
        }

        public void AddFrame(string imagePath, int duration = 100)
        {
            Frames.Add(new AnimationFrame(imagePath, duration));
        }

        public IAnimationFrame GetFrame(int index)
        {
            if (Frames.Count == 0) return null;
            return Frames[index % Frames.Count];
        }

        public int FrameCount => Frames.Count;
    }

    /// <summary>
    /// Animation player for handling pet animations
    /// </summary>
    public class AnimationPlayer
    {
        private AnimationSequence _currentSequence;
        private int _currentFrameIndex;
        private DateTime _lastFrameTime;
        
        public AnimationSequence CurrentSequence => _currentSequence;
        public int CurrentFrameIndex => _currentFrameIndex;
        public IAnimationFrame CurrentFrame => _currentSequence?.GetFrame(_currentFrameIndex);

        public event Action<IAnimationFrame> FrameChanged;
        public event Action<AnimationSequence> AnimationChanged;

        public void PlayAnimation(AnimationSequence sequence)
        {
            if (_currentSequence != sequence)
            {
                _currentSequence = sequence;
                _currentFrameIndex = 0;
                _lastFrameTime = DateTime.Now;
                AnimationChanged?.Invoke(sequence);
                FrameChanged?.Invoke(CurrentFrame);
            }
        }

        public void Update()
        {
            if (_currentSequence == null || _currentSequence.FrameCount == 0)
                return;

            var now = DateTime.Now;
            var elapsed = (now - _lastFrameTime).TotalMilliseconds;
            
            if (elapsed >= CurrentFrame.Duration)
            {
                _currentFrameIndex++;
                
                if (_currentFrameIndex >= _currentSequence.FrameCount)
                {
                    if (_currentSequence.IsLooping)
                    {
                        _currentFrameIndex = 0;
                    }
                    else
                    {
                        _currentFrameIndex = _currentSequence.FrameCount - 1;
                        return; // Stop at last frame
                    }
                }
                
                _lastFrameTime = now;
                FrameChanged?.Invoke(CurrentFrame);
            }
        }
    }
}