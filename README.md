### Space Weld
Image

## About Game <img width="25" height="25" alt="Image" src="https://github.com/user-attachments/assets/93c1c452-f50d-408d-a1f1-98669efb24f0" /> 
***Space Weld*** is a sci-fi maintenance game set aboard a deep-space cargo vessel. As the ship pushes through dangerous routes across the galaxy, the hull continuously suffers damage from debris and asteroid impacts. 
Playing as the ship’s welding robot, your job is to repair breaches, maintain ship efficiency, and keep the vessel moving at full speed. Every unrepaired hull fracture slows the ship down, and every second lost threatens the delivery schedule. 
With Captain Rhea overseeing the mission, you must keep the ship together long enough to reach each destination before the deadline.

This game was submitted to SpaceJam 2026 by Gamespace <br>
Game Engine : Unity 6000.0.60f1
<div align="center"> Game Page <br>
  <a href="https://triugames.itch.io/space-weld" target="_blank"><img src="https://img.shields.io/badge/Itch.io-FA5C5C?style=for-the-badge&logo=itch.io&logoColor=white" /></a>
</div>

## Key Features <img width="32" height="32" alt="Image" src="https://github.com/user-attachments/assets/1079a5d7-e1ea-43cc-a059-b9c4333c55a2" />
1. ***Dynamic Moving Robot***        : <br> The Robot moves exclusively along designated tracks, features full 360-degree rotation and using procedural animation for its robotic arm.
2. ***Hull Welding System***         : <br> The robot uses a welding mechanic to repair breaches in the ship's hull.
3. ***Upgrade able Robot Status***   : <br> Players can upgrade the robot's capabilities, including movement speed, welding efficiency, and arm reach.
4. ***Dynamic Progressing Mission*** : <br> Unrepaired hull breaches reduce the ship's engine efficiency, increasing travel time to the destination and negatively impacting the final performance rating.
3. ***Custom Dialogue System***      : <br> Features custom dialogue scripts complete with immersive text typing effects.

## Team & My Contribution <img width="32" height="32" alt="Image" src="https://github.com/user-attachments/assets/54638a67-ec93-473a-bb3c-dc5184bcad66" />
| Team Member | Role |
|-------------|------|
| KrapuRED | Game Designer |
| RxDuds | Game Artis |
| AndhikaAtmaja | Game Programmer |

My Contribution (AndhikaAtmaja)
- Created most of the code for game systems and features.
- Worked on the Audio System, procedural animation for its robotic arm, and Save an load.
- Created most UI and Scene in game.

## Layer / Module Design <img width="32" height="32" alt="Image" src="https://github.com/user-attachments/assets/897ab20c-94d2-4398-ad51-ee0f7859921d" />
<img width="1492" height="782" alt="Image" src="https://github.com/user-attachments/assets/bc631d6b-9605-410f-9077-aa4f54419a2e" />

## Modules and Features <img width="32" height="32" alt="Image" src="https://github.com/user-attachments/assets/718fd774-8614-4ccf-b7f7-297fdcf3c10b" />
| 📂 Name | 🎬 Scene | 📋 Responsibility |
|-----------------------|----------------------|-------------------|
| Asteroit              | Gameplay             | - Manage spawner of the asteroit <br>- Handle asteroit logic & movemenet |
| Audio System          | Gameplay & Main Menu | - Manage all audio Game |
| Boom & Arm Movement   | Gameplay             | - Handle logic procedural animation for its robotic arm |
| Character             | Gameplay & All Story | - Handle Character animation |
| Damage Hull           | Gameplay             | - Handle Random Change and Spawn hull breach event <br> - Visualization the hull breach |
| Destiantion           | Gameplay             | - Handle of the calculate current position of the ship to destionation |
| Dialogue              | Gameplay             | - Handle of the dialogue character <br> - Store dialogue data |
| Effciency Ship        | Gameplay             | - Handle of the calculate current ship engine effciency |
| Mission Controller    | Gameplay             | - Handle of Dialogue Capten Rea to give report about the ship status <br> - Store pre-dialogue report about the ship status |
| Performance           | Gameplay             | - Handle of the calculate player performance |
| Rail System           | Gameplay             | - Handle of Generate Rail Point <br> - Manage all connection for all Rail point |
| Robot System          | Gameplay             | - Handle of Robot Movement |
| Save and Load System  | Gameplay             | - Handle of all robot status and upgrade across scene |
| Typing Effect         | Gameplay & All Story | - Handle of all typing effect for text |
| Upgrade               | Gameplay             | - Handle of all robot Upgrade |
| Welding System        | Gameplay             | - Handle welding logic |

## Game Flow <img width="32" height="32" alt="Image" src="https://github.com/user-attachments/assets/7759227f-8383-4a35-8846-ad1af00389c1" />
<img width="2319" height="812" alt="Image" src="https://github.com/user-attachments/assets/88f1de4a-ac3e-40b4-877e-258729c1377c" />

## Plugin / Unity Asset <img width="32" height="32" alt="Image" src="https://github.com/user-attachments/assets/65b37d28-0412-4092-8f6f-95054849fe7a" />
Went developed this game we use some Plugin / Unity Asset for polishing and juicy
- Unity Package Smooth Camera Shaker - FirstGearGames
