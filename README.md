# wizARd app
wizARd app - using Android device's camera, provides a variety of functions that will reduce the need for physical measuring tools and allows users to receive accurate measurements with ease. The software will revolutionize the way distances, angles, and volumes are measured and calculated by incorporating AR technology.
The target users’ group can include a wide range of activity fields, starting from architects, interior designers, and furniture moving firms to any user who needs measures and object identification in his daily activities.

![WhatsApp Image 2023-09-01 at 5 29 40 PM](https://github.com/mirunaborcan/wizARdApp/assets/80709747/3f782b85-4e1d-4bd3-b5e3-a9a6f97b866e)

This application is designed for Android phones with a minimum version of 7.1 (Nougat) and uses the Android camera to detect vertical/horizontal planes and measure objects that belong to them:

It provides users with 3 tools:
1. Simple Ruler - calculates distances between two points placed on a detected plane

![simple ruler](https://github.com/mirunaborcan/wizARdApp/assets/80709747/8a3e46e0-4386-4b62-959a-40fbac7043c9)

![simple2](https://github.com/mirunaborcan/wizARdApp/assets/80709747/8ea4e2da-e46b-4bae-a0f4-a7a613364ee8)

2. Elbow Ruler - calculates distances between multiple points placed on a plane and also computes angles smaller than 180 degrees formed by any two sides

![elbow2](https://github.com/mirunaborcan/wizARdApp/assets/80709747/fdd3dbd6-1b34-41d2-b974-0c532fcdbc4d)

![elbow](https://github.com/mirunaborcan/wizARdApp/assets/80709747/79813f0c-bf29-4547-b646-fd4c05e850dc)

3. Volume Calculator - approximates the volume of objects by fitting them in a virtual 3D box for which the user can modify its dimensions (width, length and heigth)

![volume](https://github.com/mirunaborcan/wizARdApp/assets/80709747/a5a751bc-6db9-47c2-b37e-92dd113998d9)

![volume2](https://github.com/mirunaborcan/wizARdApp/assets/80709747/c7fdaba3-9f0b-4464-8f7f-d982a54eb25d)

The measurements obtained do not represent absolute values ​​but only approximations!!

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

# Documentation
[Miruna_Borcan_Documentation_CTIEN_Licence.pdf](https://github.com/mirunaborcan/wizARdApp/files/12498918/Miruna_Borcan_Documentation_CTIEN_Licence.pdf)
