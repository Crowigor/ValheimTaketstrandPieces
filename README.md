# TaketstrandPieces

This mod was created specifically for our Taketstrand co-op project.

I'm not an expert in 3D modeling or materials, so there might be minor imperfections with textures or small visual
details. Please keep that in mind.

The project originally started with the idea of working with doors — taking some assets from the BuildIt mod. I modified
these doors as best as I could, adjusting materials to use Ashwood instead. Later, I also adapted the original Blackwood
Gate from the base game by replacing its materials to match the Ashwood theme.

In addition to doors, I added two types of glass walls.

Finally, I created a Reinforced Chest with a customizable label. The chest allows you to set a custom name or
automatically display an item icon based on what you insert.

> **Note**  
> To see all the new pieces properly, you need a mod that supports scrolling in the build menu!  
> Recommended: [SearsCatalog](https://thunderstore.io/c/valheim/p/ComfyMods/SearsCatalog/).

# Doors

This mod adds **six new doors** to Valheim.  
You can find all of them in the **Building** category of the hammer menu.

![](https://github.com/Crowigor/ValheimTaketstrandPieces/blob/master/images/Doors.png?raw=true)

| Name                            | Required Station | Resources                     | Prefab Name                |
|:--------------------------------|:----------------:|:------------------------------|:---------------------------|
| Simple Ashwood Door             |    Workbench     | Blackwood ×12                 | `tkd_piece_ashwood_door`   |
| Ashwood Door with Window        | Artisan Station  | Blackwood ×7, Crystal ×2      | `tkd_piece_ashwood_door_1` |
| Ashwood Door with Double Window | Artisan Station  | Blackwood ×5, Crystal ×4      | `tkd_piece_ashwood_door_2` |
| Ashwood Door with Full Window   | Artisan Station  | Blackwood ×4, Crystal ×4      | `tkd_piece_ashwood_door_3` |
| Ashwood Door with Framed Window | Artisan Station  | Blackwood ×6, Crystal ×4      | `tkd_piece_ashwood_door_4` |
| Ashwood Gate                    |   Black Forge    | Blackwood ×16, FlametalNew ×4 | `tkd_piece_ashwood_gate`   |

## Glass walls

This mod also adds **two glass wall pieces**, perfect for modern or stylish builds.  
You can find them in the **Heavy Building** category of the hammer menu.

| Name            | Station     | Cost       | Prefab                      |
|:----------------|:------------|:-----------|:----------------------------|
| Glass Wall      | Stonecutter | 6× Crystal | `tkd_piece_glass_wall`      |
| Glass Half Wall | Stonecutter | 3× Crystal | `tkd_piece_glass_wall_half` |

## Named Chest

This mod adds a **special Reinforced Chest with a customizable name**!  
You can find it in the **Furniture** category of the hammer menu.

![](https://github.com/Crowigor/ValheimTaketstrandPieces/blob/master/images/NamedChest.png?raw=true)

| Name                       | Station   | Cost                            | Prefab                  |
|:---------------------------|:----------|:--------------------------------|:------------------------|
| Reinforced chest with sign | Workbench | 10× Fine wood, 2x Iron, 1x Coal | `tkd_piece_named_chest` |

The chest behaves like a normal container, but it has additional features:

- **Editable sign**: Press `Shift + E` to edit the chest's label.
- **Quick item name insertion**:
    - Press `[1–8]` to insert both the item name and icons.
    - Press `Shift + [1–8]` to insert only the item name (text).
    - Press `Alt + [1–8]` to insert only the item icons.
- **Icons and text display**:  
  The label dynamically shows either text, icons, or both, depending on how you insert the item.

### How it works

When editing the chest label, you can insert a special syntax:

- `#InternalID` — Inserts the name and icons of the item.
- `#InternalID#mode_all` — (default) Displays both text and icons.
- `#InternalID#mode_text` — Displays only the text (name of the item).
- `#InternalID#mode_icon` — Displays only the item icons without any text.

If you simply enter plain text (without `#`) or item wasn't find, it will show only the text on the chest front.

> **Tip**: The *InternalID* is usually the prefab name of the item, e.g., `Wood`, `Coal`, `BlackMetal`, etc.
