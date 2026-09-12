# Testing

Nearly all of this project verifies itself. Three tasks need a person, and they
are the last three sections of this file.

## The automated suite

From the repository root:

```
dotnet build
dotnet test
```

Both must pass before every commit. No test in the suite needs a gamepad, and
no test needs Grim Dawn running.

One exception to that rule is coming: the loopback tests launch a real window
and inject real operating system input at it. They pass without the game, but
they do take over the keyboard and mouse for a moment while they run, so do not
type during them.

## The three human tasks

These are the only three. Anything else that seems to need a person needs a
better fixture instead.

### HT-1 — Confirm the game accepts synthesized input

Phase 0. Not yet ready to run. The procedure is written here when the spike
executable builds.

### HT-2 — Capture calibration fixtures

Phase 3. Not yet ready to run.

### HT-3 — Feel tuning

Phase 5. Not yet ready to run.
