# Hurricane VR Fishnet Example Project
This project shows how to implement [Fishnet](https://assetstore.unity.com/packages/tools/network/fish-net-networking-evolved-207815) for [Hurricane VR](https://assetstore.unity.com/packages/tools/physics/hurricane-vr-physics-interaction-toolkit-177300). Only some parts of HurricaneVR are networked, such as: Player, Grabbables, Socketing, Destructibles, and Damage.

This project is intended to have a dedicated server and has not been fully tested in client hosted mode.

Tested:
- Unity 2022.3.11f1
- Fishnet Version: 3.11.7R
- Hurricane VR Version: 2.9.1i
- Windows/Linux Server build
- Quest 3 client build

## Getting Started
- Setup a Unity project that contains both Fishnet and Hurricane VR
- Pull this repository into the Assets folder of the project
- Add the example scene to the build settings
- Build a dedicated server: 
	- Locally build and run for your target platform
	OR
	- Build and Deploy with [PlayFlow Cloud](https://assetstore.unity.com/packages/tools/network/playflow-cloud-206903). Then assign the IP of the server to the NetworkManager Tugboat.ClientAddress in scene
- Test in the editor and/or build a client

## Notes
- There is currently a problem with scaling socketed items
- All networked grabbables should start with their transforms being in world space, if parented all parents must have their position and rotation at 0, and their scale at 1.
- All NetworkGrabbles return to server authority when a client disconnects to prevent them from being removed from the scene.