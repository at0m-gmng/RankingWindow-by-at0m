<div align="center">

# Ranking Window
<div align="center" style="font-size: 40px">

[![Image](https://img.shields.io/badge/TASK-LINK-blue)](https://docs.google.com/document/d/1gwkjEA2yfivABZTX6NOwZjedceG63mnq/edit?tab=t.0)

</div>
</div>


### Description

- Test task: implement a **Ranking window** with a sticky local-player row that pins to the top/bottom of the list when the player's row is scrolled out of view.
- The window shows a local ranking (player name, score, level, avatar, league), highlights the local player's row, marks the top-3 with league backgrounds, and supports the optional sticky player bar.
- Technology stack: Zenject (+ Signals), UniTask, DOTween, Addressables.
- Labor costs: 4h

### Architecture

- **Dependency Injection (Zenject):** global services live in `ProjectContext` (`ProjectInstaller`); the scene-level state machine is installed in `GameInstaller`.
- **State machine:** `BootstrapState → MenuState → GameState`, driven by `GameStateMachine`.
- **UI System:** windows are loaded via Addressables, instantiated through the DI container, and addressed by `UIWindowID`; their Addressables handles are owned and released by the system.
- **Event-driven flow:** Zenject `SignalBus` decouples windows from states (`MenuStartRequestedSignal`, `RankingCloseRequestedSignal`) — states no longer touch UI buttons directly.
- **Persistence:** `PersistentProgressService` and `SaveLoadService` are global singletons; ranking data is loaded from an Addressables JSON.
- **Ranking screen:** avatar/league sprites are loaded and cached via `AddressableSpriteCache`, with local-player row highlight, top-3 league backgrounds, and an optional sticky player row (`StickyPlayerController`).
- **Menu:** animated start button with a playful "runaway" behaviour.

### Download
<div align="center">

[![Github All Releases](https://img.shields.io/github/downloads/at0m-gmng/BallonsMerge/total.svg)](https://github.com/at0m-gmng/BallonsMerge/releases)

</div>

### Unity versions
- Latest release requires Unity 6.0.25f  or higher.
- Previously releases should work with older Unity versions.
- Downgrading the project version will cause conflicts with PackageManager and Unity version-dependent packages.

### Gameplay
<div style="transform: scale(0.5); transform-origin: top center;">

[![GIF](GameplayRecording.gif)](GameplayRecording.gif)

</div>