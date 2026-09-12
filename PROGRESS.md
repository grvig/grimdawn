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
| 3. Injection wrapper, virtual key and scancode | Keys done, mouse next |
| 4. Loopback harness and its test | Not started |
| 5. Self-scoring game probe | Not started |
| 6. Fallback ladder | Not started |

## Verified

- The solution builds clean with warnings as errors on .NET 8.0.425.
- `GDPilot.Spike --windows` lists visible top-level windows with their process
  names. Confirmed by running it: ten windows, correctly named. Grim Dawn was
  not running at the time, so the game window itself is still unidentified.

Nothing is verified by an automated test yet. The first test arrives with the
loopback harness, which is the next task.

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

Task 3 is half done. `KeySender` sends key down and key up in either virtual key
or scancode mode. The mouse half is not written: relative move, absolute move,
and the left and right button primitives. After that comes the loopback harness
and its test, which is the first real verification this project gets.
