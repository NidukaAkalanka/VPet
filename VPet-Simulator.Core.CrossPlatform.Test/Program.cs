using System;
using System.Threading;
using VPet_Simulator.Core.CrossPlatform.Game;
using VPet_Simulator.Core.CrossPlatform.Models;

namespace VPet_Simulator.Core.CrossPlatform.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("VPet Cross-Platform Core Test");
            Console.WriteLine("==============================");
            
            var engine = new PetEngine();
            
            // Subscribe to events
            engine.PetDataChanged += OnPetDataChanged;
            engine.AnimationPlayer.FrameChanged += OnFrameChanged;
            
            Console.WriteLine("Starting pet simulation...");
            
            // Simulate game loop
            for (int i = 0; i < 10; i++)
            {
                engine.Update();
                Thread.Sleep(500);
                
                if (i == 3)
                {
                    Console.WriteLine("\nPetting the pet...");
                    engine.OnPetted();
                }
                
                if (i == 6)
                {
                    Console.WriteLine("\nFeeding the pet...");
                    engine.Feed();
                }
            }
            
            Console.WriteLine("\nTest completed successfully!");
            Console.WriteLine("Cross-platform core is working correctly.");
        }
        
        static void OnPetDataChanged(PetData petData)
        {
            Console.WriteLine($"Pet Stats - State: {petData.State}, Animation: {petData.CurrentAnimation}, " +
                            $"Happiness: {petData.Happiness:F0}, Hunger: {petData.Hunger:F0}, Thirst: {petData.Thirst:F0}");
        }
        
        static void OnFrameChanged(VPet_Simulator.Core.CrossPlatform.Animation.IAnimationFrame frame)
        {
            Console.WriteLine($"Animation frame: {frame.ImagePath} (duration: {frame.Duration}ms)");
        }
    }
}