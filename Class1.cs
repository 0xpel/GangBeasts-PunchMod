using MelonLoader;
using HarmonyLib;
using Il2CppFemur;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[assembly: MelonInfo(typeof(GangBeastsPunchMod2.PunchMod), "PunchMod", "1.0.0", "you")]
[assembly: MelonGame("Boneloaf", "Gang Beasts")]

namespace GangBeastsPunchMod2
{
    public class PunchMod : MelonMod
    {
        private object? _coroutine = null;

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (_coroutine != null)
                MelonCoroutines.Stop(_coroutine);
            _coroutine = MelonCoroutines.Start(PollForPlayer());
        }

        private IEnumerator PollForPlayer()
        {
            yield return new WaitForSeconds(3f);
            while (true)
            {
                yield return new WaitForSeconds(1f);
                var actorsRaw = Object.FindObjectsOfType<Actor>();
                Actor[] actors = new Actor[actorsRaw.Count];
                for (int i = 0; i < actorsRaw.Count; i++) actors[i] = actorsRaw[i];
                if (actors == null || actors.Length == 0) continue;

                foreach (Actor actor in actors)
                {
                    if (actor == null) continue;
                    Traverse trv = Traverse.Create((object)actor);
                    if (IsLocalPlayer(trv))
                    {
                        BuffPlayer(actor, trv);
                    }
                }
            }
        }

        private bool IsLocalPlayer(Traverse trv)
        {
            var controlled = trv.Property("_controlledBy", null)?.GetValue<object>()?.ToString()?.ToLower() ?? "";
            if (controlled == "ai" || controlled == "cpu" || controlled == "bot" ||
                controlled == "npc" || controlled == "wave") return false;
            if (controlled == "human" || controlled == "player" || controlled == "local") return true;
            var isLocal = trv.Property("isLocalPlayer", null);
            if (isLocal != null && isLocal.GetValue<bool>()) return true;
            return false;
        }

        private void BuffPlayer(Actor actor, Traverse trv)
        {
            trv.Property("_punchForceModifer", null)?.SetValue((object)25f);
            trv.Property("_punchDamageModifer", null)?.SetValue((object)15f);
            trv.Property("groundSpeed", null)?.SetValue((object)10f);
            trv.Property("liftStrength", null)?.SetValue((object)50f);
            MelonLogger.Msg("Player buffed!");
        }
    }
}