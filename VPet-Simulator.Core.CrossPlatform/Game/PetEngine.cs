using System;
using System.Collections.Generic;
using VPet_Simulator.Core.CrossPlatform.Models;
using VPet_Simulator.Core.CrossPlatform.Animation;

namespace VPet_Simulator.Core.CrossPlatform.Game
{
    /// <summary>
    /// Core game engine for the virtual pet
    /// </summary>
    public class PetEngine
    {
        private PetData _petData;
        private AnimationPlayer _animationPlayer;
        private Dictionary<AnimationType, AnimationSequence> _animations;
        private DateTime _lastUpdate;
        
        public PetData Pet => _petData;
        public AnimationPlayer AnimationPlayer => _animationPlayer;
        public Point Position { get; set; }
        public Size Size { get; set; } = new Size(250, 250);

        public event Action<PetData> PetDataChanged;
        public event Action<Point> PositionChanged;

        public PetEngine()
        {
            _petData = new PetData();
            _animationPlayer = new AnimationPlayer();
            _animations = new Dictionary<AnimationType, AnimationSequence>();
            _lastUpdate = DateTime.Now;
            
            // Initialize with basic idle animation
            LoadDefaultAnimations();
            
            // Start with idle animation
            PlayAnimation(AnimationType.Idle);
        }

        private void LoadDefaultAnimations()
        {
            // Try to load real animations first
            try
            {
                var animationLoader = new AnimationLoader("Assets/Animations");
                var loadedAnimations = animationLoader.LoadAnimations();
                
                if (loadedAnimations.Count > 0)
                {
                    _animations = loadedAnimations;
                    return;
                }
            }
            catch (Exception ex)
            {
                // Log error but continue with fallback
                System.Diagnostics.Debug.WriteLine($"Failed to load animations: {ex.Message}");
            }

            // Fallback to placeholder animations if loading fails
            var idleAnim = new AnimationSequence(AnimationType.Idle, "idle");
            idleAnim.AddFrame("placeholder.png", 1000);
            _animations[AnimationType.Idle] = idleAnim;

            var happyAnim = new AnimationSequence(AnimationType.Happy, "happy");
            happyAnim.AddFrame("placeholder.png", 500);
            _animations[AnimationType.Happy] = happyAnim;

            var sadAnim = new AnimationSequence(AnimationType.Sad, "sad");
            sadAnim.AddFrame("placeholder.png", 800);
            _animations[AnimationType.Sad] = sadAnim;
        }

        public void LoadAnimations(Dictionary<AnimationType, AnimationSequence> animations)
        {
            _animations = animations;
        }

        public void PlayAnimation(AnimationType type)
        {
            if (_animations.TryGetValue(type, out var sequence))
            {
                _animationPlayer.PlayAnimation(sequence);
                _petData.CurrentAnimation = type;
                PetDataChanged?.Invoke(_petData);
            }
        }

        public void Update()
        {
            var now = DateTime.Now;
            var deltaTime = (now - _lastUpdate).TotalSeconds;
            _lastUpdate = now;

            // Update animation
            _animationPlayer.Update();

            // Update pet stats (simple decay)
            UpdatePetStats(deltaTime);

            // Auto-change animations based on state
            UpdateAnimationFromState();
        }

        private void UpdatePetStats(double deltaTime)
        {
            // Simple stat decay over time
            var decayRate = 1.0; // points per second

            _petData.Hunger = Math.Max(0, _petData.Hunger - decayRate * deltaTime);
            _petData.Thirst = Math.Max(0, _petData.Thirst - decayRate * deltaTime);

            // Update happiness based on hunger/thirst
            if (_petData.Hunger < 20 || _petData.Thirst < 20)
            {
                _petData.Happiness = Math.Max(0, _petData.Happiness - decayRate * deltaTime);
            }
            else if (_petData.Hunger > 80 && _petData.Thirst > 80)
            {
                _petData.Happiness = Math.Min(100, _petData.Happiness + decayRate * 0.5 * deltaTime);
            }

            // Update state based on stats
            UpdatePetState();
            
            PetDataChanged?.Invoke(_petData);
        }

        private void UpdatePetState()
        {
            if (_petData.Happiness < 30)
                _petData.State = PetState.Unhappy;
            else if (_petData.Happiness > 70)
                _petData.State = PetState.Happy;
            else
                _petData.State = PetState.Normal;
        }

        private void UpdateAnimationFromState()
        {
            // Auto-switch animations based on pet state
            var targetAnimation = _petData.State switch
            {
                PetState.Happy => AnimationType.Happy,
                PetState.Unhappy => AnimationType.Sad,
                PetState.Sleep => AnimationType.Sleeping,
                _ => AnimationType.Idle
            };

            if (_petData.CurrentAnimation != targetAnimation)
            {
                PlayAnimation(targetAnimation);
            }
        }

        public void MoveTo(Point newPosition)
        {
            Position = newPosition;
            PositionChanged?.Invoke(Position);
        }

        public void OnPetted()
        {
            _petData.Happiness = Math.Min(100, _petData.Happiness + 10);
            PlayAnimation(AnimationType.Petted);
            PetDataChanged?.Invoke(_petData);
            
            // Return to normal animation after a delay
            // Note: In a real implementation, you'd use a timer for this
        }

        public void Feed()
        {
            _petData.Hunger = Math.Min(100, _petData.Hunger + 25);
            _petData.Happiness = Math.Min(100, _petData.Happiness + 5);
            PlayAnimation(AnimationType.Eating);
            PetDataChanged?.Invoke(_petData);
        }

        public void GiveWater()
        {
            _petData.Thirst = Math.Min(100, _petData.Thirst + 25);
            _petData.Happiness = Math.Min(100, _petData.Happiness + 5);
            PetDataChanged?.Invoke(_petData);
        }
    }
}