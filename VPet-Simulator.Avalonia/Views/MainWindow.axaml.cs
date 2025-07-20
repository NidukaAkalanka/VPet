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
using Avalonia.Media.Imaging;
using Avalonia.Platform;

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
        // Update the debug text overlay
        var debugText = this.FindControl<TextBlock>("DebugText");
        if (debugText != null)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                debugText.Text = $"VPet - Linux Edition\n" +
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
        // Load and display the actual animation frame
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            LoadAnimationFrame(frame.ImagePath);
        });
    }

    private void LoadAnimationFrame(string imagePath)
    {
        try
        {
            var petImage = this.FindControl<Image>("PetImage");
            if (petImage == null) return;

            // Skip placeholder frames
            if (imagePath == "placeholder.png")
            {
                petImage.Source = null;
                return;
            }

            // Construct the full path to the animation file
            var fullPath = Path.Combine(AppContext.BaseDirectory, imagePath);
            
            if (File.Exists(fullPath))
            {
                using (var stream = File.OpenRead(fullPath))
                {
                    var bitmap = new Bitmap(stream);
                    petImage.Source = bitmap;
                }
            }
            else
            {
                // Try as an embedded resource
                var uri = new Uri($"avares://VPet-Simulator.Avalonia/{imagePath}");
                var stream = AssetLoader.Open(uri);
                if (stream != null)
                {
                    var bitmap = new Bitmap(stream);
                    petImage.Source = bitmap;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load animation frame {imagePath}: {ex.Message}");
        }
    }

    private void UpdatePetDisplay(string imagePath)
    {
        // This method is now handled by LoadAnimationFrame
        // Keep it for backward compatibility but just call LoadAnimationFrame
        LoadAnimationFrame(imagePath);
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

    // Context menu event handlers
    private void FeedMenuItem_Click(object sender, RoutedEventArgs e)
    {
        _petEngine.Feed();
    }

    private void WaterMenuItem_Click(object sender, RoutedEventArgs e)
    {
        _petEngine.GiveWater();
    }

    private void PetMenuItem_Click(object sender, RoutedEventArgs e)
    {
        _petEngine.OnPetted();
    }

    private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}