# Progress

Read this first in every session, then the plan section for the current phase.

## Current phase

Phase 0 — self-scoring feasibility probe.

## State

| Phase | Status |
| --- | --- |
| 0. Feasibility probe | In progress |
| 1. Input plumbing and safety | Not started |
| 2. Mode state machine and overlay | Not started |
| 3. Grid navigation by calibration | Not started |
| 4. Occupancy detection | Not started |
| 5. Macros and release | Not started |

## Phase 0 checklist

| Task | Status |
| --- | --- |
| 1. Spike console project | Done |
| 2. Window enumeration by observation | Done, smoke-run against a live desktop |
| 3. Injection wrapper, virtual key and scancode | Done |
| 4. Loopback harness and its test | Keyboard done and verified, mouse hook next |
| 5. Self-scoring game probe | Scoring written and parked, capture and probe not started |
| 6. Fallback ladder | Not started |

## Verified

- The solution builds clean with warnings as errors on .NET 8.0.425.
- `GDPilot.Spike --windows` lists visible top-level windows with their process
  names. Confirmed by running it. Grim Dawn was not running, so the game window
  itself is still unidentified.
- Key injection reaches the operating system. The loopback tests send W through
  `KeySender` in both modes and a global low-level hook records it: virtual key
  0x57 in virtual key mode, scan code 0x11 in scancode mode.

2 automated tests committed, both passing.

Not yet verified: mouse injection. `MouseSender` is written and builds, but the
loopback window does not hook the mouse yet.

## Blocked

Nothing. HT-1 is not yet raised because the probe it depends on is not built.

## Human tasks

| Task | Phase | Status |
| --- | --- | --- |
| HT-1 Confirm the game accepts synthesized input | 0 | Not yet raised |
| HT-2 Capture calibration fixtures | 3 | Not yet raised |
| HT-3 Feel tuning | 5 | Not yet raised |

Procedures live in [TESTING.md](TESTING.md).

## Next session starts here

1. `git stash pop`. The stash "Probe frame scoring and verdict" holds four
   files, written and passing, held back only to keep the day's commit count
   down. `FrameDifference` scores two captures over a centred region, ignoring
   alpha and the HUD margin. `ProbeVerdict` classifies a movement score against
   an idle baseline as pass, fail or inconclusive, because rain or fire can
   outscore a short walk in a quiet area. Commit as four pieces: each source
   file, then each test file.
2. Add the mouse hook to the loopback window and mouse tests for relative move,
   absolute move and both buttons.
3. Client-area capture of the game window, then the probe itself: idle capture,
   one second of forward movement, capture, score, cursor delta check.
4. The fallback ladder, then raise HT-1.
