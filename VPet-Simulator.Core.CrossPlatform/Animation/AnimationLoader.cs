using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VPet_Simulator.Core.CrossPlatform.Models;

namespace VPet_Simulator.Core.CrossPlatform.Animation
{
    /// <summary>
    /// Loads animations from the Windows VPet animation structure
    /// </summary>
    public class AnimationLoader
    {
        private readonly string _animationBasePath;

        public AnimationLoader(string animationBasePath)
        {
            _animationBasePath = animationBasePath;
        }

        /// <summary>
        /// Load all available animations from the asset directory
        /// </summary>
        public Dictionary<AnimationType, AnimationSequence> LoadAnimations()
        {
            var animations = new Dictionary<AnimationType, AnimationSequence>();

            // Load Idle animations
            var idleSequence = LoadAnimationFromDirectory(Path.Combine(_animationBasePath, "Idle"), AnimationType.Idle, "idle");
            if (idleSequence != null)
                animations[AnimationType.Idle] = idleSequence;

            // Load Happy animations
            var happySequence = LoadAnimationFromDirectory(Path.Combine(_animationBasePath, "Happy"), AnimationType.Happy, "happy");
            if (happySequence != null)
                animations[AnimationType.Happy] = happySequence;

            // Load Sad animations (fallback to idle if not available)
            var sadSequence = LoadAnimationFromDirectory(Path.Combine(_animationBasePath, "Sad"), AnimationType.Sad, "sad");
            if (sadSequence != null)
                animations[AnimationType.Sad] = sadSequence;
            else if (idleSequence != null)
                animations[AnimationType.Sad] = idleSequence; // Fallback

            // Load Eating animations
            var eatingSequence = LoadAnimationFromDirectory(Path.Combine(_animationBasePath, "Eating"), AnimationType.Eating, "eating");
            if (eatingSequence != null)
                animations[AnimationType.Eating] = eatingSequence;

            // Load Petted animations
            var pettedSequence = LoadAnimationFromDirectory(Path.Combine(_animationBasePath, "Petted"), AnimationType.Petted, "petted");
            if (pettedSequence != null)
                animations[AnimationType.Petted] = pettedSequence;

            // Add basic fallbacks if no animations were loaded
            if (animations.Count == 0)
            {
                animations[AnimationType.Idle] = CreateFallbackAnimation(AnimationType.Idle);
            }

            return animations;
        }

        /// <summary>
        /// Load animation sequence from a directory containing PNG files
        /// </summary>
        private AnimationSequence LoadAnimationFromDirectory(string directoryPath, AnimationType animationType, string name)
        {
            if (!Directory.Exists(directoryPath))
                return null;

            var pngFiles = Directory.GetFiles(directoryPath, "*.png")
                                  .OrderBy(f => Path.GetFileName(f))
                                  .ToArray();

            if (pngFiles.Length == 0)
                return null;

            var sequence = new AnimationSequence(animationType, name);

            foreach (var pngFile in pngFiles)
            {
                var fileName = Path.GetFileName(pngFile);
                var duration = ExtractDurationFromFileName(fileName);
                
                // Use relative path for the animation system
                var relativePath = Path.Combine("Assets", "Animations", 
                    Path.GetFileName(directoryPath), fileName);
                
                sequence.AddFrame(relativePath, duration);
            }

            return sequence.FrameCount > 0 ? sequence : null;
        }

        /// <summary>
        /// Extract duration from filename pattern: name_frame_duration.png
        /// </summary>
        private int ExtractDurationFromFileName(string fileName)
        {
            // Pattern: xxxxx_###_###.png where last number is duration in milliseconds
            var nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            var parts = nameWithoutExt.Split('_');
            
            if (parts.Length >= 3)
            {
                if (int.TryParse(parts[parts.Length - 1], out int duration))
                {
                    return duration;
                }
            }

            // Default duration if parsing fails
            return 125;
        }

        /// <summary>
        /// Create a simple fallback animation for testing
        /// </summary>
        private AnimationSequence CreateFallbackAnimation(AnimationType animationType)
        {
            var sequence = new AnimationSequence(animationType, "fallback");
            // Add a single dummy frame with longer duration
            sequence.AddFrame("placeholder.png", 2000);
            return sequence;
        }
    }
}