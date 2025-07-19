#!/bin/bash

# VPet Linux Launcher Script

echo "Starting VPet Simulator - Linux Edition..."
echo "=========================================="

# Check if .NET 8.0 is installed
if ! command -v dotnet &> /dev/null; then
    echo "Error: .NET 8.0 runtime not found!"
    echo "Please install .NET 8.0 SDK:"
    echo "  Ubuntu/Debian: sudo apt-get install dotnet-sdk-8.0"
    echo "  Fedora: sudo dnf install dotnet-sdk-8.0"
    echo "  Arch: sudo pacman -S dotnet-sdk"
    exit 1
fi

# Check .NET version
DOTNET_VERSION=$(dotnet --version)
echo "Found .NET version: $DOTNET_VERSION"

# Check if we're in the right directory
if [ ! -f "VPet-Simulator.Avalonia/VPet-Simulator.Avalonia.csproj" ]; then
    echo "Error: Please run this script from the VPet root directory"
    exit 1
fi

# Build the project
echo "Building VPet..."
dotnet build VPet-Simulator.Avalonia/VPet-Simulator.Avalonia.csproj

if [ $? -ne 0 ]; then
    echo "Build failed!"
    exit 1
fi

echo "Build successful!"
echo ""
echo "Starting VPet Simulator..."
echo "Right-click on the pet for interactions!"
echo ""

# Run the application
dotnet run --project VPet-Simulator.Avalonia/VPet-Simulator.Avalonia.csproj