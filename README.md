# Hitwicket Game Developer Challenge 2026 - Doofus Adventure

**Developer:** Rudra Karan Rathore (25MCA0131)  
**Target Platform:** Windows / Standalone (Unity 6+)  

## Overview
This repository contains my submission for the Hitwicket Game Developer Assignment. "Doofus Adventure" is a 3D procedural platformer where the player guides Doofus across a continuously generating path of temporary platforms (Pulpits). 

A playable executable build of the final game is available in the **Releases** section of this repository.

## Levels Completed
- [x] **Level 1:** Character movement and Pulpit placements dynamically read and applied from the provided `doofus_diary.json`.
- [x] **Level 2:** Real-time score updating after every successful move to a new Pulpit.
- [x] **Level 3:** Fully functional Start screen and Game Over screen UI.

## Technical Implementations
* **Dynamic Data Parsing:** The game parses `doofus_diary.json` on startup to configure player speed, pulpit spawn rates, and decay timers.
* **Exploit-Proof Procedural Generation:** 
  * Implemented a custom pathfinding algorithm in `PulpitManager` that tracks recent platform positions to prevent overlapping.
  * Added a fairness cooldown and straight-line override system. If a player attempts to exploit the distance-check algorithm by corner camping to force a diagonal staircase, the system actively counters it by forcing the path straight.
* **Coyote Time:** Replaced standard Y-axis ground checks with a 0.6f downward Raycast, giving the player a 0.2-second grace period to jump or adjust movement after slipping off an edge.
* **UI & Post-Processing:** Custom sliced pixel-art UI that automatically scales to the user's screen resolution using anchored layouts. The Game Over state utilizes a dual-volume post-processing setup to dynamically blur the background camera.

## Repository Structure & Proof
* **`DoofusAdventure/`**: Contains the complete Unity project folder.
* **`GameplayVideo.mp4`**: A recorded playthrough demonstrating all required levels, mechanics, and UI states.
* **`Screenshot1(Pulpit 9x9 size proof).png`**: Verification of the exact 9x9 platform dimensions.
* **`Screenshot2(Player size proof).png`**: Verification of the player scale parameters.

## How to Run

**To play the compiled game:**
1. Navigate to the **Releases** section on the right side of this GitHub repository.
2. Download the latest `.zip` release, extract it, and run the executable.

**To run the project in the Unity Editor:**
1. Clone this repository to your local machine.
2. Open Unity Hub and select **Add > Add project from disk**.
3. Select the `DoofusAdventure` folder.
4. Open the primary scene (if not loaded by default) and press **Play** in the Unity Editor.