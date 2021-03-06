﻿using NvAPIWrapper;
using NvAPIWrapper.GPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Flowframes.OS
{
    class NvApi
    {
        public static List<PhysicalGPU> gpuList = new List<PhysicalGPU>();

        public static void Init()
        {
            try
            {
                NVIDIA.Initialize();
                PhysicalGPU[] gpus = PhysicalGPU.GetPhysicalGPUs();

                if (gpus.Length == 0)
                    return;

                gpuList = gpus.ToList();

                List<string> gpuNames = new List<string>();

                foreach (PhysicalGPU gpu in gpus)
                    gpuNames.Add(gpu.FullName);

                string gpuNamesList = string.Join(", ", gpuNames);

                Logger.Log($"Initialized Nvidia API. GPU{(gpus.Length > 1 ? "s" : "")}: {gpuNamesList}");
            }
            catch (Exception e)
            {
                Logger.Log("No Nvidia GPU(s) detected. You will not be able to use CUDA implementations.");
                Logger.Log($"Failed to initialize NvApi: {e.Message}\nIgnore this if you don't have an Nvidia GPU.", true);
            }
        }

        public static float GetVramGb (int gpu = 0)
        {
            try
            {
                return (gpuList[gpu].MemoryInformation.AvailableDedicatedVideoMemoryInkB / 1000f / 1024f);
            }
            catch
            {
                return 0f;
            }
        }

        public static float GetFreeVramGb(int gpu = 0)
        {
            try
            {
                return (gpuList[gpu].MemoryInformation.CurrentAvailableDedicatedVideoMemoryInkB / 1000f / 1024f);
            }
            catch
            {
                return 0f;
            }
        }

        public static string GetGpuName()
        {
            try
            {
                NVIDIA.Initialize();
                PhysicalGPU[] gpus = PhysicalGPU.GetPhysicalGPUs();
                if (gpus.Length == 0)
                    return "";

                return gpus[0].FullName;
            }
            catch
            {
                return "";
            }
        }

        public static bool HasTensorCores (int gpu = 0)
        {
            if (gpuList == null)
                Init();

            if (gpuList == null)
                return false;

            string gpuName = gpuList[gpu].FullName;

            return (gpuName.Contains("RTX ") || gpuName.Contains("Tesla V") || gpuName.Contains("Tesla T"));
        }
    }
}
