# 🐢🐇 Enigma Tales VR: The Tortoise and the Hare <br>

**Final Year Project (PFA) – Class of 2026** <br>
**Authors:** Ouni Ala Eddine & Sidhom Khalil <br>
**Status:** 4th Year Senior Students, VR & Game Engineering <br>
**Institution:** EPI Sousse


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
<!--
## 📖 The Idea & Motivation

Today, 81% of Tunisian children aged 5-17 spend significant time in front of screens, yet 36% lack basic reading skills. Traditional reading struggles to compete with video games and animated content.

**Our solution?** Don't fight technology—leverage it. We transform reading from a passive activity into an **active, embodied experience**. The child becomes a participant, not just a spectator. By combining the richness of classic storytelling (The Tortoise and the Hare) with the immersion of VR, we stimulate imagination, creativity, and cognitive skills while making learning genuinely fun.

---
-->
## 🎮 Storytelling & Game Mechanics

The application is built around a **branching, interactive narrative** where the child's actions matter.

### Core Mechanics

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
- **Unity's Built-in Timeline:** Used for cinematic storytelling, cutscenes, and character animations throughout the narrative sequences.

### Key Scenes
1.  **Bedroom Scene:** Calm, blue-themed room with bed, desk, books, and window. Serves as the main menu environment.
2.  **Story Scene (Outdoor):** Low-poly forest with trees, rocks, grass, and a bright skybox.
3.  **Race Scene:** A dedicated track where both characters run past the child's first-person viewpoint.

<p align="center">
  
  <img width="1025" height="555" alt="Screenshot 2026-05-27 174203" src="https://github.com/user-attachments/assets/34c9904a-3243-42b1-8319-5aa470c96e6b" />
  <img width="1023" height="558" alt="Screenshot 2026-05-27 174223" src="https://github.com/user-attachments/assets/33493ebb-7ac2-432a-ab71-5e38ed9d6938" />  <br>
  <em>Final low-poly character renders: The Hare and The Tortoise.</em>
</p>

---

## 🚀 Getting Started

### Prerequisites

- **Unity Version:** 2022.3.62f3 or later
- **Version Control:** Git
- **Hardware:** Meta Quest 3s/3/2, or any OpenXR-compatible headset
- **PC:** Windows 10/11 for development

### Installation

1. **Clone the repository:**
   ```bash
   git clone https://github.com/The-SauceMan/Engima-Tales-VR-PFA-.git ** Unity 2022.3+, Visual Studio 2022+, Git.
2.**Open the project in Unity:**
--> Launch Unity Hub
--> Click "Open Project"
--> Select the cloned project folder
--> Ensure you are using Unity 2022.3.62f3
