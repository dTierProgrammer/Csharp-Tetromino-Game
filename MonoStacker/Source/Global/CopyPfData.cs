using MonoStacker.Source.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoStacker.Source.Global
{
    public static class CopyPfData
    {
        public static PlayFieldData? CopyData(PlayFieldData data) 
        {
            PlayFieldData val = new()
            {
                bufferType = data.bufferType,
                displaySetting = data.displaySetting,
                factory = data.factory,
                spawnAreaOffset = data.spawnAreaOffset,
                randomizer = data.randomizer,
                rotationSystem = data.rotationSystem,
                parsedSpins = data.parsedSpins,
                temporaryLandingSys = data.temporaryLandingSys,
                individualLcDelays = data.individualLcDelays,
                arrivalDelay = data.arrivalDelay,
                lineClearDelay = data.lineClearDelay,

            };
            return null;
        }
    }
}
