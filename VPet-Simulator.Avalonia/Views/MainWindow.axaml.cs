using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.IO;
using VPet_Simulator.Core.CrossPlatform.Game;
using VPet_Simulator.Core.CrossPlatform.Models;
using VPet_Simulator.Core.CrossPlatform.Animation;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace VPet_Simulator.Avalonia.Views;

public partial class MainWindow : Window
{
    private PetEngine _petEngine;
    private DispatcherTimer _gameTimer;
    private bool _isDragging = false;
    private Point _lastPointerPosition;

    public MainWindow()
    {
        InitializeComponent();
        InitializePetEngine();
        SetupWindow();
    }

    private void InitializePetEngine()
    {
        _petEngine = new PetEngine();
        _petEngine.PetDataChanged += OnPetDataChanged;
        _petEngine.AnimationPlayer.FrameChanged += OnAnimationFrameChanged;

        // Setup game update timer
        _gameTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(50) // 20 FPS
        };
        _gameTimer.Tick += (s, e) => _petEngine.Update();
        _gameTimer.Start();
    }

    private void SetupWindow()
    {
        // Make window click-through background but not the pet
        Background = Brushes.Transparent;
        
        // Setup interaction events
        PointerPressed += OnPointerPressed;
        PointerMoved += OnPointerMoved;
        PointerReleased += OnPointerReleased;
    }

    private void OnPetDataChanged(PetData petData)
    {
        // Update UI based on pet data changes
        // For now, just update the debug text
        if (PetDisplayBorder.Child is TextBlock textBlock)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                textBlock.Text = $"VPet - Linux Edition\n" +
                               $"State: {petData.State}\n" +
                               $"Animation: {petData.CurrentAnimation}\n" +
                               $"Happiness: {petData.Happiness:F0}\n" +
                               $"Hunger: {petData.Hunger:F0}\n" +
                               $"Thirst: {petData.Thirst:F0}";
            });
        }
    }

    private void OnAnimationFrameChanged(IAnimationFrame frame)
    {
        // TODO: Load and display the actual animation frame
        // For now, we'll just show a placeholder
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            // In a real implementation, you would load frame.ImagePath here
            UpdatePetDisplay(frame.ImagePath);
        });
    }

    private void UpdatePetDisplay(string imagePath)
    {
        // Placeholder implementation - in real code, load the image
        // For now, change background color based on animation state
        var brush = _petEngine.Pet.State switch
        {
            PetState.Happy => Brushes.LightGreen,
            PetState.Unhappy => Brushes.LightCoral,
            PetState.Sleep => Brushes.LightBlue,
            _ => Brushes.LightGray
        };

        if (PetDisplayBorder.Child is TextBlock textBlock)
        {
            textBlock.Background = brush;
        }
    }

    private void OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        var position = e.GetPosition(this);
        _lastPointerPosition = new Point(position.X, position.Y);

        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            // Start dragging
            _isDragging = true;
            this.BeginMoveDrag(e);
        }
        else if (e.GetCurrentPoint(this).Properties.IsRightButtonPressed)
        {
            // Right click - pet the animal
            _petEngine.OnPetted();
        }
    }

    private void OnPointerMoved(object sender, PointerEventArgs e)
    {
        if (_isDragging)
        {
            // Update pet position for engine
            var position = e.GetPosition(this);
            _petEngine.MoveTo(new Point(position.X, position.Y));
        }
    }

    private void OnPointerReleased(object sender, PointerReleasedEventArgs e)
    {
        _isDragging = false;
    }

    protected override void OnClosed(EventArgs e)
    {
        _gameTimer?.Stop();
        base.OnClosed(e);
    }
}