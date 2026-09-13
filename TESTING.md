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

The loopback tests in `GDPilot.Integration.Tests` are the one place the suite
touches the real operating system. They launch a small window titled
"GDPilot Loopback", inject real key and mouse events, and assert what the
window's global hooks recorded. The window swallows injected events after
logging them, so nothing lands in other applications, and your own typing
passes through untouched. The window closes when the tests finish.

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
