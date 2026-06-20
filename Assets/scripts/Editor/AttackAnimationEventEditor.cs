using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public static class AttackAnimationEventEditor
{
    [MenuItem("Tools/Battle/Add Animation Events to Attack Clips")]
    public static void AddEventsToAttackClips()
    {
        string[] guids = AssetDatabase.FindAssets("Attack t:AnimationClip");
        var configs = LoadHitEffectConfigs();
        int count = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);

            if (clip == null)
                continue;

            AnimationEvent[] existing = AnimationUtility.GetAnimationEvents(clip);
            bool hasHit = false;
            bool hasEnd = false;

            foreach (var e in existing)
            {
                if (e.functionName == "OnAttackHit")
                    hasHit = true;
                if (e.functionName == "OnAttackEnd")
                    hasEnd = true;
            }

            if (hasHit && hasEnd)
            {
                Debug.Log($"Skipped {path} — already has events");
                continue;
            }

            var events = new List<AnimationEvent>();
            var autoConfig = FindMatchingConfig(path, configs);

            if (!hasHit)
            {
                var evt = new AnimationEvent
                {
                    time = clip.length * 0.4f,
                    functionName = "OnAttackHit"
                };
                if (autoConfig != null)
                    evt.objectReferenceParameter = autoConfig;
                events.Add(evt);
            }

            if (!hasEnd)
            {
                events.Add(new AnimationEvent
                {
                    time = clip.length,
                    functionName = "OnAttackEnd"
                });
            }

            foreach (var e in existing)
                events.Add(e);

            AnimationUtility.SetAnimationEvents(clip, events.ToArray());
            EditorUtility.SetDirty(clip);

            string hitInfo = autoConfig != null ? $" + auto-linked {autoConfig.name}" : " (no HitEffectConfig found nearby — assign manually)";
            Debug.Log($"Added events to {path} (length: {clip.length:F3}s){hitInfo}");
            count++;
        }

        Debug.Log($"Done! Updated {count} Attack clips.");
    }

    private static List<HitEffectConfig> LoadHitEffectConfigs()
    {
        var configs = new List<HitEffectConfig>();
        string[] guids = AssetDatabase.FindAssets("t:HitEffectConfig");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var config = AssetDatabase.LoadAssetAtPath<HitEffectConfig>(path);
            if (config != null)
                configs.Add(config);
        }
        return configs;
    }

    private static HitEffectConfig FindMatchingConfig(string clipPath, List<HitEffectConfig> configs)
    {
        string clipDir = Path.GetDirectoryName(clipPath).Replace("\\", "/");
        string clipName = Path.GetFileNameWithoutExtension(clipPath).ToLower();

        foreach (var config in configs)
        {
            string configPath = AssetDatabase.GetAssetPath(config);
            string configDir = Path.GetDirectoryName(configPath).Replace("\\", "/");
            string configName = config.name.ToLower();

            if (configDir == clipDir && (configName.Contains(clipName) || clipName.Contains(configName)))
                return config;
        }

        foreach (var config in configs)
        {
            string configPath = AssetDatabase.GetAssetPath(config);
            string configDir = Path.GetDirectoryName(configPath).Replace("\\", "/");

            if (configDir == clipDir)
                return config;
        }

        return null;
    }
}
