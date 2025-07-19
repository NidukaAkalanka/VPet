# VPet Simulator - Linux Edition

A cross-platform port of the Virtual Pet Simulator using Avalonia UI, making it compatible with Linux systems.

## Features

- **Cross-platform**: Runs on Linux with modern GUI environments (KDE, GNOME, X11)
- **Core pet simulation**: Pet stats (hunger, thirst, happiness) with real-time updates
- **Basic animations**: State-based animation system (idle, happy, sad, etc.)
- **Interactive**: Click and drag to move, right-click to pet, context menu for feeding
- **Modern UI**: Built with Avalonia UI for native performance

## Requirements

- .NET 8.0 Runtime
- Linux with X11 or Wayland desktop environment
- Modern GPU with OpenGL support (for hardware acceleration)

## Installation

### From Source

1. Install .NET 8.0 SDK:
   ```bash
   # Ubuntu/Debian
   sudo apt-get update
   sudo apt-get install -y dotnet-sdk-8.0
   
   # Fedora
   sudo dnf install dotnet-sdk-8.0
   
   # Arch Linux
   sudo pacman -S dotnet-sdk
   ```

2. Clone and build:
   ```bash
   git clone https://github.com/NidukaAkalanka/VPet.git
   cd VPet
   dotnet build VPet-Simulator.Avalonia/VPet-Simulator.Avalonia.csproj
   ```

3. Run:
   ```bash
   dotnet run --project VPet-Simulator.Avalonia/VPet-Simulator.Avalonia.csproj
   ```

## Usage

### Basic Interactions

- **Left Click + Drag**: Move the pet around the screen
- **Right Click**: Pet the virtual pet (increases happiness)
- **Right Click + Context Menu**: 
  - Feed Pet: Increases hunger stat
  - Give Water: Increases thirst stat  
  - Pet: Same as right-click
  - Exit: Close the application

### Pet Stats

The pet has three main stats that decay over time:
- **Hunger**: Decreases slowly, feed to restore
- **Thirst**: Decreases slowly, give water to restore  
- **Happiness**: Affected by hunger/thirst levels and petting

### Pet States

Based on stats, the pet will be in different states:
- **Normal**: Default state when stats are balanced
- **Happy**: When happiness is high (>70)
- **Unhappy**: When happiness is low (<30)
- **Sleep**: Special state (not yet implemented)

## Technical Details

### Architecture

The Linux port uses a modular architecture:

- **VPet-Simulator.Core.CrossPlatform**: Platform-independent game logic
- **VPet-Simulator.Avalonia**: Avalonia UI frontend for Linux/macOS/Windows

### Key Changes from Windows Version

- **UI Framework**: WPF → Avalonia UI
- **Graphics**: DirectX → OpenGL (via SkiaSharp)
- **Target Framework**: net8.0-windows → net8.0
- **Dependencies**: Removed Windows-specific packages

### Animation System

The animation system uses a frame-based approach:
- `AnimationSequence`: Collection of frames for a specific action
- `AnimationPlayer`: Handles playback and timing
- `PetEngine`: Manages state transitions and animation selection

## Development

### Building

```bash
# Build cross-platform core
dotnet build VPet-Simulator.Core.CrossPlatform/

# Build Avalonia UI
dotnet build VPet-Simulator.Avalonia/

# Test core functionality
dotnet run --project VPet-Simulator.Core.CrossPlatform.Test/
```

### Adding Animations

1. Create animation frames (PNG images)
2. Add them to `Assets/animations/` folder
3. Update `PetEngine.LoadDefaultAnimations()` method
4. Implement image loading in `MainWindow.UpdatePetDisplay()`

## Limitations

This is a basic port focused on core functionality:

- **Limited animations**: Currently uses placeholder animations
- **No mod support**: MOD system not yet ported
- **Basic graphics**: Simplified compared to original Windows version
- **No advanced features**: Steam integration, achievements, etc. not included

## Contributing

This port demonstrates the feasibility of cross-platform VPet. To extend it:

1. Add actual pet animation assets
2. Implement image loading and display
3. Port additional features from the original Windows version
4. Add Linux-specific optimizations

## License

Same as the original VPet project. See [LICENSE](../LICENSE) for details.

Animation assets remain under the original license terms.