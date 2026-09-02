# High-Performance Custom AI Pathfinding & Vertex-Based Navigation Framework

A production-grade, highly optimized C# navigation and real-time pathfinding framework engineered within the Unity ecosystem. 

This repository serves as a flagship technical case study demonstrating advanced spatial mathematics, customized grid calculations, and low-level data structure optimization. It is built to completely bypass traditional CPU-heavy Navigation Mesh (NavMesh) overhead, facilitating smooth, simultaneous execution loops for massive agent counts.

---

## 🏗️ Architectural Overview & Design Patterns

The system is architected from the ground up prioritizing loose coupling, data-driven parameters, and absolute cache efficiency:

* **Vertex-Driven Trajectory Matrix:** Bypasses traditional raycasting and pre-baked baking grids by operating directly on spatial vertex array inputs to dynamically calculate collision boundaries and agent paths.
* **Decoupled Event Architecture:** Leverages core C# Actions and specialized event handling layers to isolate the pathfinding solver engine from gameplay managers, rendering nodes, and individual agent entities.
* **Data-Driven Configuration Profiles:** Utilizes Unity `ScriptableObjects` to separate variable data profiles (such as speed limits, turning radii, and acceleration matrices) from processing logic, allowing rapid runtime adjustment without code modification.

---

### Architectural Breakdown:
* **`PathfindingSolver.cs` (The Math Matrix):** Implements specialized search logic and heuristic matrix algorithms. It caches internal collection data structures to avoid heap allocation spikes during heavy frames.
* **`GridManager.cs` (The Spatial Engine):** Dynamically processes raw landscape vertex arrays to determine node availability and boundary thresholds without running expensive raycasts.
* **`AgentController.cs` (The Physics Decoupler):** Manages velocity updates and trajectory interpolation using event-driven C# Actions, ensuring it never stays tightly coupled to the solver engine.
* **`MovementConfig.cs` (The Data Carrier):** Utilizes Unity `ScriptableObjects` to hold distinct data configurations (such as step height bounds or turning speed vectors) so parameters can be tweaked at runtime without rebuilding scripts.

---

## ⚡ Engineering & Optimization Focus

* **SDLC Lifecycle Compression:** Designed with modular, independent script arrays that compressed integration timelines by up to 50% during system stress tests.
* **Garbage Collection (GC) Defense:** Eliminates continuous memory allocations inside critical performance loops (`Update()` blocks). The framework relies on pooled array resets to prevent frame-rate stuttering on lower-end targets.
* **Defensive Version Architecture:** Managed through clear Git version control practices, regression checking, and modular structural isolation to ensure easy deployment inside professional studio pipelines.

---

## 🚀 Integration & Running the Simulation

### Pre-requisites
* Unity 6 (Recommended) or Unity 2022.3 LTS+
* Universal Render Pipeline (URP) or High Definition Render Pipeline (HDRP) architecture configuration

### Rapid Project Drops
1. Download or clone this repository directly into your project's `Assets/` tree.
2. Link the `GridManager` to a centralized manager node inside your active runtime hierarchy.
3. Hook your agent scripts directly to the `OnPathCalculationCompleted` C# Action callback array to trigger asynchronous agent routing.

---

## 📜 Licensing & Usage
This framework is distributed under the terms of the **BSD-3-Clause License**. It acts as an open-source engineering showcase tracking defensive programming paradigms and engine profiling concepts.

---
*For B2B contracting availability, engineering consultations, or technical studio reinforcement inquiries, connect via **Roberto.Valentin.gmd@outlook.com**.*
