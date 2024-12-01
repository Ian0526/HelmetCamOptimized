# Helmet Camera Plugin - Optimized Version

## Overview

This is an optimized version of the Helmet Camera Plugin for **Lethal Company**, aimed at improving the performance while maintaining functionality. The plugin dynamically adjusts its behavior based on system performance to deliver smoother gameplay while minimizing resource usage. 

## Key Optimizations

1. **Automatic FPS Adjustments**:
   - The plugin dynamically scales the helmet camera FPS, similar to the optimizations used by the game author Zeekers.
   - Performance is maximized by maintaining visual fidelity where possible while reducing system overhead when necessary.

2. **Improved Render Logic**:
   - Removed reliance on resource-heavy methods like `Render()`.
   - Efficient toggling between enabled/disabled states to minimize runtime costs.

3. **Visibility Check**:
   - Updates now check if the monitor is in view instead of just checking if you're on the ship.
   - Reduces unnecessary processing during these conditions.

4. **Optimized Resolution**:
   - Default resolution set to 1024x1024 for a balance between performance and visual quality. (The runtime between 42x42 and 1024x1024 was a negligible difference of about +/- 16,000 ns)

## Runtime Comparison (Per Call)

| Function Name               | Runtime (ns) (OLD) | Runtime (ns) (Optimized) |
|-----------------------------|--------------------|--------------------------|
| `HelmetCamera.Plugin:Update` | **3,448,074.01**  | **34,316.39**            |

**Improvement:** ~99% reduction in runtime per call.

## Reporting Issues

If you encounter any issues or have suggestions, please contact me on Discord:  
**@readthisifbad**

Alternatively, you can create a ticket, or email me at **ian@ovchinikov.dev**

## Notes

- The plugin supports dynamic FPS scaling and visibility checks to ensure optimized resource usage.
- Default FPS is set to 30 for rendering updates, but this is adjusted dynamically based on system performance.
- If you want to profile this yourself, be sure to not have the [original](https://thunderstore.io/c/lethal-company/p/RickArg/Helmet_Cameras/) plugin installed. It is forced disabled, but prevents SimpleMonoProfiler from dumping. Try them independently.

## Credits

- **Original Plugin**: [RickArg's Helmet Cameras](https://thunderstore.io/c/lethal-company/p/RickArg/)  
- **Original Repository**: [The0therOne/Helmet_Cameras](https://github.com/The0therOne/Helmet_Cameras)
  
Thank you @The0ther0ne, I would've spent a signficant amount of time trying to figure out the knitty-gritty details of this project.
