# APPLYING AUGMENTED REALITY TO DEVELOP A GAME FOR MOBILE DEVICES - Treasure HuntAR

## Table of Contents
- [Introduction](#introduction)
- [Features](#features)
- [Getting Started](#getting-started)
- [Installation](#installation)
- [Usage](#usage)
- [Development](#development)
- [Screenshots](#screenshots)
- [Contributing](#contributing)
- [Acknowledgements](#acknowledgements)

## Introduction
Welcome to the Treasure HuntAR! This project combines the classic breakout game with cutting-edge augmented reality (AR) technology to create an engaging and educational experience. The game aims to make history interesting by allowing players to unlock and interact with historical artifacts in AR.

## Features
- **Classic Breakout Mechanics**: Enjoy the timeless fun of the breakout game with smooth controls and engaging levels.
- **Augmented Reality Integration**: Unlock historical artifacts and view them in your real-world environment using AR technology.
- **Social Features**: Log in with Facebook to share your achievements and compete with friends on leaderboards.
- **Cloud Data Management**: Save your progress and scores securely in the cloud with PlayFab integration.
- **Performance Optimizations**: Optimized for smooth gameplay and AR interactions on a wide range of mobile devices.

## Getting Started
To get started with the AR Breakout Game, you'll need to set up your development environment and install the necessary dependencies.

### Prerequisites
- Unity 2022.3 or later
- AR Foundation 5.1.4
- PlayFab SDK
- Facebook SDK for Unity

## Installation
1. **Clone the repository:**
   ```sh
   git clone https://github.com/yourusername/ar-breakout-game.git
   cd ar-breakout-game
   ```
2. **Open the project in Unity**:
- Launch Unity Hub.
- Click on "Open" and navigate to the cloned project directory.
- Install AR Foundation:
-- Open the Package Manager (Window > Package Manager).
-- Search for "AR Foundation" and install the package.
- Configure PlayFab:
-- Sign up for a PlayFab account and create a new title.
-- In Unity, go to Window > PlayFab > SDK Setup and enter your PlayFab Title ID.
- Configure Facebook SDK:
-- Follow the instructions on the Facebook Developer site to set up your app and integrate the Facebook SDK.

## Usage
Once the project is set up, you can run the game in the Unity Editor or build it for your mobile device.
### Running in the Editor
- Open the MainScene in the Unity Editor.
- Click the "Play" button to start the game.

### Building for Mobile
- Open File > Build Settings.
- Select your target platform (iOS or Android) and ensure all necessary settings are configured.
- Click "Build" and follow the prompts to deploy the game to your device.

## Development
### Project Structure:
- Assets/: Contains all game assets, scripts, and scenes.
- Assets/Scripts/: Contains C# scripts for game logic, AR integration, and social features.
- Assets/Scenes/: Contains the Unity scenes for the game.
- Assets/Prefabs/: Contains prefabs for game objects and AR artifacts.

### Core Scripts
- *GameManager.cs*: Handles game state and core gameplay logic.
- *SoundManager.cs*: Handles audio management.
- *ARManager.cs*: Manages AR sessions and interactions.
- *PlayFabManager.cs*: Manages social login and sharing features, data storage and cloud integration with PlayFab.

### Key Features
- **Breakout Game Mechanics**: Implemented using Unity's physics and collision systems.
- **AR Integration**: Uses AR Foundation for plane detection and AR interactions.
- **Social Features**: Integrated with Facebook SDK for login and sharing.
- **Cloud Data Management**: Uses PlayFab for secure data storage and retrieval.

## Contributing
Contributions are welcome! To contribute:
- Fork the repository.
- Create a new branch for your feature or bug fix.
- Commit your changes and push to your forked repository.
- Submit a pull request with a detailed description of your changes.

## Screenshots
![image](https://github.com/MikeJoester/Treasure-HuntAR/assets/74175443/95453634-f627-41d6-a22c-60eedc223b9f)

![image](https://github.com/MikeJoester/Treasure-HuntAR/assets/74175443/831a1c76-cc32-4a66-9ec2-f8444253e282)

![image](https://github.com/MikeJoester/Treasure-HuntAR/assets/74175443/abec714d-ab80-4483-bad6-5608276ee016)

![Image Sequence_010_0000](https://github.com/MikeJoester/Treasure-HuntAR/assets/74175443/11a6a98c-eabe-4192-b2a9-52c2090de214)


## Acknowledgements
- [Unity](https://unity.com/) for providing an excellent game development platform.
- [AR Foundation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@5.1/manual/index.html) for enabling easy AR integration.
- [PlayFab](https://playfab.com/) for robust cloud data management services.
- [Facebook SDK](https://developers.facebook.com/docs/unity/) for seamless social features integration.
