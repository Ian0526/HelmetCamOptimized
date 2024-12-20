# HelmetCamsReloaded - Optimized Plugin

## Overview

This is an optimized and enhanced version of the Helmet Camera Plugin for **Lethal Company**. It improves upon the original functionality with significant performance optimizations, added features like audio relaying, and support for dynamic player and body views. The plugin aims to deliver a more immersive experience while being highly efficient.

## Key Features and Optimizations

### Features:
1. **Dynamic Player Switching**:
   - Camera view automatically updates to the selected player on the map monitor.
   - Supports viewing deceased players from their last perspective (e.g., their head's position on the ground).

2. **Audio Relay System**:
   - Captures and relays nearby sounds to the monitor as though coming from a walkie-talkie.
   - Muted when the player is within the ship to prevent overlapping audio.
   - Adjusts dynamically when switching targets.

3. **Landing Bug Fix**:
   - Prevents glitches during ship landing sequences by pausing camera updates.

### Optimizations:
1. **Automatic FPS Adjustments**:
   - Dynamically scales the helmet camera FPS to balance system performance and visual fidelity.

2. **Improved Render Logic**:
   - Removed reliance on resource-heavy methods like `Render()`.
   - Efficient toggling between enabled/disabled states to minimize runtime costs.

3. **Visibility Check**:
   - Updates only run when the monitor is visible, avoiding unnecessary computations.

4. **Optimized Resolution**:
   - Default resolution set to 1024x1024 for a balance between performance and visual quality. Runtime difference between 42x42 and 1024x1024 was negligible (~16,000 ns).

5. **Audio System Optimization**:
   - Audio relaying is handled using Unity's audio sources for efficiency, updated at controlled intervals.

## Runtime Comparison (Per Call)

| Function Name               | Runtime (ns) (OLD) | Runtime (ns) (Optimized) |
|-----------------------------|--------------------|--------------------------|
| `HelmetCamera.Plugin:Update` | **3,448,074.01**  | **34,316.39**            |

**Improvement:** ~99% reduction in runtime per call.

## How It Works

- **Dynamic FPS**: The camera dynamically adjusts its rendering FPS to adapt to performance conditions.
- **Visibility Check**: Rendering updates are paused if the monitor is out of view, saving system resources.
- **Audio Relay**: Captures and plays back audio from the camera's location, simulating walkie-talkie-style communication.
- **Body Perspective**: Allows switching to a deceased player's last perspective, maintaining immersion even after death.

## Reporting Issues

If you encounter any issues or have suggestions, please contact me on Discord:  
**@readthisifbad**

Alternatively, you can create a ticket or email me at **ian@ovchinikov.dev**.

## Notes

- This plugin automatically disables the original [Helmet Cameras](https://thunderstore.io/c/lethal-company/p/RickArg/Helmet_Cameras/) plugin to prevent conflicts.
- Profiling tools like SimpleMonoProfiler may not work correctly if both plugins are installed. Test independently for reliable results.

## Credits

- **Original Plugin**: [RickArg's Helmet Cameras](https://thunderstore.io/c/lethal-company/p/RickArg/)  
- **Original Repository**: [The0therOne/Helmet_Cameras](https://github.com/The0therOne/Helmet_Cameras)
