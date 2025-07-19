namespace VPet_Simulator.Core.CrossPlatform.Models
{
    /// <summary>
    /// Animation types for the pet
    /// </summary>
    public enum AnimationType
    {
        /// <summary>
        /// Normal idle state
        /// </summary>
        Idle,
        
        /// <summary>
        /// Happy state
        /// </summary>
        Happy,
        
        /// <summary>
        /// Sad state  
        /// </summary>
        Sad,
        
        /// <summary>
        /// Eating animation
        /// </summary>
        Eating,
        
        /// <summary>
        /// Sleeping animation
        /// </summary>
        Sleeping,
        
        /// <summary>
        /// Walking animation
        /// </summary>
        Walking,
        
        /// <summary>
        /// Being petted animation
        /// </summary>
        Petted,
        
        /// <summary>
        /// Being dragged animation
        /// </summary>
        Dragged
    }

    /// <summary>
    /// Pet state information
    /// </summary>
    public enum PetState
    {
        /// <summary>
        /// Normal/Healthy
        /// </summary>
        Normal,
        
        /// <summary>
        /// Happy
        /// </summary>
        Happy,
        
        /// <summary>
        /// Unhappy
        /// </summary>
        Unhappy,
        
        /// <summary>
        /// Sleeping
        /// </summary>
        Sleep
    }

    /// <summary>
    /// Basic pet information
    /// </summary>
    public class PetData
    {
        public string Name { get; set; } = "VPet";
        public double Health { get; set; } = 100;
        public double Happiness { get; set; } = 50;
        public double Hunger { get; set; } = 50;
        public double Thirst { get; set; } = 50;
        public PetState State { get; set; } = PetState.Normal;
        public AnimationType CurrentAnimation { get; set; } = AnimationType.Idle;
    }
}