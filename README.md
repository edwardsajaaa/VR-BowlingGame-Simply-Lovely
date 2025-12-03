# VR Bowling Game - Simply Lovely 🎳

A Virtual Reality Bowling Game built with Unity and XR Interaction Toolkit.

## 📋 Overview

This is a VR bowling game that allows players to experience realistic bowling in virtual reality. The game features physics-based ball throwing, pin collision detection, and full scoring according to standard bowling rules.

## 🎮 Features

- **VR Interaction**: Pick up and throw the bowling ball using VR controllers
- **Realistic Physics**: Physics-based ball rolling and pin knockdown
- **Full Scoring System**: Complete 10-frame bowling scoring with strikes and spares
- **Visual Feedback**: Score display, strike/spare announcements
- **Haptic Feedback**: Controller vibration when grabbing and throwing
- **Pin Reset Mechanism**: Automatic pin sweep and reset

## 🛠️ Requirements

- **Unity Version**: 2022.3 LTS or newer
- **Platform**: Windows (SteamVR), Meta Quest, or other OpenXR-compatible headsets
- **Required Packages**:
  - XR Interaction Toolkit 2.5.2+
  - XR Management 4.4.0+
  - OpenXR 1.9.1+
  - Input System 1.7.0+
  - TextMeshPro 3.0.6+

## 📁 Project Structure

```
Assets/
├── Scripts/
│   ├── BowlingBall.cs       # Ball physics and VR interaction
│   ├── BowlingPin.cs        # Pin physics and knockdown detection
│   ├── BowlingLane.cs       # Lane management and pin setup
│   ├── GameManager.cs       # Game flow and scoring
│   ├── ScoreDisplay.cs      # Score UI management
│   ├── PinResetter.cs       # Pin reset animation
│   └── VRBallGrabber.cs     # Enhanced VR grab interaction
├── Scenes/
├── Prefabs/
├── Materials/
Packages/
└── manifest.json            # Unity package dependencies
ProjectSettings/
└── ProjectSettings.asset    # VR and project configuration
```

## 🚀 Getting Started

### Installation

1. Clone this repository
2. Open the project in Unity 2022.3 LTS or newer
3. Import the required packages via Package Manager if not auto-imported
4. Configure your VR device in Project Settings > XR Plug-in Management

### Setting Up a Scene

1. Create a new scene or use the sample scene
2. Add the following GameObjects:
   - **XR Origin** (from XR Interaction Toolkit samples)
   - **Bowling Lane** (with BowlingLane component)
   - **Bowling Ball** (with BowlingBall and VRBallGrabber components)
   - **Game Manager** (with GameManager component)
   - **Score Display** (with ScoreDisplay component and UI Canvas)

### Creating a Bowling Ball

1. Create a Sphere GameObject
2. Add components:
   - Rigidbody (mass: 6kg)
   - Sphere Collider
   - XR Grab Interactable
   - BowlingBall script
   - VRBallGrabber script
3. Tag it as "BowlingBall"

### Creating Bowling Pins

1. Create a Cylinder or Pin model
2. Add components:
   - Rigidbody (mass: 1.5kg)
   - Mesh Collider (convex) or Capsule Collider
   - BowlingPin script
3. Tag it as "BowlingPin"
4. Create a prefab and assign to BowlingLane

## 🎯 How to Play

1. Put on your VR headset
2. Use your controller to grab the bowling ball
3. Swing your arm and release to throw the ball
4. Try to knock down all 10 pins!
5. Score is calculated automatically following standard bowling rules

## 📊 Scoring

- **Strike (X)**: All 10 pins knocked down on first throw = 10 + next two throws
- **Spare (/)**: All pins knocked down in two throws = 10 + next throw
- **Open Frame**: Pins knocked down = sum of both throws
- **Perfect Game**: 12 strikes in a row = 300 points

## 🔧 Customization

### Ball Settings (BowlingBall.cs)
- `ballWeight`: Ball mass in kg (default: 6)
- `maxThrowForce`: Maximum throw velocity
- `gutterSpeed`: Speed in gutter

### Pin Settings (BowlingPin.cs)
- `knockdownAngleThreshold`: Angle to consider pin knocked down (default: 45°)
- `pinWeight`: Pin mass in kg (default: 1.5)

### Game Settings (GameManager.cs)
- `waitTimeAfterThrow`: Time to wait for pins to settle
- `pinResetDelay`: Delay before resetting

## 📝 License

This project is open source and available under the MIT License.

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📧 Contact

For questions or feedback, please open an issue on GitHub.

---

Made with ❤️ using Unity and XR Interaction Toolkit