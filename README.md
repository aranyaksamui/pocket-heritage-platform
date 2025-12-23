# 🏛️ Pocket Heritage AR Platform (WIP)

> **Status:** 🚧 In Development (Prototyping Phase)  
> **Tech Stack:** Unity 2022+, AR Foundation, Addressables, Firebase (Planned)

## 📖 Overview
Pocket Heritage is an Augmented Reality (AR) platform designed to preserve and visualize India's cultural heritage. The application allows users to place high-fidelity 3D models of heritage sites (like the *Rani Ki Vav* stepwell and the *Taj Mahal*) into the real world.

Key features include:
*   **Immersive Visualization:** Place scale models of monuments on flat surfaces.
*   **Smart Labels:** Context-aware UI that reveals historical details based on proximity (LOD - Level of Detail).
*   **Interactive Exploration:** Users can physically walk around the model to explore distinct features.

## 🏗️ Architecture (Current State)
This project follows a **Event-Driven Architecture** and utilizes **Data-Driven Design**.

*   **Client:** Unity (C#)
*   **AR System:** AR Foundation (ARCore/ARKit)
*   **Data Management:** JSON-based metadata loading (Milestone 1 completed).
*   **Asset Pipeline:** Unity Addressables for asynchronous asset loading (Milestone 2 completed).
*   **Pattern:** Centralized Event Bus (`AREvents`) decoupling Logic, UI, and Data layers.

## 🚀 Getting Started

### Prerequisites
*   Unity Hub & Unity 2022.3 (LTS) or higher.
*   Android Build Support (OpenJDK & Android SDK/NDK installed).
*   Git LFS (Large File Storage) initialized.

### Installation
1.  **Clone the repo:**
    ```bash
    git clone https://github.com/aranyaksamui/pocket-heritage-platform.git
    ```
2.  **Initialize LFS:**
    ```bash
    git lfs pull
    ```
3.  **Open in Unity:**
    Add the folder to Unity Hub and open.
4.  **Build Addressables:**
    Go to `Window > Asset Management > Addressables > Groups` and run `Build > New Build > Default Build Script`.
5.  **Run:**
    Connect an AR-compatible Android device and hit `Build and Run`.

## 🛣️ Roadmap

- [x] **Milestone 0:** Project Setup & Git LFS Integration.
- [x] **Milestone 1:** Dynamic Data Injection (JSON) & Smart Label System.
- [x] **Milestone 2:** Asset Pipeline Optimization (Addressables).
- [ ] **Milestone 3:** Cloud Backend Integration (Firebase).
- [ ] **Milestone 4:** Full User Interface & Navigation.
