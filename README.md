# GitHub address
# https://github.com/mirunaborcan/wizARdApp

# wizARd app

This application is designed for Android phones with a minimum version of 7.1 (Nougat) and uses the Android camera to detect vertical/horizontal planes and measure objects that belong to them:
It provides users with 3 tools:
1. Simple Ruler - calculates distances between two points placed on a detected plane
2. Elbow Ruler - calculates distances between multiple points placed on a plane and also computes angles smaller than 180 degrees formed by any two sides
3. Volume Calculator - approximates the volume of objects by fitting them in a virtual 3D box for which the user can modify its dimensions (width, length and heigth)

# Build Steps
To build and run the application you can follow the steps or copy the apk directly to your phone.

Case 1: Build and run the app in Unity:

This application is developed in Unity 2021.3.18f1. Please use the same version in order not to have errors with the differences of plugins. 
Application uses different packages. 
1. Open Unity, go to => Window => Package Manager => Unity Registry =>
- install AR Features ( containing 6 packages, including AR Foundation)
- install ARCore XR Plugin
- install XR Interaction Toolkit
2. Access following link for downloading PlayFab services: https://aka.ms/PlayFabUnityEdEx
  Go to Assets => Import package, and import downloaded package.
  Here is a tutorial to set up PlayFab:
https://youtu.be/__M9AoiVA9c
3. Go to File => Bulid settings => change platform to Android
4. Go to File => Bulid settings => Player Setting => Other Settings and here set the following:
  - at Rendering - set Color Space Gamma - OpenGLES3 (remove Vulkan)
  - disable Multithreaded Rendering
  - set minimum API to Android 7.1
  - set scripting backend to IL2CPP, .NET Standard 2.1
5. Go to File => Bulid settings => XR Plug-In Management => enable ARCore
6. Go to File => Build and Run

Case 2: Copy the apk:
Please copy the apk.apk from the repo to your mobile phone and run it.

# Code 
# You can find the sw implementation of the 3 tools under Assets/wizARd.
