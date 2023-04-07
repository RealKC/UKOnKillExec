using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Threading;

namespace UKOnKillExec;

[BepInPlugin("kc.UKOnKillExec", "UKOnKillExec", "1.0.0")]
public class UKOnKillExecPlugin : BaseUnityPlugin
{
    private Thread _processCreator;
    private static SemaphoreSlim _semaphore;

    private static ManualLogSource _logger = null;
    private ProcessStartInfo _startInfo;

    UKOnKillExecPlugin()
    {
        _startInfo = new ProcessStartInfo
        {
            FileName = Paths.ConfigPath + "\\UKOnKillExec.bat",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
        };

        _semaphore = new SemaphoreSlim(0);
    }

    private void Awake()
    {

        _logger = Logger;
        _logger.LogInfo("UKOnKillExec started...");

        _processCreator = new Thread(() =>
        {
            while (true)
            {
                _semaphore.Wait();

                var process = new Process
                {
                    StartInfo = _startInfo
                };

                try
                {
                    process.Start();

                    _logger.LogInfo("Process standard out: ");
                    while (!process.StandardOutput.EndOfStream)
                    {
                        _logger.LogInfo($"\t{process.StandardOutput.ReadLine()}");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogInfo($"Failed to start process: {e}");
                }
            }
        });
        _processCreator.Start();

        Harmony.CreateAndPatchAll(typeof(UKOnKillExecPlugin));
    }

    [HarmonyPatch(typeof(EnemyIdentifier), "Death")]
    [HarmonyPrefix]
    static private void OnEnemyDeath(EnemyIdentifier __instance)
    {
        if (__instance.dead)
        {
            return;
        }

        _semaphore.Release();
    }
}
