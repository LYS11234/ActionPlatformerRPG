# ActionPlatformerRPG
- Action Platformer RPG - Developed with Unity Engine

## 1. Abstract
- period: 2025.10.13 ~ now
- personnel: 1 (Solo Project)
- Development Environment: Unity[6000.2.10f1], C#, Visual Studio 2022
- Platform: PC/Android

## 2. Key Features
- Character System: Implemented FSM (Finite State Machine) for Movement, Attack and Damage state.
- Combat System: Hitbox based damage calculation and skill sytem with limited usage.
- UI: Real-time HP and MP status display

## 3. Technical Implementation
### 3.1 Architecture and Design Pattern
- Singleton Pattern: Applied to 'GameManager.cs' to provide global access and Manage managers efficiently.
- Observer Patter (Action): Decoupled UI from Logic. UI updates automatically via events when HP/MP Changes.

## 4. Trouble Shooting
- Issue: Profiler indicated frame drops caused by high GC allocation:
- Cause: Frequent memory allocation from using 'new' keywords and 'GetComponent' inside Update loops.
- Solution: Optimized performance by caching components and members at startup.

## 5. Future Work (ToDo)
- Integrate Google Spreadsheet for external data management.
