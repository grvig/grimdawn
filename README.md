# GDPilot

A context-aware controller layer for Grim Dawn, for Windows.

Grim Dawn's menus are a free-floating cursor over a UI built for a mouse.
GDPilot runs alongside the game, reads the gamepad directly, tracks which UI
panel is open, and synthesizes keyboard and mouse input to match. The binding
layout changes with the open panel, so the same stick aims in combat and hops
item to item in the stash.

The game is never modified. No game files are edited, no process memory is read
or written, and no game assets are redistributed. GDPilot synthesizes operating
system input events and nothing else.

## Status

Early development. See [PLAN.md](PLAN.md) for the full design and the phase
order, and [PROGRESS.md](PROGRESS.md) for where the work currently stands.

## Requirements

- Windows 11 x64
- Grim Dawn v1.3.0.0 or later
- An Xbox-layout gamepad

## Disclaimer

GDPilot is an unofficial tool. It is not affiliated with, endorsed by, or
associated with Crate Entertainment.

## Licence

MIT. See [LICENSE](LICENSE).
