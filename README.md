🎮 Multiplayer FPS Arena & Custom AI Engine
A high-paced, real-time multiplayer First-Person Shooter (FPS) game built with Unity and Photon Unity Networking (PUN 2).

Unlike standard FPS prototypes, this project is engineered with a strict focus on Software Architecture, Memory Management, and Scalable Systems. It demonstrates advanced networked player synchronization, a modular Artificial Intelligence engine, and decoupled codebase structures adhering to Object-Oriented Programming (OOP) principles.

🚀 Key Features & Systems
🌐 Robust Multiplayer Synchronization:
Seamless real-time synchronization utilizing Photon PUN. Includes lobby management, instance-based room creation, and highly optimized Remote Procedure Call (RPC) traffic to handle combat events, health states, and animations with minimal latency.

🧠 Custom AI State Machine:
A deeply integrated, modular AI engine. Enemy agents do not rely on rigid, performance-heavy Update loops. Instead, they utilize a dynamic State Machine architecture (e.g., AiChasePlayerState, AiPatrolState) for realistic decision-making, threat detection, and NavMesh-based pathfinding.

🎯 Advanced Weapon & Combat Mechanics:
Dynamic weapon systems handling everything from rapid-fire raycast weapons to precise Sniper mechanics. Fully integrated with Cinemachine for responsive camera recoil, dynamic FOV shifts, and smooth targeting transitions.

🖥️ Cross-Platform Optimization:
Configured and tested for native performance across both Windows and macOS (Apple Silicon ARM) environments.

📦 Enterprise-Level Version Control:
Large 3D models, audio, and high-res textures are strictly managed via Git Large File Storage (LFS), ensuring the codebase remains lightweight and commit histories stay clean.

📸 In-Game Showcase
🌐 Networked Combat & Synchronization
Real-time data exchange across different clients. This demonstrates accurate hit registration and state synchronization over the network without severe desync.

🤖 AI Behavior & Sensory Tracking
Demonstrating the AI State Machine in action. The agent dynamically shifts from patrolling to chasing, utilizing NavMesh for spatial awareness and path calculation.

🛠️ Technical Architecture & Clean Code
This project was built to be maintainable, readable, and highly performant. The architecture strictly isolates distinct systems to prevent tightly-coupled "spaghetti code."

1. Memory Optimization (Zero-Allocation)
Garbage Collection (GC) spikes are eliminated in critical combat loops. Instead of allocating new arrays every frame, the project uses memory-safe techniques like Physics.OverlapSphereNonAlloc for AI sensory sweeps, maintaining high FPS even when multiple bots are active.

2. Clean Code & Structural Modularity
Static Utility Classes: Repetitive mathematical operations, distance checks, and damage calculations are abstracted into stateless, static helper classes.

Extension Methods: Implementation of custom C# extension methods (e.g., safely extracting PhotonView.ViewID from dynamic network objects) to keep the core logic highly readable.

Instance Independence: Strictly avoided shared static states for individual objects. Every player and AI agent operates on its own instance data (Health, Target, Ammo), completely eliminating "Hive Mind" bugs.

3. Rendering & Physics
Utilizes Unity's Universal Render Pipeline (URP) for optimized draw calls and modern lighting solutions, alongside streamlined mesh colliders to maintain stability during intensive physics calculations.
