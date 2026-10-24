# Terrasweeper — Development Baseline

**Baseline date:** 2026-10-24  
**Unity version:** 6000.2.13f1  
**Status:** Core gameplay validated

## 1. Purpose

This document records the functional state of Terrasweeper before modernization and further development.

The baseline serves as a reference for identifying regressions during gameplay refactoring, procedural terrain development, and platform integration updates.

## 2. Existing Functionality

Terrasweeper is a 3D Minesweeper game featuring destructible terrain and multiple difficulty levels.

The existing implementation includes:

- Beginner, Intermediate, and Advanced difficulty settings.
- Procedurally placed mines.
- First-click mine protection.
- Adjacent mine indicators.
- Flag placement and removal.
- Automatic revealing of adjacent empty tiles.
- Victory and defeat conditions.
- Terrain deformation when revealing tiles.
- Explosion effects when uncovering mines.
- Restartable gameplay sessions.
- Main menu and gameplay settings.
- High-score tracking.
- Google Play Games integration.

## 3. Primary Scenes

| Scene | Purpose |
|---|---|
| `Assets/TerraMainMenu.unity` | Main menu |
| `Assets/TerraSweeper.unity` | Minesweeper gameplay |

## 4. Existing Systems

| Script | Responsibility |
|---|---|
| `Grid.cs` | Grid initialization, mine placement, neighbors, and victory conditions |
| `Tile.cs` | Tile interaction, flagging, revealing, and mine behavior |
| `GameManager.cs` | Game state transitions and session management |
| `TerraCrater.cs` | Unity Terrain heightmap deformation |
| `TextureChanger.cs` | Terrain texture modification |
| `InputManager.cs` | Input handling |
| `UIManager.cs` | User interface management |
| `SoundManager.cs` | Audio playback |

## 5. Validation Results

Validation performed in Unity `6000.2.13f1`.

| Test | Result |
|---|---|
| Open Unity project | Pass |
| Enter Play Mode | Pass |
| Load gameplay scene | Pass |
| Main menu navigation | Pass |
| Difficulty selection | Pass |
| First-click mine protection | Pass |
| Reveal tiles | Pass |
| Place flags | Pass |
| Victory condition | Pass |
| Defeat condition | Pass |
| Restart gameplay | Pass |
| Reset terrain deformation | Pass |
| Quit Game in Editor | Pass |
| Quit Game in standalone build | Pass |

The game is playable from start to finish.

Automated regression tests have not yet been established.

## 6. Known Issues and Warnings

### 6.1 ToggleGroup initialization exception

**Status:** Resolved through Inspector configuration.

Opening the difficulty selection menu previously produced:

`ArgumentOutOfRangeException` in `UnityEngine.UI.ToggleGroup.EnsureValidState()`.

Enabling **Allow Switch Off** on the affected ToggleGroup resolved the exception and restored menu navigation.

The difficulty selection configuration should be reviewed during UI modernization to ensure a valid difficulty is always selected.

### 6.2 Deprecated scene-loading callback

**Status:** Open

Unity reports that `OnLevelWasLoaded` is deprecated.

The legacy callback should be replaced with `SceneManager.sceneLoaded` where appropriate.

### 6.3 DontDestroyOnLoad warnings

**Status:** Open

Unity reports:

`DontDestroyOnLoad only works for root GameObjects or components on root GameObjects.`

Warnings occur during startup and when a mine explodes.

Persistent object ownership and lifecycle management require investigation.

### 6.4 Legacy dependencies

**Status:** Pending audit

The project contains older platform integrations and third-party assets.

Dependencies should be reviewed for compatibility, continued necessity, and maintenance status.

## 7. Architectural Observations

The current implementation combines gameplay logic and presentation responsibilities in several MonoBehaviour components.

The terrain system uses Unity Terrain heightmap modification to create and remove terrain formations.

The modernization effort should preserve existing gameplay behavior while progressively separating:

- Minesweeper board logic
- Tile state and presentation
- Terrain generation
- Terrain excavation and destruction
- Input handling
- Platform services

## 8. Planned Modernization

The next development stages are:

1. Audit and modernize third-party dependencies.
2. Introduce automated Minesweeper gameplay tests.
3. Document and refine system architecture.
4. Separate gameplay logic from Unity presentation.
5. Prototype continuous procedural terrain generation.
6. Implement seamless terrain tiles and global erosion.

## 9. Baseline Conclusion

Terrasweeper's original gameplay remains operational in Unity 6.

Core gameplay, difficulty selection, victory and defeat conditions, restarting, terrain reset, and application quitting have been validated.

The remaining warnings and legacy dependencies do not currently prevent gameplay.

This baseline is suitable for beginning incremental modernization.