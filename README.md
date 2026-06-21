# 🎮 Multiplayer FPS Game 

![Unity](https://img.shields.io/badge/Unity-100000?style=for-the-badge&logo=unity&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![Photon](https://img.shields.io/badge/Photon_PUN-000000?style=for-the-badge&logo=photon&logoColor=white)

A high-paced, real-time multiplayer First-Person Shooter (FPS) game built with Unity and Photon Unity Networking (PUN). This project demonstrates advanced gameplay mechanics, networked player synchronization, and scalable code architecture.

## 🚀 Key Features
<img width="502" height="203" alt="Animation" src="https://github.com/user-attachments/assets/3b8432e6-a7ab-42f1-9856-ee494cd930c6" />

* **🌐 Real-Time Multiplayer:** Seamless player synchronization, voice chat, and lobby management using Photon PUN.
* **🧠 AI State Machine:** Custom-built Artificial Intelligence utilizing a State Machine architecture (e.g., `AiChasePlayerState`) for realistic enemy behavior and combat mechanics.
* **🎯 Advanced Weapon & Camera Systems:** Precise shooting mechanics (including Sniper systems) integrated with Cinemachine for smooth, dynamic camera controls.
* **🖥️ Cross-Platform Support:** Fully optimized and built for both native Windows and macOS (Apple Silicon ARM) environments.
* **📦 Optimized Version Control:** Large 3D models and textures are efficiently managed using **Git LFS**, keeping the core repository clean and lightweight.

---

## 📸 Showcase

### Multiplayer Synchronization
*(Insert a 3-5 second GIF here showing two different game instances interacting, proving your network logic works)*
![Multiplayer Sync](link_to_your_gif_here)

### AI Combat & State Machine
*(Insert a screenshot or short GIF showing the AI chasing or attacking the player)*
![AI Combat](link_to_your_gif_here)

---

## 🛠️ Technical Architecture

This project strictly follows Object-Oriented Programming (OOP) principles to ensure maintainability and scalability.

* **Network Logic:** Photon PUN handles Remote Procedure Calls (RPCs) and data synchronization across the network.
* **Modularity:** Scripts are divided into strict managers (e.g., Round Management) and specific behavior classes.
* **Physics & Rendering:** Utilizes Unity's Universal Render Pipeline (URP) and optimized mesh handling to prevent physics query instability.

## 💻 Installation & Setup

Since this repository uses Git LFS for large assets (`.fbx`, `.png`, etc.), make sure you have Git LFS installed before cloning.

```bash
# 1. Install Git LFS
git lfs install

# 2. Clone the repository
git clone [https://github.com/cagriazman/Unity_Photon_Shooter_Game_Project.git](https://github.com/cagriazman/Unity_Photon_Shooter_Game_Project.git)

# 3. Pull LFS files
git lfs pull
