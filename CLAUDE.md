# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity 3D game project called "Charge" - a 3D platformer featuring a battery-powered robot character. The game uses Unity 6000.2.0f1 with Universal Render Pipeline (URP) for rendering and Unity's new Input System.

## Architecture

### Player System
The player system is built using a ScriptableObject-based architecture:
- `PlayerData.cs`: ScriptableObject containing all player configuration (movement speeds, battery settings, health)
- `PlayerController.cs`: Handles movement, animation, and input processing
- `PlayerBatteryManager.cs`: Manages battery drain mechanics and UI
- Player data asset instance: `Assets/Scripts/Player/PlayerData.asset`

### Core Components
- **Movement**: Physics-based using Rigidbody with configurable speeds for walking/running
- **Battery System**: Drains based on activity (idle/move/sprint) and kills player when depleted
- **Animation**: Integrated with Unity's Animator for movement states
- **Platform Interactions**: `PlatformTrigger.cs` handles triggered platform animations

### Unity Packages
Key packages in use:
- Universal Render Pipeline (URP) 17.2.0
- New Input System 1.14.1 (though legacy input is still used in code)
- AI Navigation 2.0.8
- Visual Scripting 1.9.7

## Development Commands

Since this is a Unity project, development is primarily done through the Unity Editor rather than command-line tools. Key workflows:

### Building
- Open project in Unity Editor
- Use Build Settings (Ctrl+Shift+B) to configure and build
- Current build settings are in `ProjectSettings/EditorBuildSettings.asset`

### Testing
- Use Unity Test Runner (Window > General > Test Runner)
- Play Mode tests run within the editor

### Scene Structure
Main scene: `Assets/Scenes/Charge test.unity`
- Contains player prefab, platforms, and lighting setup
- Uses URP with custom lighting data

## Code Conventions

- Uses Unity's coding conventions
- ScriptableObject pattern for data configuration
- Event-driven architecture (see `OnPlayerDead` event in PlayerController)
- Physics-based movement with Rigidbody
- Component-based design following Unity patterns

## Known Technical Debt

- PlayerController uses old Input System (comments indicate need to migrate to new Input System)
- Debug logs still present in TakeInputs() method
- Battery drain for jumping not yet implemented
- String-based tag comparison in PlatformTrigger (should use CompareTag)