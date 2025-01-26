# GGJ 2025 Bubble

2025 Global Game Jam project with the theme "Bubble".

## Getting Started

1. Clone this repository locally
2. Open the project in Unity (it will auto select the current version and have you download it)
3. Running the game. You'll need to load the main scene and additively load the remaining scenes to play the game.
   1. Run the `Assets/Game/Scenes/Main/Main.unity` scene
   2. Drag and drop the `Assets/Game/Scenes/Levels/Level1/Level1.unity` scene into the hierarchy to additively load the gameplay level
   3. Drag and drop the `Assets/Game/Scenes/Room/Room.unity` scene into the hierarchy to additively load the room
4. Run the game in the Unity editor to play

### Editing the project

Keep your edits confined to `Assets/Game/Users/[YOUR_NAME]` as much as possible to avoid merge conflicts. A lot of packages and plugins will dump folders in `Assets` and this will keep our source code clean.

Scripts should be in `Assets/Game/Scripts` with a proper namespace.

Game ready files should be stored under `Assets/Game/Scenes`. Edit these with caution and check with your team to avoid merge conflicts by posting to the `#dev` channel if you plan to edit something here.

## Docs

- [Making Commits](#how-to-make-commits)
- [LFS files](#how-to-add-lfs-files)
- [ASMDEF files](#how-to-edit-asmdef-files)

### How to make commits

1. Create a new branch from `develop` for your feature. Name it `feature/[FEATURE_NAME]` example `feature/player-movement`.
2. Make your changes and commit/push them to your branch.
3. When you are ready to merge your changes, create a pull request from your branch to `develop`. Please include a brief description of what you changed.
4. Post to the `#dev` channel that you have a pull request ready for review (ping @everyone please).
5. Wait for a team member to review your changes and approve the pull request.
6. Merge your changes into `develop` and delete your branch.

If you don't want to make commits with a command line, Ash strongly recommends using [GitKraken](https://www.gitkraken.com/). Which has an easy to understand GUI, free for public projects, and is well documented.

### How to add LFS files

If you are uploading a large file (image, 3D model, ect) please verify it is listed in `.gitattributes` BEFORE you commit it. If it is not listed, add this line to the file before committing or the sky may fall if the file is very large.

```
# Replace jpg with the file extension you are adding
*.jpg filter=lfs diff=lfs merge=lfs -text
```

#### Verify a file is in LFS

So you wanna quickly check if your file is tracked by LFS before you add it? This is tricky to do from a command line. Just use Git Kraken to check. It will show you if the file is being tracked by LFS.

[Git Kraken and LFS](https://help.gitkraken.com/gitkraken-desktop/git-lfs/)

![img.png](Docs/kraken-lfs.png)

### How to edit ASMDEF files

While scripting you may encounter file namespaces that cannot be found, resulting in an error. You will need to edit the `Game/game.asmdef` file to include the proper references.

![asmdef.png](Docs/asmdef.png)
