# 🐢🐇 Enigma Tales VR: The Tortoise and the Hare

[![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Unity Version](https://img.shields.io/badge/Unity-2022.3+-black.svg)](https://unity.com/)
[![Platform](https://img.shields.io/badge/Platform-Meta%20Quest%203%20%7C%20Pico%204-blue.svg)](https://www.meta.com/quest/)

**An educational Virtual Reality application that transforms classic children's stories into fully immersive, interactive adventures.**

Instead of passively reading words on a page, children step *inside* the story world. They explore environments, interact with characters, and live the narrative from within. This project was developed as an End of Year Project at EPI Digital School (VR & Game Engineering program) to combat declining reading habits by making storytelling exciting, interactive, and memorable.

<p align="center">
  <img width="1990" height="1162" alt="Screenshot 2026-05-27 172410" src="https://github.com/user-attachments/assets/579f3a7e-49c9-4ad1-a8b5-5001aeba6224" />
  <br>
  <em>The child experiences the race from a first-person perspective on the track.</em>
</p>

---

## 📖 The Idea & Motivation

Today, 81% of Tunisian children aged 5-17 spend significant time in front of screens, yet 36% lack basic reading skills. Traditional reading struggles to compete with video games and animated content.

**Our solution?** Don't fight technology—leverage it. We transform reading from a passive activity into an **active, embodied experience**. The child becomes a participant, not just a spectator. By combining the richness of classic storytelling (The Tortoise and the Hare) with the immersion of VR, we stimulate imagination, creativity, and cognitive skills while making learning genuinely fun.

---

## 🎮 Storytelling & Game Mechanics

The application is built around a **branching, interactive narrative** where the child's actions matter.

### Core Mechanics

- **Immersive Reading:** Each "page" of the story becomes a fully realized 3D scene the child can explore.
- **Environmental Interaction:** Children can interact with objects, characters, and elements in the world (e.g., touching trees, looking at characters up close).
- **Narrative Choices:** At key moments, the child makes decisions that influence the story's progression.
- **Multiple Endings:** Based on the choices made, the story can reach different conclusions, encouraging replayability.
- **First-Person Perspective:** The child sees the world through their own eyes with visible virtual hands, deepening presence.
- **Audio Narration & Subtitles:** Supports comprehension with optional spoken narration and text.

### Game Flow

1.  **Bedroom (Lobby):** The player spawns in a cozy virtual bedroom. Interactive books on a desk represent available stories.
2.  **Story Selection:** Selecting *The Tortoise and the Hare* book transports the player to the outdoor story environment.
3.  **Narrative Exploration:** The player follows the story, interacts with elements, and faces decision points.
4.  **The Race Scene:** The climax places the child *on the race track*, watching the Tortoise and Hare run past them—a moment of high immersion.
5.  **Ending & Progress:** The story concludes based on choices. Progress is saved, and achievements are tracked.

<p align="center">
  <img width="1872" height="914" alt="image" src="https://github.com/user-attachments/assets/b0d763d7-f39b-4e28-b91c-68cc2c401a25" />
<img width="1874" height="912" alt="Screenshot 2026-05-27 172842" src="https://github.com/user-attachments/assets/7e7dbca7-036e-47e3-b11f-71d7e43362aa" />  <br>
  <em>Top: The bedroom lobby with interactive books. Bottom: The floating main menu panel.</em>
</p>

---

## 🛠️ Technical Implementation

### Architecture
- **Pattern:** Model-View-Controller (MVC) to separate data logic, UI/view, and user input for maintainability.
- **Engine:** Unity 2022.3+ with **OpenXR** for cross-platform VR support.
- **Language:** C# for all scripting and gameplay logic.

### Development Tools
| Tool | Purpose |
|------|---------|
| **Unity** | Core game engine, rendering, physics, scene management |
| **Visual Studio** | C# scripting, debugging, IntelliSense |
| **Blender** | 3D character modeling (Tortoise & Hare), UV mapping, optimization |
| **Mixamo** | Automated character rigging and animations |
| **Sketchfab** | Source for some low-poly environmental assets |
| **Draw.io** | UML diagrams, use cases, flowcharts |
| **GitHub** | Version control, source code management |
| **Meta Quest 3** | Primary standalone VR headset for testing and deployment |

### Performance Optimization (for Standalone VR)
- **Target Frame Rate:** Stable 90 FPS to prevent motion sickness.
- **Character Poly Budget:** 5,000–30,000 triangles per character.
  - Rabbit: ~30k triangles
  - Turtle: ~35k triangles
- **Low-Poly Art Style:** Reduces rendering load while remaining visually appealing for children.
- **Efficient Scene Management:** Assets carefully arranged and optimized for the Meta Quest 3's mobile processor.

---

## 🎨 Characters & Environment

### Characters (Designed in Blender)
| Character | Vertices | Triangles | Style |
|-----------|----------|-----------|-------|
| **Hare (Rabbit)** | 15,151 | 30,032 | Low-poly, energetic expression |
| **Tortoise (Turtle)** | 17,976 | 35,264 | Low-poly, calm appearance |

### Key Scenes
1.  **Bedroom Scene:** Calm, blue-themed room with bed, desk, books, and window. Serves as the main menu environment.
2.  **Story Scene (Outdoor):** Low-poly forest with trees, rocks, grass, and a bright skybox.
3.  **Race Scene:** A dedicated track where both characters run past the child's first-person viewpoint.

<p align="center">
  <img src="https://via.placeholder.com/300x300?text=Rabbit+Render" alt="Rabbit Character" width="300">
  <img src="https://via.placeholder.com/300x300?text=Turtle+Render" alt="Turtle Character" width="300">
  <br>
  <em>Final low-poly character renders: The Hare and The Tortoise.</em>
</p>

---

## 🚀 Getting Started

### Prerequisites
- **Hardware:** Meta Quest 3, Pico 4 Ultra, or any OpenXR-compatible headset.
- **PC (for development):** Windows 10/11, NVIDIA GTX 1060 (or equivalent) minimum, 16+ GB RAM.
- **Software:** Unity 2022.3+, Visual Studio 2022+, Git.

### Build & Run
1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/vr-tortoise-hare.git
